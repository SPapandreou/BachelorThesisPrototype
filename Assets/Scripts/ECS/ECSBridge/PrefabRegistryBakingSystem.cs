using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ECS.ECSBridge
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(BakingSystemGroup))]
    public partial class PrefabRegistryBakingSystem : SystemBase
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(ComponentType.ReadWrite<PrefabRegistryComponent>(),
                ComponentType.ReadOnly<Prefab>());
            RequireForUpdate(_query);
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entities = _query.ToEntityArray(Allocator.TempJob);
            var components = _query.ToComponentDataArray<PrefabRegistryComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var registryEntity = ecb.CreateEntity();
                ecb.AddComponent(registryEntity, components[i]);
                ecb.RemoveComponent<PrefabRegistryComponent>(entities[i]);
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
            entities.Dispose();
            components.Dispose();
        }
    }
}