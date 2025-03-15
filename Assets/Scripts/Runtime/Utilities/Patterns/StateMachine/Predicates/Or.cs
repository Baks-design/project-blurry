using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Utilities.Patterns.StateMachine
{
    public class Or : IPredicate
    {
        [SerializeField] List<IPredicate> rules = new();

        public bool Evaluate()
        {
            foreach (var rule in rules)
                if (rule.Evaluate())
                    return true;
            return false;
        }
    }
}