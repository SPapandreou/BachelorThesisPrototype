using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.Movement.Authoring
{
    public class TeleportAuthoring : MonoBehaviour
    {
        public class TeleportBaker : Baker<TeleportAuthoring>
        {
            public override void Bake(TeleportAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<TeleportComponent>(entity);
            }
        }
    }
}