using ECS.Components;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Weapons
{
    public partial struct CooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
            
            state.Dependency = new UpdateCooldownsJob
            {
                DeltaTime = deltaTime
            }.ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(WeaponComponent))]
        public partial struct UpdateCooldownsJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;
            public void Execute(ref WeaponComponent component)
            {
                if (component.LastFired >= 0)
                {
                    component.LastFired += DeltaTime;
                }

                if (component.LastFired > component.Cooldown)
                {
                    component.LastFired = -1f;
                }
            }
        }
    }
}