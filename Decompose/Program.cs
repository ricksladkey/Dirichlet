using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Decompose.Numerics;

namespace Decompose
{
    class Program
    {
        static TextWriter output;

        static void Main(string[] args)
        {
            output = new ConsoleLogger("Decompose.log");
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
                //MsieveTest();
                //FactorTest6();
                //QuadraticSieveParametersTest();
                //QuadraticSieveStandardTest();
                //QuadraticSieveDebugTest();
                //QuadraticSieveFactorTest();
                //CunninghamTest();
                //GaussianEliminationTest1();
                //CreateSamplesTest();
                //GraphTest();
                //UInt128Test();
                //ModularInverseTest();
                PrimalityTest();
                //OperationsTest();
            }
            catch (AggregateException ex)
            {
                HandleException(ex);
                ex.Handle(HandleException);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        static bool HandleException(Exception ex)
        {
            output.WriteLine("Exception: {0}", ex.Message);
            output.WriteLine("Stack trace:");
            output.WriteLine(ex.StackTrace);
            return true;
        }

        static void FindPrimeTest1()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
            var limit = BigInteger.One << (32 * 4);
            var x = random.Next(limit);
            while (!IntegerMath.IsPrime(x))
                ++x;
            output.WriteLine("x = {0}", x);
        }

        static void BarrettReductionTest1()
        {
            var p = BigInteger.Parse("10023859281455311421");
            var random = new MersenneTwister(0).Create<BigInteger>();
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
            var actual = reducer.ToResidue(z).Value();
            if (actual != expected)
                throw new InvalidOperationException();
        }

        static void BarrettReductionTest2()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var random1 = new MersenneTwister(0).Create<BigInteger>();
            var random2 = new MersenneTwister(0).Create<BigInteger>();
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

            output.WriteLine("elapsed1 = {0}, elapsed2 = {1}", elapsed1, elapsed2);
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
            var random1 = new MersenneTwister(0).Create<BigInteger>();
            var random2 = new MersenneTwister(0).Create<BigInteger>();
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

            output.WriteLine("{0}: elapsed1 = {1}, elapsed2 = {2}", label, elapsed1, elapsed2);
        }

        static void FactorTest1()
        {
            var n = BigInteger.Parse("10023859281455311421");
            int threads = 1;
            bool debug = false;

            output.WriteLine("bits = {0}", n.GetBitLength());

            //FactorTest(debug, 25, n, new PollardRhoBrent(threads, 0));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new BigIntegerReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new Radix32IntegerReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new BarrettReduction()));
            //FactorTest(debug, 25, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));

            var config = new QuadraticSieve.Config
            {
                DiagnosticsOutput = output,
                IntervalSize = 32 * 1024,
                BlockSize = 32 * 1024,
                Multiplier = 1,
                Diagnostics = QuadraticSieve.Diag.Verbose,
                ThresholdExponent = 1.05,
                ErrorLimit = 1,
                FactorBaseSize = 80,
            };
            for (int i = 0; i < 1; i++)
            {
                output.WriteLine();
                //FactorTest(debug, 100, n, new PollardRhoBrent(threads, 0));
                //FactorTest(debug, 100, n, new PollardRhoReduction(threads, 0, new Word32IntegerReduction()));
                //FactorTest(debug, 100, n, new PollardRhoReduction(threads, 0, new BarrettReduction()));
                //FactorTest(debug, 100, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));
                FactorTest(debug, 1, n, new QuadraticSieve(config));
            }
            config.Diagnostics = QuadraticSieve.Diag.None;
            FactorTest(debug, 1, n, new QuadraticSieve(config));
            FactorTest(debug, 3000, n, new QuadraticSieve(config));
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
                output.WriteLine("i = {0}, n = {1}", i, n);
                if (IntegerMath.IsPrime(n))
                    continue;
                int threads = 4;
                var factors = null as BigInteger[];
                //factors = FactorTest(true, 1, n, new PollardRho(threads, 0));
                factors = FactorTest(true, 5, n, new PollardRhoReduction(threads, 0, new Word32IntegerReduction()));
                //factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, 0, new BarrettReduction()));
                //factors = FactorTest(true, 5, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));
                foreach (var factor in factors)
                    output.WriteLine("{0}", factor);
            }

        }

        static void FactorTest4()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
            for (int i = 5; i <= 16; i++)
            {
                var limit = BigInteger.Pow(new BigInteger(10), i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                int threads = 8;
                var factors = null as BigInteger[];
                //factors = FactorTest(true, 1, n, new PollardRho(threads, 0));
                //factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, 0, new Word32IntegerReduction()));
                factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));
                //factors = FactorTest(true, 1, n, new QuadraticSieve(new QuadraticSieve.Config { Threads = threads }));
            }
        }

        static void MsieveTest()
        {
            //var n = BigInteger.Parse("87463");
            //var n = BigInteger.Parse("10023859281455311421");
            var n = BigInteger.Parse("5382000000735683358022919837657883000000078236999000000000000063"); // https://sites.google.com/site/shouthillgc/Home/gc1p8qn/factorizing-tool
            //var sample = samples[20]; var n = sample.P * sample.Q;

            output.WriteLine("n = {0}", n);
            //FactorTest(debug, 500, n, new PollardRhoReduction(pollardThreads, new MontgomeryReduction()));
            var config = new QuadraticSieve.Config
            {
                DiagnosticsOutput = output,
                Threads = 8,
                //FactorBaseSize = 5400,
                //LowerBoundPercent = 65,
                //IntervalSize = 12 * 32 * 1024,
                //Multiplier = 3,
                Diagnostics = QuadraticSieve.Diag.Verbose,
            };
            var factors = FactorTest(false, 1, n, new QuadraticSieve(config));
            foreach (var factor in factors)
                output.WriteLine("{0}", factor);
        }

        static void FactorTest6()
        {
            var n = BigInteger.Parse("18446744073709551617");
            //var n = BigInteger.Parse("12345678901");
            FactorTest(false, 100, n, new QuadraticSieve(new QuadraticSieve.Config { Threads = 8 }));
        }

        static void QuadraticSieveParametersTest()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
            int i = 50;
            var sample = samples[i];
            var p = sample.P;
            var q = sample.Q;
            var n = sample.N;
            output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
