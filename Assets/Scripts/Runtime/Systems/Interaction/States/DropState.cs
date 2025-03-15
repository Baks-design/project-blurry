using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class DropState : IState
    {
        readonly InteractionController controller;

        public DropState(InteractionController controller) => this.controller = controller;

        public void OnEnter() => controller.OnDrop();

        public void Update() => controller.UpdateDrop();
    }
}