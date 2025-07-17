namespace OOP.PlayerLogic.Controller
{
    public readonly struct ShootEvent : IPlayerInputEvent
    {
        public static readonly ShootEvent ShootingStarted = new(true);
        public static readonly ShootEvent ShootingCanceled = new(false);

        private ShootEvent(bool isShooting) => IsShooting = isShooting;
        
        public readonly bool IsShooting;
        public string EventName => IsShooting ? "ShootingStarted" : "ShootingCanceled";
    }
}