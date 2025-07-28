using ECS.Components.EnemyAI;
using ECS.Components.Input;
using ECS.Components.Movement;
using ECS.ECSExtensions;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems.Movement
{
    [BurstCompile]
    [WithAll(typeof(LookAtPlayerTag))]
    public partial struct LookAtPlayer : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LookAtPlayerTag>();
            state.RequireForUpdate<LookAtData>();
            state.RequireForUpdate<BlackboardTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerPosition = state.GetBlackboardComponent<PlayerData>().PlayerPosition;

            state.Dependency =
                new LookAtPlayerJob { PlayerPosition = playerPosition }.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    [WithAll(typeof(LookAtPlayerTag))]
    public partial struct LookAtPlayerJob : IJobEntity
    {
        public float3 PlayerPosition;

        public void Execute(ref LookAtData data)
        {
            data.Target = PlayerPosition;
        }
    }
}