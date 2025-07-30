using ECS.Data.Movement;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Movement
{
    public class LookAtDataAuthoring : MonoBehaviour
    {
        public class LookAtDataBaker : Baker<LookAtDataAuthoring>
        {
            public override void Bake(LookAtDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<LookAtData>(entity);
            }
        }
    }
}