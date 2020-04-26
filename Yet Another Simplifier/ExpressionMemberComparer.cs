using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yet_Another_Simplifier.Tokens;

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
