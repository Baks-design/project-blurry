using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class SearchState : IState
    {
        readonly InteractionController controller;

        public SearchState(InteractionController controller) => this.controller = controller;

        public void Update()
        {
            Debug.Log("Search State");
            controller.OnCheck();
        }
    }
}