using ECS.Components.Input;
using ECS.Components.Movement;
using ECS.ECSExtensions;
using Unity.Entities;
using Unity.Physics.Systems;

namespace ECS.Systems.PlayerSystems
{
    // [UpdateInGroup(typeof(PhysicsSystemGroup))]
    // [UpdateBefore(typeof(PhysicsSimulationGroup))]
    public partial struct PlayerMovement : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputData>();
            state.RequireForUpdate<PlayerData>();
            state.RequireForUpdate<ThrusterData>();
            state.RequireForUpdate<BlackboardTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerEntity = state.GetBlackboardComponent<PlayerData>()
                .PlayerEntity;
            var input = state.GetBlackboardComponent<PlayerInputData>();

            var thrusterData = SystemAPI.GetComponent<ThrusterData>(playerEntity);
            var lookAtData = SystemAPI.GetComponent<LookAtData>(playerEntity);

            thrusterData.IsFiring = input.IsThrusting;
            lookAtData.Target = input.MouseInput;

            SystemAPI.SetComponent(playerEntity, thrusterData);
            SystemAPI.SetComponent(playerEntity, lookAtData);
        }
    }
}