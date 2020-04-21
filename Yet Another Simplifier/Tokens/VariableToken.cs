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
    }
}
