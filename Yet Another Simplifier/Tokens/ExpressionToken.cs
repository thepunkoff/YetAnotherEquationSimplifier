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

        public override void NegateValue()
        {
            foreach (var member in Members)
            {
                if (member is BinaryOperationToken)
                {
                    continue;
                }

                member.NegateValue();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var member in Members)
            {
                sb.Append(member.ToString());
            }

            return sb.ToString();
        }
    }
}
