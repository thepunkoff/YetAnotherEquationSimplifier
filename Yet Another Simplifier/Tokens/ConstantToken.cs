using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public class ConstantToken : ExpressionMemberToken, IHasNumericValue
    {
        public ConstantToken(decimal numericValue)
        {
            NumericValue = numericValue;
        }

        public decimal NumericValue { get; set; }
        public override ExpressionMemberPrecedence Precedence { get => ExpressionMemberPrecedence.Constant; }

        public override Token Clone()
        {
            return new ConstantToken(NumericValue);
        }

        public override int CompareTo(ExpressionMemberToken other)
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

        public override decimal GreatestCommonDivisor()
        {
            return NumericValue;
        }

        public override void NegateValue()
        {
            NumericValue *= -1;
        }

        public override string ToString()
        {
            return NumericValue.ToString();
        }
    }
}
