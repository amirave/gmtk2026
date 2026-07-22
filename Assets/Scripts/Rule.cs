namespace Rule
{
    public enum RuleConcatinator
    {
        None,
        And,
        ButNot,
    }

    public class Rule : Property.Property
    {
        public Rule first;
        public RuleConcatinator concatinator;
        public Rule second;
    }
}