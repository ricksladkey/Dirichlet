using System;
using System.Linq;
using System.Numerics;
using Decompose.Numerics;
using System.Diagnostics;

namespace Decompose
{
    class Program
    {
        static void Main(string[] args)
        {
            int iterations = 25;
            var elapsed = new double[iterations];
            for (int i = 0; i < iterations; i++)
            {
                GC.Collect();
                var timer = new Stopwatch();
                timer.Start();
                var n = BigInteger.Parse("10023859281455311421");
                var factors = MathUtils.FactorPollard(n).OrderBy(factor => factor).ToArray();
                var product = factors.Aggregate((sofar, current) => sofar * current);
                if (factors.Length != 2)
                    throw new InvalidOperationException();
                if (n != product)
                    throw new InvalidOperationException();
                elapsed[i] = timer.ElapsedMilliseconds;
                Console.WriteLine("elapsed = {0}", elapsed[i]);
            }
            var total = elapsed.Aggregate((sofar, current) => sofar + current);
            Console.WriteLine("{0} iterations in {1} msec, {2} msec/iteration", iterations, total, total / iterations);
        }
    }
}
