using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier
{
    public struct Variable
    {
        public char Letter { get; set; }
        public decimal Exponent { get; set; }

        public Variable(char letter, decimal exponent)
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

        public override bool Equals(object obj)
        {
            if (!(obj is Variable v))
            {
                return false;
            }
            else
            {
                return v.Exponent == Exponent && v.Letter == Letter;
            }
        }
    }
}
