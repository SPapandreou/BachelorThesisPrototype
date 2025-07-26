using Controller;
using ECS.Components;
using ECS.Movement.Components;
using ECS.Weapons;
using Unity.Entities;
using UnityEngine.InputSystem;

namespace ECS.PlayerLogic.Input
{
    public partial class PlayerInputSystem : SystemBase, InputDefinition.IPlayerActions
    {
        private InputDefinition _inputDefinition;
        private bool _isShooting;
        private bool _isThrusting;

        protected override void OnCreate()
        {
            RequireForUpdate<PlayerComponent>();
            
            _inputDefinition = new InputDefinition();
            _inputDefinition.Player.AddCallbacks(this);
        }

        protected override void OnStartRunning()
        {
            _inputDefinition.Player.Enable();
        }

        protected override void OnStopRunning()
        {
            _inputDefinition.Player.Disable();
        }


        protected override void OnUpdate()
        {
            var isThrusting = _isThrusting;
            var isShooting = _isShooting;
            
            Entities.WithAll<PlayerComponent>().ForEach((ref ThrusterComponent thrusterComponent, ref WeaponControllerComponent controller) =>
            {
                thrusterComponent.IsThrusting = isThrusting;
                controller.IsShooting = isShooting;
            }).Schedule();
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isShooting = true;
                return;
            }

            if (context.canceled)
            {
                _isShooting = false;
            }
        }

        public void OnThrust(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isThrusting = true;
                return;
            }

            if (context.canceled)
            {
                _isThrusting = false;
            }
        }
    }
}