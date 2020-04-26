using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public class ConstantToken : ValueToken, IExpressionMemberComparable, IHasNumericValue, IEliminatable
    {
        public ConstantToken(decimal numericValue)
        {
            NumericValue = numericValue;
        }

        public decimal NumericValue { get; set; }

        public override Token Clone()
        {
            return new ConstantToken(NumericValue);
        }

        public int CompareTo(IExpressionMemberComparable other)
        {
            if (other is ConstantToken)
            {
                if (NumericValue > ((ConstantToken)other).NumericValue)
                {
                    return 1;
                }
                else if (NumericValue < ((ConstantToken)other).NumericValue)
                {
                    return -1;
                }
                else return 0;
            }

            if (other is VariableToken)
            {
                return -1;
            }

            if (other is FractionToken)
            {
                return -1;
            }

            throw new Exception("Unknown comparison type.");
        }

        public ValueToken Eliminate(decimal value)
        {
            return new ConstantToken(NumericValue / value);
        }

        public override decimal GreatestCommonDivisor()
        {
            if (NumericValue == 0) return 1;
            else return NumericValue < 0 ? NumericValue * -1 : NumericValue;
        }

        public override void Negate()
        {
            NumericValue *= -1;
        }

        public override string ToString()
        {
            return NumericValue.ToString();
        }
    }
}
