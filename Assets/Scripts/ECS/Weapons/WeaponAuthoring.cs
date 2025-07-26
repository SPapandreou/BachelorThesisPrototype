using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.Weapons
{
    public class WeaponAuthoring : MonoBehaviour
    {
        public GameObject projectilePrefab;
        public float projectileSpeed;
        public float cooldown;
        
        public class WeaponBaker : Baker<WeaponAuthoring>
        {
            public override void Bake(WeaponAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WeaponComponent
                {
                    ProjectilePrefab = GetEntity(authoring.projectilePrefab, TransformUsageFlags.None),
                    Cooldown = authoring.cooldown,
                    ProjectileSpeed = authoring.projectileSpeed
                });
            }
        }
    }
}