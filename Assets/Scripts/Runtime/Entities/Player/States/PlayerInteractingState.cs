using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player.States
{
    public class PlayerInteractingState : IState
    {
        readonly PlayerMovementController controller;

        public PlayerInteractingState(PlayerMovementController controller) => this.controller = controller;

        public void Update()
        {
            Debug.Log("PlayerInteractionMovementState");
            controller.OnInteraction();
        }
    }
}