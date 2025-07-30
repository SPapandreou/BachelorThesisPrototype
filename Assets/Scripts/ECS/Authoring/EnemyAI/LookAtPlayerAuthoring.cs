using ECS.Data.EnemyAI;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.EnemyAI
{
    public class LookAtPlayerAuthoring : MonoBehaviour
    {
        public class LookAtPlayerBaker : Baker<LookAtPlayerAuthoring>
        {
            public override void Bake(LookAtPlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<LookAtPlayerTag>(entity);
            }
        }
    }
}