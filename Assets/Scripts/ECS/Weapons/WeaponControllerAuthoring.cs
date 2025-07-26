using Unity.Entities;
using UnityEngine;

namespace ECS.Weapons
{
    public class WeaponControllerAuthoring : MonoBehaviour
    {
        public class WeaponControllerBaker : Baker<WeaponControllerAuthoring>
        {
            public override void Bake(WeaponControllerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent<WeaponControllerComponent>(entity);
            }
        }
    }
}