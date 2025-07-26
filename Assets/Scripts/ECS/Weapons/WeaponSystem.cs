using ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Weapons
{
    public partial struct WeaponSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<WeaponControllerComponent>();
            state.RequireForUpdate<WeaponComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var parentLookup = SystemAPI.GetComponentLookup<Parent>(true);
            var controllerLookup = SystemAPI.GetComponentLookup<WeaponControllerComponent>(true);
            
            state.Dependency = new FireWeaponsJob
            {
                Ecb = ecb,
                ParentLookup = parentLookup,
                ControllerLookup = controllerLookup
            }.ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(WeaponComponent))]
        public partial struct FireWeaponsJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<Parent> ParentLookup;
            [ReadOnly] public ComponentLookup<WeaponControllerComponent> ControllerLookup;
            public EntityCommandBuffer.ParallelWriter Ecb;


            public void Execute([EntityIndexInQuery] int entityIndex, Entity entity, ref WeaponComponent weapon,
                in LocalToWorld transform)
            {
                
                if (!ParentLookup.TryGetComponent(entity, out var parent))
                {
                    return;
                }

                if (!ControllerLookup.TryGetComponent(parent.Value, out var controller))
                {
                    return;
                }

                if (!controller.IsShooting)
                {
                    return;
                }

                if (weapon.LastFired >= 0)
                {
                    return;
                }

                Entity projectile = Ecb.Instantiate(entityIndex, weapon.ProjectilePrefab);
                var projectileTransform = LocalTransform.FromPositionRotation(transform.Position, transform.Rotation);
                var direction = math.rotate(transform.Rotation, math.right());
                direction *= weapon.ProjectileSpeed;
                
                Ecb.SetComponent(entityIndex, projectile, projectileTransform);
                Ecb.SetComponent(entityIndex, projectile, new VelocityComponent
                {
                    Velocity = direction,
                    Drag = 0f,
                    MaxSpeed = weapon.ProjectileSpeed
                });
                weapon.LastFired = 0;
            }
        }
    }
}