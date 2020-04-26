using System.Collections.Generic;

namespace Yet_Another_Simplifier
{
    public static class Const
    {
        public static List<char> Parentheses = new List<char> { '(', ')' };

        public static char LeftParenthesis = '(';
        public static char RightParenthesis = ')';

        public static List<char> ExclusivelyBinarySigns = new List<char> { '+', '*', '/', '^'};

        public static char Add = '+';
        public static char Subtract= '-';
        public static char Multiply = '*';
        public static char Divide = '/';
        public static char Exponentiate = '^';
        public static char Equal = '=';

        public static string Digits = "1234567890";

        public static char Point = '.';
    }
}
