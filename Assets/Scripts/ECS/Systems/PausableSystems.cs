using ECS.Systems.Movement;
using ECS.Systems.PhysicsSystems;
using ECS.Systems.PlayerSystems;
using Latios;

namespace ECS.Systems
{
    public partial class PausableSystems : SuperSystem
    {
        protected override void CreateSystems()
        {
            //GetOrCreateAndAddUnmanagedSystem<TestSystem>();
            GetOrCreateAndAddUnmanagedSystem<PlayerMovement>();
            GetOrCreateAndAddUnmanagedSystem<PhysicsSolver>();
            GetOrCreateAndAddUnmanagedSystem<PhysicsIntegration>();
            GetOrCreateAndAddUnmanagedSystem<UpdatePlayerPosition>();
            GetOrCreateAndAddUnmanagedSystem<PlayingFieldWrap>();
        }
    }
}