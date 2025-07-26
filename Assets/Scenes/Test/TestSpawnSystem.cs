using System;
using ECS.Components;
using ECS.Projectiles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;


namespace Scenes.Test
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(LocalToWorldSystem))]
    public partial struct TestSpawnSystem : ISystem
    {
        private Random _random;
        private EntityQuery _query;

        public void OnCreate(ref SystemState state)
        {
            _random = new Random(12341);
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<VelocityComponent, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .Build(ref state);
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            var prefab = _query.ToEntityArray(Allocator.Temp)[0];

            float2 randomDirection = _random.NextFloat2(-1f, 1f);
            float3 velocity = float3.zero;
            velocity.xy = randomDirection;
            velocity = math.normalize(velocity);
            float angle = math.atan2(randomDirection.y, randomDirection.x);
            quaternion rotation = quaternion.RotateZ(angle);
            velocity *= _random.NextFloat(1f, 50f);
            if (math.length(velocity) < 1f)
            {
                Debug.Log("Velocity" + velocity);
            }

            var entity = state.EntityManager.Instantiate(prefab);
            state.EntityManager.SetComponentData(entity, new VelocityComponent
            {
                Drag = 0,
                MaxSpeed = 50,
                Velocity = velocity
            });
            state.EntityManager.SetComponentData(entity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = rotation,
                Scale = _random.NextFloat(1f, 5f)
            });
            state.EntityManager.SetComponentData(entity, new ProjectileComponent
            {
                Age = 0,
                Lifetime = 5
            });
        }
    }
}