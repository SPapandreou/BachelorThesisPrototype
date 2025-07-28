using Unity.Entities;

namespace ECS.Components.Movement
{
    public struct ThrusterData : IComponentData
    {
        public float ThrustForce;
        public float MaxSpeed;
    }
}