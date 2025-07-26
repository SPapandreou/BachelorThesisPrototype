using Unity.Entities;
using Unity.Transforms;

namespace Scenes.Test
{
    public class TestBaker : Baker<TestBakerAuthoring>
    {
        public override void Bake(TestBakerAuthoring authoring)
        {
            var testEntity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
            var prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.None);
            AddComponent(testEntity, new TestReferenceComponent
            {
                Prefab = prefabEntity
            });
        }
    }
}