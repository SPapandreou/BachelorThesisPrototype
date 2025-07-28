using ECS.Components.EnemyAI;
using ECS.Components.Input;
using ECS.Components.Movement;
using Latios;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace ECS.Systems.Movement
{
    public partial struct LookAt : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LookAtData>();
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<ThrusterData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.CompleteDependency();
            state.Dependency = new LookAtJob().ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct LookAtJob : IJobEntity
        {
            public void Execute(ref PhysicsVelocity velocity, in LocalTransform transform, in ThrusterData thruster,
                in LookAtData data)
            {
                var lookDirection = math.normalize(math.rotate(transform.Rotation, math.right())).xy;
                var targetDirection = (data.Target - transform.Position).xy;

                float signedAngle = math.atan2(targetDirection.y, targetDirection.x) -
                                    math.atan2(lookDirection.y, lookDirection.x);
                signedAngle = math.atan2(math.sin(signedAngle), math.cos(signedAngle));

                if (math.abs(signedAngle) > 0.01f)
                {
                    velocity.Angular.z = signedAngle * thruster.TurnSpeed;
                }
                else
                {
                    velocity.Angular.z = 0f;
                }
            }
        }
    }
}