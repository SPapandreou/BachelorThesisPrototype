using System;
using System.Collections.Generic;
using System.Threading;
using ECS.Data.Particles;
using ECS.Data.Projectiles;
using ECS.Managed.ECSBridge;
using ECS.Systems.Particles;
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
        public List<GameObject> initKillGraphs;
        public List<GameObject> initOnlyGraphs;

        public int bufferSize = 1000;
        public int bufferingLevel = 3;

        private readonly List<VisualEffect> _initGraphs = new();
        private readonly List<VisualEffect> _killGraphs = new();

        private readonly Dictionary<VisualEffect, Hash128> _hashLookup = new();
        
        private BufferManager<ParticleInitData> _spawnBufferManager;
        private BufferManager<ParticleKillData> _killBufferManager;

        private bool _initialized;

        private JobHandle _handles;

        private readonly Queue<IGraphicsBufferHandle> _processThisFrame = new();
        private readonly Queue<IGraphicsBufferHandle> _processNextFrame = new();


        private void Awake()
        {
            foreach (var graph in initKillGraphs)
            {
                if (!graph.TryGetComponent<VisualEffect>(out var graphInstance))
                {
                    throw new InvalidOperationException("Registered Graph has no VFX Graph component.");
                }

                if (!graph.TryGetComponent<GameObjectId>(out var gameObjectId))
                {
                    throw new InvalidOperationException("Registered Graph has no GameObjectId component.");
                }
                
                _hashLookup[graphInstance] = gameObjectId.hash;
                _initGraphs.Add(graphInstance);
                _killGraphs.Add(graphInstance);
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
                
                _hashLookup[graphInstance] =  gameObjectId.hash;
                _initGraphs.Add(graphInstance);
            }

            _spawnBufferManager = new BufferManager<ParticleInitData>(initKillGraphs.Count + initOnlyGraphs.Count,
                bufferingLevel, bufferSize, "SpawnBuffer", "SpawnBufferCount");
            _killBufferManager = new BufferManager<ParticleKillData>(initKillGraphs.Count, bufferingLevel, bufferSize,
                "KillBuffer", "KillBufferCount");
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

            foreach (var bufferHandle in _processThisFrame)
            {
                bufferHandle.Unlock();
                bufferHandle.Upload();
            }
            _processThisFrame.Clear();

            foreach (var bufferHandle in _processNextFrame)
            {
                bufferHandle.Unlock();
                _processThisFrame.Enqueue(bufferHandle);
            }
            _processNextFrame.Clear();
        }

        private void OnDestroy()
        {
            _spawnBufferManager.Dispose();
            _killBufferManager.Dispose();
        }

        public void FillSpawnBuffer(ref NativeList<RawParticleSpawnData> spawnData)
        {
            var handles = new JobHandle();

            foreach (var graph in _initGraphs)
            {
                unsafe
                {
                    var buffer = _spawnBufferManager.GetBuffer(graph);
                    var handle = new FillSpawnBufferJob
                    {
                        SpawnData = spawnData.AsArray(),
                        Buffer = buffer.Lock(),
                        Hash = _hashLookup[graph],
                        Counter = buffer.GetCounterPtr()
                    }.Schedule(spawnData.Length, 64);

                    handles = JobHandle.CombineDependencies(handles, handle);
                    _processNextFrame.Enqueue(buffer);
                }
            }
            spawnData.Dispose(handles);

            _handles = JobHandle.CombineDependencies(handles, _handles);
        }

        public void FillKillBuffer(ref NativeList<RawParticleKillData> killData)
        {
            var handles = new JobHandle();

            foreach (var graph in _killGraphs)
            {
                var buffer = _killBufferManager.GetBuffer(graph);
                unsafe
                {
                    var handle = new FillKillBufferJob
                    {
                        KillData = killData.AsArray(),
                        Buffer = buffer.Lock(),
                        Hash = _hashLookup[graph],
                        Counter = buffer.GetCounterPtr()
                    }.Schedule(killData.Length, 64);
                    _processThisFrame.Enqueue(buffer);

                    handles = JobHandle.CombineDependencies(handles, handle);
                }
            }

            killData.Dispose(handles);

            _handles = JobHandle.CombineDependencies(handles, _handles);
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
                            Id = spawn.ParticleId,
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
                            Id = kill.ParticleId
                        };
                    }
                }
            }
        }
    }
}