using System;
using System.Collections.Generic;
using System.Text;

namespace Yet_Another_Simplifier.Tokens
{
    public interface IHasNumericValue
    {
        public decimal NumericValue { get; set; }
    }
}
