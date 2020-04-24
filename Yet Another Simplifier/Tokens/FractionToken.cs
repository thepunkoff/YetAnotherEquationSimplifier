using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public class FractionToken : Token
    {
        public FractionToken(Token numerator, Token denominator)
        {
            var gcd = 

            Numerator = numerator;
            Denominator = denominator;
        }

        public Token Numerator { get; set; }
        public Token Denominator { get; set; }

        public override Token Clone()
        {
            return new FractionToken(Numerator.Clone(), Denominator.Clone());
        }

        public override decimal GreatestCommonDivisor()
        {
            return Utility.GreatestCommonDivisor(Numerator.GreatestCommonDivisor(), Denominator.GreatestCommonDivisor());
        }

        public override void NegateValue()
        {
            Numerator.NegateValue();
        }

        public override string ToString()
        {
            var numeratorString = string.Empty;
            var denominatorString = string.Empty;

            if (Numerator is ExpressionToken)
            {
                numeratorString = $"({Numerator.ToString()})";
            }
            else
            {
                numeratorString = Numerator.ToString();
            }

            if (Denominator is ExpressionToken)
            {
                denominatorString = $"({Denominator.ToString()})";
            }
            else
            {
                denominatorString = Denominator.ToString();
            }

            return $"[{numeratorString}/{denominatorString}]";

        }
    }
}
