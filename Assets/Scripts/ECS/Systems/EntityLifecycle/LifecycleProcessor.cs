using ECS.Data.EntityLifecycle;
using ECS.Data.Particles;
using ECS.Data.Projectiles;
using ECS.Systems.Particles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace ECS.Systems.EntityLifecycle
{
    [UpdateBefore(typeof(ParticleManager))]
    [BurstCompile]
    public partial struct LifecycleProcessor : ISystem
    {
        private ComponentLookup<ExplosionData> _explosionLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<LifecycleData>();
            state.RequireForUpdate<LocalTransform>();
            _explosionLookup = SystemAPI.GetComponentLookup<ExplosionData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _explosionLookup.Update(ref state);

            var bufferSystemHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<ParticleBufferSystem>();
            ref var bufferSystem =
                ref state.WorldUnmanaged.GetUnsafeSystemRef<ParticleBufferSystem>(bufferSystemHandle);

            var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

            state.Dependency = new ProcessLifecycleJob
            {
                ExplosionLookup = _explosionLookup,
                Ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                ParticleBuffer = bufferSystem.ParticleBuffer.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);

            bufferSystem.Dependencies = JobHandle.CombineDependencies(state.Dependency, bufferSystem.Dependencies);
        }


        [BurstCompile]
        public partial struct ProcessLifecycleJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<ExplosionData> ExplosionLookup;
            public EntityCommandBuffer.ParallelWriter Ecb;
            public NativeList<RawParticleSpawnData>.ParallelWriter ParticleBuffer;

            public void Execute([EntityIndexInQuery] int index, Entity entity, in LifecycleData data,
                in LocalTransform transform)
            {
                if (!data.IsExpired) return;

                Ecb.DestroyEntity(index, entity);

                if (!ExplosionLookup.HasComponent(entity)) return;

                var explosionData = ExplosionLookup[entity];
                ParticleBuffer.AddNoResize(new RawParticleSpawnData
                {
                    ParticleType = ParticleType.Unmanaged,
                    Position = transform.Position,
                    VfxId = explosionData.VfxId,
                    Size = explosionData.Size
                });
            }
        }
    }
}