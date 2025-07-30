using ECS.Data.Projectiles;
using ECS.Data.Weapons;
using ECS.Managed.ECSBridge;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Authoring.Weapons
{
    public class WeaponDataAuthoring : MonoBehaviour
    {
        public GameObjectId vfxGraph;
        public float projectileSize;
        public float cooldown;
        public float projectileSpeed;
        public float damage;
        public HitboxType hitbox;
        public LayerMask hitboxLayer;
        public float hitboxSize;
        
        public class WeaponDataBaker : Baker<WeaponDataAuthoring>
        {
            public override void Bake(WeaponDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var parent = authoring.transform.parent;
                AddComponent(entity, new Parent
                {
                    Value = GetEntity(parent, TransformUsageFlags.None)
                });
                AddComponent(entity, new WeaponData
                {
                    ParticleHash = authoring.vfxGraph.hash,
                    ParticleSize = authoring.projectileSize,
                    Cooldown = authoring.cooldown,
                    ProjectileSpeed = authoring.projectileSpeed,
                    Damage = authoring.damage,
                    Hitbox = authoring.hitbox,
                    HitboxLayer = authoring.hitboxLayer,
                    HitboxSize = authoring.hitboxSize
                });
            }
        }
    }
}