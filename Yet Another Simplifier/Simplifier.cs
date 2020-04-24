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

            Token result = null;

            if (operation.Value == Const.Add.ToString()) result = Add(leftOperand, rightOperand);
            if (operation.Value == Const.Subtract.ToString()) result = Subtract(leftOperand, rightOperand);
            if (operation.Value == Const.Multiply.ToString()) result = Multiply(leftOperand, rightOperand);
            if (operation.Value == Const.Divide.ToString()) result = Divide(leftOperand, rightOperand);
            if (operation.Value == Const.Exponentiate.ToString()) result = Exponentiate(leftOperand, rightOperand);

            if (result != null)
            {
                if (result is VariableToken vToken)
                {
                    if (vToken.Quotient == 0)
                    {
                        return new ConstantToken(0);
                    }
                }
                if (result is ExpressionToken exToken)
                {
                    foreach (var memer in exToken.Members.ToList())
                    {
                        if (memer is ConstantToken c && c.NumericValue == 0)
                        {
                            DeleteFromExpression(c, exToken);
                        }

                        if (memer is VariableToken v && v.Quotient == 0)
                        {
                            DeleteFromExpression(v, exToken);
                        }
                    }

                    if (exToken.Members.Count == 1)
                    {
                        return exToken.Members[0];
                    }
                }

                return result;
            }

            throw new Exception("Unknown operation value in token.");
        }

        private static void DeleteFromExpression(Token t, ExpressionToken ex)
        {
            if (ex.Members.IndexOf(t) == 0)
            {
                ex.Members.RemoveAt(0);
                ex.Members.RemoveAt(0);
            }
            else
            {
                var index = ex.Members.IndexOf(t) - 1;

                ex.Members.RemoveAt(index);
                ex.Members.RemoveAt(index);
            }

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

            if (left is ConstantToken && right is FractionToken)
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

            if (left is VariableToken && right is FractionToken)
            {
                return new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right });
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
                var finalNoSigns = new List<Token>();

                var leftMembers = ((ExpressionToken)left).Members.Where(x => !(x is BinaryOperationToken));
                var rightMembers = ((ExpressionToken)right).Members.Where(x => !(x is BinaryOperationToken));

                foreach (var l in leftMembers)
                {
                    var found = false;

                    foreach (var r in rightMembers)
                    {
                        if (l is VariableToken lV && r is VariableToken rV)
                        {
                            if (lV.Variables.SequenceEqual(rV.Variables))
                            {
                                finalNoSigns.Add(Add(lV, rV));
                                found = true;
                                break;
                            }
                        }

                        if (l is ConstantToken lC && r is ConstantToken rC)
                        {
                            finalNoSigns.Add(Add(lC, rC));
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        finalNoSigns.Add(l);
                    }
                }

                foreach (var r in rightMembers)
                {
                    if (r is VariableToken rV)
                    {
                        var allVars = finalNoSigns.Where(x => x is VariableToken).Select(x => ((VariableToken)x).Variables);

                        if (!allVars.Any(x => x.Except(rV.Variables).Count() == 0))
                        {
                            finalNoSigns.Add(r);
                        }
                    }

                    if (r is ConstantToken rC)
                    {
                        if (!finalNoSigns.Any(x => x is ConstantToken))
                        {
                            finalNoSigns.Add(r);
                        }
                    }
                }

                var finalWithSigns = new List<Token>();
                
                foreach (var t in finalNoSigns)
                {
                    finalWithSigns.Add(t);
                    
                    if (finalNoSigns.IndexOf(t) != finalNoSigns.Count - 1)
                    {
                        finalWithSigns.Add(new BinaryOperationToken { Value = "+" });
                    }
                }

                return new ExpressionToken(finalWithSigns);
            }

            if (left is ExpressionToken && right is FractionToken)
            {
                return new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right });
            }

            if (left is FractionToken && right is ConstantToken)
            {
                return Add(right, left);
            }

            if (left is FractionToken && right is VariableToken)
            {
                return Add(right, left);
            }

            if (left is FractionToken && right is ExpressionToken)
            {
                return Add(right, left);
            }

            if (left is FractionToken && right is FractionToken)
            {
                if (((FractionToken)left).Denominator.ToString() == ((FractionToken)right).Denominator.ToString())
                {
                    var newNum = Add(((FractionToken)left).Numerator, ((FractionToken)right).Numerator);
                    var newDenom = ((FractionToken)left).Denominator.Clone();
                    return new FractionToken(newNum, newDenom);
                }
                else
                {
                    return new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right });
                }
            }

            throw new Exception("Unknown token pair for addition rule.");
        }

        private static Token AddMultiple(List<Token> tokens)
        {
            return tokens.Aggregate((a, b) => Add(a, b));
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

            if (left is ConstantToken && right is ExpressionToken)
            {
                var list = new List<Token>();

                foreach (var member in ((ExpressionToken)right).Members.Where(x => !(x is BinaryOperationToken)))
                {
                    var token = Multiply(left, member);
                    list.Add(token);
                }

                return AddMultiple(list);
            }

            if (left is ConstantToken && right is FractionToken)
            {               
                var newNum = Multiply(new ConstantToken(((ConstantToken)left).NumericValue), ((FractionToken)right).Numerator);
                var newDenom = ((FractionToken)right).Denominator.Clone();

                var newFraction = new FractionToken(newNum, newDenom);

                var gcd = newFraction.GreatestCommonDivisor();

                newNum = Divide(newNum, new ConstantToken(gcd));
                newDenom = Divide(newDenom, new ConstantToken(gcd));

                return new FractionToken(newNum, newDenom);
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

            if (left is VariableToken && right is ExpressionToken)
            {
                var list = new List<Token>();

                foreach (var member in ((ExpressionToken)right).Members.Where(x => !(x is BinaryOperationToken)))
                {
                    var token = Multiply(left, member);
                    list.Add(token);
                }

                return AddMultiple(list);
            }

            if (left is VariableToken && right is FractionToken)
            {
                var newNum = Multiply(new VariableToken(((VariableToken)left).Quotient, ((VariableToken)left).Variables), ((FractionToken)right).Numerator);
                var newDenom = ((FractionToken)right).Denominator.Clone();

                return new FractionToken(newNum, newDenom);
            }

            if (left is ExpressionToken && right is ConstantToken)
            {
                var list = new List<Token>();

                foreach (var member in ((ExpressionToken)left).Members)
                {
                    if (member is BinaryOperationToken)
                    {
                        continue;
                    }

                    var token = Multiply(right, member);
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

            if (left is ExpressionToken && right is FractionToken)
            {
                var newNum = Multiply(new ExpressionToken(new List<Token>(((ExpressionToken)left).Members)), ((FractionToken)right).Numerator);
                var newDenom = ((FractionToken)right).Denominator.Clone();

                return new FractionToken(newNum, newDenom);
            }

            if (left is FractionToken && right is ConstantToken)
            {
                return Multiply(right, left);
            }

            if (left is FractionToken && right is VariableToken)
            {
                return Multiply(right, left);
            }

            if (left is FractionToken && right is ExpressionToken)
            {
                return Multiply(right, left);
            }

            if (left is FractionToken && right is FractionToken)
            {
                var newNum = Multiply(((FractionToken)left).Numerator, ((FractionToken)right).Numerator);
                var newDenom = Multiply(((FractionToken)left).Denominator, ((FractionToken)right).Denominator);

                return new FractionToken(newNum, newDenom);
            }

            throw new Exception("Unknown token pair for multiplication rule.");
        }

        private static Token Divide(Token left, Token right)
        {
            if (left is ConstantToken && right is ConstantToken)
            {
                return new ConstantToken(((ConstantToken)left).NumericValue / ((ConstantToken)right).NumericValue);
            }

            if (left is ConstantToken && right is VariableToken)
            {
                var gcd = Utility.GreatestCommonDivisor(((ConstantToken)left).NumericValue, ((VariableToken)right).Quotient);

                var newNum = new ConstantToken(((ConstantToken)left).NumericValue / gcd);
                var newDenom = new VariableToken(((VariableToken)right).Quotient / gcd, new List<Variable>(((VariableToken)right).Variables));

                return new FractionToken(newNum, newDenom);
            }

            if (left is ConstantToken && right is ExpressionToken)
            {
                var allTokens = new List<decimal>(((ExpressionToken)right).Members
                    .Where(x => x is IHasNumericValue)
                    .Select(x => ((IHasNumericValue)x).NumericValue));

                allTokens.Add(((IHasNumericValue)left).NumericValue);

                var gcd = Utility.GreatestCommonDivisor(allTokens);

                var newConst = new ConstantToken(((ConstantToken)left).NumericValue / gcd);

                var newExpr = new ExpressionToken(new List<Token>(((ExpressionToken)right).Members));
                foreach (var member in newExpr.Members)
                {
                    if (member is BinaryOperationToken)
                    {
                        continue;
                    }

                    ((IHasNumericValue)member).NumericValue = ((IHasNumericValue)member).NumericValue / gcd; 
                }

                return new FractionToken(newConst, newExpr);
            }

            if (left is ConstantToken && right is FractionToken)
            {
                var newNum = Multiply(left.Clone(), ((FractionToken)right).Denominator.Clone());
                var newDenom = ((FractionToken)right).Numerator.Clone();

                return new FractionToken(newNum, newDenom);
            }

            return Multiply(left, Divide(new ConstantToken(1), right));
        }

        private static Token Exponentiate(Token left, Token right)
        {
            if (left is ConstantToken && right is ConstantToken)
            {
                var power = ((ConstantToken)right).NumericValue;

                var numericValue = ((ConstantToken)left).NumericValue;

                ((ConstantToken)left).NumericValue = (decimal)Math.Pow((double)numericValue, (double)power);
            }

            if (left is ConstantToken && right is VariableToken)
            {
                Console.WriteLine("Expression exponents not supported.");

                return null;
            }

            if (left is ConstantToken && right is ExpressionToken)
            {
                Console.WriteLine("Expression exponents not supported.");

                return null;
            }

            if (left is ConstantToken && right is FractionToken)
            {
                Console.WriteLine("Fraction exponents not supported.");

                return null;
            }

            if (left is VariableToken && right is ConstantToken)
            {
                var power = ((ConstantToken)right).NumericValue;

                var quotient = ((VariableToken)left).Quotient;

                var newQuotient = (decimal)Math.Pow((double)quotient, (double)power);

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

            if (left is VariableToken && right is ExpressionToken)
            {
                Console.WriteLine("Expression exponents not supported.");

                return null;
            }

            if (left is VariableToken && right is FractionToken)
            {
                Console.WriteLine("Fraction exponents not supported.");

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

            if (left is ExpressionToken && right is FractionToken)
            {
                Console.WriteLine("Fraction exponents not supported.");

                return null;
            }

            if (left is FractionToken && right is ConstantToken)
            {
                Token result = (FractionToken)left;

                for (int i = 0; i < ((ConstantToken)right).NumericValue - 1; i++)
                {
                    result = Multiply(result, (FractionToken)left);
                }

                return result;
            }

            if (left is FractionToken && right is VariableToken)
            {
                Console.WriteLine("Variable exponents not supported.");

                return null;
            }

            if (left is FractionToken && right is ExpressionToken)
            {
                Console.WriteLine("Expression exponents not supported.");

                return null;
            }

            if (left is FractionToken && right is FractionToken)
            {
                Console.WriteLine("Fraction exponents not supported.");

                return null;
            }

            throw new Exception("Unknown token pair for exponentiation rule.");
        }
    }
}
