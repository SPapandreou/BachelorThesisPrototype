using OOP.Infrastructure.Pausing;
using OOP.PlayerLogic.Controller;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OOP.PlayerLogic
{
    public class PlayerScope : LifetimeScope
    {
        public Player playerPrefab;
        public Camera mainCamera;
        public CinemachineCamera cinemachineCamera;
        public PlayingField playingField;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.InstallPausing();
            
            builder.RegisterComponent(mainCamera);
            builder.RegisterComponent(cinemachineCamera);
            builder.RegisterComponent(playingField);
            builder.RegisterComponentInNewPrefab(playerPrefab, Lifetime.Singleton);
            
            builder.RegisterEntryPoint<Accelerator>();
            builder.RegisterEntryPoint<Rotator>();
            builder.RegisterEntryPoint<PlayerTeleporter>();
            
            builder.RegisterEntryPoint<PlayerInputListener>().AsSelf();
            builder.RegisterEntryPoint<CameraController>();
        }
        
        
    }
}