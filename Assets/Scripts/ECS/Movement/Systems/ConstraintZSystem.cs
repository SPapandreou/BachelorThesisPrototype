using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace ECS.Movement.Systems
{
    public partial struct ConstraintZSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsMass>();
            state.RequireForUpdate<LocalTransform>();
        }
        
        
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new ConstraintZJob().ScheduleParallel(state.Dependency);
        }


        [WithAll(typeof(LocalTransform), typeof(PhysicsMass))]
        public partial struct ConstraintZJob : IJobEntity
        {
            public void Execute(ref LocalTransform transform)
            {
                transform.Position.z = 0;
            }
        }
    }
}