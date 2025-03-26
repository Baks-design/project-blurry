using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class InteractionSearchState : IState
    {
        readonly InteractionController controller;

        public InteractionSearchState(InteractionController controller) => this.controller = controller;

        public void Update()
        {
            Debug.Log("SearchState");
            controller.OnCheck();
        }
    }
}