#if true
            for (int size = 100000; size <= 200000; size += 10000)
            {
                var config = new QuadraticSieve.Config
                {
                    DiagnosticsOutput = output,
                    Threads = 8,
                    FactorBaseSize = size,
                    //Diagnostics = QuadraticSieve.Diag.Verbose,
                    ReportingInterval = 60,
                    SieveTimeLimit = 60,
                    ThresholdExponent = 2.25,
                    //LargePrimeOptimization = true,
                    ProcessPartialPartialRelations = true,
                };
                output.WriteLine("size = {0}", size);
                RunParameterTest(config, n);
            }
#endif
#if false
            for (int percent = 80; percent <= 80; percent += 5)
            {
                var config = new QuadraticSieve.Config
                {
                    DiagnosticsOutput = output,
                    Threads = threads,
                    FactorBaseSize = size,
                    LowerBoundPercent = percent,
                    //Diagnostics = QuadraticSieve.Diag.Verbose,
                };
                output.WriteLine("percent = {0}", percent);
                RunParameterTest(config, n);
            }
#endif
#if false
            var blockSize = 32 * 1024;
            for (int size = 4 * blockSize; size <= 32 * blockSize; size += blockSize)
            {
                var config = new QuadraticSieve.Config
                {
                    DiagnosticsOutput = output,
                    Threads = 1,
                    IntervalSize = size,
                    ReportingInterval = 60,
                    SieveTimeLimit = 60,
                    //Diagnostics = QuadraticSieve.Diag.Verbose,
                };
                output.WriteLine("interval size = {0}", size);
                RunParameterTest(config, n);
            }
