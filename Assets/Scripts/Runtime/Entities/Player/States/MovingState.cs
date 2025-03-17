using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Entities.Player.States
{
    public class MovingState : IState
    {
        readonly PlayerMovementController controller;

        public MovingState(PlayerMovementController controller) => this.controller = controller;

        public void Update()
        {
            controller.AnimateMovement();
            controller.AnimateRotation();
        }
    }
}