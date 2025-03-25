using UnityEngine;
using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class PickUpState : IState
    {
        readonly InteractionController controller;

        public PickUpState(InteractionController controller) => this.controller = controller;

        public void OnEnter() => controller.OnPickUp();

        public void Update()
        {
            Debug.Log("PickUp State");
            controller.OnPickUp();
        }
    }
}