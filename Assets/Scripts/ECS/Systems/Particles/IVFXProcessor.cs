using ECS.Data.Projectiles;
using Unity.Collections;

namespace ECS.Systems.VFX
{
    public interface IVFXProcessor
    {
        public void FillSpawnBuffer(ref NativeList<RawParticleSpawnData> spawnData);
        public void FillKillBuffer(ref NativeList<RawParticleKillData> killData);
    }
}