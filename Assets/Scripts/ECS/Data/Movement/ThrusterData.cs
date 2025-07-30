using Unity.Entities;

namespace ECS.Data.Movement
{
    public struct ThrusterData : IComponentData
    {
        public float ThrustForce;
        public float MaxSpeed;
        public float TurnSpeed;
        public bool IsFiring;
    }
}