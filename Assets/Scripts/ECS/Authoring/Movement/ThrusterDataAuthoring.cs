using ECS.Components.Movement;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Movement
{
    public class ThrusterDataAuthoring : MonoBehaviour
    {
        public float thrustForce;
        public float maxSpeed;
        public float turnSpeed;

        public class ThrusterDataBaker : Baker<ThrusterDataAuthoring>
        {
            public override void Bake(ThrusterDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                
                AddComponent(entity, new ThrusterData
                {
                    ThrustForce = authoring.thrustForce,
                    MaxSpeed = authoring.maxSpeed,
                    TurnSpeed =  authoring.turnSpeed
                });
            }
        }

    }
}