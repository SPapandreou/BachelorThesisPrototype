using ECS.Components.EnemyAI;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.EnemyAI
{
    public class FollowPlayerDataAuthoring : MonoBehaviour
    {
        public float distance;
        
        public class FollowPlayerTagBaker : Baker<FollowPlayerDataAuthoring>
        {
            public override void Bake(FollowPlayerDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(entity, new FollowPlayerData
                {
                    Distance = authoring.distance
                });
            }
        }
    }
}