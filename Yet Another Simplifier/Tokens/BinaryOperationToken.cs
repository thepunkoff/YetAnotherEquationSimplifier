namespace Yet_Another_Simplifier.Tokens
{
    public class BinaryOperationToken : Token
    {
        public override Token Clone()
        {
            return new BinaryOperationToken { Value = Value };
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
