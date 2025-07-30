using Unity.Entities;

namespace ECS.Data.Projectiles
{
    public struct RawParticleKillData
    {
        public Entity ParticleId;
        public Hash128 VfxId;
    }
}