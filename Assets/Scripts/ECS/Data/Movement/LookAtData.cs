using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Data.Movement
{
    public struct LookAtData : IComponentData
    {
        public float3 Target;
    }
}