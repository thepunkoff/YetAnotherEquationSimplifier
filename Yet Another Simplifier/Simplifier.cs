using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yet_Another_Simplifier.Tokens;

namespace Yet_Another_Simplifier
{
    public static class Simplifier
    {
        public static OperationResult DoOperation(BinaryOperationToken operation, ValueToken leftOperand, ValueToken rightOperand)
        {
            if (!(operation is BinaryOperationToken))
            {
                throw new Exception("Invalid token passed as operation into simplification");
            }

            ValueToken result = null;

            if (operation.Value == Const.Add.ToString())
            {
                var add = Add(leftOperand, rightOperand);
                if (add.Success) result = add.Result;
                else return OperationResult.CreateFailure(add.ErrorMessage);
            }
            if (operation.Value == Const.Subtract.ToString())
            {
                var sub = Subtract(leftOperand, rightOperand);
                if (sub.Success) result = sub.Result;
                else return OperationResult.CreateFailure(sub.ErrorMessage);
            }
            if (operation.Value == Const.Multiply.ToString())
            {
                var mult = Multiply(leftOperand, rightOperand);
                if (mult.Success) result = mult.Result;
                else return OperationResult.CreateFailure(mult.ErrorMessage);
            }
            if (operation.Value == Const.Divide.ToString())
            {
                var div = Divide(leftOperand, rightOperand);
                if (div.Success) result = div.Result;
                else return OperationResult.CreateFailure(div.ErrorMessage);
            }
            if (operation.Value == Const.Exponentiate.ToString())
            {
                var exp = Exponentiate(leftOperand, rightOperand);
                if (exp.Success) result = exp.Result;
                else return OperationResult.CreateFailure(exp.ErrorMessage);
            }

            if (result != null)
            {
                if (result is VariableToken vToken)
                {
                    if (vToken.Quotient == 0)
                    {
                        return OperationResult.CreateSuccess(new ConstantToken(0));
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
                        return OperationResult.CreateSuccess((ValueToken)exToken.Members[0]);
                    }

                    if (exToken.Members.Count == 0)
                    {
                        return OperationResult.CreateSuccess((new ConstantToken(0)));
                    }
                }
                if (result is FractionToken)
                {
                    if (((FractionToken)result).Numerator is IHasNumericValue)
                    {
                        if (((IHasNumericValue)((FractionToken)result).Numerator).NumericValue == 0)
                        {
                            return OperationResult.CreateSuccess(new ConstantToken(0));
                        }
                    }

                    var gcd = result.GreatestCommonDivisor();
                    return OperationResult.CreateSuccess(((IEliminatable)result).Eliminate(gcd));
                }

                return OperationResult.CreateSuccess(result);
            }

            throw new Exception($"Couldnt find an appropriate operation method for sign '{((BinaryOperationToken)operation).Value}'");
        }

        private static void DeleteFromExpression(Token t, ExpressionToken ex)
        {
            if (ex.Members.IndexOf(t) == 0)
            {
                ex.Members.RemoveAt(0);
                if (ex.Members.Count > 0)
                {
                    ex.Members.RemoveAt(0);
                }
            }
            else
            {
                var index = ex.Members.IndexOf(t) - 1;

                ex.Members.RemoveAt(index);
                ex.Members.RemoveAt(index);
            }

        }

        private static OperationResult Add(ValueToken left, ValueToken right)
        {
            if (left is ConstantToken && right is ConstantToken)
            {
                var token = new ConstantToken(((ConstantToken)left).NumericValue + ((ConstantToken)right).NumericValue);

                return OperationResult.CreateSuccess(token);
            }

            if (left is ConstantToken && right is VariableToken)
            {
                return OperationResult.CreateSuccess(
                    new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right })
                    );
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

                return OperationResult.CreateSuccess(newExpression);
            }

