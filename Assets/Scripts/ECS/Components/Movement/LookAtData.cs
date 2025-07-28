using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Movement
{
    public struct LookAtData : IComponentData
    {
        public float3 Target;
    }
}