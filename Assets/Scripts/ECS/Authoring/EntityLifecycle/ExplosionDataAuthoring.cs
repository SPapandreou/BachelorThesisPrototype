using ECS.Data.EntityLifecycle;
using ECS.Managed.ECSBridge;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.EntityLifecycle
{
    public class ExplosionDataAuthoring : MonoBehaviour
    {
        public GameObjectId vfxGraph;
        public float size;

        public class ExplosionDataBaker : Baker<ExplosionDataAuthoring>
        {
            public override void Bake(ExplosionDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(entity, new ExplosionData
                {
                    VfxId = authoring.vfxGraph.hash,
                    Size = authoring.size
                });
            }
        }
    }
}