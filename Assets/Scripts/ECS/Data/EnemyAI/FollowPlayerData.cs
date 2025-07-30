using Unity.Entities;

namespace ECS.Data.EnemyAI
{
    public struct FollowPlayerData : IComponentData
    {
        public float Distance;
        public float Fov;
    }
}