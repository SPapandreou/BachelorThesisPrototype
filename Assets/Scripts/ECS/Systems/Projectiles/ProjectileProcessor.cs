using ECS.Data.EntityLifecycle;
using ECS.Data.Particles;
using ECS.Data.PlayingArea;
using ECS.Data.Projectiles;
using ECS.Data.Status;
using ECS.Data.Visuals;
using ECS.Systems.Particles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


namespace ECS.Systems.Projectiles
{
    [BurstCompile]
    [UpdateBefore(typeof(ParticleManager))]
    public partial struct ProjectileProcessor : ISystem
    {
        private ComponentLookup<HealthData> _healthLookup;
        private ComponentLookup<LifecycleData> _lifecycleLookup;
        private ComponentLookup<HitResponseData> _hitResponseLookup;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProjectileData>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<PlayingFieldData>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            
            _healthLookup = SystemAPI.GetComponentLookup<HealthData>(true);
            _lifecycleLookup = SystemAPI.GetComponentLookup<LifecycleData>(true);
            _hitResponseLookup = SystemAPI.GetComponentLookup<HitResponseData>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            _healthLookup.Update(ref state);
            _lifecycleLookup.Update(ref state);
            _hitResponseLookup.Update(ref state);
            
            var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
            var bufferSystemHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<ParticleBufferSystem>();
            ref var bufferSystem =
                ref state.WorldUnmanaged.GetUnsafeSystemRef<ParticleBufferSystem>(bufferSystemHandle);
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var playingField = SystemAPI.GetSingleton<PlayingFieldData>();

            var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            state.Dependency = new ProcessProjectilesJob
            {
                DeltaTime = deltaTime,
                CollisionWorld = physicsWorld.CollisionWorld,
                HealthLookup = _healthLookup,
                LifecycleLookup = _lifecycleLookup,
                HitResponseLookup = _hitResponseLookup,
                KillData = bufferSystem.KillBuffer.AsParallelWriter(),
                PlayingField = playingField.Bounds,
                Ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
                
            }.ScheduleParallel(state.Dependency);
            bufferSystem.Dependencies = JobHandle.CombineDependencies(bufferSystem.Dependencies, state.Dependency);
        }

        [BurstCompile]
        public partial struct ProcessProjectilesJob : IJobEntity
        {
            [ReadOnly]public float DeltaTime;
            [ReadOnly]public CollisionWorld CollisionWorld;
            [ReadOnly]public ComponentLookup<HealthData> HealthLookup;
            [ReadOnly]public ComponentLookup<LifecycleData> LifecycleLookup;
            [ReadOnly]public ComponentLookup<HitResponseData> HitResponseLookup;
            public NativeList<RawParticleKillData>.ParallelWriter KillData;
            [ReadOnly]public Rect PlayingField;
            public EntityCommandBuffer.ParallelWriter Ecb;

            public void Execute([EntityIndexInQuery] int index, Entity entity, ref ProjectileData data,
                ref LocalTransform transform)
            {
                if (!PlayingField.Contains(transform.Position))
                {
                    KillData.AddNoResize(new RawParticleKillData
                    {
                        Entity = entity,
                        ParticleId = data.ParticleId,
                        VfxId = data.VfxId
                    });
                    return;
                }

                transform.Position += data.Velocity * DeltaTime;

                var filter = new CollisionFilter
                {
                    BelongsTo = (uint)data.CollisionLayer.value,
                    CollidesWith = (uint)data.HitboxLayer.value,
                    GroupIndex = 0
                };
                var hits = new NativeList<DistanceHit>(16, Allocator.Temp);
                if(CollisionWorld.OverlapSphere(transform.Position, data.HitboxSize, ref hits, filter))
                {
                    foreach (var hit in hits)
                    {
                        if (!HealthLookup.HasComponent(hit.Entity) || !LifecycleLookup.HasComponent(hit.Entity))
                            continue;
                        
                        var healthData = HealthLookup[hit.Entity];
                        healthData.Health -= data.Damage;
                        Ecb.SetComponent(index, hit.Entity, healthData);
                        
                        var hitResponseData = HitResponseLookup[hit.Entity];
                        hitResponseData.Value = 1f;
                        Ecb.SetComponent(index, hit.Entity, hitResponseData);
                        
                        if (healthData.Health <= 0)
                        {
                            var lifecycleData = LifecycleLookup[hit.Entity];
                            lifecycleData.IsExpired = true;
                            Ecb.SetComponent(index, hit.Entity, lifecycleData);
                        }
                        
                    }
                    KillData.AddNoResize(new RawParticleKillData
                    {
                        Entity = entity,
                        ParticleId = data.ParticleId,
                        VfxId = data.VfxId
                    });
                }

                hits.Dispose();
            }
        }
    }
}