using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player.States
{
    public class PlayerIdlingState : IState
    {
        readonly PlayerMovementController controller;

        public PlayerIdlingState(PlayerMovementController controller) => this.controller = controller;

        public void Update()
        {
            Debug.Log("PlayerIdlingState");
            controller.OnIdling();
        }
    }
}