using Unity.Entities;

namespace Scenes.Test
{
    public struct TestReferenceComponent : IComponentData
    {
        public Entity Prefab;
    }
}