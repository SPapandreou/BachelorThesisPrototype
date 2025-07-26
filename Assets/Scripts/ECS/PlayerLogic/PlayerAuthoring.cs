using Unity.Entities;
using UnityEngine;

namespace ECS.PlayerLogic
{
    public class PlayerAuthoring  : MonoBehaviour
    {
        public class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<PlayerComponent>(entity);
            }
        }
    }
}