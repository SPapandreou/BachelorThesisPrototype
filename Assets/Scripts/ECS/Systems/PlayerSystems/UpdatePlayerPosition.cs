using ECS.Components.Input;
using Latios;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace ECS.Systems.PlayerSystems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(ExportPhysicsWorld))]
    public partial struct UpdatePlayerPosition : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerData = state.GetLatiosWorldUnmanaged().worldBlackboardEntity.GetComponentData<PlayerData>();
            var transform = SystemAPI.GetComponent<LocalTransform>(playerData.PlayerEntity);
            playerData.PlayerPosition = transform.Position;
            
            state.GetLatiosWorldUnmanaged().worldBlackboardEntity.SetComponentData(playerData);
        }
    }
}