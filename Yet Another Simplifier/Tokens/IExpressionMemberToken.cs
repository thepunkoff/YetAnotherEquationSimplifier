﻿using System;
using System.Collections.Generic;
using System.Text;
using Yet_Another_Simplifier.Tokens;

namespace Yet_Another_Simplifier.Tokens
{
    public interface IExpressionMemberComparable
    {
        int CompareTo(IExpressionMemberComparable other);
    }
}
