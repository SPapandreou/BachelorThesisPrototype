using ECS.Components.Input;
using ECS.Components.Movement;
using ECS.Components.PhysicsComponents;
using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Systems.Movement
{
    public partial struct PlayerMovement : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerData>();
            state.RequireForUpdate<PlayerInputData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerInputData = state.GetLatiosWorldUnmanaged().worldBlackboardEntity.GetComponentData<PlayerInputData>();
            state.CompleteDependency();
            
            var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
            
            var playerData = state.GetLatiosWorldUnmanaged().worldBlackboardEntity.GetComponentData<PlayerData>();
            var rigidBody = SystemAPI.GetComponent<RigidBodyData>(playerData.PlayerEntity);
            var thruster = SystemAPI.GetComponent<ThrusterData>(playerData.PlayerEntity);
            var transform = SystemAPI.GetComponent<WorldTransform>(playerData.PlayerEntity);

            if (playerInputData.IsThrusting)
            {
                var currentSpeed = math.length(rigidBody.Velocity.linear);
            
                if (currentSpeed >= thruster.MaxSpeed) return;

                var thrust = math.rotate(transform.rotation, math.right());
                thrust *= thruster.ThrustForce * deltaTime;
            
                UnitySim.ApplyFieldImpulse(ref rigidBody.Velocity, in rigidBody.Mass, thrust);    
            }
            
            var lookDirection = math.normalize(math.rotate(transform.worldTransform.rotation, math.right()));
            var direction = math.normalize(playerInputData.MouseInput - transform.worldTransform.position);
            //
            // float angle = math.acos(math.clamp(math.dot(lookDirection, direction), -1f, 1f));
            //
            // if (math.abs(angle) > 0.01f)
            // {
            //     float sign = math.sign(lookDirection.x * direction.y - lookDirection.y * direction.x);
            //     rigidBody.Velocity.angular.z = sign * angle * 5f;
            // }
            // else
            // {
            //     rigidBody.Velocity.angular.z = 0;
            // }
            
            float2 look = math.normalize(lookDirection.xy);
            float2 toTarget = math.normalize(direction.xy);

            float signedAngle = math.atan2(toTarget.y, toTarget.x) - math.atan2(look.y, look.x);

            signedAngle = math.atan2(math.sin(signedAngle), math.cos(signedAngle));
            
            if (math.abs(signedAngle) > 0.01f)
            {
                rigidBody.Velocity.angular.z = signedAngle * 5f;
            }
            else
            {
                rigidBody.Velocity.angular.z = 0f;
            }
            
            
            SystemAPI.SetComponent(playerData.PlayerEntity, rigidBody);
            
        }
    }
}