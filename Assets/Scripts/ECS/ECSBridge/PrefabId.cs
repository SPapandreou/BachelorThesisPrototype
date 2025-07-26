using System;
using Unity.Entities;
using UnityEngine;

namespace ECS.ECSBridge
{
    public class PrefabId : MonoBehaviour
    {
        public string stringValue;
        public Guid Value => Guid.Parse(stringValue);
        public bool initialized;

        private void OnValidate()
        {
            if (!initialized)
            {
                stringValue = Guid.NewGuid().ToString();
                initialized = true;
            }
        }

        public class PrefabIdBaker : Baker<PrefabId>
        {
            public override void Bake(PrefabId authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PrefabRegistryComponent
                {
                    PrefabId = authoring.Value,
                    Entity = entity
                });
            }
        }
    }
}