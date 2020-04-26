using System.Text.RegularExpressions;
using Yet_Another_Simplifier.ResultProcessing;

namespace Yet_Another_Simplifier.Core
{
    public static class Syntax
    {
        public static SyntaxCheckResult CheckSyntax(char preceding, char current)
        {
            var currentType = GetType(current);

            if (currentType == '\0')
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = $"Unknown symbol: '{current}'"
                };
            }

            if (preceding == '\0')
            {
                return new SyntaxCheckResult
                {
                    Success = true
                };
            }

            var precedingType = GetType(preceding);
            
            if ((currentType == 'p' && precedingType != 'c') || (precedingType == 'p' && currentType != 'c'))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: point sign can go only after or before a digit"
                };
            }

            if (currentType == Const.Subtract && "=(".Contains(precedingType))
            {
                return new SyntaxCheckResult
                {
                    Success = true
                };
            }

            if (currentType == 'c' && "vr".Contains(precedingType))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: number cannot go after a variable letter or a right parenthesis"
                };
            }

            if (currentType == 'v' && "r".Contains(precedingType))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: variable letter cannot go after a variable letter or a right parenthesis"
                };
            }

            if (currentType == 'b' && "blem".Contains(precedingType))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: binary sign cannot go after a binary sign a left parenthesis or an equal sign"
                };
            }

            if (currentType == 'r' && "blem".Contains(precedingType))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: right parenthesis cannot go after a binary sign a left parenthesis or an equal sign"
                };
            }

            if (currentType == 'e' && "bem".Contains(precedingType))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: equal sign cannot go after a binary sign or an equal sign"
                };
            }

            if (currentType == 'm' && "bm".Contains(precedingType))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: minus sign cannot go after a binary sign"
                };
            }

            return new SyntaxCheckResult
            {
                Success = true
            };
        }

        private static char GetType(char character)
        {
            if (Const.Digits.Contains(character))
            {
                return 'c';
            }
            else if (Regex.IsMatch(character.ToString(),"[a-z]"))
            {
                return 'v';
            }
            else if(Const.ExclusivelyBinarySigns.Contains(character))
            {
                return 'b';
            }
            else if (character == Const.Subtract)
            {
                return 'm';
            }
            else if(character == Const.Equal)
            {
                return 'e';
            }
            else if(character == Const.LeftParenthesis)
            {
                return 'l';
            }
            else if(character == Const.RightParenthesis)
            {
                return 'r';
            }
            else
            {
                return '\0';
            }
        }
    }
}
