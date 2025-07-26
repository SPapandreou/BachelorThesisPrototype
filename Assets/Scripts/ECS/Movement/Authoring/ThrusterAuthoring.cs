using ECS.Components;
using ECS.Movement.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.Movement.Authoring
{
    public class ThrusterAuthoring : MonoBehaviour
    {
        public float thrust;

        public class ThrusterBaker : Baker<ThrusterAuthoring>
        {
            public override void Bake(ThrusterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(entity, new ThrusterComponent
                {
                    Thrust = authoring.thrust
                });
            }
        }
    }
}