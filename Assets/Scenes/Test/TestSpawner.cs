using ECS.EnemyAI.Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenes.Test
{
    public partial struct TestSpawner : ISystem
    {
        private EntityQuery _prefabQuery;
        private Unity.Mathematics.Random _random;

        public void OnCreate(ref SystemState state)
        {
            _prefabQuery = state.GetEntityQuery(ComponentType.ReadOnly<PlayerFollowComponent>(),
                ComponentType.ReadOnly<Prefab>());
            _random = new Unity.Mathematics.Random(1231241);
            state.RequireForUpdate(_prefabQuery);
        }


        public void OnUpdate(ref SystemState state)
        {
            var entity = _prefabQuery.GetSingletonEntity();
            var spawnedEntity = state.EntityManager.Instantiate(entity);
            var random = _random.NextFloat2(-10, 10);
            var position = float3.zero;
            position.xy = random;
            
            state.EntityManager.SetComponentData(spawnedEntity, new LocalTransform
            {
                Position = position,
                Scale = 0.1f
            });
        }
    }
}