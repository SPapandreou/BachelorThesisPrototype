using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace ECS.Data.Projectiles
{
    public struct RawParticleSpawnData
    {
        public Hash128 VfxId;
        public float Angle;
        public float3 Position;
        public float Size;
        public float3 Velocity;
        public HitboxType Hitbox;
        public float HitboxSize;
        public Entity ParticleId;
        public float Damage;
        public LayerMask HitboxLayer;
    }
}