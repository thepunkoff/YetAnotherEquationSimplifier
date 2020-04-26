using System.Collections.Generic;
using Yet_Another_Simplifier.Interfaces;

namespace Yet_Another_Simplifier
{
    public class ExpressionMemberComparer : IComparer<IExpressionMemberComparable>
    {
        public int Compare(IExpressionMemberComparable x, IExpressionMemberComparable y)
        {
            return x.CompareTo(y);
        }
    }
}
