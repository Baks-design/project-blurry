using Assets.Scripts.Runtime.Entities.Player;
using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class IdleState : IState
    {
        readonly PlayerMovementController controller;

        public IdleState(PlayerMovementController controller) => this.controller = controller;
    }
}