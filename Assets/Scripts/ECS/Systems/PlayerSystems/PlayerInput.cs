using ECS.Data.Input;
using ECS.Data.Movement;
using ECS.Data.Weapons;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems.PlayerSystems
{
    [BurstCompile]
    public partial struct PlayerInput : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputData>();
            state.RequireForUpdate<PlayerData>();
            state.RequireForUpdate<ThrusterData>();
            state.RequireForUpdate<WeaponControllerData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerEntity = SystemAPI.GetSingleton<PlayerData>().PlayerEntity;
            var input = SystemAPI.GetSingleton<PlayerInputData>();

            var thrusterData = SystemAPI.GetComponent<ThrusterData>(playerEntity);
            var lookAtData = SystemAPI.GetComponent<LookAtData>(playerEntity);
            var weaponController = SystemAPI.GetComponent<WeaponControllerData>(playerEntity);

            thrusterData.IsFiring = input.IsThrusting;
            lookAtData.Target.xy = input.MouseInput.xy;
            weaponController.IsFiring = input.IsShooting;

            SystemAPI.SetComponent(playerEntity, thrusterData);
            SystemAPI.SetComponent(playerEntity, lookAtData);
            SystemAPI.SetComponent(playerEntity, weaponController);
        }
    }
}