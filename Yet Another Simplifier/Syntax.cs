﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Yet_Another_Simplifier
{
    public static class Syntax
    {
        public static SyntaxCheckResult CheckSyntax(char preceding, char current)
        {
            if (preceding == '\0')
            {
                return new SyntaxCheckResult
                {
                    Success = true
                };
            }

            preceding = GetType(preceding);
            current = GetType(current);

            if ((current == 'p' && preceding != 'c') || (preceding == 'p' && current != 'c'))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: point sign can go only after or before a digit"
                };
            }

            if (current == Const.Subtract && "=(".Contains(preceding))
            {
                return new SyntaxCheckResult
                {
                    Success = true
                };
            }

            if (current == 'c' && "vr".Contains(preceding))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: number cannot go after a variable letter or a right parenthesis"
                };
            }

            if (current == 'v' && "r".Contains(preceding))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: variable letter cannot go after a variable letter or a right parenthesis"
                };
            }

            if (current == 'b' && "blem".Contains(preceding))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: binary sign cannot go after a binary sign a left parenthesis or an equal sign"
                };
            }

            if (current == 'r' && "blem".Contains(preceding))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: right parenthesis cannot go after a binary sign a left parenthesis or an equal sign"
                };
            }

            if (current == 'e' && "bem".Contains(preceding))
            {
                return new SyntaxCheckResult
                {
                    ErrorMessage = "Invalid syntax: equal sign cannot go after a binary sign or an equal sign"
                };
            }

            if (current == 'm' && "bm".Contains(preceding))
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
                throw new Exception("Undefined character type.");
            }
        }
    }
}
