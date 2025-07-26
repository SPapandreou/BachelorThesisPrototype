using Unity.Entities;

namespace ECS.Projectiles
{
    public struct ProjectileComponent : IComponentData
    {
        public float Lifetime;
        public float Age;
    }
}