using ECS.Components;
using Unity.Entities;
using Unity.Transforms;

namespace ECS.Movement.Systems
{
    public partial struct MovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VelocityComponent>();
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach (var (transform, velocity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<VelocityComponent>>())
            {
                transform.ValueRW.Position += velocity.ValueRO.Velocity * deltaTime;
            }
        }
    }
}