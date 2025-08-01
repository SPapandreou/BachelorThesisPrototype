using ECS.Data.Visuals;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Visuals
{
    public class HitResponseAuthoring : MonoBehaviour
    {
        public class HitResponseBaker : Baker<HitResponseAuthoring>
        {
            public override void Bake(HitResponseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<HitResponseData>(entity);
            }
        }
    }
}