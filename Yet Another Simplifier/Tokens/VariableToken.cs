using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Yet_Another_Simplifier.Tokens
{
    public class VariableToken : ExpressionMemberToken, IHasNumericValue
    {
        public VariableToken(decimal quotient, List<Variable> variables)
        {
            Quotient = quotient;
            Variables = variables;
        }

        public decimal Quotient { get; set; }
        public List<Variable> Variables { get; set; }
        public override ExpressionMemberPrecedence Precedence { get => ExpressionMemberPrecedence.Variable; }

        public decimal NumericValue { get => Quotient; set => Quotient = value; }

        public decimal GetNumericValue()
        {
            return Quotient;
        }

        public void SetNumericValue(decimal value)
        {
            Quotient = value;
        }

        public override void NegateValue()
        {
            Quotient *= -1;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Quotient == 1)
            {
                sb.Append(string.Empty);
            }
            else if (Quotient == -1)
            {
                sb.Append("-");
            }
            else
            {
                sb.Append(Quotient.ToString());
            }

            Variables = Variables.OrderBy(x => x.Letter).ToList();

            foreach (var variable in Variables)
            {
                sb.Append(variable.ToString());
            }

            var result = sb.ToString();

            return result;
        }

        public override Token Clone()
        {
            return new VariableToken(Quotient, Variables);
        }

        public override decimal GreatestCommonDivisor()
        {
            return Quotient < 0 ? Quotient * -1 : Quotient;
        }

        public override int CompareTo(ExpressionMemberToken other)
        {
            if (other is ConstantToken c)
            {
                return 1;
            }

            if (other is VariableToken v)
            {
                var selfExponentIndex = ExponentIndex(this);
                var otherExponentIndex = ExponentIndex(v);

                if (selfExponentIndex > otherExponentIndex)
                {
                    return 1;
                }
                else if (selfExponentIndex < otherExponentIndex)
                {
                    return -1;
                }
                else
                {
                    if (Variables.Count > v.Variables.Count)
                    {
                        return 1;
                    }
                    else if (Variables.Count < v.Variables.Count)
                    {
                        return -1;
                    }
                    else
                    {
                        var selfLetterIndex = LetterIndex(this);
                        var otherLetterIndex = LetterIndex(v);

                        if (selfLetterIndex < otherLetterIndex)
                        {
                            return 1;
                        }
                        else if (selfLetterIndex > otherLetterIndex)
                        {
                            return -1;
                        }
                        else
                        {
                            if (NumericValue > v.NumericValue)
                            {
                                return 1;
                            }
                            else if (NumericValue < v.NumericValue)
                            {
                                return -1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }                    
                }
            }

            if (other is FractionToken f)
            {
                return -1;
            }

            throw new Exception("Unknown comparison type.");
        }

        private int LetterIndex(VariableToken v)
        {
            return v.Variables.Select(x => (int)x.Letter).Aggregate((a, b) => a + b);
        }

        private decimal ExponentIndex(VariableToken v)
        {
            return v.Variables.Select(x => x.Exponent).Aggregate((a, b) => a * b);
        }
    }
}
