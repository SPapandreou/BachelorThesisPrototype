using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace ECS.Data.Projectiles
{
    public struct ProjectileData : IComponentData
    {
        public float3 Velocity;
        public HitboxType Hitbox;
        public float HitboxSize;
        public float Damage;
        public LayerMask HitboxLayer;
        public LayerMask CollisionLayer;
        public Hash128 VfxId;
        public uint ParticleId;
    }
}