using System;
using Controller;
using ECS.Components.Input;
using Latios;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECS.Managed.Input
{
    public class InputListener : MonoBehaviour, InputDefinition.IPlayerActions
    {
        public Camera mainCamera;
        
        private bool _isShooting;
        private bool _isThrusting;
        private Vector3 _mouseInput;
        
        private InputDefinition _inputDefinition;

        private void Awake()
        {
            var latiosWorld = World.DefaultGameObjectInjectionWorld as LatiosWorld;
            latiosWorld!.worldBlackboardEntity.AddComponent<PlayerInputData>();
            
            _inputDefinition = new InputDefinition();
            _inputDefinition.Player.AddCallbacks(this);
        }

        private void OnEnable()
        {
            _inputDefinition.Player.Enable();
        }

        private void OnDisable()
        {
            _inputDefinition.Player.Disable();
        }

        private void Update()
        {
            _mouseInput = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            var latiosWorld = World.DefaultGameObjectInjectionWorld as LatiosWorld;
            latiosWorld!.worldBlackboardEntity.SetComponentData(new PlayerInputData
            {
                IsShooting = _isShooting,
                IsThrusting = _isThrusting,
                MouseInput = _mouseInput
            });
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
            
            if(context.canceled)
            {
                _isThrusting = false;
            }
        }
    }
}