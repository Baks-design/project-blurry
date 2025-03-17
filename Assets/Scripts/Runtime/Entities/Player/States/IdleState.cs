using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Entities.Player.States
{
    public class IdleState : IState
    {
        readonly PlayerMovementController controller;

        public IdleState(PlayerMovementController controller) => this.controller = controller;

        public void Update()
        {
            controller.AnimateMovement();
            controller.AnimateRotation();
        }
    }
}