using OOP.Infrastructure.Pausing;
using OOP.PlayerLogic.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OOP.PlayerLogic
{
    public class PlayerScope : LifetimeScope
    {
        public Player playerPrefab;
        public Camera mainCamera;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.InstallPausing();
            
            builder.RegisterComponent(mainCamera);
            builder.RegisterComponentInNewPrefab(playerPrefab, Lifetime.Singleton);
            builder.RegisterEntryPoint<PlayerInputListener>().AsSelf();
            builder.RegisterEntryPoint<Accelerator>();
            builder.RegisterEntryPoint<Rotator>();
        }
        
        
    }
}