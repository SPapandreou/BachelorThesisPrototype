using ECS.Components;
using ECS.Projectiles;
using Unity.Entities;
using Unity.Mathematics;

namespace Scenes.Test
{
    public class TestProjectileBaker : Baker<TestProjectileAuthoring>
    {
        public override void Bake(TestProjectileAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new VelocityComponent
            {
                Drag = 0,
                MaxSpeed = 20,
                Velocity = new float3(20,0,0)
            });
            AddComponent<ProjectileComponent>(entity);
            AddComponent<Prefab>(entity);
            
        }
    }
}