using Unity.Entities;

namespace ECS.Movement.Components
{
    public struct ThrusterComponent : IComponentData
    {
        public bool IsThrusting;
        public float Thrust;
    }
}