#endif
        }

        static void RunParameterTest(QuadraticSieve.Config config, BigInteger n)
        {
            if (config.SieveTimeLimit != 0)
            {
                GC.Collect();
                new QuadraticSieve(config).Factor(n).ToArray();
            }
            else
                FactorTest(false, 1, n, new QuadraticSieve(config));
        }

        static void CreateSamplesTest()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
            for (int i = 1; i <= 10; i++)
            {
                var limit = BigInteger.Pow(10, i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                output.WriteLine("new SampleComposite");
                output.WriteLine("{");
                output.WriteLine("    Digits = {0},", 2 * i);
                output.WriteLine("    P = BigInteger.Parse(\"{0}\"),", p);
                output.WriteLine("    Q = BigInteger.Parse(\"{0}\"),", q);
                output.WriteLine("},");
            }
        }

        static void QuadraticSieveStandardTest()
        {
            new QuadraticSieve(new QuadraticSieve.Config()).Factor(samples[10].N).ToArray();
            //new QuadraticSieve(new QuadraticSieve.Config()).Factor(35095264073).ToArray();
            var config = new QuadraticSieve.Config
            {
#if !DEBUG
                Threads = 8,
#endif
                DiagnosticsOutput = output,
                //ProcessPartialPartialRelations = true,
            };
            for (int i = 20; i <= 35; i++)
            {
                var sample = samples[i];
                var p = sample.P;
                var q = sample.Q;
                var n = sample.N;
                output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                //output.WriteLine("n = {0}", n);
                var algorithm = new QuadraticSieve(config);
                FactorTest(false, 1, n, algorithm);
            }
        }

        static void QuadraticSieveDebugTest()
        {
            new QuadraticSieve(new QuadraticSieve.Config()).Factor(samples[10].N).ToArray();
            //new QuadraticSieve(new QuadraticSieve.Config()).Factor(35095264073).ToArray();
            var config = new QuadraticSieve.Config
            {
#if !DEBUG
                Threads = 8,
#endif
                Diagnostics = QuadraticSieve.Diag.Verbose,
                DiagnosticsOutput = output,
                ReportingInterval = 60,
                //ProcessPartialPartialRelations = true,
                MergeLimit = 10,
                //FactorBaseSize = 45000,
                //BlockSize = 1024 * 1024,
                //IntervalSize = 1024 * 1024,
                //CofactorCutoff = 4096 * 4,
                //ErrorLimit = 1,
                //NumberOfFactors = 12,
                ThresholdExponent = 2.6,
                //LargePrimeOptimization = false,
                //UseCountTable = false,
                //CofactorCutoff = 1024,
            };
            var i = 50;
            var sample = samples[i];
            var p = sample.P;
            var q = sample.Q;
            var n = sample.N;
            output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
            output.WriteLine("n = {0}", n);
            var algorithm = new QuadraticSieve(config);
            FactorTest(false, 1, n, algorithm);
        }

        static void QuadraticSieveFactorTest()
        {
            new QuadraticSieve(new QuadraticSieve.Config()).Factor(samples[10].N).ToArray();
            //new QuadraticSieve(new QuadraticSieve.Config()).Factor(35095264073).ToArray();
            var config = new QuadraticSieve.Config
            {
#if !DEBUG
                Threads = 8,
#endif
                Diagnostics = QuadraticSieve.Diag.Verbose,
                DiagnosticsOutput = output,
                ReportingInterval = 60,
                ThresholdExponent = 2.5,
                MergeLimit = 10,
            };
            var i = 50;
            var sample = samples[i];
            var p = sample.P;
            var q = sample.Q;
            var n = sample.N;
            output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
            output.WriteLine("n = {0}", n);
            var algorithm = new QuadraticSieve(config);
            FactorTest(false, 1, n, algorithm);
        }

        static void CunninghamTest()
        {
            var n = BigInteger.Pow(3, 225) - 1;
            output.WriteLine("n = {0}", n);
            var algorithm = new HybridPollardRhoQuadraticSieve(4, 1000000, new QuadraticSieve.Config());
            foreach (var factor in algorithm.Factor(n))
                output.WriteLine("{0}", factor);
        }

        static void GaussianEliminationTest1()
        {
            var threads = 8;
            var mergeLimit = 5;
            //var file = @"..\..\..\..\matrix-12001.txt.gz";
            var file = @"..\..\..\..\matrix-18401.txt.gz";
            //var file = @"..\..\..\..\matrix-150001.txt.gz";
            var lines = GetLinesGzip(file);
            var timer = new Stopwatch();

#if false
            var solver = new GaussianElimination<Word64BitArray>(threads);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<Word64BitMatrix>);
