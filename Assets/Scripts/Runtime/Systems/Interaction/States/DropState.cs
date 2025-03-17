using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class DropState : IState
    {
        readonly InteractionController controller;

        public DropState(InteractionController controller) => this.controller = controller;

        public void OnEnter()
        {
            controller.OnDrop();
            Debug.Log("DropState");
        }

        public void Update() => controller.UpdateDrop();
    }
}