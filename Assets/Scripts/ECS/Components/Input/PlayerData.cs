using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Input
{
    public struct PlayerData : IComponentData
    {
        public Entity PlayerEntity;
        public float3 PlayerPosition;
    }
}