using System;

namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine
{
    public abstract class Transition
    {
        public IState To { get; protected set; }
        public abstract bool Evaluate();
    }

    public class Transition<T> : Transition
    {
        public readonly T condition;

        public Transition(IState to, T condition)
        {
            To = to;
            this.condition = condition;
        }

        public override bool Evaluate()
        {
            if (condition is Func<bool> funcCondition)
                return funcCondition.Invoke();

            if (condition is ActionPredicate actionPredicateCondition)
                return actionPredicateCondition.Evaluate();

            if (condition is IPredicate predicateCondition)
                return predicateCondition.Evaluate();

            return false;
        }
    }
}