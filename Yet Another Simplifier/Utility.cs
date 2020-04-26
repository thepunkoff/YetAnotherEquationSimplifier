using System.Collections.Generic;
using System.Linq;

namespace Yet_Another_Simplifier
{
    public static class Utility
    {
        public static decimal GreatestCommonDivisor(IEnumerable<decimal> numbers)
        {
            return numbers.Select(x => x < 0 ? x * -1 : x).Aggregate(GreatestCommonDivisor);
        }

        public static decimal GreatestCommonDivisor(decimal x, decimal y)
        {
            if (x == 1 || y == 1) return 1;
            else return y == 0 ? x : GreatestCommonDivisor(y, x % y);
        }
    }
}
