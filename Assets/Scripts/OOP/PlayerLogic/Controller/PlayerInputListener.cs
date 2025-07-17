using System;
using Controller;
using R3;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace OOP.PlayerLogic.Controller
{
    public class PlayerInputListener : InputDefinition.IPlayerActions, IDisposable, IStartable
    {
        public Observable<IPlayerInputEvent> PlayerEvents => _playerEvents;
        private readonly Subject<IPlayerInputEvent> _playerEvents = new();
        
        private readonly InputDefinition _inputDefinition;

        public PlayerInputListener()
        {
            _inputDefinition = new InputDefinition();
            _inputDefinition.Player.AddCallbacks(this);
        }
        
        public void Start()
        {
            SetEnabled(true);
        }

        public void SetEnabled(bool enabled)
        {
            if (enabled)
            {
                _inputDefinition.Player.Enable();
                return;
            }
            
            _inputDefinition.Player.Disable();
        }
        
        public void OnShoot(InputAction.CallbackContext context)
        {
            if (context.performed) return;
            
            _playerEvents.OnNext(context.started ? ShootEvent.ShootingStarted : ShootEvent.ShootingCanceled);
        }

        public void OnThrust(InputAction.CallbackContext context)
        {
            if(context.performed) return;
            
            _playerEvents.OnNext(context.started ? ThrustEvent.ThrustingStarted : ThrustEvent.ThrustingCanceled);
        }

        public void Dispose()
        {
            _playerEvents.Dispose();
            _inputDefinition.Dispose();
        }
    }
}