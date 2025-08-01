using ECS.Data.Particles;
using ECS.Data.Projectiles;
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
        private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;
        private BeginSimulationEntityCommandBufferSystem _beginSimulationEcbSystem;

        private uint _offset;

        protected override void OnCreate()
        {
            Enabled = false;
            _endSimulationEcbSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            _beginSimulationEcbSystem = World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var bufferSystemHandle = World.Unmanaged.GetExistingUnmanagedSystem<ParticleBufferSystem>();
            ref var bufferSystem = ref World.Unmanaged.GetUnsafeSystemRef<ParticleBufferSystem>(bufferSystemHandle);
            var handles = JobHandle.CombineDependencies(bufferSystem.Dependencies, Dependency);
            handles.Complete();

            var ecbSpawn = _endSimulationEcbSystem.CreateCommandBuffer();
            var ecbKill = _beginSimulationEcbSystem.CreateCommandBuffer();
            
            
            var offset = _offset;
            _offset += (uint)bufferSystem.ParticleBuffer.Length;
            var handle = new SpawnParticlesJob
            {
                Offset = offset,
                Data = bufferSystem.ParticleBuffer.AsArray(),
                CommandBuffer = ecbSpawn.AsParallelWriter()
            }.Schedule(bufferSystem.ParticleBuffer.Length, 64, Dependency);
            _endSimulationEcbSystem.AddJobHandleForProducer(handle);

            Dependency = JobHandle.CombineDependencies(Dependency, handle);
            
            handle = new KillParticlesJob
            {
                Data = bufferSystem.KillBuffer.AsArray(),
                CommandBuffer = ecbKill.AsParallelWriter()
            }.Schedule(bufferSystem.KillBuffer.Length, 64, Dependency);
            _beginSimulationEcbSystem.AddJobHandleForProducer(handle);
            
            Dependency = JobHandle.CombineDependencies(Dependency, handle);

            Dependency.Complete();
            _vfxProcessor.FillSpawnBuffer(ref bufferSystem.ParticleBuffer);
            _vfxProcessor.FillKillBuffer(ref bufferSystem.KillBuffer);
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
            [ReadOnly] public uint Offset;
            public NativeArray<RawParticleSpawnData> Data;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;


            public void Execute(int index)
            {
                var spawnData = Data[index];
                if (spawnData.ParticleType == ParticleType.Unmanaged) return;
                
                var entity = CommandBuffer.CreateEntity(index);
                uint particleId = (uint)index + Offset;
                CommandBuffer.AddComponent(index, entity, LocalTransform.FromPosition(spawnData.Position));
                CommandBuffer.AddComponent(index, entity, new ProjectileData
                {
                    Velocity = spawnData.Velocity,
                    Hitbox = spawnData.Hitbox,
                    HitboxSize = spawnData.HitboxSize,
                    HitboxLayer = spawnData.HitboxLayer,
                    Damage = spawnData.Damage,
                    VfxId = spawnData.VfxId,
                    CollisionLayer = spawnData.CollisionLayer,
                    ParticleId = particleId
                    
                });
                spawnData.ParticleId = particleId;
                Data[index] = spawnData;
            }
        }

        [BurstCompile]
        public struct KillParticlesJob : IJobParallelFor
        {
            public NativeArray<RawParticleKillData> Data;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            public void Execute(int index)
            {
                var entity = Data[index].Entity;
                CommandBuffer.DestroyEntity(index, entity);
            }
        }
    }
}