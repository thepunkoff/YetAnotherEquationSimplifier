using System;
using Yet_Another_Simplifier.Interfaces;

namespace Yet_Another_Simplifier.Tokens
{
    public class FractionToken : ValueToken, IExpressionMemberComparable, IEliminatable
    {
        public FractionToken(ValueToken numerator, ValueToken denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public ValueToken Numerator { get; set; }
        public ValueToken Denominator { get; set; }

        public override Token Clone()
        {
            return new FractionToken((ValueToken)Numerator.Clone(), (ValueToken)Denominator.Clone());
        }

        public int CompareTo(IExpressionMemberComparable other)
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

        public ValueToken Eliminate(decimal value)
        {
            if (value == 1)
            {
                return this;
            }
            else
            {
                var newNum = ((IEliminatable)Numerator).Eliminate(value);
                var newDenom = ((IEliminatable)Denominator).Eliminate(value);

                if (newDenom is ConstantToken c)
                {
                    if (c.NumericValue == 1)
                    {
                        return newNum;
                    }
                }

                return new FractionToken(newNum, newDenom);
            }
        }

        public override  decimal GreatestCommonDivisor()
        {
            return Utility.GreatestCommonDivisor(Numerator.GreatestCommonDivisor(), Denominator.GreatestCommonDivisor());
        }

        public override void Negate()
        {
            Numerator.Negate();
        }

        public override string ToString()
        {
            var numeratorString = Numerator.ToString();
            var denominatorString = Denominator.ToString();
            
            return $"[{numeratorString}/{denominatorString}]";
        }
    }
}
