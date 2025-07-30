using UnityEngine.VFX;

namespace ECS.Managed.VFX
{
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    public struct ParticleKillData
    {
        public uint Id;
    }
}