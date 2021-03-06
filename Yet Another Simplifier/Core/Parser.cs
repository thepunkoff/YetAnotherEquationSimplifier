﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Yet_Another_Simplifier.Interfaces;
using Yet_Another_Simplifier.Models;
using Yet_Another_Simplifier.ResultProcessing;
using Yet_Another_Simplifier.Tokens;

namespace Yet_Another_Simplifier.Core
{
    public class Parser
    {
        private string Input { get; set; }
        private int Pointer { get; set; }

        public Token LeftHandSide { get; set; }
        public Token RightHandSide { get; set; }

        private Precedence LastPrecedence { get
            {
                if (_parenthesesStack.Count == 0)
                {
                    if (_operationStack.Count != 0)
                    {
                        return GetPrecedence(_operationStack.Peek().Value[0]);
                    }
                    else
                    {
                        return Precedence.Default;
                    }
                }
                else
                {
                    if (_parenthesesStack.Count == 1)
                    {
                        return Precedence.Default;
                    }
                    else
                    {
                        return GetPrecedence(_parenthesesStack.Peek());
                    }
                }

            }
            set { } }
        private char LastCharacter { get; set; }
        public Parser(string input)
        {
            Input = input;
            Pointer = -1;
        }

        private Stack<char> _parenthesesStack = new Stack<char>();

        private Stack<ValueToken> _expressionStack = new Stack<ValueToken>();
        private Stack<BinaryOperationToken> _operationStack = new Stack<BinaryOperationToken>();

        private bool NegateFlag { get; set; }
        
        public OperationResult ParseAndSimplify()
        {
            while (Pointer + 1 <= Input.Length - 1)
            {
                Pointer++;

                var result = Syntax.CheckSyntax(LastCharacter, Input[Pointer]);

                if (!result.Success)
                {
                    return OperationResult.CreateFailure(result.ErrorMessage);
                }

                if (IsEqualSign()) return ParseLeftSide();
                else if (IsParenthesis()) return ParseParentheses(); 
                else if (IsUnaryMinus()) return ParseUnaryMinus();
                else if (IsBinarySign()) return ParseBinarySign();
                else if (IsVariable()) return ParseVariable();
                else if (IsConstant()) return ParseConstant();
                else return OperationResult.CreateFailure($"Invalid token at pos. {Pointer}");
            }

            return ParseRightSide();
        }

        private OperationResult ParseRightSide()
        {
            var unwind = UnwindStacks();

            if (unwind.Success)
            {
                RightHandSide = unwind.Result;
            }
            else
            {
                return OperationResult.CreateFailure(unwind.ErrorMessage);
            }

            if (LeftHandSide == null)
            {
                return OperationResult.CreateSuccess((ValueToken)RightHandSide);
            }
            else
            {
                var final = Simplifier.DoOperation(new BinaryOperationToken { Value = "-" }, (ValueToken)LeftHandSide, (ValueToken)RightHandSide);

                if (final.Success)
                {
                    var gcd = final.Result.GreatestCommonDivisor();

                    var answer = ((IEliminatable)final.Result).Eliminate(gcd);

                    answer = Simplifier.Order(answer);

                    if (answer.ToString() == "0")
                    {
                        return OperationResult.CreateFailure("That's right!");
                    }

                    return OperationResult.CreateSuccess(
                        new ExpressionToken(new List<Token> { answer, new BinaryOperationToken { Value = "=" }, new ConstantToken(0) })
                    );
                }
                else
                {
                    return OperationResult.CreateFailure(final.ErrorMessage);
                }
            }
        }

        private OperationResult ParseLeftSide()
        {
            if (LeftHandSide != null)
            {
                return OperationResult.CreateFailure("More than one equal sign in equation.");
            }

            var eq = UnwindStacks();

            if (eq.Success)
            {
                LeftHandSide = eq.Result;

                return ParseAndSimplify();
            }
            else
            {
                return OperationResult.CreateFailure(eq.ErrorMessage);
            }
        }

