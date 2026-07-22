using System;

namespace Game
{
    public enum RuleConcatinator
    {
        None,
        And,
        ButNot,
    }

    [Serializable]
    public class Rule
    {
        public Property property;
    }
}