using Unity.Entities;

namespace ECS.Data.Particles
{
    public struct RawParticleKillData
    {
        public Entity Entity;
        public uint ParticleId;
        public Hash128 VfxId;
    }
}