        private OperationResult ParseParentheses()
        {
            if (Input[Pointer] == Const.LeftParenthesis)
            {
                if (Const.Digits.Contains(LastCharacter) || Regex.IsMatch(LastCharacter.ToString(), "[a-z]") || LastCharacter == Const.RightParenthesis)
                {
                    _operationStack.Push(new BinaryOperationToken { Value = "*" });
                }

                _parenthesesStack.Push(Input[Pointer]);

                LastCharacter = Input[Pointer];

                return ParseAndSimplify();
            }
            else if (Input[Pointer] == Const.RightParenthesis)
            {
                _parenthesesStack.Push(Input[Pointer]);

                if (UnwindLastParentheses(out string errorMessage))
                {
                    return ParseAndSimplify();
                }
                else
                {
                    return OperationResult.CreateFailure(errorMessage);
                }
            }
            else
            {
                throw new Exception("Wrong parenthesis token. Check Const class.");
            }
        }

        private OperationResult ParseUnaryMinus()
        {
            NegateFlag = true;
            return ParseAndSimplify();
        }

        private OperationResult ParseBinarySign()
        {
            if (_parenthesesStack.Count > 0)
            {
                if (_parenthesesStack.Peek() == Const.LeftParenthesis)
                {
                    _parenthesesStack.Push(Input[Pointer]);

                    _operationStack.Push(new BinaryOperationToken { Value = Input[Pointer].ToString() });

                    return ParseAndSimplify();
                }
                else
                {
                    return CheckPrecedenceAndAssociativity();
                }
            }
            else
            {
                return CheckPrecedenceAndAssociativity();
            }
        }

        private OperationResult ParseVariable()
        {
            if (Const.Digits.Contains(LastCharacter) || Regex.IsMatch(LastCharacter.ToString(), "[a-z]") || LastCharacter == Const.RightParenthesis)
            {
                _operationStack.Push(new BinaryOperationToken { Value = "*" });
                if (_parenthesesStack.Count >= 1)
                {
                    _parenthesesStack.Push('*');
                }
            }

            LastCharacter = Input[Pointer];
            var token = new VariableToken(1, new List<Variable> { new Variable { Letter = Input[Pointer], Exponent = 1 } });

            if (NegateFlag && _parenthesesStack.Count == 0)
            {
                token.Negate();
                NegateFlag = false;
            }

            _expressionStack.Push(token);
            return ParseAndSimplify();
        }

        private OperationResult ParseConstant()
        {
            string value = null;
            var sb = new StringBuilder();
            string untrimmedFinal = null;

            var wasPoint = false;

            while (Const.Digits.Contains(Input[Pointer]) || Input[Pointer] == Const.Point)
            {
                if (Input[Pointer] == Const.Point)
                {
                    if (!wasPoint)
                    {
                        wasPoint = true;
                    }
                    else
                    {
                        return OperationResult.CreateFailure("A number cannot have multiple points.");
                    }
                }

                sb.Append(Input[Pointer]);

                if (Pointer + 1 >= Input.Length || (Input[Pointer + 1] != Const.Point && !Const.Digits.Contains(Input[Pointer + 1])))
                {
                    break;
                }
                else
                {
                    Pointer++;
                }
            }

            LastCharacter = Input[Pointer];

            untrimmedFinal = sb.ToString();

            value = untrimmedFinal == "0" ? untrimmedFinal : untrimmedFinal.TrimStart('0');

            var parsedDecimal = decimal.Parse(value);

            var token = new ConstantToken(parsedDecimal);

            if (NegateFlag && _parenthesesStack.Count == 0)
            {
                token.Negate();
                NegateFlag = false;
            }

            _expressionStack.Push(token);
            return ParseAndSimplify();
        }

        private OperationResult CheckPrecedenceAndAssociativity()
        {
            var precedence = GetPrecedence(Input[Pointer]);

            if (precedence > LastPrecedence)
            {
                _operationStack.Push(new BinaryOperationToken { Value = Input[Pointer].ToString() });

                if (_parenthesesStack.Count >= 1)
                {
                    _parenthesesStack.Push(Input[Pointer]);
                }

                return ParseAndSimplify();
            }
            else if (precedence < LastPrecedence)
            {
                return BinaryProceed();
            }
            else
            {
                if (GetAssociativity() == Associativity.Left)
                {
                    return BinaryProceed();
                }
                else
                {
                    _operationStack.Push(new BinaryOperationToken { Value = Input[Pointer].ToString() });
                    return ParseAndSimplify();
                }
            }
        }

        private bool UnwindLastParentheses(out string errorMessage)
        {
            while(_parenthesesStack.Peek() != Const.LeftParenthesis)
            {
                if (_parenthesesStack.Pop() == Const.RightParenthesis)
                {
                    continue;
                }

                if (!TryEvaluateLastBinaryOperation(out string error))
                {
                    errorMessage = error;

                    return false;
                }
            }

            _parenthesesStack.Pop();

            errorMessage = null;

            return true;
        }

