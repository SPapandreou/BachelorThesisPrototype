using Data;
using Unity.Entities;

namespace ECS.Components
{
    public struct WeaponComponent : IComponentData
    {
        public WeaponSpawnShape SpawnShape;
        public float Radius;
        public int ProjectileAmount;
        public float Cooldown;
        public float LastFired;
        public float ProjectileSize;
        public float ProjectileSpeed;

        public Entity ProjectilePrefab;
    }
}