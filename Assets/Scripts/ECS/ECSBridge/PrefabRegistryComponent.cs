using System;
using Unity.Entities;

namespace ECS.ECSBridge
{
    public struct PrefabRegistryComponent : IComponentData
    {
        public Guid PrefabId;
        public Entity Entity;
    }
}