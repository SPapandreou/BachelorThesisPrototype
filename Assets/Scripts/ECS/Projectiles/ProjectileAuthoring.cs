using Unity.Entities;
using UnityEngine;

namespace ECS.Projectiles
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        public float lifetime;

        public class ProjectileBaker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(entity, new ProjectileComponent
                {
                    Lifetime = authoring.lifetime
                });
            }
        }
    }
}