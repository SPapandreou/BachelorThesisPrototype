using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.Movement.Authoring
{
    public class RotationAuthoring : MonoBehaviour
    {
        public float turnSpeed;
        
        public class RotationBaker : Baker<RotationAuthoring>
        {
            public override void Bake(RotationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(entity, new RotationComponent
                {
                    TurnSpeed = authoring.turnSpeed
                });
            }
        }
    }
}