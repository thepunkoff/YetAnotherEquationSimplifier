using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public class ExpressionToken : Token
    {
        public ExpressionToken(List<Token> members)
        {
            Members = members;
        }

        public List<Token> Members { get; set; }
    }
}
