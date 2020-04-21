using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public class VariableToken : Token
    {
        public VariableToken(double quotient, List<Variable> variables)
        {
            Quotient = quotient;
            Variables = variables;
        }

        public double Quotient { get; set; }
        public List<Variable> Variables { get; set; }

        public override void NegateValue()
        {
            Quotient *= -1;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Quotient == 1 ? string.Empty : Quotient.ToString());

            foreach (var variable in Variables)
            {
                sb.Append(variable.ToString());
            }

            return sb.ToString();
        }
    }
}
