using System;
using ECS.Data.ECSBridge;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Systems.ECSBridge
{
    public partial class PrefabRegistrySystem : SystemBase
    {
        public NativeHashMap<Hash128, Entity> Registry => _lookup;
        
        private NativeHashMap<Hash128, Entity> _lookup;
        private bool _initialized;
        private EntityQuery _query;

        public Entity GetEntity(Hash128 gameObjectId)
        {
            if (!_lookup.TryGetValue(gameObjectId, out var entity))
            {
                throw new InvalidOperationException("Prefab not registered in registry.");
            }

            return entity;
        }
        
        protected override void OnCreate()
        {
            _query = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabRegistryEntry>());
            RequireForUpdate(_query);
        }

        protected override void OnStartRunning()
        {
            if (_initialized) return;
            _initialized = true;
            
            var buffer = _query.GetSingletonBuffer<PrefabRegistryEntry>();
            if (buffer.Length == 0) return;
            
            _lookup = new NativeHashMap<Hash128, Entity>(buffer.Length, Allocator.Persistent);
            foreach (var entry in buffer)
            {
                _lookup[entry.GameObjectId] = entry.Entity;
            }
            
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            if (!_lookup.IsCreated) return;
            _lookup.Dispose();
        }
    }
}