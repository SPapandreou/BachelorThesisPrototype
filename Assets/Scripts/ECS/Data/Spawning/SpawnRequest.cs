using Unity.Entities;

namespace ECS.Data.Spawning
{
    public struct SpawnRequest : IBufferElementData
    {
        public Hash128 PrefabId;
        public int Amount;
        public SpawnDistribution Distribution;
    }
}