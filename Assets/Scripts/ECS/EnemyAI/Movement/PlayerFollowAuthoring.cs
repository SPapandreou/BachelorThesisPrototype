using Unity.Entities;
using UnityEngine;

namespace ECS.EnemyAI.Movement
{
    public class PlayerFollowAuthoring : MonoBehaviour
    {
        public float distance;
        public float fovAngle;

        public class PlayerFollowBaker : Baker<PlayerFollowAuthoring>
        {
            public override void Bake(PlayerFollowAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(entity, new PlayerFollowComponent
                {
                    Distance = authoring.distance,
                    FovAngle = authoring.fovAngle
                });
            }
        }
    }
}