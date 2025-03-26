using System;
using UnityEngine;

namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine
{
    public abstract class StatefulEntity : MonoBehaviour
    {
        protected StateMachine stateMachine;

        protected virtual void Awake() => stateMachine = new StateMachine();

        protected virtual void Update() => stateMachine.Update();
        
        protected virtual void FixedUpdate() => stateMachine.FixedUpdate();

        protected void At<T>(IState from, IState to, Func<T> condition) => stateMachine.AddTransition(from, to, condition);

        protected void Any<T>(IState to, T condition) => stateMachine.AddAnyTransition(to, condition);
    }
}