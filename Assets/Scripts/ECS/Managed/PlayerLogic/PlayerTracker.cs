using ECS.Data.Input;
using Unity.Entities;
using UnityEngine;

namespace ECS.Managed.PlayerLogic
{
    public class PlayerTracker : MonoBehaviour
    {
        private EntityQuery _playerDataQuery;

        private void Awake()
        {
            _playerDataQuery =
                World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(PlayerData));
        }

        private void LateUpdate()
        {
            if (_playerDataQuery.CalculateEntityCount() == 0) return;

            transform.position = _playerDataQuery.GetSingleton<PlayerData>()
                .PlayerPosition;
        }
    }
}