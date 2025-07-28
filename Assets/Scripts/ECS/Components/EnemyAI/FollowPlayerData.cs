using Unity.Entities;

namespace ECS.Components.EnemyAI
{
    public struct FollowPlayerData : IComponentData
    {
        public float Distance;
        public float Fov;
    }
}