using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.PlayerLogic
{
    public partial struct PlayerSpawner : ISystem
    {
        private EntityQuery _query;

        public void OnCreate(ref SystemState state)
        {
            _query = state.GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>(), ComponentType.ReadOnly<Prefab>());
            state.RequireForUpdate(_query);
        }

        public void OnUpdate(ref SystemState state)
        {
            var prefab = _query.GetSingletonEntity();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var playerEntity = ecb.Instantiate(prefab);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            Debug.Log("Spawned Player");

            state.Enabled = false;
        }
    }
}