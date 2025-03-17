using System;

namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine.Predicates
{
    public class ActionPredicate : IPredicate
    {
        public bool flag;

        public ActionPredicate(ref Action eventReaction) => eventReaction += () => { flag = true; };

        public bool Evaluate()
        {
            var result = flag;
            flag = false;
            return result;
        }
    }
}