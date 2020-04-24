using NUnit.Framework;
using Yet_Another_Simplifier;

namespace Tests
{
    [TestFixture]
    public class SimplifierTests
    {
        [TestCase("z+2xy^2+3yx^2-3=xy^2+yx^2+4+f", "z+xy^2+2x^2y-7-f=0")]
        public void Test1(string input, string output)
        {
            var parser = new Parser(input);
            Assert.AreEqual(parser.ParseAndSimplify().ToString(), output);
        }
    }
}