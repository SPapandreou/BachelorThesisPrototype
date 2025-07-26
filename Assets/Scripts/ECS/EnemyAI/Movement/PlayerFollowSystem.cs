using ECS.Components;
using ECS.Movement.Components;
using ECS.PlayerLogic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.EnemyAI.Movement
{
    public partial struct PlayerFollowSystem : ISystem
    {
        private EntityQuery _playerQuery;

        public void OnCreate(ref SystemState state)
        {
            _playerQuery = state.GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>(),
                ComponentType.ReadOnly<LocalTransform>());
            state.RequireForUpdate(_playerQuery);
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerTransform = _playerQuery.GetSingleton<LocalTransform>();

            state.Dependency = new FollowPlayerJob
            {
                PlayerTransform = playerTransform
            }.ScheduleParallel(state.Dependency);
        }

        public partial struct FollowPlayerJob : IJobEntity
        {
            [ReadOnly] public LocalTransform PlayerTransform;

            public void Execute(ref ThrusterComponent thruster, ref RotationComponent rotator,
                in LocalTransform transform, in PlayerFollowComponent follow)
            {
                rotator.LookAt = PlayerTransform.Position;

                var lookDirection = math.rotate(transform.Rotation, math.right());
                var targetDirection = PlayerTransform.Position - transform.Position;
                var fov = math.cos(follow.FovAngle);
                var dot = math.dot(math.normalize(lookDirection), math.normalize(targetDirection));

                if (math.length(targetDirection) > follow.Distance && dot >= fov)
                {
                    thruster.IsThrusting = true;
                }
                else
                {
                    thruster.IsThrusting = false;
                }
            }
        }
    }
}