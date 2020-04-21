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
                ((ConstantToken)left).NumericValue += ((ConstantToken)right).NumericValue;

                return left;
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
                if (((VariableToken)left).Variables == ((VariableToken)right).Variables)
                {
                    ((VariableToken)left).Quotient += ((VariableToken)right).Quotient;
                    return left;
                }
                else
                {
                    return new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right });
                }
            }

            if (left is ConstantToken && right is ExpressionToken)
            {
                var members = ((ExpressionToken)right).Members;

                if (members.Any(x => x is ConstantToken))
                {
                    var c = members.Where(x => x is ConstantToken).FirstOrDefault() as ConstantToken;
                    c.NumericValue += ((ConstantToken)left).NumericValue;
                }
                else
                {
                    members.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, left });
                }

                return (ExpressionToken)right;
            }

            if (left is ExpressionToken && right is ConstantToken)
            {
                var members = ((ExpressionToken)left).Members;

                if (members.Any(x => x is ConstantToken))
                {
                    var c = members
                        .Where(x => x is ConstantToken)
                        .FirstOrDefault() as ConstantToken;
                    c.NumericValue += ((ConstantToken)right).NumericValue;
                }
                else
                {
                    members.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, right });
                }

                return (ExpressionToken)left;
            }

            if (left is VariableToken && right is ExpressionToken)
            {
                var members = ((ExpressionToken)right).Members;

                if (members.Any(x => x is VariableToken v && v.Variables == ((VariableToken)left).Variables))
                {
                    var v = members
                        .Where(x => x is VariableToken v && v.Variables == ((VariableToken)left).Variables)
                        .FirstOrDefault() as VariableToken;

                    v.Quotient += ((VariableToken)left).Quotient;
                }
                else
                {
                    members.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, left });
                }

                return (ExpressionToken)right;
            }

            if (left is ExpressionToken && right is VariableToken)
            {
                var members = ((ExpressionToken)left).Members;

                if (members.Any(x => x is VariableToken v && v.Variables == ((VariableToken)right).Variables))
                {
                    var v = members
                        .Where(x => x is VariableToken v && v.Variables == ((VariableToken)right).Variables)
                        .FirstOrDefault() as VariableToken;

                    v.Quotient += ((VariableToken)right).Quotient;
                }
                else
                {
                    members.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, right });
                }

                return (ExpressionToken)left;
            }

            if (left is ExpressionToken && right is ExpressionToken)
            {
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

                            var.Quotient += tokenWithSimilarVariables.Quotient;

                            ((ExpressionToken)right).Members.Remove(tokenWithSimilarVariables);
                        }
                    }

                    if (element is ConstantToken con)
                    {
                        if (((ExpressionToken)right).Members.Any(x => x is ConstantToken))
                        {
                            var rightConstantToken = ((ExpressionToken)right).Members
                                .Where(x => x is ConstantToken)
                                .FirstOrDefault() as ConstantToken;

                            con.NumericValue += rightConstantToken.NumericValue;

                            ((ExpressionToken)right).Members.Remove(rightConstantToken);
                        }
                    }
                }

                if (((ExpressionToken)right).Members.Count == 0)
                {
                    return left;
                }
                else
                {
                    var restMembers = new List<Token>();

                    foreach (var member in ((ExpressionToken)right).Members)
                    {
                        if (member is BinaryOperationToken)
                        {
                            continue;
                        }

                        restMembers.AddRange(new List<Token> { new BinaryOperationToken { Value = "+" }, member });
                    }

                    ((ExpressionToken)left).Members.AddRange(restMembers);

                    return left;
                }
            }

            throw new Exception("Unknown token pair for addition rule.");
        }

        private static Token Subtract(Token leftOperand, Token rightOperand)
        {
            throw new NotImplementedException();
        }

        private static Token Multiply(Token leftOperand, Token rightOperand)
        {
            throw new NotImplementedException();
        }

        private static Token Divide(Token leftOperand, Token rightOperand)
        {
            throw new NotImplementedException();
        }

        private static Token Exponentiate(Token leftOperand, Token rightOperand)
        {
            throw new NotImplementedException();
        }
    }
}
