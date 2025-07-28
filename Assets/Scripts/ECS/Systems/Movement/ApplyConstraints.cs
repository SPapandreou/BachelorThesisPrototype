using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace ECS.Systems.Movement
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(ExportPhysicsWorld))]
    [BurstCompile]
    public partial struct ApplyConstraints : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new ApplyConstraintsJob().ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct ApplyConstraintsJob : IJobEntity
        {
            public void Execute(ref PhysicsVelocity velocity, ref LocalTransform transform)
            {
                velocity.Linear.z = 0f;
                velocity.Angular.xy = 0f;

                transform.Position.z = 0f;
                
                var right = math.mul(transform.Rotation, math.right());
                var angle = math.atan2(right.y, right.x);
                transform.Rotation = quaternion.RotateZ(angle);
            }
        }
    }
}