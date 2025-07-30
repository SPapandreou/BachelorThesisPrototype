using ECS.Data.Input;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Input
{
    public class PlayerDataAuthoring : MonoBehaviour
    {
        public GameObject playerObject;
        
        public class PlayerDataBaker : Baker<PlayerDataAuthoring>
        {
            public override void Bake(PlayerDataAuthoring authoring)
            {
                var playerEntity = GetEntity(authoring.playerObject, TransformUsageFlags.None);
                var entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new PlayerData
                {
                    PlayerEntity = playerEntity
                });

            }
        }
    }
}