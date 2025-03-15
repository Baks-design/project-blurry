namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}