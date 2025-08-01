using ECS.Data.ECSBridge;
using ECS.Data.PlayingArea;
using ECS.Data.Spawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;
using Random = Unity.Mathematics.Random;

namespace ECS.Systems.Spawning
{
    [BurstCompile]
    public partial struct SpawnProcessor : ISystem
    {
        private Random _rng;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayingFieldData>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SpawnQueue>();
            state.RequireForUpdate<PrefabRegistryData>();
            _rng = new Random(12412);
        }

        public void OnUpdate(ref SystemState state)
        {
            var requestBuffer = SystemAPI.GetSingletonBuffer<SpawnRequest>();
            var spawnQueue = SystemAPI.GetSingleton<SpawnQueue>();
            var prefabRegistryData = SystemAPI.GetSingleton<PrefabRegistryData>();
            var playingFieldData = SystemAPI.GetSingleton<PlayingFieldData>();
            var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            state.Dependency = new SpawnJob
            {
                Ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                SpawnRequests = requestBuffer.AsNativeArray(),
                PrefabRegistry = prefabRegistryData.PrefabRegistry,
                BaseSeed = _rng.NextUInt(),
                PlayingField = playingFieldData.Bounds,
                SpawnMargin = 1f
            }.Schedule(requestBuffer.Length, 10, state.Dependency);
            
            spawnQueue.WriteDependency = JobHandle.CombineDependencies(spawnQueue.WriteDependency, state.Dependency);
            
            SystemAPI.SetSingleton(spawnQueue);
        }

        [BurstCompile]
        public struct SpawnJob : IJobParallelFor
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            [ReadOnly] public NativeArray<SpawnRequest> SpawnRequests;
            [ReadOnly] public NativeHashMap<Hash128, Entity> PrefabRegistry;
            [ReadOnly] public uint BaseSeed;
            [ReadOnly] public Rect PlayingField;
            [ReadOnly] public float SpawnMargin;

            public void Execute(int index)
            {
                var rng = new Random(BaseSeed + (uint)index);
                var request = SpawnRequests[index];
                var prefab = PrefabRegistry[request.PrefabId];

                for (int i = 0; i < request.Amount; i++)
                {
                    var position = RandomOutsideRect(ref PlayingField, SpawnMargin, ref rng);
                    var entity = Ecb.Instantiate(index, prefab);
                    Ecb.SetComponent(index, entity, LocalTransform.FromPosition(position));
                }
            }
            
            public static float3 RandomOutsideRect(ref Rect rect, float margin, ref Random rng)
            {
                int side = rng.NextInt(4);
                float x = 0f, y = 0f;

                switch (side)
                {
                    case 0:
                        x = rect.xMin - margin;
                        y = rng.NextFloat(rect.yMin, rect.yMax);
                        break;
                    case 1:
                        x = rect.xMax + margin;
                        y = rng.NextFloat(rect.yMin, rect.yMax);
                        break;
                    case 2:
                        x = rng.NextFloat(rect.xMin, rect.xMax);
                        y = rect.yMin - margin;
                        break;
                    case 3:
                        x = rng.NextFloat(rect.xMin, rect.xMax);
                        y = rect.yMax + margin;
                        break;
                }

                return new float3(x, y, 0f);
            }
        }
    }
}