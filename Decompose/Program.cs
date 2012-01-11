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
                //CunninghamTest();
                //GaussianEliminationTest1();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
        }

        static void FindPrimeTest1()
        {
            var random = new MersenneTwisterBigInteger(0);
            var limit = BigInteger.One << (32 * 4);
            var x = random.Next(limit);
            while (!IntegerMath.IsPrime(x))
                ++x;
            Console.WriteLine("x = {0}", x);
        }

        static void BarrettReductionTest1()
        {
            var p = BigInteger.Parse("10023859281455311421");
            var random = new MersenneTwisterBigInteger(0);
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
            var random1 = new MersenneTwisterBigInteger(0);
            var random2 = new MersenneTwisterBigInteger(0);
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
            var random1 = new MersenneTwisterBigInteger(0);
            var random2 = new MersenneTwisterBigInteger(0);
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
            var random = new MersenneTwisterBigInteger(0);
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
                //LowerBoundPercent = 65,
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
            var random = new MersenneTwisterBigInteger(0);
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

        static void CreateSamplesTest()
        {
            var random = new MersenneTwisterBigInteger(0);
            for (int i = 10; i <= 40; i++)
            {
                var limit = BigInteger.Pow(10, i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                Console.WriteLine("new SampleComposite");
                Console.WriteLine("{");
                Console.WriteLine("    Digits = {0},", 2 * i);
                Console.WriteLine("    P = BigInteger.Parse(\"{0}\"),", p);
                Console.WriteLine("    Q = BigInteger.Parse(\"{0}\"),", q);
                Console.WriteLine("},");
            }
        }

        static void QuadraticSieveDigitsTest()
        {
            for (int i = 10; i <= 30; i++)
            {
                var sample = samples[i - 10];
                var p = sample.P;
                var q = sample.Q;
                var n = p * q;
                Console.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                var config = new QuadraticSieve.Config
                {
                    Threads = 8,
                    //Diagnostics = QuadraticSieve.Diag.Verbose,
                    ReportingInterval = 10,
                };
                FactorTest(false, 1, n, new QuadraticSieve(config));
            }
        }

        static void CunninghamTest()
        {
            var n = BigInteger.Pow(3, 225) - 1;
            Console.WriteLine("n = {0}", n);
            var algorithm = new HybridPollardRhoQuadraticSieve(4, 1000000, new QuadraticSieve.Config());
            foreach (var factor in algorithm.Factor(n))
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

        private static BigInteger NextPrime(IRandomNumberAlgorithm<BigInteger> random, BigInteger limit)
        {
            var digits = IntegerMath.GetDigitLength(limit - 1, 10);
            var n = random.Next(limit);
            while (IntegerMath.GetDigitLength(n, 10) < digits)
                n = random.Next(limit);
            return IntegerMath.NextPrime(n);
        }

        private static BigInteger[] FactorTest(bool debug, int iterations, BigInteger n, IFactorizationAlgorithm<BigInteger> algorithm)
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

        private class SampleComposite
        {
            public int Digits { get; set; }
            public BigInteger P { get; set; }
            public BigInteger Q { get; set; }
            public BigInteger N { get { return P * Q; } }
        }

        private static SampleComposite[] samples =
        {
            new SampleComposite
            {
                Digits = 20,
                P = BigInteger.Parse("6359727791"),
                Q = BigInteger.Parse("4501387931"),
            },
            new SampleComposite
            {
                Digits = 22,
                P = BigInteger.Parse("81112462157"),
                Q = BigInteger.Parse("65534533327"),
            },
            new SampleComposite
            {
                Digits = 24,
                P = BigInteger.Parse("922920006683"),
                Q = BigInteger.Parse("718097069881"),
            },
            new SampleComposite
            {
                Digits = 26,
                P = BigInteger.Parse("9752697519233"),
                Q = BigInteger.Parse("6069293365573"),
            },
            new SampleComposite
            {
                Digits = 28,
                P = BigInteger.Parse("68645165989399"),
                Q = BigInteger.Parse("16044355135579"),
            },
            new SampleComposite
            {
                Digits = 30,
                P = BigInteger.Parse("600557957340779"),
                Q = BigInteger.Parse("931078023997103"),
            },
            new SampleComposite
            {
                Digits = 32,
                P = BigInteger.Parse("3860645359479661"),
                Q = BigInteger.Parse("6660723000417049"),
            },
            new SampleComposite
            {
                Digits = 34,
                P = BigInteger.Parse("92857397119223941"),
                Q = BigInteger.Parse("56396174139495167"),
            },
            new SampleComposite
            {
                Digits = 36,
                P = BigInteger.Parse("589014875010582533"),
                Q = BigInteger.Parse("736526309666503507"),
            },
            new SampleComposite
            {
                Digits = 38,
                P = BigInteger.Parse("7581008776610031599"),
                Q = BigInteger.Parse("4941861830454018857"),
            },
            new SampleComposite
            {
                Digits = 40,
                P = BigInteger.Parse("43760554345460349857"),
                Q = BigInteger.Parse("59780183007898320881"),
            },
            new SampleComposite
            {
                Digits = 42,
                P = BigInteger.Parse("166737023217956147843"),
                Q = BigInteger.Parse("266578926331401913067"),
            },
            new SampleComposite
            {
                Digits = 44,
                P = BigInteger.Parse("2341864285495243696819"),
                Q = BigInteger.Parse("6862234825035212041109"),
            },
            new SampleComposite
            {
                Digits = 46,
                P = BigInteger.Parse("81173737403430771942307"),
                Q = BigInteger.Parse("64443839329396315580339"),
            },
            new SampleComposite
            {
                Digits = 48,
                P = BigInteger.Parse("547109861053076293701181"),
                Q = BigInteger.Parse("218393091309815530058033"),
            },
            new SampleComposite
            {
                Digits = 50,
                P = BigInteger.Parse("3150497848572778166077033"),
                Q = BigInteger.Parse("1419806924383975860451933"),
            },
            new SampleComposite
            {
                Digits = 52,
                P = BigInteger.Parse("71553049119880264914826637"),
                Q = BigInteger.Parse("47084627876852469799938151"),
            },
            new SampleComposite
            {
                Digits = 54,
                P = BigInteger.Parse("668223788916828667974184073"),
                Q = BigInteger.Parse("475483409844077547574254721"),
            },
            new SampleComposite
            {
                Digits = 56,
                P = BigInteger.Parse("8816136275150582711618365859"),
                Q = BigInteger.Parse("3044375869342823579225836457"),
            },
            new SampleComposite
            {
                Digits = 58,
                P = BigInteger.Parse("79616313635618041020736352419"),
                Q = BigInteger.Parse("36109767180534668555739700277"),
            },
            new SampleComposite
            {
                Digits = 60,
                P = BigInteger.Parse("289209603456915754116025390283"),
                Q = BigInteger.Parse("651974473535965720429005879311"),
            },
            new SampleComposite
            {
                Digits = 62,
                P = BigInteger.Parse("2135862096280413518185169595631"),
                Q = BigInteger.Parse("4946257487231393668368584144179"),
            },
            new SampleComposite
            {
                Digits = 64,
                P = BigInteger.Parse("90484786924920304861107250841203"),
                Q = BigInteger.Parse("93819551150572322228605590151427"),
            },
            new SampleComposite
            {
                Digits = 66,
                P = BigInteger.Parse("752171408379662548633208575616947"),
                Q = BigInteger.Parse("727656629739503749143743912576693"),
            },
            new SampleComposite
            {
                Digits = 68,
                P = BigInteger.Parse("1452509806874688515960900257523821"),
                Q = BigInteger.Parse("6091273916310546401352905015178757"),
            },
            new SampleComposite
            {
                Digits = 70,
                P = BigInteger.Parse("39590771509308181367548580586664897"),
                Q = BigInteger.Parse("10420963447932920373972059082256531"),
            },
            new SampleComposite
            {
                Digits = 72,
                P = BigInteger.Parse("780748679899154536912466202699047849"),
                Q = BigInteger.Parse("379148146974432114134205653148097117"),
            },
            new SampleComposite
            {
                Digits = 74,
                P = BigInteger.Parse("1944657516979563550209812173755027707"),
                Q = BigInteger.Parse("8281713719538507681810654356958903461"),
            },
            new SampleComposite
            {
                Digits = 76,
                P = BigInteger.Parse("20221947695032137682483252436640950611"),
                Q = BigInteger.Parse("90083006931593945195319860191440222337"),
            },
            new SampleComposite
            {
                Digits = 78,
                P = BigInteger.Parse("234578372300126118738414197724923928917"),
                Q = BigInteger.Parse("789827369472404641442870779900199537177"),
            },
            new SampleComposite
            {
                Digits = 80,
                P = BigInteger.Parse("4866868528098421482780624850039639204477"),
                Q = BigInteger.Parse("7613720546717210496574998828026372396927"),
            },
        };
    }
}
