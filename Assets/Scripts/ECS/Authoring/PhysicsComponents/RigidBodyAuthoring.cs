using ECS.Components.PhysicsComponents;
using Latios.Transforms;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.PhysicsComponents
{
    public class RigidBodyAuthoring : MonoBehaviour
    {
        public float mass;
        public Vector3 staticStretch;
        public float linearDamping;
        public float angularDamping;
        public float friction;
        public float restitution;

        // public class RigidBodyBaker : Baker<RigidBodyAuthoring>
        // {
        //     public override void Bake(RigidBodyAuthoring authoring)
        //     {
        //         var qvvs = new TransformQvvs
        //         {
        //             position = authoring.transform.position,
        //             rotation = authoring.transform.rotation,
        //             stretch = authoring.transform.lossyScale,
        //             scale = 1f,
        //             worldIndex = 0
        //         };
        //
        //         var collider = GetComponent<BoxCollider>();
        //
        //         var latiosCollider = new Latios.Psyshock.BoxCollider
        //         {
        //             center = collider.center,
        //             halfSize = collider.size / 2f
        //         };
        //
        //         var data = RigidBodyData.CreateInBaking(qvvs, latiosCollider, 1f / authoring.mass,
        //             authoring.staticStretch, authoring.linearDamping, authoring.angularDamping, authoring.friction,  authoring.restitution);
        //         var entity = GetEntity(TransformUsageFlags.Dynamic);
        //         AddComponent(entity, data);
        //     }
        // }
    }
}