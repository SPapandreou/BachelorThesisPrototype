using ECS.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Movement.Systems
{
    public partial struct TeleportSystem : ISystem
    {
        private const float Margin = 0.01f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TeleportComponent>();
            state.RequireForUpdate<WorldBoundsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var bounds = SystemAPI.GetSingleton<WorldBoundsComponent>().Value;

            foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<TeleportComponent>())
            {
                transform.ValueRW.Position.x = ConvertX(transform.ValueRW.Position.x, bounds);
                transform.ValueRW.Position.y = ConvertY(transform.ValueRW.Position.y, bounds);
            }
        }

        private float ConvertX(float x, Rect bounds)
        {
            if (x < bounds.xMin)
            {
                return bounds.xMax - Margin;
            }

            if (x > bounds.xMax)
            {
                return bounds.xMin + Margin;
            }

            return x;
        }

        private float ConvertY(float y, Rect bounds)
        {
            if (y < bounds.yMin)
            {
                return bounds.yMax - Margin;
            }

            if (y > bounds.yMax)
            {
                return bounds.yMin + Margin;
            }

            return y;
        }
    }
}