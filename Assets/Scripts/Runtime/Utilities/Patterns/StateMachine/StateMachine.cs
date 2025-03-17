using System;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine.Predicates;

namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine
{
    public class StateMachine
    {
        StateNode _currentNode;
        readonly Dictionary<Type, StateNode> _nodes = new(); 
        readonly HashSet<Transition> _anyTransitions = new();

        public IState CurrentState => _currentNode.State;

        public void Update()
        {
            var transition = GetTransition();

            if (transition != null)
            {
                ChangeState(transition.To);

                // Reset action predicate flags for all transitions
                foreach (var node in _nodes.Values)
                    ResetActionPredicateFlags(node.Transitions);

                ResetActionPredicateFlags(_anyTransitions);
            }

            _currentNode.State?.Update();
        }

        static void ResetActionPredicateFlags(HashSet<Transition> transitions)
        {
            foreach (var transition in transitions)
                if (transition is Transition<ActionPredicate> actionTransition)
                    actionTransition.condition.flag = false;
        }

        public void FixedUpdate() => _currentNode.State?.FixedUpdate();

        public void SetState(IState state)
        {
            _currentNode = _nodes[state.GetType()];
            _currentNode.State?.OnEnter(); 
        }

        void ChangeState(IState state)
        {
            if (state == _currentNode.State) return; // Skip if the state is the same

            var previousState = _currentNode.State;
            var nextState = _nodes[state.GetType()].State;

            previousState?.OnExit(); 
            nextState.OnEnter();
            _currentNode = _nodes[state.GetType()]; // Update the current node
        }

        public void AddTransition<T>(IState from, IState to, T condition)
            => GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);

        public void AddAnyTransition<T>(IState to, T condition)
            => _anyTransitions.Add(new Transition<T>(GetOrAddNode(to).State, condition));

        Transition GetTransition()
        {
            // Check "any" transitions first
            foreach (var transition in _anyTransitions)
                if (transition.Evaluate())
                    return transition;

            // Check transitions from the current state
            foreach (var transition in _currentNode.Transitions)
                if (transition.Evaluate())
                    return transition;

            return null; 
        }

        StateNode GetOrAddNode(IState state)
        {
            if (!_nodes.TryGetValue(state.GetType(), out var node))
            {
                node = new StateNode(state); // Create a new node if it doesn't exist
                _nodes[state.GetType()] = node; // Add it to the dictionary
            }

            return node;
        }

        class StateNode
        {
            public IState State { get; } 
            public HashSet<Transition> Transitions { get; } 

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<Transition>(); 
            }

            public void AddTransition<T>(IState to, T predicate)
                => Transitions.Add(new Transition<T>(to, predicate));
        }
    }
}