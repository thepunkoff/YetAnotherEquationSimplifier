﻿namespace Yet_Another_Simplifier.Tokens
{
    public abstract class Token
    {
        public string Value { get; set; }
        public abstract Token Clone();
    }
}
