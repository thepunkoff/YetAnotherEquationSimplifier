using System;
using System.Collections.Generic;
using System.Linq;
using Yet_Another_Simplifier.Tokens;

namespace Yet_Another_Simplifier
{
    public static class Simplifier
    {
        public static Token DoOperation(Token operation, Token leftOperand, Token rightOperand)
        {
            if (!(operation is BinaryOperationToken))
            {
                throw new Exception("Invalid token passed as operation into simplification");
            }

            if (operation.Value == Const.Add.ToString()) return Add(leftOperand, rightOperand);
            if (operation.Value == Const.Subtract.ToString()) return Subtract(leftOperand, rightOperand);
            if (operation.Value == Const.Multiply.ToString()) return Multiply(leftOperand, rightOperand);
            if (operation.Value == Const.Divide.ToString()) return Divide(leftOperand, rightOperand);
            if (operation.Value == Const.Exponentiate.ToString()) return Exponentiate(leftOperand, rightOperand);

            throw new Exception("Unknown operation value in token.");
        }

        private static Token Add(Token left, Token right)
        {
            if (left is ConstantToken && right is ConstantToken)
            {
                var token = new ConstantToken(((ConstantToken)left).NumericValue + ((ConstantToken)right).NumericValue);

                return token;
            }

            if (left is ConstantToken && right is VariableToken)
            {
                return new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right });
            }

