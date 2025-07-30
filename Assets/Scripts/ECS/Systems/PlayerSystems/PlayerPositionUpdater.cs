using ECS.Data.Input;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace ECS.Systems.PlayerSystems
{
    // [UpdateInGroup(typeof(PhysicsSystemGroup))]
    // [UpdateAfter(typeof(PhysicsSimulationGroup))]
    // [UpdateBefore(typeof(ExportPhysicsWorld))]
    public partial struct PlayerPositionUpdater : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerData = SystemAPI.GetSingleton<PlayerData>();
            var transform = SystemAPI.GetComponent<LocalTransform>(playerData.PlayerEntity);
            playerData.PlayerPosition = transform.Position;
            
            SystemAPI.SetSingleton(playerData);
        }
    }
}