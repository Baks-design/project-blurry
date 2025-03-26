namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public interface IInteractionController
    {
        void SetInteractionMessage(string message);
        void ToggleCrosshair(bool visible);
    }
}