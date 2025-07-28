using ECS.Components.Input;
using Latios;
using Latios.Transforms;
using Unity.Entities;

namespace ECS.Systems.PlayerSystems
{
    public partial struct UpdatePlayerPosition : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerData = state.GetLatiosWorldUnmanaged().worldBlackboardEntity.GetComponentData<PlayerData>();
            var transform = SystemAPI.GetComponent<WorldTransform>(playerData.PlayerEntity);
            playerData.PlayerPosition = transform.position;
            
            state.GetLatiosWorldUnmanaged().worldBlackboardEntity.SetComponentData(playerData);
        }
    }
}