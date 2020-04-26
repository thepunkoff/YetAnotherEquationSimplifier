namespace Yet_Another_Simplifier.Tokens
{
    public abstract class ValueToken : Token
    {
        public abstract decimal GreatestCommonDivisor();
        public abstract void Negate();
    }
}
