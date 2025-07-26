using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct RotationComponent : IComponentData
    {
        public float3 LookAt;
        public float TurnSpeed;
    }
}