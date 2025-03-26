using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class InteractionDropState : IState
    {
        readonly InteractionController controller;

        public InteractionDropState(InteractionController controller) => this.controller = controller;

        public void Update()
        {
            Debug.Log("DropState State");
            controller.OnDrop();
        }
    }
}