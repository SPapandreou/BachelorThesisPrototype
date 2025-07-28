using ECS.Components.Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace ECS.Systems.Movement
{
    [BurstCompile]
    public partial struct ThrusterLogic : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<ThrusterData>();
            state.RequireForUpdate<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = state.World.Time.DeltaTime;
            state.CompleteDependency();

            state.Dependency = new ThrusterJob { DeltaTime = deltaTime }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct ThrusterJob : IJobEntity
        {
            public float DeltaTime;

            public void Execute(ref PhysicsVelocity velocity, in ThrusterData data, in LocalTransform transform)
            {
                if (!data.IsFiring) return;

                var lookDirection = math.rotate(transform.Rotation, math.right());

                var currentVelocity = velocity.Linear;
                var currentSpeed = math.length(velocity.Linear);

                currentVelocity += math.normalize(lookDirection) * data.ThrustForce * DeltaTime;

                if (currentSpeed <= data.MaxSpeed || math.length(currentVelocity) < currentSpeed)
                {
                    velocity.Linear = currentVelocity;
                }
            }
        }
    }
}