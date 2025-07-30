using Unity.Entities;

namespace ECS.Data.Weapons
{
    public struct WeaponControllerData : IComponentData
    {
        public bool IsFiring;
    }
}