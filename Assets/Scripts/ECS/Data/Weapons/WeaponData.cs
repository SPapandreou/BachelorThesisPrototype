using ECS.Data.Projectiles;
using Unity.Entities;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace ECS.Data.Weapons
{
    public struct WeaponData : IComponentData
    {
        public Hash128 ParticleHash;
        public float ParticleSize;
        public float Cooldown;
        public float LastFired;
        public float ProjectileSpeed;
        public float Damage;
        public HitboxType Hitbox;
        public LayerMask HitboxLayer;
        public LayerMask CollisionLayer;
        public float HitboxSize;
    }
}