using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.Movement.Authoring
{
    public class VelocityAuthoring : MonoBehaviour
    {
        public float drag;
        public float maxSpeed;
        
        public class VelocityBaker : Baker<VelocityAuthoring>
        {
            public override void Bake(VelocityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(entity, new VelocityComponent
                {
                    Drag = authoring.drag,
                    MaxSpeed = authoring.maxSpeed
                });
            }
        }
    }
}