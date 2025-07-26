using Unity.Entities;

namespace ECS.Projectiles
{
    public partial struct ProjectileSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
            var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (projectile, entity) in SystemAPI.Query<RefRW<ProjectileComponent>>().WithEntityAccess())
            {
                if(ECS.Extensions.MathExtensions.Approximately(projectile.ValueRW.Lifetime, 0f)) continue;
                projectile.ValueRW.Age += deltaTime;

                if (projectile.ValueRO.Age > projectile.ValueRO.Lifetime)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}