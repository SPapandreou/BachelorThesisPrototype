using ECS.Data.EntityLifecycle;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.EntityLifecycle
{
    public class LifecycleDataAuthoring : MonoBehaviour
    {
        public class LifecycleDataBaker : Baker<LifecycleDataAuthoring>
        {
            public override void Bake(LifecycleDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<LifecycleData>(entity);
            }
        }
    }
}