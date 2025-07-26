using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace ECS.Movement.UnityPhysics.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct RotationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RotationComponent>();
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new UpdateRotationJob().ScheduleParallel(state.Dependency);
        }


        public partial struct UpdateRotationJob : IJobEntity
        {
            public void Execute(ref PhysicsVelocity velocity, in RotationComponent rotation,
                in LocalTransform transform)
            {
                var lookDirection = math.normalize(math.rotate(transform.Rotation, math.right()));
                var direction = math.normalize(rotation.LookAt - transform.Position);

                float angle = math.acos(math.clamp(math.dot(lookDirection, direction), -1f, 1f));

                if (math.abs(angle) > 0.01f)
                {
                    float sign = math.sign(lookDirection.x * direction.y - lookDirection.y * direction.x);
                    velocity.Angular.z = sign * angle * rotation.TurnSpeed;
                }
                else
                {
                    velocity.Angular.z = 0;
                }
            }
        }
    }
}