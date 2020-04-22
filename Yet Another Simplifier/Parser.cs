using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Yet_Another_Simplifier.Tokens;

namespace Yet_Another_Simplifier
{
    public class Parser
    {
        private string Input { get; set; }
        private int Pointer { get; set; }
        

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

        private Stack<Token> _expressionStack = new Stack<Token>();
        private Stack<Token> _operationStack = new Stack<Token>();

        private bool NegateFlag { get; set; }

        public Token ParseAndSimplify()
        {
            while (Pointer + 1 <= Input.Length - 1)
            {
                Pointer++;

                var result = Syntax.CheckSyntax(LastCharacter, Input[Pointer]);

                if (!result.Success)
                {
                    Console.WriteLine(result.ErrorMessage);
                    return null;
                }
                if (ParseEqualSign())
                {
                    return UnwindStacks();
                }
                else if (ParseParentheses())
                {
                    if (Input[Pointer] == Const.LeftParenthesis)
                    {
                        if (Const.Digits.Contains(LastCharacter) || Regex.IsMatch(LastCharacter.ToString(), "[a-z]")|| LastCharacter == Const.RightParenthesis)
                        {
                            _operationStack.Push(new BinaryOperationToken { Value = "*"});
                            //LastPrecedence = Precedence.Multiply;
                        }

                        _parenthesesStack.Push(Input[Pointer]);

                        LastCharacter = Input[Pointer];
                        //LastPrecedence = Precedence.Default;

                        return ParseAndSimplify();
                    }
                    else if (Input[Pointer] == Const.RightParenthesis)
                    {
                        _parenthesesStack.Push(Input[Pointer]);

                        //LastPrecedence = GetPrecedence(_operationStack.Peek().Value[0]);

                        if (UnwindLastParentheses())
                        {
                            return ParseAndSimplify();
                        }

                        throw new Exception("Something's wrong with the parenteses stack. Take a look!");
                    }
                    else
                    {
                        throw new Exception("Wrong parenthesis token. Check Const class.");
                    }
                }
                else if (ParseUnaryMinus())
                {
                    NegateFlag = true;
                    return ParseAndSimplify();
                }
                else if (ParseBinarySign())
                {
                    if (_parenthesesStack.Count > 0)
                    {
                        if (_parenthesesStack.Count == 1)
                        {
                            _parenthesesStack.Push(Input[Pointer]);

                            _operationStack.Push(new BinaryOperationToken { Value = Input[Pointer].ToString() });

                            //LastPrecedence = GetPrecedence(Input[Pointer]);

                            return ParseAndSimplify();
                        }
                        if (_parenthesesStack.Count >= 2)
                        {
                            return CheckPrecedenceAndAssociativity();
                        }
                    }
                    else
                    {
                        if (LastPrecedence == Precedence.Default)
                        {
                            _operationStack.Push(new BinaryOperationToken { Value = Input[Pointer].ToString() });

                            //LastPrecedence = GetPrecedence(Input[Pointer]);

                            return ParseAndSimplify();
                        }
                        else
                        {
                            return CheckPrecedenceAndAssociativity();
                        }
                    }
                }
                else if (TryParseVariable())
                {
                    if (Const.Digits.Contains(LastCharacter) || Regex.IsMatch(LastCharacter.ToString(), "[a-z]") || LastCharacter == Const.RightParenthesis)
                    {
                        _operationStack.Push(new BinaryOperationToken { Value = "*" });
                        if (_parenthesesStack.Count >= 1)
                        {
                            _parenthesesStack.Push('*');
                        }
                        //LastPrecedence = Precedence.Multiply;
                    }

                    LastCharacter = Input[Pointer];
                    var token = new VariableToken(1, new List<Variable> { new Variable { Letter = Input[Pointer], Exponent = 1 } });

                    if (NegateFlag && _parenthesesStack.Count == 0)
                    {
                        token.NegateValue();
                        NegateFlag = false;
                    }

                    _expressionStack.Push(token);
                    return ParseAndSimplify();
                }
                else if (TryParseConstant(out string value))
                {
                    var parsedDouble = double.Parse(value);

                    var token = new ConstantToken(parsedDouble);

                    if (NegateFlag && _parenthesesStack.Count == 0)
                    {
                        token.NegateValue();
                        NegateFlag = false;
                    }

                    _expressionStack.Push(token);
                    return ParseAndSimplify();
                }
                else
                {
                    Console.WriteLine($"Invalid token at pos. {Pointer}");
                    return null;
                }
            }

            return UnwindStacks();
        }

