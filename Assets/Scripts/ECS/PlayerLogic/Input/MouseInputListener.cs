using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.PlayerLogic.Input
{
    public class MouseInputListener : MonoBehaviour
    {
        public Camera mainCamera;

        private EntityQuery _playerQuery;
        private EntityManager _manager;

        private void Awake()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _playerQuery = _manager.CreateEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        }

        private void Update()
        {
            if (_playerQuery.CalculateEntityCount() == 0) return;

            var playerEntity = _playerQuery.GetSingletonEntity();
            var rotationComponent = _manager.GetComponentData<RotationComponent>(playerEntity);
            
            var target = mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            rotationComponent.LookAt = target;
            
            _manager.SetComponentData(playerEntity, rotationComponent);
        }
    }
}