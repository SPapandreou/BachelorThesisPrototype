using Unity.Entities;
using Hash128 = Unity.Entities.Hash128;

namespace ECS.Data.EntityLifecycle
{
    public struct ExplosionData : IComponentData
    {
        public Hash128 VfxId;
        public float Size;
    }
}