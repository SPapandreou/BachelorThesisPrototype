using ECS.Data.EnemyAI;
using ECS.Data.Input;
using ECS.Data.Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Movement
{
    // [UpdateInGroup(typeof(PhysicsSystemGroup))]
    // [UpdateBefore(typeof(ThrusterLogic))]
    public partial struct FollowPlayer : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ThrusterData>();
            state.RequireForUpdate<FollowPlayerData>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<PlayerData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerPosition = SystemAPI.GetSingleton<PlayerData>().PlayerPosition;

            state.Dependency =
                new FollowPlayerJob { PlayerPosition = playerPosition }.ScheduleParallel(state.Dependency);
        }

        public partial struct FollowPlayerJob : IJobEntity
        {
            public float3 PlayerPosition;

            public void Execute(ref ThrusterData thruster, in FollowPlayerData followPlayer,
                in LocalTransform transform)
            {
                var lookDirection = math.rotate(transform.Rotation, math.right());
                var targetDirection = PlayerPosition - transform.Position;
                var fov = math.cos(followPlayer.Fov);
                var dot = math.dot(math.normalize(lookDirection), math.normalize(targetDirection));

                if (math.length(targetDirection) > followPlayer.Distance && dot >= fov)
                {
                    thruster.IsFiring = true;
                }
                else
                {
                    thruster.IsFiring = false;
                }
            }
        }
    }
}