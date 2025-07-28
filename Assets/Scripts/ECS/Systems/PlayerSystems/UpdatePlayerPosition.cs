using ECS.Components.Input;
using ECS.ECSExtensions;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace ECS.Systems.PlayerSystems
{
    // [UpdateInGroup(typeof(PhysicsSystemGroup))]
    // [UpdateAfter(typeof(PhysicsSimulationGroup))]
    // [UpdateBefore(typeof(ExportPhysicsWorld))]
    public partial struct UpdatePlayerPosition : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerData>();
            state.RequireForUpdate<BlackboardTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerData = state.GetBlackboardComponent<PlayerData>();
            var transform = SystemAPI.GetComponent<LocalTransform>(playerData.PlayerEntity);
            playerData.PlayerPosition = transform.Position;
            
            state.SetBlackboardComponent(playerData);
        }
    }
}