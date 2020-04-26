using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Yet_Another_Simplifier.Interfaces;

namespace Yet_Another_Simplifier.Tokens
{
    public class ExpressionToken : ValueToken, IEliminatable
    {
        public ExpressionToken(List<Token> members)
        {
            Members = members;
        }

        public List<Token> Members { get; set; }
        
        public override Token Clone()
        {
            var newMembers = new List<Token>();

            foreach (var member in Members)
            {
                newMembers.Add(member.Clone());
            }

            return new ExpressionToken(newMembers);
        }

        public ValueToken Eliminate(decimal value)
        {
            var list = new List<Token>();

            foreach (var member in Members)
            {
                if (member is BinaryOperationToken)
                {
                    list.Add(member);
                }
                else
                {
                    list.Add(((IEliminatable)member).Eliminate(value));
                }

            }

            return new ExpressionToken(list);
        }

        public override decimal GreatestCommonDivisor()
        {
            if (AppSettings.ConsiderConstantsInGcd)
            {
                return Utility.GreatestCommonDivisor(Members.Where(x => x is IHasNumericValue).Select(x => ((ValueToken)x).GreatestCommonDivisor()));
            }
            else
            {
                return Utility.GreatestCommonDivisor(Members.Where(x => x is VariableToken).Select(x => ((ValueToken)x).GreatestCommonDivisor()));
            }
        }

        public override void Negate()
        {
            foreach (var member in Members)
            {
                if (member is BinaryOperationToken)
                {
                    continue;
                }

                ((ValueToken)member).Negate();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var member in Members)
            {
                sb.Append($"{member.ToString()}");
            }

            var result = sb.ToString();

            result = Regex.Replace(result, @"\+-", "-");

            return result;
        }
    }
}
