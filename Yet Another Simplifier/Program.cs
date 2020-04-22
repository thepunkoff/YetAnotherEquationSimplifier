using System;
using Yet_Another_Simplifier.Extensions;

namespace Yet_Another_Simplifier
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var input = Console.ReadLine();

                var parser = new Parser(input);

                var output = parser.ParseAndSimplify();

                if (output == null)
                {
                    continue;
                }

                var result = output.ToString();

                Console.WriteLine(result);
            }
        }
    }
}
