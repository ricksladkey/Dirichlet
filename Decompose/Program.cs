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
            Radix32Test1();
            //FactorTest1();
            //FactorTest2();
            //MulModTest1();
        }

        static void Radix32Test1()
        {
            for (int i = 0; i < 2; i++)
            {
                Radix32Test1("sum:     ", (c, a, b) => c.SetSum(a, b), (a, b) => a + b);
                Radix32Test1("product: ", (c, a, b) => c.SetProduct(a, b), (a, b) => a * b);
            }
        }

        static void Radix32Test1(string label,
            Action<Radix32Integer, Radix32Integer, Radix32Integer> operation1,
            Func<BigInteger, BigInteger, BigInteger> operation2)
        {
            var n = BigInteger.Parse("10023859281455311421");
            var length = (BigIntegerUtils.GetBitLength(n) * 2 + 31) / 32;
            var random1 = new MersenneTwister32(0);
            var random2 = new MersenneTwister32(0);
            var timer1 = new Stopwatch();
            var timer2 = new Stopwatch();
            var iterations1 = 1000;
            var iterations2 = 1000;

            timer1.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var bits = new uint[3 * length];
                var a = new Radix32Integer(bits, 0 * length, length);
                var b = new Radix32Integer(bits, 1 * length, length);
                var c = new Radix32Integer(bits, 2 * length, length);
                a.Set(random1.Next(n));
                b.Set(random1.Next(n));

                for (int j = 0; j < iterations2; j++)
                    operation1(c, a, b);
            }
            var elapsed1 = timer1.ElapsedMilliseconds;

            timer2.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var a = random2.Next(n);
                var b = random2.Next(n);
                var c = BigInteger.Zero;

                for (int j = 0; j < iterations2; j++)
                    c = operation2(a, b);
            }
            var elapsed2 = timer1.ElapsedMilliseconds;

            Console.WriteLine("{0}: elapsed1 = {1}, elapsed2 = {2}", label, elapsed1, elapsed2);
        }

        static void FactorTest1()
        {
            var algorithm = new PollardRhoMontgomery(4);
            var n = BigInteger.Parse("10023859281455311421");
            int iterations = 25;
            var elapsed = new double[iterations];
            for (int i = 0; i < iterations; i++)
            {
                GC.Collect();
                var timer = new Stopwatch();
                timer.Start();
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
            var algorithm = new PollardRhoMontgomery(4);
            var n = BigInteger.Parse("10023859281455311421");
            int iterations = 250;
            for (int i = 0; i < iterations; i++)
            {
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
