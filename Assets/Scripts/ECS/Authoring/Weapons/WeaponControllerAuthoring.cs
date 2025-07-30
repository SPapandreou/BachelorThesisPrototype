using ECS.Data.Weapons;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Weapons
{
    public class WeaponControllerAuthoring : MonoBehaviour
    {
        public class WeaponControllerBaker : Baker<WeaponControllerAuthoring>
        {
            public override void Bake(WeaponControllerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<WeaponControllerData>(entity);
            }
        }
    }
}