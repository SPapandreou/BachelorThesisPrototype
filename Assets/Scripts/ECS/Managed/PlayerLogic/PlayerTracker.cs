using ECS.Components.Input;
using Latios;
using Unity.Entities;
using UnityEngine;

namespace ECS.Managed.PlayerLogic
{
    public class PlayerTracker : MonoBehaviour
    {
        private void LateUpdate()
        {
            var world = World.DefaultGameObjectInjectionWorld as LatiosWorld;
            if (!world!.worldBlackboardEntity.HasComponent<PlayerData>()) return;
            
            transform.position = world!.worldBlackboardEntity.GetComponentData<PlayerData>().PlayerPosition;
        }
    }
}