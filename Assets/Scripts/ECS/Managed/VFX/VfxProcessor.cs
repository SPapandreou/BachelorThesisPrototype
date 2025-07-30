using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using ECS.Data.Projectiles;
using ECS.Managed.ECSBridge;
using ECS.Systems.Particles;
using ECS.Systems.VFX;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.VFX;
using Hash128 = Unity.Entities.Hash128;

namespace ECS.Managed.VFX
{
    public class VfxProcessor : MonoBehaviour, IVFXProcessor
    {
        public List<GameObject> graphs;
        public List<GameObject> initOnlyGraphs;

        public int bufferSize = 1000;

        private readonly Dictionary<Hash128, VisualEffect> _graphLookup = new();
        private readonly Dictionary<Hash128, GraphicsBuffer> _spawnBufferLookup = new();
        private readonly Dictionary<Hash128, GraphicsBuffer> _killBufferLookup = new();
        private readonly Dictionary<Hash128, NativeReference<int>> _spawnBufferSize = new();
        private readonly Dictionary<Hash128, NativeReference<int>> _killBufferSize = new();

        private bool _initialized;

        private JobHandle _handles;
        private bool _spawnBufferLocked;
        private bool _killBufferLocked;


        private void Awake()
        {
            foreach (var graph in graphs)
            {
                if (!graph.TryGetComponent<VisualEffect>(out var graphInstance))
                {
                    throw new InvalidOperationException("Registered Graph has no VFX Graph component.");
                }

                if (!graph.TryGetComponent<GameObjectId>(out var gameObjectId))
                {
                    throw new InvalidOperationException("Registered Graph has no GameObjectId component.");
                }

                _graphLookup[gameObjectId.hash] = graphInstance;

                _spawnBufferLookup[gameObjectId.hash] = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                    GraphicsBuffer.UsageFlags.LockBufferForWrite, bufferSize,
                    Marshal.SizeOf<ParticleInitData>());
                graphInstance.SetGraphicsBuffer("SpawnBuffer", _spawnBufferLookup[gameObjectId.hash]);

                _killBufferLookup[gameObjectId.hash] = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                    GraphicsBuffer.UsageFlags.LockBufferForWrite, bufferSize,
                    Marshal.SizeOf<ParticleKillData>());
                _graphLookup[gameObjectId.hash].SetGraphicsBuffer("KillBuffer", _killBufferLookup[gameObjectId.hash]);

                _spawnBufferSize[gameObjectId.hash] = new NativeReference<int>(Allocator.Persistent);
                _killBufferSize[gameObjectId.hash] = new NativeReference<int>(Allocator.Persistent);
            }

            foreach (var graph in initOnlyGraphs)
            {
                if (!graph.TryGetComponent<VisualEffect>(out var graphInstance))
                {
                    throw new InvalidOperationException("Registered Graph has no VFX Graph component.");
                }

                if (!graph.TryGetComponent<GameObjectId>(out var gameObjectId))
                {
                    throw new InvalidOperationException("Registered Graph has no GameObjectId component.");
                }

                _graphLookup[gameObjectId.hash] = graphInstance;

                _spawnBufferLookup[gameObjectId.hash] = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bufferSize,
                    Marshal.SizeOf<ParticleInitData>());
                graphInstance.SetGraphicsBuffer("SpawnBuffer", _spawnBufferLookup[gameObjectId.hash]);

