using ECS.Data.Projectiles;
using ECS.Systems.VFX;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace ECS.Systems.Particles
{
    public partial class ParticleManager : SystemBase
    {
        private IVFXProcessor _vfxProcessor;
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            Enabled = false;
            _ecbSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var bufferSystemHandle = World.Unmanaged.GetExistingUnmanagedSystem<ParticleBufferSystem>();
            ref var bufferSystem = ref World.Unmanaged.GetUnsafeSystemRef<ParticleBufferSystem>(bufferSystemHandle);
            var handles = JobHandle.CombineDependencies(bufferSystem.Dependencies, Dependency);
            handles.Complete();

            var ecb = _ecbSystem.CreateCommandBuffer();

            Dependency = new SpawnParticlesJob
            {
                Data = bufferSystem.ParticleBuffer.AsArray(),
                CommandBuffer = ecb.AsParallelWriter()
            }.Schedule(bufferSystem.ParticleBuffer.Length, 64, Dependency);
            _ecbSystem.AddJobHandleForProducer(Dependency);

            Dependency.Complete();
            _vfxProcessor.FillSpawnBuffer(ref bufferSystem.ParticleBuffer);
            bufferSystem.ResetBuffer();
        }

        public void RegisterVFXProcessor(IVFXProcessor processor)
        {
            _vfxProcessor = processor;
            Enabled = true;
        }

        [BurstCompile]
        public struct SpawnParticlesJob : IJobParallelFor
        {
            public NativeArray<RawParticleSpawnData> Data;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;


            public void Execute(int index)
            {
                var entity = CommandBuffer.CreateEntity(index);
                var spawnData = Data[index];
                CommandBuffer.AddComponent(index, entity, LocalTransform.FromPosition(spawnData.Position));
                CommandBuffer.AddComponent(index, entity, new ProjectileData
                {
                    Velocity = spawnData.Velocity,
                    Hitbox = spawnData.Hitbox,
                    HitboxSize = spawnData.HitboxSize,
                    HitboxLayer = spawnData.HitboxLayer,
                    Damage = spawnData.Damage
                });
                
                spawnData.ParticleId = entity;
                Data[index] = spawnData;
            }
        }
    }
}