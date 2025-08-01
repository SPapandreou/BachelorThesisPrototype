using System.Collections.Generic;
using ECS.Data.ECSBridge;
using ECS.Managed.ECSBridge;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.ECSBridge
{
    public class PrefabRegistryAuthoring : MonoBehaviour
    {
        public List<GameObjectId> prefabs = new();
        
        public class PrefabRegistryBaker : Baker<PrefabRegistryAuthoring>
        {
            public override void Bake(PrefabRegistryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var buffer = AddBuffer<PrefabRegistryEntry>(entity);

                foreach (var prefab in authoring.prefabs)
                {
                    if (prefab == null) continue;
                    var prefabEntity = GetEntity(prefab.gameObject, TransformUsageFlags.None);
                    buffer.Add(new PrefabRegistryEntry
                    {
                        Entity = prefabEntity,
                        GameObjectId = prefab.hash
                    });
                }
            }
        }
    }
}