using Controller;
using ECS.Data.Input;
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

        private Entity _inputDataEntity;
        
        private InputDefinition _inputDefinition;

        private void Awake()
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _inputDataEntity = manager.CreateEntity();
            manager.AddComponent<PlayerInputData>(_inputDataEntity);
            
            
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
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _mouseInput = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            manager.SetComponentData(_inputDataEntity, new PlayerInputData
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