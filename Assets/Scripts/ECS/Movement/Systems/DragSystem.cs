using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Movement.Systems
{
    public partial class DragSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<VelocityComponent>();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            var deltaTime = World.Time.DeltaTime;

            Entities.ForEach((ref VelocityComponent velocityComponent) =>
            {
                var speed = math.length(velocityComponent.Velocity);
                speed -= velocityComponent.Drag * deltaTime;
                speed = math.max(speed, 0);
                speed = math.min(speed, velocityComponent.MaxSpeed);

                if (speed > 0f)
                {
                    velocityComponent.Velocity = math.normalize(velocityComponent.Velocity) * speed;
                }
                else
                {
                    velocityComponent.Velocity = float3.zero;
                }
            }).ScheduleParallel();
        }
    }
}