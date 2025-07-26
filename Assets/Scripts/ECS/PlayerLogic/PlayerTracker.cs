using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.PlayerLogic
{
    public class PlayerTracker : MonoBehaviour
    {
        private EntityQuery _playerQuery;
        private EntityManager _manager;

        private void Awake()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _playerQuery = _manager.CreateEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        }

        private void LateUpdate()
        {
            if (_playerQuery.CalculateEntityCount() == 0) return;

            var playerEntity = _playerQuery.GetSingletonEntity();
            var playerTransform = _manager.GetComponentData<LocalTransform>(playerEntity);

            transform.position = playerTransform.Position;
        }
    }
}