using ECS.Movement.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace ECS.Movement.UnityPhysics.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ThrusterSystem : ISystem
    {
        private EntityQuery _entityQuery;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ThrusterComponent>();
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
            state.Dependency = new ApplyThrustJob
            {
                DeltaTime = deltaTime
            }.ScheduleParallel(state.Dependency);
        }

        public partial struct ApplyThrustJob : IJobEntity
        {
            public float DeltaTime;

            public void Execute(ref PhysicsVelocity velocity,
                in ThrusterComponent thruster, in LocalTransform transform)
            {
                if (!thruster.IsThrusting) return;
                float3 thrustVector = math.rotate(transform.Rotation, math.right());
                thrustVector *= thruster.Thrust * DeltaTime;
                velocity.Linear += thrustVector;
            }
        }
    }
}