namespace OOP.PlayerLogic.Controller
{
    public readonly struct ThrustEvent : IPlayerInputEvent
    {
        public static readonly ThrustEvent ThrustingStarted = new(true);
        public static readonly ThrustEvent ThrustingCanceled = new(false);

        private ThrustEvent(bool isThrusting) => IsThrusting = isThrusting;
        
        public readonly bool IsThrusting;
        public string EventName =>  IsThrusting ? "ThrustingStarted" : "ThrustingCanceled";
    }
}