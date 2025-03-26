using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player.States
{
    public class PlayerMovingState : IState
    {
        readonly PlayerMovementController controller;

        public PlayerMovingState(PlayerMovementController controller) => this.controller = controller;

        public void Update()
        {
            Debug.Log("PlayerMovingState");
            controller.OnMoving();
        }
    }
}