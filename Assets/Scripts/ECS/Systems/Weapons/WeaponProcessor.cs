using ECS.Data.Particles;
using ECS.Data.Projectiles;
using ECS.Data.Weapons;
using ECS.Systems.Particles;
using ECS.Systems.Projectiles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Weapons
{
    [BurstCompile]
    [UpdateBefore(typeof(ProjectileProcessor))]
    public partial struct WeaponProcessor : ISystem
    {
        private ComponentLookup<WeaponControllerData> _controllerLookup;
        private ComponentLookup<LocalTransform> _transformLookup;
        private ParticleBufferSystem _bufferSystem;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponData>();
            state.RequireForUpdate<Parent>();
            state.RequireForUpdate<WeaponControllerData>();

            _controllerLookup = state.GetComponentLookup<WeaponControllerData>();
            _transformLookup = state.GetComponentLookup<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _controllerLookup.Update(ref state);
            _transformLookup.Update(ref state);

            var bufferSystemHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<ParticleBufferSystem>();
            ref var bufferSystem =
                ref state.WorldUnmanaged.GetUnsafeSystemRef<ParticleBufferSystem>(bufferSystemHandle);

            var deltaTime = state.WorldUnmanaged.Time.DeltaTime;

            state.Dependency = new ProcessWeaponsJob
            {
                DeltaTime = deltaTime,
                ParticleBuffer = bufferSystem.ParticleBuffer.AsParallelWriter(),
                ControllerLookup = _controllerLookup,
                TransformLookup = _transformLookup
            }.ScheduleParallel(state.Dependency);

            bufferSystem.Dependencies = JobHandle.CombineDependencies(bufferSystem.Dependencies, state.Dependency);
        }

        [BurstCompile]
        public partial struct ProcessWeaponsJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public ComponentLookup<WeaponControllerData> ControllerLookup;
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
            public NativeList<RawParticleSpawnData>.ParallelWriter ParticleBuffer;

            public void Execute(ref WeaponData weapon, in LocalTransform transform, in Parent parent)
            {
                if (weapon.LastFired >= 0)
                {
                    weapon.LastFired += DeltaTime;
                    if (weapon.LastFired > weapon.Cooldown)
                    {
                        weapon.LastFired = -1f;
                    }
                    else
                    {
                        return;
                    }
                }

                var controller = ControllerLookup[parent.Value];

                if (!controller.IsFiring) return;

                weapon.LastFired = 0f;

                var parentTransform = TransformLookup[parent.Value];
                var worldRotation = math.mul(parentTransform.Rotation, transform.Rotation);
                var forward = math.rotate(worldRotation, math.right());
                float angle = math.degrees(math.atan2(forward.y, forward.x));

                ParticleBuffer.AddNoResize(new RawParticleSpawnData
                {
                    ParticleType = ParticleType.Managed,
                    VfxId = weapon.ParticleHash,
                    Position = parentTransform.Position + math.rotate(parentTransform.Rotation, transform.Position),
                    Angle = angle,
                    Velocity = forward * weapon.ProjectileSpeed,
                    HitboxSize = weapon.HitboxSize,
                    Hitbox = weapon.Hitbox,
                    HitboxLayer = weapon.HitboxLayer,
                    Size = weapon.ParticleSize,
                    Damage = weapon.Damage,
                    CollisionLayer = weapon.CollisionLayer
                });
            }
        }
    }
}