using System;
using System.Collections.Generic;
using System.Text;
using Yet_Another_Simplifier.Tokens;

namespace Yet_Another_Simplifier.Tokens
{
    public abstract class ExpressionMemberToken : Token
    {
        public abstract ExpressionMemberPrecedence Precedence { get; }

        public abstract int CompareTo(ExpressionMemberToken other);
    }
}
