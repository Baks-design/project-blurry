using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class SearchState : IState
    {
        readonly InteractionController controller;

        public SearchState(InteractionController controller)
        {
            this.controller = controller;
            Debug.Log("SaveState");
        }

        public void Update() => controller.CheckForInteractable();
    }
}