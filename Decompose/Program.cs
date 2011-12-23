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
            FactorTest1();
            //FactorTest2();
            //MulModTest1();
        }

        static void FactorTest1()
        {
            int iterations = 25;
            var elapsed = new double[iterations];
            for (int i = 0; i < iterations; i++)
            {
                GC.Collect();
                var timer = new Stopwatch();
                timer.Start();
                var n = BigInteger.Parse("10023859281455311421");
                var algorithm = new PollardRho(4);
                var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
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

        static void FactorTest2()
        {
            int iterations = 250;
            for (int i = 0; i < iterations; i++)
            {
                var n = BigInteger.Parse("10023859281455311421");
                var algorithm = new PollardRho(4);
                var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
                var product = factors.Aggregate((sofar, current) => sofar * current);
                if (factors.Length != 2)
                    throw new InvalidOperationException();
                if (n != product)
                    throw new InvalidOperationException();
            }
        }

        static void MulModTest1()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var x = BigInteger.Parse("1234567890123456789");
            if (BigIntegerUtils.AddMod(x, x, n) != (x + x) % n)
                throw new InvalidOperationException();
            int iterations = 1000000;
            GC.Collect();
            var timer1 = new Stopwatch();
            timer1.Start();
            for (int i = 0; i < iterations; i++)
            {
                var x1 = BigIntegerUtils.AddMod(x, x, n);
            }
            var elapsed1 = timer1.ElapsedMilliseconds;
            GC.Collect();
            var timer2 = new Stopwatch();
            timer2.Start();
            for (int i = 0; i < iterations; i++)
            {
                var x1 = (x + x) % n;
            }
            var elapsed2 = timer2.ElapsedMilliseconds;
            Console.WriteLine("elapsed1 = {0}, elapsed2 = {1}", elapsed1, elapsed2);
        }
    }
}
