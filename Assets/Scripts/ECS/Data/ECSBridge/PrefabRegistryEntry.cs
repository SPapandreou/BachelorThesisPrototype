using Unity.Entities;

namespace ECS.Data.ECSBridge
{
    public struct PrefabRegistryEntry : IBufferElementData
    {
        public Entity Entity;
        public Hash128 GameObjectId;
    }
}