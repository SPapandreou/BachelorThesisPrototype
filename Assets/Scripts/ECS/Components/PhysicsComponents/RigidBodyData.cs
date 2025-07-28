using Latios.Psyshock;
using Latios.Transforms;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.PhysicsComponents
{
    public struct RigidBodyData : IComponentData
    {
        public UnitySim.Velocity Velocity;
        public UnitySim.Mass Mass;
        public RigidTransform InertialPoseWorldTransform;
        public float LinearDamping;
        public float AngularDamping;
        public float Friction;
        public float Restitution;
        public UnitySim.MotionExpansion MotionExpansion;
        
        

        public static RigidBodyData CreateInBaking(TransformQvvs worldTransform, Collider collider, float inverseMass,
            float3 staticStretch, float linearDamping, float angularDamping, float  friction, float restitution)
        {
            var localInertiaTensorDiagonal = UnitySim.LocalInertiaTensorFrom(in collider, staticStretch);
            var centerOfMass = UnitySim.LocalCenterOfMassFrom(in collider);
            UnitySim.ConvertToWorldMassInertia(worldTransform, localInertiaTensorDiagonal, centerOfMass, inverseMass,
                out var mass, out var inertialPoseWorldTransform);

            return new RigidBodyData
            {
                Velocity = default,
                Mass = mass,
                InertialPoseWorldTransform = inertialPoseWorldTransform,
                LinearDamping = linearDamping,
                AngularDamping = angularDamping,
                Friction = friction,
                Restitution = restitution
            };
        }
    }
}