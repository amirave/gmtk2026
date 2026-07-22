namespace Game
{
    public enum RuleConcatinator
    {
        None,
        And,
        ButNot,
    }

    public class Rule : Property
    {
        public Rule first;
        public RuleConcatinator concatinator;
        public Rule second;
    }
}