#endif
#if false
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<Word64BitMatrix>);
#endif
#if false
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads, true);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<ByRowSetBitMatrix>);
#endif
#if false
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads, true);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<ByColSetBitMatrix>);
#endif
#if true
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads, mergeLimit, true);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<SetBitMatrix>);
#endif

            timer.Restart();
            var matrix = getter(lines);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            output.WriteLine("Rows = {0}, Cols = {1}", matrix.Rows, matrix.Cols);

            timer.Restart();
            var solutions = solver.Solve(matrix).ToArray();
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);

            output.WriteLine("solutions = {0}", solutions.Length);
        }

        private static void GraphTest()
        {
            //var file = @"..\..\..\..\pprs-23906.txt.gz";
            var file = @"..\..\..\..\pprs-42726.txt.gz";
            var lines = GetLinesGzip(file);
            var pprs = lines
                .Select(line => line.Split(' ').Select(field => long.Parse(field)).ToArray())
                .Select(pair => Tuple.Create(pair[0], pair[1]))
                .ToArray();

            for (int i = 0; i < 1; i++)
                GraphTest(pprs);
        }

        private static void GraphTest(Tuple<long, long>[] pprs)
        {
            //var algorithm = new QuadraticSieve(new QuadraticSieve.Config());
            //var algorithm = new PollardRhoReduction(1, int.MaxValue, new Word32IntegerReduction());
            //var algorithm = new PollardRhoReduction(1, int.MaxValue, new MontgomeryReduction());
            //var algorithm = new ShanksSquareForms();
            var algorithm = new PollardRhoLong();

            var timer = new Stopwatch();
            timer.Restart();
            int processed = 0;
            int found = 0;
            int total = 0;
            int failed = 0;
#if false
            var referenceGraph = new Graph<long, Edge<long>>();
#endif
            //var graph = new Graph<long, Edge<long>>();
            var graph = new PartialRelationGraph<int>();
            foreach (var ppr in pprs)
            {
                var cofactor1 = ppr.Item1;
                var cofactor2 = ppr.Item2;
                var cofactor = cofactor1 * cofactor2;
                if (cofactor2 != 1)
                {
#if false
                for (int i = 0; i < 1000; i++)
                    IntegerMath.IsProbablePrime(cofactor1);
#endif
#if true
                    var factor = algorithm.GetDivisor(cofactor);
                    if (factor != cofactor1 && factor != cofactor2)
                        ++failed;
#endif
                }
                var cycle = graph.FindPath(cofactor1, cofactor2);
#if false
                var referenceCycle = referenceGraph.FindPath(cofactor1, cofactor2);
                if ((referenceCycle == null) != (cycle == null))
                {
                    Debugger.Break();
                    cycle = graph.FindPath(cofactor1, cofactor2);
                }
#endif
                if (cycle == null)
                {
#if false
                    referenceGraph.AddEdge(cofactor1, cofactor2);
#endif
                    graph.AddEdge(cofactor1, cofactor2, ref processed);
                }
                else
                {
#if false
                    if (referenceCycle.Count != cycle.Count)
                    {
                        Debugger.Break();
                        cycle = graph.FindPath(cofactor1, cofactor2);
                    }
                    var referenceCofactors = new[] { cofactor1, cofactor2 }
                        .Concat(referenceCycle.SelectMany(edge => new[] { edge.Vertex1, edge.Vertex2 }))
                        .Distinct()
                        .OrderBy(vertex => vertex)
                        .ToArray();
                    var cofactors = new[] { cofactor1, cofactor2 }
                        .Concat(cycle.SelectMany(edge => new[] { edge.Vertex1, edge.Vertex2 }))
                        .Distinct()
                        .OrderBy(vertex => vertex)
                        .ToArray();
                    if (cofactors.Length != cycle.Count + 1)
                    {
                        Debugger.Break();
                        cycle = graph.FindPath(cofactor1, cofactor2);
                    }
                    else if (!referenceCofactors.SequenceEqual(cofactors))
                    {
                        Debugger.Break();
                        cycle = graph.FindPath(cofactor1, cofactor2);
                    }
#endif
                    total += cycle.Count + 1;
                    //output.WriteLine("cycle = {0}", cycle.Count + 1);
#if false
                    foreach (var edge in referenceCycle)
                        referenceGraph.RemoveEdge(edge);
#endif
                    foreach (var edge in cycle)
                        graph.RemoveEdge(edge);
                    ++found;
                }
                ++processed;
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            output.WriteLine("processed = {0}; found = {1}; total = {2}; average = {3:F3}", processed, found, total, (double)total / found);
            output.WriteLine("failed = {0}", failed);
        }

        private static void UInt128Test()
        {
            var timer = new Stopwatch();
            var random = new MersenneTwister(0).Create<ulong>();
            var max = (ulong)1 << 60;
            timer.Start();
            for (int i = 0; i < 5000000; i++)
            {
                var value = random.Next(max);
                var exponent = random.Next(max);
                var modulus = random.Next(max);
                if ((modulus & 1) == 0)
                    ++modulus;
#if false
                var result = BigInteger.ModPow(value, exponent, modulus);
#else
                var result = IntegerMath.ModularPower(value, exponent, modulus);
#endif
                //var result = IntegerMath.IsPrime(value);
#if false
                if (result != BigInteger.ModPow(value, exponent, modulus))
                    throw new InvalidOperationException("miscalculation");
#endif
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        static void ModularInverseTest()
        {
            var random = new MersenneTwister(0).Create<ulong>();
            //var inverse = new Func<long, long, long>(HybridRSSModularInverse);
            //var inverse = new Func<long, long, long>(RSSSimpleModularInverse);
            var inverse = new Func<long, long, long>(RSSModularInverse);
            //var a = (long)43760554345460349;
            //var m = (long)76055434546034911;
            //var aInv = IntegerMath.ModularInverse(a, m);
            //Console.WriteLine("a = {0}, n = {1}, aInv = {2}, a * aInv % n = {3}", a, m, aInv, IntegerMath.ModularProduct(a, aInv, m));
            //var aInv2 = inverse(a, m);
            //Console.WriteLine("aInv2 = {0}", aInv2);

            var max = (ulong)1 << 60;
            var pairs = random.Sequence(max)
                .Zip(random.Sequence(max), (a, b) => new { A = (long)a, B = (long)b })
                .Where(pair => IntegerMath.GreatestCommonDivisor(pair.A, pair.B) == 1)
                .Take(1000)
                .ToArray();
            int count = 20000;

            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < count; i++)
            {
                foreach (var pair in pairs)
                    IntegerMath.ModularInverse(pair.A, pair.B);
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);

#if false
            timer.Restart();
            for (int i = 0; i < count; i++)
            {
                foreach (var pair in pairs)
                    inverse(pair.A, pair.B);
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif

            Console.WriteLine();
        }

        private static long RSSModularInverse(long a, long m)
        {
            long u = m;
            long v = a;
            long r = 0;
            long s = 1;
            while (v > 0)
            {
                while ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                while ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            if (u > 1)
                return 0;
            while (r < 0)
                r += m;
            while (r >= m)
                r -= m;
            return r;
        }

        private static long HybridRSSModularInverse(long a, long m)
        {
            long u = m;
            long v = a;
            long r = 0;
            long s = 1;
            while (v > int.MaxValue)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            while (v > 0 && u > int.MaxValue)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            return HybridRSSModularInverse((int)u, (int)v, r, s, m);
        }

        private static long HybridRSSModularInverse(int u, int v, long r, long s, long m)
        {
            while (v > 0)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            if (u > 1)
                return 0;
            while (r < 0)
                r += m;
            while (r >= m)
                r -= m;
            return r;
        }

        private static long RSSSimpleModularInverse(long a, long m)
        {
            long u = m;
            long v = a;
            long r = 0;
            long s = 1;
            while (v > 0)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            if (u > 1)
                return 0;
            while (r < 0)
                r += m;
            while (r >= m)
                r -= m;
            return r;
        }

        private static int RSSSimpleModularInverse(int a, int m)
        {
            int u = m;
            int v = a;
            int r = 0;
            int s = 1;
            while (v > 0)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            if (u > 1)
                return 0;
            while (r < 0)
                r += m;
            while (r >= m)
                r -= m;
            return r;
        }

        private static ulong RSUModularInverse(ulong a, ulong m)
        {
            ulong u = m;
            ulong v = a;
            ulong r = 0;
            ulong s = 1;
            while (v > 0)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    if (r < s)
                        r += m - s;
                    else
                        r -= s;
                }
                else
                {
                    v -= u;
                    if (s < r)
                        s += m - r;
                    else
                        s -= r;
                }
            }
            if (u > 1)
                return 0;
            return r;
        }

        private static ulong SEUModularInverse(ulong a, ulong m)
        {
            ulong u;
            ulong v;
            ulong s;
            ulong r;
            if (a < m)
            {
                u = m;
                v = a;
                r = 0;
                s = 1;
            }
            else
            {
                v = m;
                u = a;
                s = 0;
                r = 1;
            }
            while (v > 1)
            {
                var f = u.GetBitLength() - v.GetBitLength();
                if (u < v << f)
                    --f;
                u -= v << f;
                var t = s;
                for (int i = 0; i < f; i++)
                {
                    t <<= 1;
                    if (t > m)
                        t -= m;
                }
                if (r < t)
                    r += m;
                r -= t;
                if (u < v)
                {
                    t = u; u = v; v = t;
                    t = r; r = s; s = t;
                }
            }
            if (v == 0)
                s = 0;
            return s;
        }

        static void PrimalityTest()
        {
#if false
            {
                int count = 1000000;
                var random = new MersenneTwister(0).Create<ulong>();
                //var algorithm = new OldMillerRabin(16);
                var algorithm = MillerRabin.Create(16, new UInt64MontgomeryReduction());
                var max = (ulong)1 << 60;
                var timer = new Stopwatch();
                timer.Start();
                for (int i = 0; i < count; i++)
                {
                    var result = algorithm.IsPrime(random.Next(max));
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if true
            {
                int count = 20000;
                var random = new MersenneTwister(0).Create<BigInteger>();
                //var algorithm = new OldMillerRabin(16);
                //var algorithm = MillerRabin.Create(16, new BigIntegerReduction());
                //var algorithm = MillerRabin.Create(16, new Word32IntegerReduction());
                var algorithm = MillerRabin.Create(16, new MontgomeryReduction());
                var max = BigInteger.One << 256;
                var timer = new Stopwatch();
                timer.Start();
                for (int i = 0; i < count; i++)
                {
                    var result = algorithm.IsPrime(random.Next(max));
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
        }

        static void OperationsTest()
        {
            int count = 50000;
            var max = (BigInteger)1 << 128;
            var source = new MersenneTwister(0).Create<BigInteger>();
            var pairs = source.Sequence(max)
                .Zip(source.Sequence(max), (a, b) => new { A = a, B = b })
                .Where(pair => IntegerMath.GreatestCommonDivisor(pair.A, pair.B) == 1)
                .Take(count)
                .ToArray();
#if false
            {
                Console.WriteLine("hand coded 32");
                var max = (int)1 << 30;
                int c;
                int d;
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<int>();
                timer.Restart();
                for (int i = 0; i < count; i++)
                    ExtendedGreatestCommonDivisor(random.Next(max), random.Next(max), out c, out d);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if false
            {
                Console.WriteLine("hand coded 64");
                var max = (long)1 << 62;
                long c;
                long d;
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<long>();
                timer.Restart();
                for (int i = 0; i < count; i++)
                    ExtendedGreatestCommonDivisor(random.Next(max), random.Next(max), out c, out d);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if true
            {
                Console.WriteLine("hand coded BigInteger");
                BigInteger c;
                BigInteger d;
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<BigInteger>();
                timer.Restart();
                for (int i = 0; i < count; i++)
                {
                    var a = pairs[i].A;
                    var b = pairs[i].B;
                    IntegerMath.ExtendedGreatestCommonDivisor(a, b, out c, out d);
                    if (c < 0)
                        c += b;
                    if (a * c % b != 1)
                        throw new InvalidOperationException("miscalculation");
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if true
            {
                Console.WriteLine("new BigInteger");
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<BigInteger>();
                timer.Restart();
                for (int i = 0; i < count; i++)
                {
                    var a = pairs[i].A;
                    var b = pairs[i].B;
                    var c = IntegerMath.ModularInverse(a, b);
                    if (a * c % b != 1)
                        throw new InvalidOperationException("miscalculation");
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if false
            OperationsTest(new Int32Operations(), (int)1 << 30, count);
            OperationsTest(new UInt64Operations(), (ulong)1 << 62, count);
            OperationsTest(new BigIntegerOperations(), (BigInteger)1 << 128, count);
#endif
        }

        static void OperationsTest<T>(IOperations<T> ops, T max, int count)
        {
            Console.WriteLine("type = {0}", typeof(T));
            T c;
            T d;
            for (int j = 0; j < 1; j++)
            {
#if true
                {
                    GC.Collect();
                    var timer = new Stopwatch();
                    var random = new MersenneTwister(0).Create<T>();
                    timer.Restart();
                    for (int i = 0; i < count; i++)
                        Core.ExtendedGreatestCommonDivisor(ops, random.Next(max), random.Next(max), out c, out d);
                    output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
#endif
#if true
                {
                    GC.Collect();
                    var timer = new Stopwatch();
                    var random = new MersenneTwister(0).Create<T>();
                    timer.Restart();
                    for (int i = 0; i < count; i++)
                        ExtendedGreatestCommonDivisor(ops, random.Next(max), random.Next(max), out c, out d);
                    output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
#endif
            }
        }

        public static void ExtendedGreatestCommonDivisor(int a, int b, out int c, out int d)
        {
            var x = (int)0;
            var lastx = (int)1;
            var y = (int)1;
            var lasty = (int)0;

            while (b != 0)
            {
                var quotient = a / b;
                var tmpa = a;
                a = b;
                b = tmpa - quotient * b;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        public static void ExtendedGreatestCommonDivisor(long a, long b, out long c, out long d)
        {
            var x = (long)0;
            var lastx = (long)1;
            var y = (long)1;
            var lasty = (long)0;

            while (b != 0)
            {
                var quotient = a / b;
                var tmpa = a;
                a = b;
                b = tmpa - quotient * b;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        public static void ExtendedGreatestCommonDivisor(BigInteger a, BigInteger b, out BigInteger c, out BigInteger d)
        {
            var x = (BigInteger)0;
            var lastx = (BigInteger)1;
            var y = (BigInteger)1;
            var lasty = (BigInteger)0;

            while (b != 0)
            {
                var quotient = a / b;
                var tmpa = a;
                a = b;
                b = tmpa - quotient * b;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        public static void ExtendedGreatestCommonDivisor<T>(IOperations<T> ops, T a, T b, out T c, out T d)
        {
            var zero = ops.Wrap(ops.Convert(0));
            var one = ops.Wrap(ops.Convert(1));
            var x = zero;
            var lastx = one;
            var y = one;
            var lasty = zero;

            var p = ops.Wrap(a);
            var q = ops.Wrap(b);
            while (q != zero)
            {
                var quotient = p / q;
                var tmpa = p;
                p = q;
                q = tmpa - quotient * q;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
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
            if (!lines[0].Contains(' '))
            {
                int rows = lines.Length;
                int cols = lines[0].Length;
                var matrix = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), rows, cols);
                for (int i = 0; i < rows; i++)
                {
                    var line = lines[i];
                    for (int j = 0; j < cols; j++)
                    {
                        if (line[j] == '1')
                            matrix[i, j] = true;
                    }
                }
                return matrix;
            }
            else
            {
                var fields = lines[0].Split(' ').Select(field => int.Parse(field)).ToArray();
                int rows = fields[0];
                int cols = fields[1];
                var matrix = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), rows, cols);
                for (int i = 0; i < rows; i++)
                {
                    var indices = lines[i + 1].Split(' ')
                        .Where(field => field != "")
                        .Select(field => int.Parse(field));
                    foreach (var j in indices)
                        matrix[i, j] = true;
                }
                return matrix;
            }
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
                results.Add(algorithm.Factor(n).OrderBy(factor => factor).ToArray());
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
            output.WriteLine("{0} iterations in {1:F0} msec, {2:F3} msec/iteration", iterations, elapsed, elapsed / iterations);
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
            null,
            new SampleComposite
            {
                Digits = 2,
                P = BigInteger.Parse("5"),
                Q = BigInteger.Parse("11"),
            },
            new SampleComposite
            {
                Digits = 4,
                P = BigInteger.Parse("37"),
                Q = BigInteger.Parse("61"),
            },
            new SampleComposite
            {
                Digits = 6,
                P = BigInteger.Parse("967"),
                Q = BigInteger.Parse("379"),
            },
            new SampleComposite
            {
                Digits = 8,
                P = BigInteger.Parse("5431"),
                Q = BigInteger.Parse("8513"),
            },
            new SampleComposite
            {
                Digits = 10,
                P = BigInteger.Parse("83497"),
                Q = BigInteger.Parse("85691"),
            },
            new SampleComposite
            {
                Digits = 12,
                P = BigInteger.Parse("906869"),
                Q = BigInteger.Parse("422759"),
            },
            new SampleComposite
            {
                Digits = 14,
                P = BigInteger.Parse("7901401"),
                Q = BigInteger.Parse("3580393"),
            },
            new SampleComposite
            {
                Digits = 16,
                P = BigInteger.Parse("38900077"),
                Q = BigInteger.Parse("71049871"),
            },
            new SampleComposite
            {
                Digits = 18,
                P = BigInteger.Parse("646868797"),
                Q = BigInteger.Parse("400433141"),
            },
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
            new SampleComposite
            {
                Digits = 82,
                P = BigInteger.Parse("53017745789889873705716127297403311033037"),
                Q = BigInteger.Parse("63409172442847344082610334892821781304101"),
            },
            new SampleComposite
            {
                Digits = 84,
                P = BigInteger.Parse("702295892578093110207938507434822530060641"),
                Q = BigInteger.Parse("342910649327212340825095863090818452864541"),
            },
            new SampleComposite
            {
                Digits = 86,
                P = BigInteger.Parse("8060168753150190457135241985925594282360297"),
                Q = BigInteger.Parse("5042676674972408672634448697950601300773747"),
            },
            new SampleComposite
            {
                Digits = 88,
                P = BigInteger.Parse("75263710689283058958166085987687804905069441"),
                Q = BigInteger.Parse("33942351450819488061478933627681638281912579"),
            },
            new SampleComposite
            {
                Digits = 90,
                P = BigInteger.Parse("452844597483732905842925404011206264228330341"),
                Q = BigInteger.Parse("205800250143096562811007221948045665761100741"),
            },
            new SampleComposite
            {
                Digits = 92,
                P = BigInteger.Parse("2167793402583216605038837020418528979230958321"),
                Q = BigInteger.Parse("6664463122751216293054375161162506956965538477"),
            },
            new SampleComposite
            {
                Digits = 94,
                P = BigInteger.Parse("73732608257939745378758147488940890654818425039"),
                Q = BigInteger.Parse("57602646692423215509569301754817211741313757471"),
            },
            new SampleComposite
            {
                Digits = 96,
                P = BigInteger.Parse("961585810072942641419044210351839016563811857989"),
                Q = BigInteger.Parse("669160867166029748266517367050911502647054495753"),
            },
            new SampleComposite
            {
                Digits = 98,
                P = BigInteger.Parse("3250569350138484738612442549714146714656127558157"),
                Q = BigInteger.Parse("9560816127493079446607942921966503110052699050311"),
            },
            new SampleComposite
            {
                Digits = 100,
                P = BigInteger.Parse("83268231017568575873466457691034899781471011837411"),
                Q = BigInteger.Parse("74270905395447293272574406310979848354158902086579"),
            },
            new SampleComposite
            {
                Digits = 102,
                P = BigInteger.Parse("940105563157766963437551355357600515466121772468943"),
                Q = BigInteger.Parse("300886532442207423100152787577295958421952407123783"),
            },
            new SampleComposite
            {
                Digits = 104,
                P = BigInteger.Parse("5536329130409356621737988070075431271124056972802493"),
                Q = BigInteger.Parse("5851320369599896666775748578020494083502437377067011"),
            },
            new SampleComposite
            {
                Digits = 106,
                P = BigInteger.Parse("95900006031561380146239548171173219385987444898113507"),
                Q = BigInteger.Parse("46423691457059816496580873421795656785311585406499793"),
            },
            new SampleComposite
            {
                Digits = 108,
                P = BigInteger.Parse("838036008579477237125953812571666198225228325843227391"),
                Q = BigInteger.Parse("308620559987457306004333345866922348787381950691797739"),
            },
            new SampleComposite
            {
                Digits = 110,
                P = BigInteger.Parse("9524348126127653470279298471608115636122260642046731149"),
                Q = BigInteger.Parse("5702432440887332287364432342498409227833457737037139601"),
            },
            new SampleComposite
            {
                Digits = 112,
                P = BigInteger.Parse("64133826838501466019612032067826809933583367965508355767"),
                Q = BigInteger.Parse("99826430536817182079559885207489818716914667714908780143"),
            },
            new SampleComposite
            {
                Digits = 114,
                P = BigInteger.Parse("712133265209559049499052940959542547383824060164846054611"),
                Q = BigInteger.Parse("241200306130797999905681076607776919657048436860510005749"),
            },
            new SampleComposite
            {
                Digits = 116,
                P = BigInteger.Parse("5449429390031135421251122886094693214317098752213974003953"),
                Q = BigInteger.Parse("4928204216229834813833069935962566956055542612200013379597"),
            },
            new SampleComposite
            {
                Digits = 118,
                P = BigInteger.Parse("67357275717568959943813687439058017341150058378917075494077"),
                Q = BigInteger.Parse("97759569419716544178270844827738415358315121039984564684707"),
            },
            new SampleComposite
            {
                Digits = 120,
                P = BigInteger.Parse("886161863194892046675575153172350650178464242408235539428789"),
                Q = BigInteger.Parse("700580068918316266402809321693665323840644845195667826345171"),
            },
        };
    }
}
