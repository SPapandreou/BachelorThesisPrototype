using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace ECS.Movement.Baking
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(BakingSystemGroup))]
    public partial class PhysicsMassBakingSystem : SystemBase
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            Debug.Log("On Create");
        }


        protected override void OnUpdate()
        {
            Debug.Log("Updating");
            Entities.WithEntityQueryOptions(EntityQueryOptions.IncludePrefab).ForEach((ref PhysicsMass mass) =>
            {
                Debug.Log("In Job");
                mass.InverseInertia.x = 0f;
                mass.InverseInertia.y = 0f;
            }).Run();
        }
    }
}