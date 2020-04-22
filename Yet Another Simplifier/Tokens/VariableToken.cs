using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Yet_Another_Simplifier.Tokens
{
    public class VariableToken : Token
    {
        public VariableToken(decimal quotient, List<Variable> variables)
        {
            Quotient = quotient;
            Variables = variables;
        }

        public decimal Quotient { get; set; }
        public List<Variable> Variables { get; set; }

        public override void NegateValue()
        {
            Quotient *= -1;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Quotient == 1)
            {
                sb.Append(string.Empty);
            }
            else if (Quotient == -1)
            {
                sb.Append("-");
            }
            else
            {
                sb.Append(Quotient.ToString());
            }

            Variables = Variables.OrderBy(x => x.Letter).ToList();

            foreach (var variable in Variables)
            {
                sb.Append(variable.ToString());
            }

            var result = sb.ToString();

            return result;
        }
    }
}
