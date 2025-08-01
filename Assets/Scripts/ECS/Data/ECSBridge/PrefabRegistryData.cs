using Unity.Collections;
using Unity.Entities;

namespace ECS.Data.ECSBridge
{
    public struct PrefabRegistryData : IComponentData
    {
        public NativeHashMap<Hash128, Entity> PrefabRegistry;
    }
}