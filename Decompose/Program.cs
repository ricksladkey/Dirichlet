using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Decompose.Numerics;

namespace Decompose
{
    class Program
    {
        static void Main(string[] args)
        {
            //FindPrimeTest1();
            //BarrettReductionTest1();
            //BarrettReductionTest2();
            //Radix32Test1();
            //FactorTest1();
            //FactorTest2();
            //FactorTest3();
            //FactorTest4();
            //FactorTest5();
            FactorTest6();
            //FactorTest7();
        }

        static void FindPrimeTest1()
        {
            var random = new MersenneTwister32(0);
            var limit = BigInteger.One << (32 * 4);
            var x = random.Next(limit);
            while (!IntegerMath.IsPrime(x))
                ++x;
            Console.WriteLine("x = {0}", x);
        }

        static void BarrettReductionTest1()
        {
            var p = BigInteger.Parse("10023859281455311421");
            var random = new MersenneTwister32(0);
            var x = random.Next(p);
            var y = random.Next(p);
            var z = x * y;
            var expected = z % p;
            var bLength = 32;
            var b = BigInteger.One << bLength;
            var k = (p.GetBitLength() - 1) / bLength + 1;
            var mu = BigInteger.Pow(b, 2 * k) / p;
            var bToTheKPlusOne = BigInteger.Pow(b, k + 1);

            var qhat = (z >> (bLength * (k - 1))) * mu >> (bLength * (k + 1));
            var r = z % bToTheKPlusOne - qhat * p % bToTheKPlusOne;
            if (r.Sign == -1)
                r += bToTheKPlusOne;
            while (r >= p)
                r -= p;
            if (r != expected)
                throw new InvalidOperationException();

            var reducer = new BarrettReduction().GetReducer(p);
            var actual = reducer.ToResidue(z).ToBigInteger();
            if (actual != expected)
                throw new InvalidOperationException();
        }

        static void BarrettReductionTest2()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var random1 = new MersenneTwister32(0);
            var random2 = new MersenneTwister32(0);
            var timer1 = new Stopwatch();
            var timer2 = new Stopwatch();
            var iterations1 = 1000;
            var iterations2 = 1000;
            var reducer = new BarrettReduction().GetReducer(n);

            timer1.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var a = reducer.ToResidue(random1.Next(n));
                var b = reducer.ToResidue(random1.Next(n));
                var c = reducer.ToResidue(0);

                for (int j = 0; j < iterations2; j++)
                    c.Set(a).Multiply(b);
            }
            var elapsed1 = timer1.ElapsedMilliseconds;

            timer2.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var a = random2.Next(n);
                var b = random2.Next(n);
                var c = BigInteger.Zero;

