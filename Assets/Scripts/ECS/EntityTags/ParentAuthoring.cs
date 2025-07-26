using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.EntityTags
{
    public class ParentAuthoring : MonoBehaviour
    {
        public class ParentBaker : Baker<ParentAuthoring>
        {
            public override void Bake(ParentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                if (authoring.transform.parent != null)
                {
                    AddComponent(entity, new Parent
                    {
                        Value = GetEntity(authoring.transform.parent.gameObject, TransformUsageFlags.None)
                    });
                }
            }
        }
    }
}