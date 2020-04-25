using NUnit.Framework;
using Yet_Another_Simplifier;

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

        [TestCase("x^2+3.5xy+y=-4+y^2-yx+y", "x^2-y^2+4.5xy+4=0")]
        [TestCase("z+2xy^2+3yx^2-3=xy^2+yx^2+4+f", "2x^2y+xy^2-f+z-7=0")]
        public void Test1(string input, string output)
        {
            var parser = new Parser(input);
            Assert.AreEqual(parser.ParseAndSimplify().ToString(), output);
        }
    }
}