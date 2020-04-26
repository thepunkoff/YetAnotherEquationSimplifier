using System.Text.RegularExpressions;

namespace Yet_Another_Simplifier.Extensions
{
    public static class StringExtensions
    {
        public static string BeautifyExpression(this string s)
        {
            s = Regex.Replace(s, @"\+-", "-");
            s = Regex.Replace(s, @"1([a-z]+(\^\d+)?)", "$1");

            return s;
        }
    }
}
