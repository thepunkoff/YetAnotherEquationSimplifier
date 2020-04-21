using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public class BinaryOperationToken : Token
    {
        public override void NegateValue()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
