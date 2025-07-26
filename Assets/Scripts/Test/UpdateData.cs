using UnityEngine;
using UnityEngine.VFX;

namespace Test
{
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    public struct UpdateData
    {
        public Vector3 Position;
        public float Angle;
        public int Alive;
    }
}