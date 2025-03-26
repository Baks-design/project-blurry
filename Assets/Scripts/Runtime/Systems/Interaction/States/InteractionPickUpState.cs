using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class InteractionPickUpState : IState
    {
        readonly InteractionController controller;

        public InteractionPickUpState(InteractionController controller) => this.controller = controller;

        public void Update()
        {
            Debug.Log("PickUpState State");
            controller.OnInteract();
        }
    }
}