using NUnit.Framework;
using System.Collections.Generic;
using Yet_Another_Simplifier;
using Yet_Another_Simplifier.Tokens;

namespace Tests
{
    [TestFixture]
    public class SimplifierTests
    {
        [TestCase("x+x", "2x")]
        [TestCase("x+y", "x+y")]
        [TestCase("x-x", "0")]
        [TestCase("x-y", "x-y")]
        [TestCase("x*x", "x^2")]
        [TestCase("x*y", "xy")]
        [TestCase("x/x", "1")]
        [TestCase("x/y", "[x/y]")]
        [TestCase("x^x", "Variable exponents not supported.")]
        [TestCase("x^y", "Variable exponents not supported.")]
        [TestCase("x/0", "Cannot divide by 0.")]
        [TestCase("X+x", "Unknown symbol: 'X'")]
        [TestCase("(x+y)/2", "0.5x+0.5y")]
        [TestCase("xy/az+7/a^2", "[xy/az]+[7/a^2]")]
        [TestCase("xy/(az+7)/a^2", "[xy/(a^3)z+7a^2]")]

        [TestCase("x^2+3.5xy+y=-4+y^2-yx+y", "x^2-y^2+4.5xy+4=0")]
        [TestCase("z+2xy^2+3yx^2-3=xy^2+yx^2+4+f", "2(x^2)y+x(y^2)-f+z-7=0")]
        [TestCase("-2x(a+b)(x-y^2)^1=1", "ax(y^2)+bx(y^2)-a(x^2)-b(x^2)-0.5=0")]
        [TestCase("x=y=0=^2", "More than one equal sign in equation.")]
        [TestCase("0=^2", "Invalid syntax: binary sign cannot go after a binary sign a left parenthesis or an equal sign")]
        
        public void ParseAndSimplify_AnyInput_ExpectedOutput(string input, string output)
        {
            AppSettings.ConsiderConstantsInGcd = false;

            var parser = new Parser(input);
            Assert.AreEqual(parser.ParseAndSimplify().ToString(), output);
        }

        [TestCase("ab(x^2)+a(x^2)")]
        public void Order_UnOrderedMembers_OrderedMembers(string output)
        {
            var x = Simplifier.Order(
                new ExpressionToken(
                    new List<Token> {
                        new VariableToken(1, new List<Variable> {
                            new Variable('a', 1), new Variable('x', 2), }),
                        new BinaryOperationToken{Value = "+"},
                        new VariableToken(1, new List<Variable> {
                            new Variable('a', 1), new Variable('b', 1), new Variable('x', 2), })
                        }
                    ));

            Assert.AreEqual(Simplifier.Order(x).ToString(), output);
        }

        [TestCase(3.5, 1, 1)]
        [TestCase(2.4, 4.8, 2.4)]
        public void GreatestCommonDivisor_Decimal_Decimal_RealGcd(decimal x, decimal y, decimal result)
        {
            var gcd = Utility.GreatestCommonDivisor(x, y);
            Assert.AreEqual(gcd, result);
        }
    }
}