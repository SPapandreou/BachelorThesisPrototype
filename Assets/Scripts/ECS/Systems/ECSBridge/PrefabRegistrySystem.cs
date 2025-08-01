using ECS.Data.ECSBridge;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Systems.ECSBridge
{
    public partial struct PrefabRegistrySystem : ISystem
    {
        private EntityQuery _query;

        public void OnCreate(ref SystemState state)
        {
            _query = state.GetEntityQuery(ComponentType.ReadOnly<PrefabRegistryEntry>());
            state.RequireForUpdate(_query);
        }

        public void OnUpdate(ref SystemState state)
        {
            var buffer = _query.GetSingletonBuffer<PrefabRegistryEntry>();
            NativeHashMap<Hash128, Entity> prefabRegistry =
                new NativeHashMap<Hash128, Entity>(128, Allocator.Persistent);

            foreach (var entry in buffer)
            {
                prefabRegistry[entry.GameObjectId] = entry.Entity;
            }

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<PrefabRegistryData>(entity);
            state.EntityManager.SetComponentData(entity, new PrefabRegistryData
            {
                PrefabRegistry = prefabRegistry
            });
            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {
            var registry = SystemAPI.GetSingleton<PrefabRegistryData>();
            registry.PrefabRegistry.Dispose();
        }
    }
}