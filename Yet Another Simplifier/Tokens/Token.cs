using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public abstract class Token
    {
        public string Value { get; set; }

        public abstract Token Clone();

        public abstract void NegateValue();

        public abstract decimal GreatestCommonDivisor();
    }
}
