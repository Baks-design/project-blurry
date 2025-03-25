namespace Assets.Scripts.Runtime.Systems.EventBus.Events
{
    public struct LockPlayerMovementEvent : IEvent
    {
        public bool isMoveValid;
    }
}