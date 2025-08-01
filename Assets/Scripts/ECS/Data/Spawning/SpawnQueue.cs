using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Data.Spawning
{
    public struct SpawnQueue : IComponentData
    {
        public JobHandle WriteDependency;
    }
}