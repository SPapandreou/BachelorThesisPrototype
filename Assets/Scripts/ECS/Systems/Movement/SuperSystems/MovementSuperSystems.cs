using Latios;
using Unity.Entities;
using Unity.Physics.Systems;

namespace ECS.Systems.Movement.SuperSystems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateBefore(typeof(PhysicsSimulationGroup))]
    public partial class MovementSuperSystem : SuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddUnmanagedSystem<LookAtPlayer>();
            GetOrCreateAndAddUnmanagedSystem<LookAt>();
            GetOrCreateAndAddUnmanagedSystem<ThrusterLogic>();
        }
    }
}