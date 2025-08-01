using ECS.Data.Visuals;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems.Visuals
{
    [BurstCompile]
    public partial struct HitResponseProcessor : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HitResponseData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var data in SystemAPI.Query<RefRW<HitResponseData>>())
            {
                if (data.ValueRO.Value > 0f)
                {
                    data.ValueRW.Value -= 3f * state.WorldUnmanaged.Time.DeltaTime;
                }

                if (data.ValueRO.Value < 0f)
                {
                    data.ValueRW.Value = 0f;
                }
            }
        }
    }
}