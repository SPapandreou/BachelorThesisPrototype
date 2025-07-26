using Unity.Entities;

namespace ECS.EnemyAI.Movement
{
    public struct PlayerFollowComponent : IComponentData
    {
        public float Distance;
        public float FovAngle;
    }
}