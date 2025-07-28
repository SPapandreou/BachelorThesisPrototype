using Latios;
using Unity.Entities;

namespace ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class TestRootSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddManagedSystem<PausableSystems>();
        }
    }
}