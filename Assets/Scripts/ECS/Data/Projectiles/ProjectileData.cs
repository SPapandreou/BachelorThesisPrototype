using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Data.Projectiles
{
    public struct ProjectileData : IComponentData
    {
        public float3 Velocity;
        public HitboxType Hitbox;
        public float HitboxSize;
        public float Damage;
        public LayerMask HitboxLayer;
    }
}