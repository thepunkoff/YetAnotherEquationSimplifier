using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public class FractionToken : ExpressionMemberToken
    {
        public FractionToken(Token numerator, Token denominator)
        {
            var gcd = 

            Numerator = numerator;
            Denominator = denominator;
        }

        public Token Numerator { get; set; }
        public Token Denominator { get; set; }
        public override ExpressionMemberPrecedence Precedence { get => ExpressionMemberPrecedence.Fraction; }

        public override Token Clone()
        {
            return new FractionToken(Numerator.Clone(), Denominator.Clone());
        }

        public override int CompareTo(ExpressionMemberToken other)
        {
            if (other is ConstantToken)
            {
                return 1;
            }

            if (other is VariableToken)
            {
                return 1;
            }

            if (other is FractionToken f)
            {
                if (GetHashCode() > f.GetHashCode())
                {
                    return 1;
                }
                else if (GetHashCode() < f.GetHashCode())
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            throw new Exception("Unknown comparison type.");
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
            var numeratorString = Numerator.ToString();
            var denominatorString = Denominator.ToString();
            
            return $"[{numeratorString}/{denominatorString}]";
        }
    }
}
