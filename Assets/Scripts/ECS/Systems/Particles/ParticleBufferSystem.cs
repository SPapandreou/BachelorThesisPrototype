using ECS.Data.Projectiles;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems.VFX
{
    public partial struct ParticleBufferSystem : ISystem
    {
        public const int Capacity = 1000;
        
        public JobHandle Dependencies;
        public NativeList<RawParticleSpawnData> ParticleBuffer;
        public NativeList<RawParticleKillData> KillBuffer;

        public void OnCreate(ref SystemState state)
        {
            ResetBuffer();
        }

        public void ResetBuffer()
        {
            ParticleBuffer = new NativeList<RawParticleSpawnData>(Capacity,Allocator.Persistent);
            //KillBuffer = new NativeList<RawParticleKillData>(Capacity, Allocator.Persistent);
        }

        public void OnDestroy(ref SystemState state)
        {
            if (ParticleBuffer.IsCreated)
            {
                ParticleBuffer.Dispose();
            }

            if (KillBuffer.IsCreated)
            {
                KillBuffer.Dispose();
            }
        }
    }
}