            if (left is VariableToken && right is ConstantToken)
            {
                return new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right });
            }

            if (left is VariableToken && right is VariableToken)
            {
                if (((VariableToken)left).Variables.Except(((VariableToken)right).Variables).Count() == 0)
                {
                    var token = new VariableToken(((VariableToken)left).Quotient + ((VariableToken)right).Quotient, ((VariableToken)left).Variables);
                    
                    return token;
                }
                else
                {
                    return new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right });
                }
            }

            if (left is ConstantToken && right is ExpressionToken)
            {
                var members = ((ExpressionToken)right).Members;

                var newExpression = new ExpressionToken(new List<Token>(members));

                if (members.Any(x => x is ConstantToken))
                {
                    var c = newExpression.Members.Where(x => x is ConstantToken).FirstOrDefault() as ConstantToken;

                    c.NumericValue += ((ConstantToken)left).NumericValue;
                }
                else
                {
                    newExpression.Members.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, left });
                }

                return newExpression;
            }

            if (left is ExpressionToken && right is ConstantToken)
            {
                var members = ((ExpressionToken)left).Members;

                var newExpression = new ExpressionToken(new List<Token>(members));

                if (members.Any(x => x is ConstantToken))
                {
                    var c = newExpression.Members.Where(x => x is ConstantToken).FirstOrDefault() as ConstantToken;

                    c.NumericValue += ((ConstantToken)right).NumericValue;
                }
                else
                {
                    newExpression.Members.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, right });
                }

                return newExpression;
            }

            if (left is VariableToken && right is ExpressionToken)
            {
                var members = ((ExpressionToken)right).Members;

                var newExpression = new ExpressionToken(new List<Token>(members));

                if (members.Any(x => x is VariableToken v && v.Variables.Except(((VariableToken)left).Variables).Count() == 0))
                {
                    var v = newExpression.Members
                        .Where(x => x is VariableToken v && v.Variables.Except(((VariableToken)left).Variables).Count() == 0)
                        .FirstOrDefault() as VariableToken;

                    v.Quotient += ((VariableToken)left).Quotient;
                }
                else
                {
                    newExpression.Members.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, left });
                }

                return newExpression;
            }

            if (left is ExpressionToken && right is VariableToken)
            {
                var members = ((ExpressionToken)left).Members;

                var newExpression = new ExpressionToken(new List<Token>(members));

                if (members.Any(x => x is VariableToken v && v.Variables.Except(((VariableToken)right).Variables).Count() == 0))
                {
                    var v = newExpression.Members
                        .Where(x => x is VariableToken v && v.Variables.Except(((VariableToken)right).Variables).Count() == 0)
                        .FirstOrDefault() as VariableToken;

                    v.Quotient += ((VariableToken)right).Quotient;
                }
                else
                {
                    newExpression.Members.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, right });
                }

                return newExpression;
            }

            if (left is ExpressionToken && right is ExpressionToken)
            {
                var finalList = new List<Token>();
                var combinedList = new List<Token>();

                foreach (var element in ((ExpressionToken)left).Members)
                {
                    if (element is BinaryOperationToken)
                    {
                        continue;
                    }

                    if (element is VariableToken var)
                    {
                        if (((ExpressionToken)right).Members.Any(x => x is VariableToken v && v.Variables == var.Variables))
                        {
                            var tokenWithSimilarVariables = ((ExpressionToken)right).Members
                                .Where(x => x is VariableToken v && v.Variables == var.Variables)
                                .FirstOrDefault() as VariableToken;

                            var token = new VariableToken(var.Quotient + tokenWithSimilarVariables.Quotient, var.Variables);
                            combinedList.Add(token);
                        }
                    }

                    if (element is ConstantToken con)
                    {
                        if (((ExpressionToken)right).Members.Any(x => x is ConstantToken))
                        {
                            var rightConstantToken = ((ExpressionToken)right).Members
                                .Where(x => x is ConstantToken)
                                .FirstOrDefault() as ConstantToken;

                            var token = new ConstantToken(con.NumericValue + rightConstantToken.NumericValue);
                            combinedList.Add(token);
                        }
                    }
                }

                var union = new ExpressionToken(new List<Token>(((ExpressionToken)left).Members.Concat(((ExpressionToken)right).Members)));

                if (combinedList.Count == 0)
                {
                    return union;
                }
                else
                {
                    foreach (var member in combinedList)
                    {
                        if (member is BinaryOperationToken)
                        {
                            continue;
                        }

                        if (member is ConstantToken)
                        {
                            foreach (var unionMember in union.Members)
                            {
                                if (unionMember is ConstantToken)
                                {
                                    continue;
                                }
                                else
                                {
                                    finalList.Add(new ExpressionToken(new List<Token> { new BinaryOperationToken { Value = "+" }, unionMember }));
                                }
                            }
                        }

                        if (member is VariableToken vM)
                        {
                            foreach (var unionMember in union.Members)
                            {
                                if (unionMember is VariableToken vU)
                                {
                                    if (vM.Variables == vU.Variables)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        finalList.Add(new ExpressionToken(new List<Token>{ new BinaryOperationToken { Value = "+" }, unionMember }));
                                    }
                                }
                            }
                        }

                        foreach (var combinedMember in combinedList)
                        {
                            finalList.Add(new ExpressionToken(new List<Token> { new BinaryOperationToken { Value = "+" }, combinedMember }));
                        }
                    }

                    var finalToken = new ExpressionToken(finalList);

                    return finalToken;
                }
            }

            throw new Exception("Unknown token pair for addition rule.");
        }

        private static Token AddMultiple(List<Token> tokens)
        {
            Token result = tokens[0];

            for (int i = 1; i < tokens.Count; i++)
            {
                result = Add(result, tokens[i]);
            }

            return result;
        }


        private static Token Subtract(Token left, Token right)
        {
            right.NegateValue();
            return Add(left, right);
        }

        private static Token Multiply(Token left, Token right)
        {
            if (left is ConstantToken && right is ConstantToken)
            {
                return new ConstantToken(((ConstantToken)left).NumericValue * ((ConstantToken)right).NumericValue);
            }

            if (left is ConstantToken && right is VariableToken)
            {
                return new VariableToken(((VariableToken)right).Quotient * ((ConstantToken)left).NumericValue, ((VariableToken)right).Variables);
            }

            if (left is VariableToken && right is ConstantToken)
            {
                return new VariableToken(((VariableToken)left).Quotient * ((ConstantToken)right).NumericValue, ((VariableToken)left).Variables);
            }

            if (left is VariableToken && right is VariableToken)
            {
                var newQuotient = ((VariableToken)left).Quotient * ((VariableToken)right).Quotient;

                var newVars = new List<Variable>();

                for (int i = 0; i < ((VariableToken)left).Variables.Count; i++)
                {
                    var leftVar = ((VariableToken)left).Variables[i];

                    for (int j = 0; j < ((VariableToken)right).Variables.Count; j++)
                    {
                        var rightVar = ((VariableToken)right).Variables[j];

                        if (leftVar.Letter == rightVar.Letter)
                        {
                            newVars.Add(new Variable(leftVar.Letter, leftVar.Exponent += rightVar.Exponent));
                        }
                    }
                }

                var concat = ((VariableToken)left).Variables.Concat(((VariableToken)right).Variables);

                foreach (var variable in concat)
                {
                    var newLetters = newVars.Select(x => x.Letter);
                    if (!newLetters.Contains(variable.Letter))
                    {
                        newVars.Add(new Variable(variable.Letter, variable.Exponent));
                    }
                }

                return new VariableToken(newQuotient, newVars.ToList());
            }

            if (left is ConstantToken && right is ExpressionToken)
            {
                var list = new List<Token>();

                foreach (var member in ((ExpressionToken)right).Members)
                {
                    var token = Multiply(left,  member);
                    list.Add(token);
                }

                return AddMultiple(list);
            }

            if (left is ExpressionToken && right is ConstantToken)
            {
                var list = new List<Token>();

                foreach (var member in ((ExpressionToken)left).Members)
                {
                    var token = Multiply(right, member);
                    list.Add(token);
                }

                return AddMultiple(list);
            }

            if (left is VariableToken && right is ExpressionToken)
            {
                var list = new List<Token>();

                foreach (var member in ((ExpressionToken)right).Members)
                {
                    var token = Multiply(left, member);
                    list.Add(token);
                }

                return AddMultiple(list);
            }

            if (left is ExpressionToken && right is VariableToken)
            {
                var list = new List<Token>();

                foreach (var member in ((ExpressionToken)left).Members)
                {
                    var token = Multiply(right, member);
                    list.Add(token);
                }

                return AddMultiple(list);
            }

            if (left is ExpressionToken && right is ExpressionToken)
            {
                var list = new List<Token>();

                foreach (var leftMember in ((ExpressionToken)left).Members)
                {
                    if (leftMember is BinaryOperationToken)
                    {
                        continue;
                    }

                    foreach (var rightMember in ((ExpressionToken)right).Members)
                    {
                        if (rightMember is BinaryOperationToken)
                        {
                            continue;
                        }

                        var result = Multiply(leftMember, rightMember);
                        list.Add(result);
                    }
                }

                return AddMultiple(list);
            }

            throw new Exception("Unknown token pair for multiplication rule.");
        }

        private static Token Divide(Token leftOperand, Token rightOperand)
        {
            throw new NotImplementedException();
        }

        private static Token Exponentiate(Token left, Token right)
        {
            if (left is ConstantToken && right is ConstantToken)
            {
                var power = ((ConstantToken)right).NumericValue;

                ((ConstantToken)left).NumericValue = Math.Pow(((ConstantToken)left).NumericValue, power);
            }

            if (left is ConstantToken && right is VariableToken)
            {
                Console.WriteLine("Expression exponents not supported.");

                return null;
            }

            if (left is VariableToken && right is ConstantToken)
            {
                var power = ((ConstantToken)right).NumericValue;

                var newQuotient = Math.Pow(((VariableToken)left).Quotient, power);

                var newVars = new List<Variable>(((VariableToken)left).Variables).ToArray();

                for (int i = 0; i < newVars.Length; i++)
                {
                    newVars[i].Exponent *= power;
                }

                return new VariableToken(newQuotient, newVars.ToList());
            }

            if (left is VariableToken && right is VariableToken)
            {
                Console.WriteLine("Variable exponents not supported.");

                return null;
            }

            if (left is ConstantToken && right is ExpressionToken)
            {
                Console.WriteLine("Expression exponents not supported.");

                return null;
            }

            if (left is ExpressionToken && right is ConstantToken)
            {
                Token result = (ExpressionToken)left;

                for (int i = 0; i < ((ConstantToken)right).NumericValue - 1; i++)
                {
                    result = Multiply(result, (ExpressionToken)left);
                }

                return result;
            }

            if (left is VariableToken && right is ExpressionToken)
            {
                Console.WriteLine("Expression exponents not supported.");

                return null;
            }

            if (left is ExpressionToken && right is VariableToken)
            {
                Console.WriteLine("Variable exponents not supported.");

                return null;
            }

            if (left is ExpressionToken && right is ExpressionToken)
            {
                Console.WriteLine("Expression exponents not supported.");

                return null;
            }

            throw new Exception("Unknown token pair for exponentiation rule.");
        }
    }
}
