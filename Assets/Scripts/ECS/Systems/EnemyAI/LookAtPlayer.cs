using ECS.Data.EnemyAI;
using ECS.Data.Input;
using ECS.Data.Movement;
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
            state.RequireForUpdate<PlayerData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerPosition = SystemAPI.GetSingleton<PlayerData>().PlayerPosition;

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