            if (left is ConstantToken && right is FractionToken)
            {
                return OperationResult.CreateSuccess(
                    new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right })
                    );
            }

            if (left is VariableToken && right is ConstantToken)
            {
                return OperationResult.CreateSuccess(
                    new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right })
                    );
            }

            if (left is VariableToken && right is VariableToken)
            {
                if (((VariableToken)left).Variables.Except(((VariableToken)right).Variables).Count() == 0)
                {
                    var token = new VariableToken(((VariableToken)left).Quotient + ((VariableToken)right).Quotient, ((VariableToken)left).Variables);
                    
                    return OperationResult.CreateSuccess(token);
                }
                else
                {
                    return OperationResult.CreateSuccess(
                        new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right })
                        );
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

                return OperationResult.CreateSuccess(newExpression);
            }

            if (left is VariableToken && right is FractionToken)
            {
                return OperationResult.CreateSuccess(
                    new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right })
                    );
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

                return OperationResult.CreateSuccess(newExpression);
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

                return OperationResult.CreateSuccess(newExpression);
            }

            if (left is ExpressionToken && right is ExpressionToken)
            {
                var finalNoSigns = new List<Token>();

                var leftMembers = ((ExpressionToken)left).Members.Where(x => !(x is BinaryOperationToken)).ToList();
                var rightMembers = ((ExpressionToken)right).Members.Where(x => !(x is BinaryOperationToken)).ToList();

                var z = rightMembers[2];

                foreach (var l in leftMembers)
                {
                    var found = false;

                    foreach (var r in rightMembers)
                    {
                        if (l is VariableToken lV && r is VariableToken rV)
                        {
                            if (lV.Variables.Except(rV.Variables).Union(rV.Variables.Except(lV.Variables)).Count() == 0)
                            {
                                var sum = Add(lV, rV);

                                if (sum.Success)
                                {
                                    finalNoSigns.Add(sum.Result);
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    return sum;
                                }
                            }
                        }

                        if (l is ConstantToken lC && r is ConstantToken rC)
                        {
                            var sum = Add(lC, rC);

                            if (sum.Success)
                            {
                                finalNoSigns.Add(sum.Result);
                                found = true;
                                break;
                            }
                            else
                            {
                                return sum;
                            }
                        }
                    }

                    if (!found)
                    {
                        finalNoSigns.Add(l.Clone());
                    }
                }

                foreach (var r in rightMembers)
                {
                    if (r is VariableToken rV)
                    {
                        var allVars = finalNoSigns.Where(x => x is VariableToken).Select(x => ((VariableToken)x).Variables);

                        if (!allVars.Any(x => x.Except(rV.Variables).Count() == 0))
                        {
                            finalNoSigns.Add(r.Clone());
                        }
                    }

                    if (r is ConstantToken rC)
                    {
                        if (!finalNoSigns.Any(x => x is ConstantToken))
                        {
                            finalNoSigns.Add(r.Clone());
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

                return OperationResult.CreateSuccess(new ExpressionToken(finalWithSigns));
            }

            if (left is ExpressionToken && right is FractionToken)
            {
                return OperationResult.CreateSuccess(
                    new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right })
                    );
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
                    var newDenom = (ValueToken)((FractionToken)left).Denominator.Clone();

                    if (newNum.Success)
                    {
                        return OperationResult.CreateSuccess(new FractionToken(newNum.Result, newDenom));
                    }
                    else
                    {
                        return newNum;
                    }

                    
                }
                else
                {
                    return OperationResult.CreateSuccess(
                        new ExpressionToken(new List<Token> { left, new BinaryOperationToken { Value = "+" }, right })
                        );
                }
            }

            throw new Exception("Unknown token pair for addition rule.");
        }

        private static OperationResult AddMultiple(List<ValueToken> tokens)
        {
            var result = tokens[0];

            for (int i = 1; i < tokens.Count; i++)
            {
                var t = tokens[i];

                var sum = Add(result, t);

                if (sum.Success)
                {
                    result = sum.Result;
                }
                else
                {
                    return OperationResult.CreateFailure(sum.ErrorMessage);
                }
            }

            return OperationResult.CreateSuccess(result);
        }


        private static OperationResult Subtract(ValueToken left, ValueToken right)
        {
            right.Negate();
            return Add(left, right);
        }

        private static OperationResult Multiply(ValueToken left, ValueToken right)
        {
            if (left is ConstantToken && right is ConstantToken)
            {
                return OperationResult.CreateSuccess(
                    new ConstantToken(((ConstantToken)left).NumericValue * ((ConstantToken)right).NumericValue)
                    );
            }

            if (left is ConstantToken && right is VariableToken)
            {
                return OperationResult.CreateSuccess(
                    new VariableToken(((VariableToken)right).Quotient * ((ConstantToken)left).NumericValue, ((VariableToken)right).Variables)
                    );
            }

            if (left is ConstantToken && right is ExpressionToken)
            {
                var list = new List<ValueToken>();

                foreach (var member in ((ExpressionToken)right).Members.Where(x => !(x is BinaryOperationToken)))
                {
                    var token = Multiply(left, (ValueToken)member);

                    if (token.Success)
                    {
                        list.Add(token.Result);
                    }
                    else
                    {
                        return OperationResult.CreateFailure(token.ErrorMessage);
                    }
                }

                return AddMultiple(list);
            }

            if (left is ConstantToken && right is FractionToken)
            {               
                var newNum = Multiply(new ConstantToken(((ConstantToken)left).NumericValue), ((FractionToken)right).Numerator);
                var newDenom = (ValueToken)((FractionToken)right).Denominator.Clone();

                ValueToken finalNum = null;
                ValueToken finalDenom = newDenom;

                if (newNum.Success)
                {
                    finalNum = newNum.Result;
                }
                else
                {
                    return OperationResult.CreateFailure(newNum.ErrorMessage);
                }

                FractionToken newFraction = new FractionToken(finalNum, finalDenom);

                var gcd = newFraction.GreatestCommonDivisor();

                var divisionNum = Divide(newNum.Result, new ConstantToken(gcd));
                var divisionDenom = Divide(newDenom, new ConstantToken(gcd));

                if (divisionNum.Success)
                {
                    newFraction.Numerator = divisionNum.Result;
                }
                else
                {
                    return OperationResult.CreateFailure(divisionNum.ErrorMessage);
                }

                if (divisionDenom.Success)
                {
                    newFraction.Denominator = divisionDenom.Result;
                }
                else
                {
                    return OperationResult.CreateFailure(divisionDenom.ErrorMessage);
                }

                return OperationResult.CreateSuccess(newFraction);
            }

            if (left is VariableToken && right is ConstantToken)
            {
                return OperationResult.CreateSuccess(
                    new VariableToken(((VariableToken)left).Quotient * ((ConstantToken)right).NumericValue, ((VariableToken)left).Variables)
                    );
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

                return OperationResult.CreateSuccess(
                    new VariableToken(newQuotient, newVars.ToList())
                    );
            }

            if (left is VariableToken && right is ExpressionToken)
            {
                var list = new List<ValueToken>();

                foreach (var member in ((ExpressionToken)right).Members.Where(x => !(x is BinaryOperationToken)))
                {
                    var token = Multiply(left, (ValueToken)member);

                    if (token.Success)
                    {
                        list.Add(token.Result);
                    }
                    else
                    {
                        return OperationResult.CreateFailure(token.ErrorMessage);
                    }
                }

                return AddMultiple(list);
            }

            if (left is VariableToken && right is FractionToken)
            {
                var newNum = Multiply(new VariableToken(((VariableToken)left).Quotient, ((VariableToken)left).Variables), ((FractionToken)right).Numerator);
                var newDenom = (ValueToken)((FractionToken)right).Denominator.Clone();

                if (newNum.Success)
                {
                    return OperationResult.CreateSuccess(new FractionToken(newNum.Result, newDenom));
                }
                else
                {
                    return OperationResult.CreateFailure(newNum.ErrorMessage);
                }
            }

            if (left is ExpressionToken && right is ConstantToken)
            {
                var list = new List<ValueToken>();

                foreach (var member in ((ExpressionToken)left).Members)
                {
                    if (member is BinaryOperationToken)
                    {
                        continue;
                    }

                    var token = Multiply(right, (ValueToken)member);

                    if (token.Success)
                    {
                        list.Add(token.Result);
                    }
                    else
                    {
                        return OperationResult.CreateFailure(token.ErrorMessage);
                    }
                }

                return AddMultiple(list);
            }

            if (left is ExpressionToken && right is VariableToken)
            {
                var list = new List<ValueToken>();

                foreach (var member in ((ExpressionToken)left).Members)
                {
                    if (member is BinaryOperationToken)
                    {
                        continue;
                    }

                    var token = Multiply(right, (ValueToken)member);

                    if (token.Success)
                    {
                        list.Add(token.Result);
                    }
                    else
                    {
                        return OperationResult.CreateFailure(token.ErrorMessage);
                    }
                }

                return AddMultiple(list);
            }

            if (left is ExpressionToken && right is ExpressionToken)
            {
                var list = new List<ValueToken>();

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

                        var result = Multiply((ValueToken)leftMember, (ValueToken)rightMember);

                        if (result.Success)
                        {
                            list.Add(result.Result);
                        }
                        else
                        {
                            return OperationResult.CreateFailure(result.ErrorMessage);
                        }
                    }
                }

                return AddMultiple(list);
            }

            if (left is ExpressionToken && right is FractionToken)
            {
                var newNum = Multiply(new ExpressionToken(new List<Token>(((ExpressionToken)left).Members)), ((FractionToken)right).Numerator);
                var newDenom = (ValueToken)((FractionToken)right).Denominator.Clone();

                if (newNum.Success)
                {
                    return OperationResult.CreateSuccess(new FractionToken(newNum.Result, newDenom));
                }
                else
                {
                    return OperationResult.CreateFailure(newNum.ErrorMessage);
                }
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

                if (newNum.Success)
                {
                    if (newDenom.Success)
                    {
                        return OperationResult.CreateSuccess(new FractionToken(newNum.Result, newDenom.Result));
                    }
                    else
                    {
                        return OperationResult.CreateFailure(newDenom.ErrorMessage);
                    }
                }
                else
                {
                    return OperationResult.CreateFailure(newNum.ErrorMessage);
                }
            }

            return OperationResult.CreateFailure($"Unknown token pair for multiplication rule. Left: {left}, Right: {right}");
        }

        private static OperationResult Divide(ValueToken left, ValueToken right)
        {
            if (right is IHasNumericValue rnv)
            {
                if (rnv.NumericValue == 0)
                {
                    return OperationResult.CreateFailure("Cannot divide by 0.");
                }
            }

            if (left is IHasNumericValue lnv)
            {
                if (lnv.NumericValue == 0)
                {
                    return OperationResult.CreateSuccess(new ConstantToken(0));
                }
            }

            if (right is ConstantToken c)
            {
                if (c.NumericValue == 1)
                {
                    return OperationResult.CreateSuccess((ValueToken)left.Clone());
                }
            }
            
            if (left.ToString() == right.ToString())
            {
                return OperationResult.CreateSuccess(new ConstantToken(1));
            }

            if (left is ConstantToken && right is ConstantToken)
            {
                return OperationResult.CreateSuccess(
                    new ConstantToken(((ConstantToken)left).NumericValue / ((ConstantToken)right).NumericValue)
                    );
            }

            if (left is ConstantToken && right is VariableToken)
            {
                var gcd = Utility.GreatestCommonDivisor(((ConstantToken)left).NumericValue, ((VariableToken)right).Quotient);

                var newNum = new ConstantToken(((ConstantToken)left).NumericValue / gcd);
                var newDenom = new VariableToken(((VariableToken)right).Quotient / gcd, new List<Variable>(((VariableToken)right).Variables));

                return OperationResult.CreateSuccess(new FractionToken(newNum, newDenom));
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

                return OperationResult.CreateSuccess(new FractionToken(newConst, newExpr));
            }

            if (left is ConstantToken && right is FractionToken)
            {
                var newNum = Multiply((ValueToken)left.Clone(), (ValueToken)((FractionToken)right).Denominator.Clone());
                var newDenom = (ValueToken)((FractionToken)right).Numerator.Clone();

                if (newNum.Success)
                {
                    return OperationResult.CreateSuccess(new FractionToken(newNum.Result, newDenom));
                }
                else
                {
                    return OperationResult.CreateFailure(newNum.ErrorMessage);
                }

            }

            if (left is VariableToken && right is ConstantToken)
            {
                return OperationResult.CreateSuccess(new FractionToken((ValueToken)left.Clone(), (ValueToken)right.Clone()));
            }

            if (left is ExpressionToken && right is ConstantToken)
            {
                if (!((ExpressionToken)left).Members.Any(x => x is FractionToken))
                {
                    return OperationResult.CreateSuccess(new FractionToken((ValueToken)left.Clone(), (ValueToken)right.Clone()));
                }
                else
                {
                    var list = new List<ValueToken>();

                    foreach (var member in ((ExpressionToken)left).Members)
                    {
                        if (member is BinaryOperationToken)
                        {
                            continue;
                        }

                        var newMem = Divide((ValueToken)member, new ConstantToken(((ConstantToken)right).NumericValue));

                        if (newMem.Success)
                        {
                            list.Add(newMem.Result);
                        }
                        else
                        {
                            return OperationResult.CreateFailure(newMem.ErrorMessage);
                        }
                    }

                    return AddMultiple(list);
                }
            }

            if (left is FractionToken && right is ConstantToken)
            {
                var newNum = (ValueToken)((FractionToken)left).Numerator.Clone();
                var newDenom = Multiply(
                    (ValueToken)((FractionToken)left).Denominator.Clone(),
                    new ConstantToken(((ConstantToken)right).NumericValue)
                );

                return Divide(newNum, newDenom.Result);
            }

            var invertedRight = Divide(new ConstantToken(1), right);

            if (invertedRight.Success)
            {
                return Multiply(left, invertedRight.Result);

            }
            else
            {
                return OperationResult.CreateFailure(invertedRight.ErrorMessage);
            }
        }

        private static OperationResult Exponentiate(ValueToken left, ValueToken right)
        {
            if (left is ConstantToken && right is ConstantToken)
            {
                var power = ((ConstantToken)right).NumericValue;

                var numericValue = ((ConstantToken)left).NumericValue;

                return OperationResult.CreateSuccess(new ConstantToken((decimal)Math.Pow((double)numericValue, (double)power)));
            }

            if (left is ConstantToken && right is VariableToken)
            {
                return OperationResult.CreateFailure("Expression exponents not supported.");
            }

            if (left is ConstantToken && right is ExpressionToken)
            {
                return OperationResult.CreateFailure("Expression exponents not supported.");
            }

            if (left is ConstantToken && right is FractionToken)
            {
                return OperationResult.CreateFailure("Fraction exponents not supported.");
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

                return OperationResult.CreateSuccess(new VariableToken(newQuotient, newVars.ToList()));
            }

            if (left is VariableToken && right is VariableToken)
            {
                return OperationResult.CreateFailure("Variable exponents not supported.");
            }

            if (left is VariableToken && right is ExpressionToken)
            {
                return OperationResult.CreateFailure("Expression exponents not supported.");
            }

            if (left is VariableToken && right is FractionToken)
            {
                return OperationResult.CreateFailure("Fraction exponents not supported.");
            }

            if (left is ExpressionToken && right is ConstantToken)
            {
                ValueToken result = (ExpressionToken)left;

                for (int i = 0; i < ((ConstantToken)right).NumericValue - 1; i++)
                {
                    var mult = Multiply(result, (ExpressionToken)left);

                    if (mult.Success)
                    {
                        result = mult.Result;
                    }
                    else
                    {
                        return OperationResult.CreateFailure(mult.ErrorMessage);
                    }
                }

                return OperationResult.CreateSuccess(result);
            }
            
            if (left is ExpressionToken && right is VariableToken)
            {
                return OperationResult.CreateFailure("Variable exponents not supported.");
            }

            if (left is ExpressionToken && right is ExpressionToken)
            {
                return OperationResult.CreateFailure("Expression exponents not supported.");
            }

            if (left is ExpressionToken && right is FractionToken)
            {
                return OperationResult.CreateFailure("Fraction exponents not supported.");
            }

            if (left is FractionToken && right is ConstantToken)
            {
                ValueToken result = (FractionToken)left;

                for (int i = 0; i < ((ConstantToken)right).NumericValue - 1; i++)
                {
                    var mult = Multiply(result, (FractionToken)left);

                    if (mult.Success)
                    {
                        result = mult.Result;
                    }
                    else
                    {
                        return OperationResult.CreateFailure(mult.ErrorMessage);
                    }
                }

                return OperationResult.CreateSuccess(result);
            }

            if (left is FractionToken && right is VariableToken)
            {
                return OperationResult.CreateFailure("Variable exponents not supported.");
            }

            if (left is FractionToken && right is ExpressionToken)
            {
                return OperationResult.CreateFailure("Expression exponents not supported.");
            }

            if (left is FractionToken && right is FractionToken)
            {
                return OperationResult.CreateFailure("Fraction exponents not supported.");
            }

            return OperationResult.CreateFailure($"Unknown token pair for exponentiation rule. Left: {left}, Right: {right}");
        }

        public static ValueToken Order(Token t)
        {
            if (t is ExpressionToken ex)
            {
                var memsNoSign = ex.Members.Where(x => x is IExpressionMemberComparable).ToList();

                memsNoSign = memsNoSign.OrderByDescending(x => x as IExpressionMemberComparable, new ExpressionMemberComparer()).ToList();

                var orderedMembers = new List<Token>();

                foreach (var member in memsNoSign)
                {
                    orderedMembers.Add(member);

                    if (memsNoSign.IndexOf(member) != memsNoSign.Count - 1)
                    {
                        orderedMembers.Add(new BinaryOperationToken { Value = "+" });
                    }
                }

                return new ExpressionToken(orderedMembers);
            }
            else
            {
                return (ValueToken)t;
            }
        }
    }
}
