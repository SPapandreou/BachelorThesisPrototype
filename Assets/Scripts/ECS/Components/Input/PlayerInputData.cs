using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Input
{
    public struct PlayerInputData : IComponentData
    {
        public bool IsThrusting;
        public bool IsShooting;
        public float3 MouseInput;
    }
}