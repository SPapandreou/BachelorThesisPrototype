using ECS.Components.PhysicsComponents;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems.PhysicsSystems
{
    [BurstCompile]
    public partial struct PhysicsIntegration : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RigidBodyData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = state.WorldUnmanaged.Time.DeltaTime;
            
            state.Dependency = new SimJob{DeltaTime = deltaTime}.Schedule(state.Dependency);
        }

        [BurstCompile]
        public partial struct SimJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;

            public void Execute(TransformAspect transform, ref RigidBodyData rigidBody)
            {
                var oldInertialPoseWorldTransform = rigidBody.InertialPoseWorldTransform;
                rigidBody.Velocity.linear.z = 0f;
                rigidBody.Velocity.angular.xy = 0f;
                
                UnitySim.Integrate(ref rigidBody.InertialPoseWorldTransform, ref rigidBody.Velocity,
                    rigidBody.LinearDamping, rigidBody.AngularDamping, DeltaTime);

                rigidBody.InertialPoseWorldTransform.pos.z = 0f;
                var angle = math.atan2(rigidBody.InertialPoseWorldTransform.rot.value.z, rigidBody.InertialPoseWorldTransform.rot.value.w) * 2f;
                rigidBody.InertialPoseWorldTransform.rot = quaternion.RotateZ(angle);

                transform.worldTransform = UnitySim.ApplyInertialPoseWorldTransformDeltaToWorldTransform(
                    transform.worldTransform, oldInertialPoseWorldTransform, rigidBody.InertialPoseWorldTransform);
                
            }
        }
    }
}