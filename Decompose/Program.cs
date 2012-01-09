using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using Decompose.Numerics;
using System.Reflection;

namespace Decompose
{
    class Program
    {
        static void Main(string[] args)
        {
            try
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
                //FactorTest6();
                //QuadraticSieveParametersTest();
                QuadraticSieveDigitsTest();
                //CunnihamTest();
                //GaussianEliminationTest1();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
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
            Action<Word32Integer, Word32Integer, Word32Integer> operation1,
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
                var store = new Word32IntegerStore(length);
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

            //FactorTest(debug, 25, n, new PollardRhoBrent(threads, 0));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new BigIntegerReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new Radix32IntegerReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new BarrettReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));

            FactorTest(debug, 100, n, new PollardRhoBrent(threads, 0));
            FactorTest(debug, 100, n, new PollardRhoReduction(threads, 0, new Word32IntegerReduction()));
            FactorTest(debug, 100, n, new PollardRhoReduction(threads, 0, new BarrettReduction()));
            FactorTest(debug, 100, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));

            //FactorTest(debug, 500, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));
        }

        static void FactorTest2()
        {
            var p = BigInteger.Parse("287288745765902964785862069919080712937");
            var q = BigInteger.Parse("7660450463");
            var n = p * q;
            int threads = 4;
            bool debug = false;
            FactorTest(debug, 25, n, new PollardRhoBrent(threads, 0));
            FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new Word32IntegerReduction()));
            FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new BarrettReduction()));
            FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));
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
                //factors = FactorTest(true, 1, n, new PollardRho(threads, 0));
                factors = FactorTest(true, 5, n, new PollardRhoReduction(threads, 0, new Word32IntegerReduction()));
                //factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, 0, new BarrettReduction()));
                //factors = FactorTest(true, 5, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));
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
                //factors = FactorTest(true, 1, n, new PollardRho(threads, 0));
                //factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, 0, new Radix32IntegerReduction()));
                //factors = FactorTest(true, 10, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));
                factors = FactorTest(true, 1, n, new QuadraticSieve(new QuadraticSieve.Config { Threads = threads }));
            }
        }

        static void FactorTest5()
        {
            //var n = BigInteger.Parse("87463");
            //var n = BigInteger.Parse("10023859281455311421");
            var n = BigInteger.Parse("5382000000735683358022919837657883000000078236999000000000000063"); // https://sites.google.com/site/shouthillgc/Home/gc1p8qn/factorizing-tool
            const int threads = 8;
            bool debug = false;

            Console.WriteLine("n = {0}", n);
            //FactorTest(debug, 500, n, new PollardRhoReduction(pollardThreads, new MontgomeryReduction()));
            var config = new QuadraticSieve.Config
            {
                Threads = threads,
                //FactorBaseSize = 24000,
                //LowerBoundPercent = 35,
                Multiplier = 3,
                Diagnostics = QuadraticSieve.Diag.Verbose,
            };
            var factors = FactorTest(debug, 1, n, new QuadraticSieve(config));
            foreach (var factor in factors)
                Console.WriteLine("{0}", factor);
        }

        static void FactorTest6()
        {
            var n = BigInteger.Parse("18446744073709551617");
            //var n = BigInteger.Parse("12345678901");
            FactorTest(false, 100, n, new QuadraticSieve(new QuadraticSieve.Config { Threads = 8 }));
        }

        static void QuadraticSieveParametersTest()
        {
            var random = new MersenneTwister32(0);
            int threads = 8;
            for (int i = 10; i <= 30; i++)
            {
                var limit = BigInteger.Pow(10, i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                if (i < 30)
                    continue;
                Console.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                for (int size = 12000; size <= 24000; size += 1000)
                {
                    Console.WriteLine("size = {0}", size);
                    for (int percent = 80; percent <= 80; percent += 5)
                    {
                        Console.WriteLine("percent = {0}", percent);
                        var config = new QuadraticSieve.Config
                        {
                            Threads = threads,
                            FactorBaseSize = size,
                            LowerBoundPercent = percent,
                            //Diagnostics = QuadraticSieve.Diag.Verbose,
                        };
                        FactorTest(false, 1, n, new QuadraticSieve(config));
                    }
                }
                break;
            }
        }

        static void QuadraticSieveDigitsTest()
        {
            var random = new MersenneTwister32(0);
            int threads = 8;
            for (int i = 10; i <= 30; i++)
            {
                var limit = BigInteger.Pow(10, i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                Console.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                FactorTest(false, 1, n, new QuadraticSieve(new QuadraticSieve.Config { Threads = threads }));
            }
        }

        static void CunnihamTest()
        {
            var n = BigInteger.Pow(3, 225) - 1;
            Console.WriteLine("n = {0}", n);
            var pollard = new PollardRhoReduction(4, 1000000, new MontgomeryReduction());
            var smallFactors = pollard.Factor(n);
            foreach (var factor in smallFactors)
                Console.WriteLine("{0}", factor);
            var c = n / smallFactors.Aggregate((sofar, factor) => sofar * factor);
            Console.WriteLine("c = {0}", c);
            var qr = new QuadraticSieve(new QuadraticSieve.Config { Threads = 8 });
            var factors = qr.Factor(c);
            foreach (var factor in factors)
                Console.WriteLine("{0}", factor);
        }

        static void GaussianEliminationTest1()
        {
            var threads = 8;
            var file = @"..\..\..\..\matrix-18401.txt.gz";
            var lines = GetLinesGzip(file);
            var timer = new Stopwatch();

#if false
            var solver = new GaussianElimination<Word64BitArray>(threads);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<Word64BitMatrix>);
#else
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<Word64BitMatrix>);
#endif

            timer.Restart();
            var matrix = getter(lines);
            Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            Console.WriteLine("Rows = {0}, Cols = {1}", matrix.Rows, matrix.Cols);

            timer.Restart();
            var solutions = solver.Solve(matrix).ToArray();
            Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);

            Console.WriteLine("solutions = {0}", solutions.Length);
        }


        private static string[] GetLinesGzip(string file)
        {
            using (var stream = new StreamReader(new GZipStream(File.OpenRead(file), CompressionMode.Decompress)))
            {
                var lines = new List<string>();
                while (true)
                {
                    var line = stream.ReadLine();
                    if (line == null)
                        break;
                    lines.Add(line);
                }
                return lines.ToArray();
            }
        }

        private static IBitMatrix GetBitMatrix<TMatrix>(string[] lines) where TMatrix : IBitMatrix
        {
            int rows = lines.Length;
            int cols = lines[0].Length;
            var matrix = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), rows, cols);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j] == '1')
                        matrix[i, j] = true;
                }
            }
            return matrix;
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
            {
                results.Add(algorithm.Factor(n).OrderBy(factor => factor).ToArray());
            }
            var elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000;
            foreach (var factors in results)
            {
                if (factors.Length < 2)
                    throw new InvalidOperationException("too few factors");
                var product = factors.Aggregate((sofar, current) => sofar * current);
                if (factors.Any(factor => factor == BigInteger.One || factor == n || !IntegerMath.IsPrime(factor)))
                    throw new InvalidOperationException("invalid factor");
                if (n != product)
                    throw new InvalidOperationException("validation failure");
            }
            Console.WriteLine("{0} iterations in {1:F0} msec, {2:F3} msec/iteration", iterations, elapsed, elapsed / iterations);
            return results[0];
        }
    }
}
