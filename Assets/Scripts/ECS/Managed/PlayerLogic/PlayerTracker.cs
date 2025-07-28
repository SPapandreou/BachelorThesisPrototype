using ECS.Components.Input;
using ECS.ECSExtensions;
using Unity.Entities;
using UnityEngine;

namespace ECS.Managed.PlayerLogic
{
    public class PlayerTracker : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (World.DefaultGameObjectInjectionWorld.HasBlackboardComponent<PlayerData>()) return;

            transform.position = World.DefaultGameObjectInjectionWorld.GetBlackboardComponent<PlayerData>()
                .PlayerPosition;
        }
    }
}