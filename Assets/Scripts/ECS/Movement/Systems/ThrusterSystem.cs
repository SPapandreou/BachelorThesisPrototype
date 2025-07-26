using ECS.Components;
using ECS.Movement.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Movement.Systems
{
    [UpdateBefore(typeof(Movement.Systems.DragSystem))]
    public partial class ThrusterSystem : SystemBase
    {
        private static readonly float3 RightVector = new float3(1, 0, 0);
        
        protected override void OnCreate()
        {
            RequireForUpdate<ThrusterComponent>();
            RequireForUpdate<VelocityComponent>();
            RequireForUpdate<LocalTransform>();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            var deltaTime = World.Time.DeltaTime;
            
            Entities.ForEach((ref VelocityComponent velocity,
                in LocalTransform transform, in ThrusterComponent thruster) =>
            {
                if (!thruster.IsThrusting) return;
                
                float3 thrustVector = math.mul(transform.Rotation, RightVector);
                thrustVector *= thruster.Thrust * deltaTime;
                velocity.Velocity.x += thrustVector.x;
                velocity.Velocity.y += thrustVector.y;
            }).ScheduleParallel();
        }
    }
}