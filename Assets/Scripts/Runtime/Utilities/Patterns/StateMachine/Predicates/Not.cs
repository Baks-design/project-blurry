using UnityEngine;

namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine
{
    public class Not : IPredicate
    {
        [SerializeField, LabelWidth(80)] IPredicate rule;

        public bool Evaluate() => !rule.Evaluate();
    }
}