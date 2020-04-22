using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier
{
    public struct Variable
    {
        public char Letter { get; set; }
        public double Exponent { get; set; }

        public Variable(char letter, double exponent)
        {
            Letter = letter;
            Exponent = exponent;
        }

        public override string ToString()
        {
            var exponent = Exponent == 1
                ? string.Empty
                : $"^{Exponent}";

            return $"{Letter}{exponent}";
        }
    }
}
