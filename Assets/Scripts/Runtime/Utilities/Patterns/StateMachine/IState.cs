﻿namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine
{
    public interface IState
    {
        void Update() { }
        void FixedUpdate() { }
        void OnEnter() { }
        void OnExit() { }
    }
}