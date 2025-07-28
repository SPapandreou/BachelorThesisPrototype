using ECS.Components.EnemyAI;
using ECS.Components.Movement;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Input
{
    public class LookAtMouseAuthoring : MonoBehaviour
    {
        public class LookAtMouseBaker : Baker<LookAtMouseAuthoring>
        {
            public override void Bake(LookAtMouseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<LookAtMouseTag>(entity);
            }
        }
    }
}