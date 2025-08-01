using Unity.Entities;

namespace ECS.Data.EntityLifecycle
{
    public struct LifecycleData : IComponentData
    {
        public bool IsExpired;
    }
}