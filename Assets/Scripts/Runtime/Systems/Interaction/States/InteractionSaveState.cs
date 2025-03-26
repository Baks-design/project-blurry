using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class InteractionSaveState : IState
    {
        readonly InteractionController controller;

        public InteractionSaveState(InteractionController controller) => this.controller = controller;

        public void Update()
        {
            Debug.Log("SaveState State");
            controller.OnSave();
        }
    }
}