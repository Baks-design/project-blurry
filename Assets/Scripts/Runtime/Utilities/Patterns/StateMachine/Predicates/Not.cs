using UnityEngine;

namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine.Predicates
{
    public class Not : IPredicate
    {
        [SerializeField, LabelWidth(80f)] IPredicate rule;

        public bool Evaluate() => !rule.Evaluate();
    }
}