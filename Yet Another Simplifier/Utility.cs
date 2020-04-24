using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yet_Another_Simplifier
{
    public static class Utility
    {
        public static decimal GreatestCommonDivisor(IEnumerable<decimal> numbers)
        {
            return numbers.Aggregate(GreatestCommonDivisor);
        }

        public static decimal GreatestCommonDivisor(decimal x, decimal y)
        {
            return y == 0 ? x : GreatestCommonDivisor(y, x % y);
        }
    }
}