        private Token CheckPrecedenceAndAssociativity()
        {
            var precedence = GetPrecedence(Input[Pointer]);

            if (_parenthesesStack.Count >= 1)
            {
                _parenthesesStack.Push(Input[Pointer]);
            }

            if (precedence > LastPrecedence)
            {
                _operationStack.Push(new BinaryOperationToken { Value = Input[Pointer].ToString() });

                //LastPrecedence = precedence;

                return ParseAndSimplify();
            }
            else if (precedence < LastPrecedence)
            {
                //LastPrecedence = precedence;
                return BinaryProceed();
            }
            else
            {
                if (GetAssociativity() == Associativity.Left)
                {
                    //LastPrecedence = precedence;
                    return BinaryProceed();
                }
                else
                {
                    //LastPrecedence = precedence;

                    _operationStack.Push(new BinaryOperationToken { Value = Input[Pointer].ToString() });
                    return ParseAndSimplify();
                }
            }
        }

        private bool UnwindLastParentheses()
        {
            while(_parenthesesStack.Peek() != Const.LeftParenthesis)
            {
                if (_parenthesesStack.Pop() == Const.RightParenthesis)
                {
                    continue;
                }

                if (!TryEvaluateLastBinaryOperation())
                {
                    return false;
                }
            }

            _parenthesesStack.Pop();

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

        private Token BinaryProceed()
        {
            if (TryEvaluateLastBinaryOperation())
            {
                _operationStack.Push(new BinaryOperationToken { Value = Input[Pointer].ToString() });

                //if (_parenthesesStack.Count > 0)
                //{
                //    _parenthesesStack.Push(Input[Pointer]);
                //}

                return ParseAndSimplify();
            }
            else
            {
                return null;
            }
        }

        private bool TryEvaluateLastBinaryOperation()
        {
            if (_expressionStack.Count != _operationStack.Count + 1)
            {
                Console.WriteLine($"Invalid token at pos. {Pointer}");
                return false;
            }

            var right = _expressionStack.Pop();
            var left  = _expressionStack.Pop();

            var simplifiedResult = Simplifier.DoOperation(_operationStack.Pop(), left, right);

            if (simplifiedResult == null)
            {
                return false;
            }

            _expressionStack.Push(simplifiedResult);

            return true;
        }

        private Token UnwindStacks()
        {
            if (_expressionStack.Count != _operationStack.Count + 1)
            {
                Console.WriteLine($"Invalid token at pos. {Pointer}");
                return null;
            }

            if (_expressionStack.Count == 0 && _operationStack.Count == 0)
            {
                Console.WriteLine("Stacks were empty. Probable equal sign in the beginning of the input.");
                return null;
            }

            if (_parenthesesStack.Count != 0)
            {
                Console.WriteLine($"{_parenthesesStack.Count} parentheses weren't closed.");
            }

            while (_operationStack.Count > 0)
            {
                TryEvaluateLastBinaryOperation();
            }

            if (_expressionStack.Count != 1)
            {
                Console.WriteLine("Invalid expression count on the expression stack after final unwinding.");
            }

            return _expressionStack.Pop();
        }

        private bool TryParseVariable()
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

        private bool TryParseConstant(out string value)
        {
            var sb = new StringBuilder();

            if (!IsDigit(Input[Pointer]))
            {
                value = null;
                return false;
            }

            while (IsDigit(Input[Pointer]))
            {
                sb.Append(Input[Pointer]);

                if (Pointer + 1 >= Input.Length)
                {
                    value =  sb.ToString().TrimStart('0');

                    return true;
                }
                else
                {
                    Pointer++;
                }
            }

            Pointer--;

            LastCharacter = Input[Pointer];

            value = sb.ToString().TrimStart('0');

            return true;
        }

        private bool IsDigit(char input)
        {
            return Const.Digits.Contains(input);
        }

        private bool ParseUnaryMinus()
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

        private bool ParseBinarySign()
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

        private bool ParseParentheses()
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

        private bool ParseEqualSign()
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
