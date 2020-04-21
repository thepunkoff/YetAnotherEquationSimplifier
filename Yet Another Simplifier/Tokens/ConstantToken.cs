using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public class ConstantToken : Token
    {
        public ConstantToken(double numericValue)
        {
            NumericValue = numericValue;
        }

        public double NumericValue { get; set; }

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
