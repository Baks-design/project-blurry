using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine.Predicates;

namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}