                _spawnBufferSize[gameObjectId.hash] = new NativeReference<int>(Allocator.Persistent);
            }
        }

        private void Update()
        {
            if (!_initialized)
            {
                var system = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ParticleManager>();

                if (system == null) return;
                system.RegisterVFXProcessor(this);
                _initialized = true;
            }

            if (!_handles.IsCompleted)
            {
                _handles.Complete();
            }

            _handles = new JobHandle();

            if (_spawnBufferLocked)
            {
                foreach (var (hash, counter) in _spawnBufferSize)
                {
                    _spawnBufferLookup[hash].UnlockBufferAfterWrite<ParticleInitData>(counter.Value);
                    _graphLookup[hash].SetInt("SpawnBufferSize", counter.Value);
                }
                
                _spawnBufferLocked = false;
            }

            if (_killBufferLocked)
            {
                foreach (var (hash, counter) in _killBufferSize)
                {
                    _killBufferLookup[hash].UnlockBufferAfterWrite<ParticleKillData>(counter.Value);
                    _graphLookup[hash].SetInt("KillBufferSize", counter.Value);
                }

                _killBufferLocked = false;
            }
        }

        private void OnDestroy()
        {
            foreach (var buffer in _spawnBufferLookup.Values)
            {
                buffer.Release();
                buffer.Dispose();
            }

            foreach (var buffer in _killBufferLookup.Values)
            {
                buffer.Release();
                buffer.Dispose();
            }

            foreach (var counter in _spawnBufferSize.Values)
            {
                counter.Dispose();
            }

            foreach (var counter in _killBufferSize.Values)
            {
                counter.Dispose();
            }
        }

        public void FillSpawnBuffer(ref NativeList<RawParticleSpawnData> spawnData)
        {
            var handles = new JobHandle();

            foreach (var (hash, buffer) in _spawnBufferLookup)
            {
                var counter = _spawnBufferSize[hash];
                counter.Value = 0;
                unsafe
                {
                    var handle = new FillSpawnBufferJob
                    {
                        SpawnData = spawnData.AsArray(),
                        Buffer = buffer.LockBufferForWrite<ParticleInitData>(0, bufferSize),
                        Hash = hash,
                        Counter = counter.GetUnsafePtr()
                    }.Schedule(spawnData.Length, 64);

                    handles = JobHandle.CombineDependencies(handles, handle);
                }

                spawnData.Dispose(handles);

            }

            _handles = JobHandle.CombineDependencies(handles, _handles);
            _spawnBufferLocked = true;
        }

        public void FillKillBuffer(ref NativeList<RawParticleKillData> killData)
        {
            var handles = new JobHandle();

            foreach (var (hash, buffer) in _killBufferLookup)
            {
                var counter = _killBufferSize[hash];
                counter.Value = 0;
                unsafe
                {
                    var handle = new FillKillBufferJob
                    {
                        KillData = killData.AsArray(),
                        Buffer = buffer.LockBufferForWrite<ParticleKillData>(0, bufferSize),
                        Hash = hash,
                        Counter = counter.GetUnsafePtr()
                    }.Schedule(killData.Length, 64);

                    handles = JobHandle.CombineDependencies(handles, handle);    
                }
            }

            _handles = JobHandle.CombineDependencies(handles, _handles);
            _killBufferLocked = true;
        }

        [BurstCompile]
        private struct FillSpawnBufferJob : IJobParallelFor
        {
            [NativeDisableParallelForRestriction] [ReadOnly]
            public NativeArray<RawParticleSpawnData> SpawnData;

            [NativeDisableParallelForRestriction] public NativeArray<ParticleInitData> Buffer;

            public Hash128 Hash;

            [NativeDisableUnsafePtrRestriction] [NativeDisableParallelForRestriction]
            public unsafe int* Counter;

            public void Execute(int index)
            {
                var spawn = SpawnData[index];

                if (spawn.VfxId == Hash)
                {
                    unsafe
                    {
                        int i = Interlocked.Increment(ref *Counter) - 1;
                        Buffer[i] = new ParticleInitData
                        {
                            Size = spawn.Size,
                            Position = spawn.Position,
                            Velocity = spawn.Velocity,
                            Id = (uint)spawn.ParticleId.Index,
                            Angle = spawn.Angle
                        };
                    }
                }
            }
        }

        [BurstCompile]
        private struct FillKillBufferJob : IJobParallelFor
        {
            [NativeDisableParallelForRestriction] [ReadOnly]
            public NativeArray<RawParticleKillData> KillData;

            [NativeDisableParallelForRestriction] public NativeArray<ParticleKillData> Buffer;

            public Hash128 Hash;

            [NativeDisableUnsafePtrRestriction] public unsafe int* Counter;

            public void Execute(int index)
            {
                var kill = KillData[index];

                if (kill.VfxId == Hash)
                {
                    unsafe
                    {
                        int i = Interlocked.Increment(ref *Counter) - 1;
                        Buffer[i] = new ParticleKillData
                        {
                            Id = (uint)kill.ParticleId.Index
                        };
                    }
                }
            }
        }
    }
}