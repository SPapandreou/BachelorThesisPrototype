using ECS.Data.Projectiles;
using Unity.Mathematics;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace ECS.Data.Particles
{
    public struct RawParticleSpawnData
    {
        public ParticleType ParticleType;
        public Hash128 VfxId;
        public float Angle;
        public float3 Position;
        public float Size;
        public float3 Velocity;
        public HitboxType Hitbox;
        public float HitboxSize;
        public uint ParticleId;
        public float Damage;
        public LayerMask HitboxLayer;
        public LayerMask CollisionLayer;
    }
}