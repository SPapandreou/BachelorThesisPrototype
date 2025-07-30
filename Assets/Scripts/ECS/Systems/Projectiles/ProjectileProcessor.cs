using ECS.Data.Projectiles;
using ECS.Systems.Particles;
using ECS.Systems.VFX;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems.Projectiles
{
    [BurstCompile]
    [UpdateAfter(typeof(ParticleManager))]
    public partial struct ProjectileProcessor : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileData>();
            state.RequireForUpdate<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
            var bufferSystemHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<ParticleBufferSystem>();
            ref var bufferSystem =
                ref state.WorldUnmanaged.GetUnsafeSystemRef<ParticleBufferSystem>(bufferSystemHandle);

            bufferSystem.Dependencies.Complete();
            
            state.Dependency = new ProcessProjectilesJob { DeltaTime = deltaTime }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct ProcessProjectilesJob : IJobEntity
        {
            public float DeltaTime;

            public void Execute(ref ProjectileData data, ref LocalTransform transform)
            {
                transform.Position += data.Velocity * DeltaTime;
                Debug.DrawRay(transform.Position, Vector3.up * 0.5f, Color.green, 0f, false);
            }
        }
    }
}