using UnityEngine;
using UnityEngine.VFX;

namespace ECS.Managed.VFX
{
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    public struct ParticleInitData
    {
        public uint Id;
        public float Size;
        public Vector3 Velocity;
        public Vector3 Position;
        public float Angle;
    }
}