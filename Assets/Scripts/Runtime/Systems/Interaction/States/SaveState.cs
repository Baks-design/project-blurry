using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class SaveState : IState
    {
        readonly InteractionController controller;

        public SaveState(InteractionController controller) => this.controller = controller;

        public void OnEnter() => controller.OnSave();

        public void Update() => controller.UpdateSave();
    }
}