                for (int j = 0; j < iterations2; j++)
                {
                    c = a * b;
                    c %= n;
                }
            }
            var elapsed2 = timer1.ElapsedMilliseconds;

            Console.WriteLine("elapsed1 = {0}, elapsed2 = {1}", elapsed1, elapsed2);
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
            var length = (n.GetBitLength() * 2 + 31) / 32;
            var random1 = new MersenneTwister32(0);
            var random2 = new MersenneTwister32(0);
            var timer1 = new Stopwatch();
            var timer2 = new Stopwatch();
            var iterations1 = 1000;
            var iterations2 = 1000;

            timer1.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var store = new Radix32Store(length);
                var a = store.Create();
                var b = store.Create();
                var c = store.Create();
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
            var n = BigInteger.Parse("10023859281455311421");
            int threads = 4;
            bool debug = false;

            //FactorTest(debug, 25, n, new PollardRhoBrent(threads));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, new BigIntegerReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, new Radix32IntegerReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, new BarrettReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, new MontgomeryReduction()));

            FactorTest(debug, 100, n, new PollardRhoBrent(threads));
            FactorTest(debug, 100, n, new PollardRhoReduction(threads, new Radix32IntegerReduction()));
            FactorTest(debug, 100, n, new PollardRhoReduction(threads, new BarrettReduction()));
            FactorTest(debug, 100, n, new PollardRhoReduction(threads, new MontgomeryReduction()));

            //FactorTest(debug, 500, n, new PollardRhoReduction(threads, new MontgomeryReduction()));
        }

        static void FactorTest2()
        {
            var p = BigInteger.Parse("287288745765902964785862069919080712937");
            var q = BigInteger.Parse("7660450463");
            var n = p * q;
            int threads = 4;
            bool debug = false;
            FactorTest(debug, 25, n, new PollardRhoBrent(threads));
            FactorTest(debug, 25, n, new PollardRhoReduction(threads, new Radix32IntegerReduction()));
            FactorTest(debug, 25, n, new PollardRhoReduction(threads, new BarrettReduction()));
            FactorTest(debug, 25, n, new PollardRhoReduction(threads, new MontgomeryReduction()));
        }

        static void FactorTest3()
        {
            for (int i = 214; i <= 214; i++)
            {
                var plus = true;
                var n = (BigInteger.One << i) + (plus ? 1 : -1);
                Console.WriteLine("i = {0}, n = {1}", i, n);
                if (IntegerMath.IsPrime(n))
                    continue;
                int threads = 4;
                var factors = null as BigInteger[];
                //factors = FactorTest(true, 1, n, new PollardRho(threads));
                factors = FactorTest(true, 5, n, new PollardRhoReduction(threads, new Radix32IntegerReduction()));
                //factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, new BarrettReduction()));
                //factors = FactorTest(true, 5, n, new PollardRhoReduction(threads, new MontgomeryReduction()));
                foreach (var factor in factors)
                    Console.WriteLine("{0}", factor);
            }

        }

        static void FactorTest4()
        {
            var random = new MersenneTwister32(0);
            for (int i = 16; i <= 16; i++)
            {
                var limit = BigInteger.Pow(new BigInteger(10), i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                Console.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                int threads = 8;
                var factors = null as BigInteger[];
                //factors = FactorTest(true, 1, n, new PollardRho(threads));
                //factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, new Radix32IntegerReduction()));
                //factors = FactorTest(true, 10, n, new PollardRhoReduction(threads, new MontgomeryReduction()));
                factors = FactorTest(true, 1, n, new QuadraticSieve(threads, 0, 0));
            }
        }

        static void FactorTest5()
        {
            //var n = BigInteger.Parse("87463");
            var n = BigInteger.Parse("10023859281455311421");
            const int pollardThreads = 4;
            const int quadraticSieveThreads = 8;
            bool debug = false;

            //FactorTest(debug, 500, n, new PollardRhoReduction(pollardThreads, new MontgomeryReduction()));
            FactorTest(debug, 1, n, new QuadraticSieve(quadraticSieveThreads, 0, 0));
        }

        static void FactorTest6()
        {
            var random = new MersenneTwister32(0);
            int threads = 8;
            for (int i = 25; i <= 25; i++)
            {
                var limit = BigInteger.Pow(10, i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
#if true
                if (i < 18)
                    continue;
#endif
                var n = p * q;
                Console.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                FactorTest(false, 1, n, new QuadraticSieve(threads, 0, 0));
            }
        }

        static void FactorTest7()
        {
            var random = new MersenneTwister32(0);
            int threads = 8;
            for (int i = 5; i <= 5; i++)
            {
                var limit = BigInteger.Pow(10, i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                Console.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                for (int size = 10; size <= 60; size += 5)
                {
                    Console.WriteLine("size = {0}", size);
                    for (int percent = 85; percent <= 85; percent += 1)
                    {
                        Console.WriteLine("percent = {0}", percent);
                        FactorTest(false, 1000, n, new QuadraticSieve(threads, size, percent));
                    }
                }
            }
        }

        static BigInteger NextPrime(MersenneTwister32 random, BigInteger limit)
        {
            var n = random.Next(limit);
            while (GetDigitLength(n, 10) < GetDigitLength(limit - 1, 10))
                n = random.Next(limit);
            while (!IntegerMath.IsPrime(n))
                ++n;
            return n;
        }

        static int GetDigitLength(BigInteger n, int b)
        {
            int i = 0;
            while (!n.IsZero)
            {
                ++i;
                n /= b;
            }
            return i;
        }

        static BigInteger[] FactorTest(bool debug, int iterations, BigInteger n, IFactorizationAlgorithm<BigInteger> algorithm)
        {
            var results = new List<BigInteger[]>();
            GC.Collect();
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < iterations; i++)
                results.Add(algorithm.Factor(n).OrderBy(factor => factor).ToArray());
            var elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000;
            foreach (var factors in results)
            {
                var product = factors.Aggregate((sofar, current) => sofar * current);
                if (factors.Any(factor => factor == BigInteger.One || factor == n || !IntegerMath.IsPrime(factor)))
                    throw new InvalidOperationException("invalid factor");
                if (factors.Length < 2)
                    throw new InvalidOperationException("too few factors");
                if (n != product)
                    throw new InvalidOperationException("validation failure");
            }
            Console.WriteLine("{0} iterations in {1:F0} msec, {2:F3} msec/iteration", iterations, elapsed, elapsed / iterations);
            return results[0];
        }
    }
}
