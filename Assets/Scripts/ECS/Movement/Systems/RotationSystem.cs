using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Movement.Systems
{
    public partial class RotationSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<RotationComponent>();
            RequireForUpdate<LocalTransform>();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            var deltaTime = World.Time.DeltaTime;

            Entities.ForEach((ref LocalTransform transform, in RotationComponent rotation) =>
            {
                
                float3 direction = rotation.LookAt - transform.Position;

                if (math.lengthsq(direction) < 0.0001f)
                    return;

                direction = math.normalize(direction);

                // Compute target angle in radians for 2D rotation
                float targetAngle = math.atan2(direction.y, direction.x);

                // Extract current angle from quaternion assuming rotation around Z only
                float currentAngle = math.atan2(2f * (transform.Rotation.value.z * transform.Rotation.value.w),
                    1f - 2f * (transform.Rotation.value.z * transform.Rotation.value.z));

                // Compute shortest angular difference
                float deltaAngle = targetAngle - currentAngle;
                deltaAngle = math.atan2(math.sin(deltaAngle), math.cos(deltaAngle)); // Normalize between -π and π

                // Clamp rotation step
                float maxStep = rotation.TurnSpeed * deltaTime;
                float clampedDelta = math.clamp(deltaAngle, -maxStep, maxStep);

                // Apply rotation
                float newAngle = currentAngle + clampedDelta;
                transform.Rotation = quaternion.RotateZ(newAngle);
                
            }).ScheduleParallel();
        }
    }
}