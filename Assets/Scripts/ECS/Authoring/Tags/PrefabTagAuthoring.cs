using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Tags
{
    public class PrefabTagAuthoring : MonoBehaviour
    {
        public class PrefabTagBaker : Baker<PrefabTagAuthoring>
        {
            public override void Bake(PrefabTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<Prefab>(entity);
            }
        }
    }
}