        private Precedence GetPrecedence(char sign)
        {
            if (sign == Const.Add)
            {
                return Precedence.Add;
            }
            else if (sign == Const.Subtract)
            {
                return Precedence.Subtract;
            }
            else if (sign == Const.Multiply)
            {
                return Precedence.Multiply;
            }
            else if (sign == Const.Divide)
            {
                return Precedence.Divide;
            }
            else if (sign == Const.Exponentiate)
            {
                return Precedence.Exponentiate;
            }
            else
            {
                throw new Exception("Wrong binary sign token. Check Const class.");
            }
        }

        private Associativity GetAssociativity()
        {
            if (Input[Pointer] == Const.Add)
            {
                return Associativity.Left;
            }
            else if (Input[Pointer] == Const.Subtract)
            {
                return Associativity.Left;
            }
            else if (Input[Pointer] == Const.Multiply)
            {
                return Associativity.Left;
            }
            else if (Input[Pointer] == Const.Divide)
            {
                return Associativity.Left;
            }
            else if (Input[Pointer] == Const.Exponentiate)
            {
                return Associativity.Right;
            }
            else
            {
                throw new Exception("Wrong binary sign token. Check Const class.");
            }
        }

        private OperationResult BinaryProceed()
        {
            if (TryEvaluateLastBinaryOperation(out string errorMessage))
            {
                if (_parenthesesStack.Count > 1)
                {
                    _parenthesesStack.Pop();
                }

                Pointer--;

                LastCharacter = Input[Pointer];

                return ParseAndSimplify();
            }
            else
            {
                return OperationResult.CreateFailure(errorMessage);
            }
        }

        private bool TryEvaluateLastBinaryOperation(out string errorMessage)
        {
            if (_expressionStack.Count != _operationStack.Count + 1)
            {
                errorMessage = $"Invalid token at pos. {Pointer}";
                return false;
            }

            var right = _expressionStack.Pop();
            var left  = _expressionStack.Pop();

            ValueToken simplifiedResult = null;

            var op = Simplifier.DoOperation(_operationStack.Pop(), left, right);

            if (op.Success)
            {
                simplifiedResult = op.Result;
            }
            else
            {
                errorMessage = op.ErrorMessage;


                return false;
            }

            if (simplifiedResult == null)
            {
                throw new Exception("simplified result is null");
            }

            _expressionStack.Push(simplifiedResult);

            errorMessage = null;

            return true;
        }

        private OperationResult UnwindStacks()
        {
            if (_expressionStack.Count != _operationStack.Count + 1)
            {
                return OperationResult.CreateFailure($"Invalid token at pos. {Pointer}");
            }

            if (_expressionStack.Count == 0 && _operationStack.Count == 0)
            {
                return OperationResult.CreateFailure("Stacks were empty. Probable equal sign in the beginning of the input.");
            }

            if (_parenthesesStack.Count != 0)
            {
                return OperationResult.CreateFailure($"{_parenthesesStack.Count} parentheses weren't closed.");
            }

            while (_operationStack.Count > 0)
            {
                if (!TryEvaluateLastBinaryOperation(out string errorMessage))
                {
                    return OperationResult.CreateFailure(errorMessage);
                }
            }

            if (_expressionStack.Count != 1)
            {
                throw new Exception("Invalid expression count on the expression stack after final unwinding.");
            }

            return OperationResult.CreateSuccess(_expressionStack.Pop());
        }

        private bool IsVariable()
        {
            var stringRepresentation = Input[Pointer].ToString();

            if (Regex.IsMatch(stringRepresentation, "[a-z]"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsConstant()
        {
            

            if (Const.Digits.Contains(Input[Pointer]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsUnaryMinus()
        {
            if (Input[Pointer] == Const.Subtract && "\0=(".Contains(LastCharacter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsBinarySign()
        {
            if (Const.ExclusivelyBinarySigns.Contains(Input[Pointer]) || Input[Pointer] == Const.Subtract)
            {
                LastCharacter = Input[Pointer];

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsParenthesis()
        {
            if (Const.Parentheses.Contains(Input[Pointer]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsEqualSign()
        {
            if (Input[Pointer] == '=')
            {
                LastCharacter = Input[Pointer];

                return true; 
            }
            else
            {
                return false;
            }
        }
    }
}
