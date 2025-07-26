using Unity.Entities;
using UnityEngine;

namespace ECS.EntityTags
{
    public class PrefabAuthoring : MonoBehaviour
    {
        public class PrefabBaker : Baker<PrefabAuthoring>
        {
            public override void Bake(PrefabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<Prefab>(entity);
            }
        }
    }
}