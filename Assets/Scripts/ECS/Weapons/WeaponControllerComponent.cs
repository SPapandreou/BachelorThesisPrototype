using Unity.Entities;

namespace ECS.Weapons
{
    public struct WeaponControllerComponent : IComponentData
    {
        public bool IsShooting;
    }
}