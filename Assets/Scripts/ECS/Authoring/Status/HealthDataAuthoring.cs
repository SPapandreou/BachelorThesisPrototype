using ECS.Data.Status;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Status
{
    public class HealthDataAuthoring : MonoBehaviour
    {
        public float health;
        
        public class HealthDataBaker : Baker<HealthDataAuthoring>
        {
            public override void Bake(HealthDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(entity, new HealthData
                {
                    Health = authoring.health
                });
            }
        }
    }
}