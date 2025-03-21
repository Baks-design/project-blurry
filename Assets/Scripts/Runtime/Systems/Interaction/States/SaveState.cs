using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction.States
{
    public class SaveState : IState
    {
        readonly InteractionController controller;

        public SaveState(InteractionController controller) => this.controller = controller;

        public void OnEnter()
        {
            controller.OnSave();
            Debug.Log("SaveState");
        }

        public void Update() => controller.UpdateSave();
    }
}