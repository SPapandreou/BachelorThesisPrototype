using Unity.Entities;
using UnityEngine;

namespace ECS.ECSExtensions
{
    public class BlackboardAuthoring : MonoBehaviour
    {
        public class BlackboardBaker : Baker<BlackboardAuthoring>
        {
            public override void Bake(BlackboardAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<BlackboardTag>(entity);
            }
        }
    }
}