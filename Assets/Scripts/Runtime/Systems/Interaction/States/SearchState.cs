using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class SearchState : IState
    {
        readonly InteractionController controller;

        public SearchState(InteractionController controller) => this.controller = controller;

        public void Update() => controller.CheckForInteractable();
    }
}