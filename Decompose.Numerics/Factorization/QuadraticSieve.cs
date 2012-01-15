﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using CountInt = System.Byte;

#if false
using BitMatrix = Decompose.Numerics.Word64BitMatrix;
using Solver = Decompose.Numerics.GaussianElimination<Decompose.Numerics.Word64BitArray>;
#endif

#if false
using BitMatrix = Decompose.Numerics.Word64BitMatrix;
using Solver = Decompose.Numerics.StructuredGaussianElimination<Decompose.Numerics.Word64BitArray, Decompose.Numerics.Word64BitMatrix>;
#endif

#if true
using BitMatrix = Decompose.Numerics.SetBitMatrix;
using Solver = Decompose.Numerics.StructuredGaussianElimination<Decompose.Numerics.Word64BitArray, Decompose.Numerics.Word64BitMatrix>;
#endif

namespace Decompose.Numerics
{
    public class QuadraticSieve : IFactorizationAlgorithm<BigInteger>
    {
        [Flags]
        public enum Diag
        {
            None = 0x0,
            Summary = 0x1,
            Solutions = 0x2,
            Sieve = 0x4,
            Timing = 0x8,
            Solving = 0x10,
            SaveMatrix = 0x20,
            Polynomials = 0x40,
            Verbose = Summary | Sieve | Timing | Solving,
        }

        public enum Algorithm
        {
            None = 0,
            QuadraticSieve = 1,
            SelfInitializingQuadraticSieve = 2,
        }

        public class Config
        {
            public Algorithm Algorithm { get; set; }
            public Diag Diagnostics { get; set; }
            public int Threads { get; set; }
            public int FactorBaseSize { get; set; }
            public int IntervalSize { get; set; }
            public int LowerBoundPercent { get; set; }
            public int Multiplier { get; set; }
            public int ReportingInterval { get; set; }
        }

        public QuadraticSieve(Config config)
        {
            this.config = config;
            diag = config.Diagnostics;
            random = new MersenneTwister32(0);
            smallIntegerFactorer = new TrialDivisionFactorization();
            allPrimes = new SieveOfErostothones();
            solver = new Solver(config.Threads, (diag & Diag.Solving) != 0);
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            if (n.IsOne)
            {
                yield return BigInteger.One;
                yield break;
            }
            while (!IntegerMath.IsPrime(n))
            {
                var divisor = GetDivisor(n);
                if (divisor.IsZero || divisor.IsOne)
                    yield break;
                foreach (var factor in Factor(divisor))
                    yield return factor;
                n /= divisor;
            }
            yield return n;
        }

        private struct ExponentEntry
        {
            public int Row { get; set; }
            public int Exponent { get; set; }
            public override string ToString()
            {
                return string.Format("Row[{0}] ^ {1}", Row, Exponent);
            }
        }

        private class FactorBaseEntry
        {
            public int P { get; set; }
            public CountInt LogP { get; set; }
            public int Root { get; set; }
            public int RootDiff { get; set; }
            public int Offset { get; set; }
            public FactorBaseEntry(int p, BigInteger n, BigInteger OffsetX)
            {
                P = p;
                LogP = LogScale(p);
                Root = IntegerMath.ModularSquareRoot(n, p);
                RootDiff = (P - Root) - Root;
                Offset = ((int)((Root - OffsetX) % P) + P) % P;
            }
            public override string ToString()
            {
                return string.Format("P = {0}", P);
            }
        }

        private class Polynomial
        {
            public BigInteger A { get; set; }
            public BigInteger B { get; set; }
        }

        private class Relation
        {
            public BigInteger X { get; set; }
            public Polynomial Polynomial { get; set; }
            public ExponentEntry[] Entries { get; set; }
            public long Cofactor { get; set; }
            public int Exponent { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}", X);
            }
        }

        private class Interval
        {
            public int Id { get; set; }
            public long X { get; set; }
            public long OffsetX { get; set; }
            public Polynomial Polynomial { get; set; }
            public int Size { get; set; }
            public byte[] Exponents { get; set; }
            public CountInt[] Counts { get; set; }
            public int[] Offsets1 { get; set; }
            public int[] Offsets2 { get; set; }
            public int[] OffsetsDiff { get; set; }
            public Siqs Siqs { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}, Size = {1}", X, Size);
            }
        }

        private struct PartialRelation
        {
            public Polynomial Polynomial { get; set; }
            public long X { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}", X);
            }
        }

        private class Siqs
        {
            public int Index { get; set; }
            public int[] QIndicies { get; set; }
            public bool[] IsQIndex { get; set; } 
            public int S { get; set; }
            public BigInteger[] CapB { get; set; }
            public int[,] Bainv2 { get; set; }
            public Polynomial Polynomial { get; set; }
            public int[] Solution1 { get; set; }
            public int[] Solution2 { get; set; }
        }

        private const int subIntervalSize = 256 * 1024;
        private const int maximumCycleLenth = 32 * 1024;
        private const int maximumIntervalSize = 256 * 1024 * 8;
        private const int lowerBoundPercentDefault = 75;
        private const int cofactorScaleFactor = 4096;
        private const int surplusRelations = 10;
        private const int reportingIntervalDefault = 60;
        private readonly BigInteger smallFactorCutoff = (BigInteger)int.MaxValue;
        private readonly Tuple<int, int>[] sizePairs =
        {
            Tuple.Create(1, 2),
            Tuple.Create(6, 5),
            Tuple.Create(10, 30),
            Tuple.Create(20, 60),
            Tuple.Create(30, 500),
            Tuple.Create(40, 1200),
            Tuple.Create(50, 4500),
            Tuple.Create(60, 12000),
            Tuple.Create(70, 35000),
            Tuple.Create(80, 150000),

            // Untested.
            Tuple.Create(90, 300000),
            Tuple.Create(100, 1000000),
            Tuple.Create(110, 3000000),
        };

        private Config config;
        private IRandomNumberAlgorithm<uint> random;
        private IFactorizationAlgorithm<int> smallIntegerFactorer;
        private IEnumerable<int> allPrimes;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> solver;

        private int intervalSize;
        private int nextIntervalId;

        private Diag diag;
        private Algorithm algorithm;
        private int multiplier;
        private int[] multiplierFactors;
        private BigInteger nOrig;
        private BigInteger n;
        private BigInteger sqrtN;
        private int factorBaseSize;
        private int desired;
        private int digits;
        private FactorBaseEntry[] factorBase;
        private int[] primes;
        private int maximumDivisor;
        private long maximumCofactorSize;
        private CountInt logMaximumDivisorSquared;
        private int mediumPrimeIndex;
        private int largePrimeIndex;
        private int lowerBoundPercent;
        private int reportingInterval;

        private int threads;
        private BlockingCollection<Relation> relationBuffer;
        private Relation[] relations;
        private Dictionary<long, PartialRelation> partialRelations;
        private IBitMatrix matrix;
        private Stopwatch timer;

        private int cycleLength;
        private byte[] cycle;
        private int[] cInv;
        private int[] cProduct;

        private int intervalsProcessed;
        private int valuesChecked;
        private int partialRelationsProcessed;
        private int partialRelationsConverted;

        private void FactorCore(BigInteger n, List<BigInteger> factors)
        {
            if (n == 1)
                return;
            if (IntegerMath.IsPrime(n))
            {
                factors.Add(n);
                return;
            }
            var divisor = GetDivisor(n);
            if (!divisor.IsZero)
            {
                FactorCore(divisor, factors);
                FactorCore(n / divisor, factors);
            }
        }

        private BigInteger GetDivisor(BigInteger nOrig)
        {
            if (nOrig.IsEven)
                return BigIntegers.Two;

            if ((diag & Diag.Timing) != 0)
            {
                timer = new Stopwatch();
                timer.Start();
            }

            Initialize(nOrig);

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Initialization: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            if ((diag & Diag.Summary) != 0)
            {
                Console.WriteLine("algorithm = {0}", algorithm);
                Console.WriteLine("digits = {0}; factorBaseSize = {1:N0}; desired = {2:N0}", digits, factorBaseSize, desired);
                Console.WriteLine("interval size = {0:N0}; threads = {1}; lowerBoundPercent = {2}", intervalSize, threads, lowerBoundPercent);
                Console.WriteLine("first few factors: {0}", string.Join(", ", factorBase.Select(entry => entry.P).Take(10).ToArray()));
            }

            Sieve();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Sieving: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            if ((diag & Diag.Summary) != 0)
            {
                Console.WriteLine("intervals processed = {0:N0}; values processsed = {1:N0}; values checked = {2:N0}", intervalsProcessed, (long)intervalsProcessed * intervalSize, valuesChecked);
                Console.WriteLine("partial relations processed = {0:N0}; partial relations converted = {1:N0}", partialRelationsProcessed, partialRelationsConverted);
            }

            ProcessRelations();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Processing relations: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            var result = Solve();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Stop();
                Console.WriteLine("Solving: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            return result;
        }

        private void Initialize(BigInteger nOrig)
        {
            algorithm = config.Algorithm != Algorithm.None ? config.Algorithm : Algorithm.QuadraticSieve;
            multiplier = config.Multiplier != 0 ? config.Multiplier : 1;
            multiplierFactors = smallIntegerFactorer.Factor(multiplier).ToArray();
            this.nOrig = nOrig;
            n = nOrig * multiplier;
            sqrtN = IntegerMath.Sqrt(this.n);
            digits = (int)Math.Ceiling(BigInteger.Log(n, 10));
            factorBaseSize = CalculateFactorBaseSize();
            factorBase = allPrimes
                .Where(p => p == 2 || IntegerMath.JacobiSymbol(n, p) == 1)
                .Take(factorBaseSize)
                .Select(p => new FactorBaseEntry(p, n, sqrtN))
                .ToArray();
            primes = factorBase.Select(entry => entry.P).ToArray();
            desired = factorBaseSize + 1 + surplusRelations;
            maximumDivisor = factorBase[factorBaseSize - 1].P;
            long maximumDivisorSquared = (long)maximumDivisor * maximumDivisor;
            logMaximumDivisorSquared = LogScale(maximumDivisorSquared);
            largePrimeIndex = Enumerable.Range(0, factorBaseSize + 1)
                .Where(index => index == factorBaseSize || factorBase[index].P >= subIntervalSize)
                .First();
            maximumCofactorSize = Math.Min((long)maximumDivisor * cofactorScaleFactor, maximumDivisorSquared);
            lowerBoundPercent = config.LowerBoundPercent != 0 ? config.LowerBoundPercent : lowerBoundPercentDefault;
            reportingInterval = config.ReportingInterval != 0 ? config.ReportingInterval : reportingIntervalDefault;

            intervalsProcessed = 0;
            valuesChecked = 0;
            partialRelationsProcessed = 0;
            partialRelationsConverted = 0;

            CalculateNumberOfThreads();
            SetupIntervals();
            SetupSmallPrimeCycle();
        }

        private Siqs FirstPolynomial(Siqs siqs)
        {
            var m = intervalSize;
            var target = BigInteger.Log(n * 2, 10) / 2 - Math.Log(m, 10);
            var candidates = Enumerable.Range(0, factorBaseSize)
                .Where(index => factorBase[index].P >= 2000 && factorBase[index].P <= 4000)
                .ToArray();
            if (candidates.Length == 0)
                candidates = Enumerable.Range(factorBaseSize / 2, factorBaseSize - factorBaseSize / 2)
                .ToArray();
            var logCandidates = candidates
                .Select(index => Math.Log(factorBase[index].P, 10))
                .ToArray();
            var permuation = null as int[];
            var s = 0;
            var error = 0.0;
            for (int i = 0; i < 10; i++)
            {
                var numbers = random.Series((uint)candidates.Length).Take(candidates.Length).ToArray();
                var p = Enumerable.Range(0, candidates.Length).OrderBy(index => numbers[index]).ToArray();
                var sum = 0.0;
                int count = 0;
                while (count < logCandidates.Length && sum + logCandidates[p[count]] < target)
                    sum += logCandidates[p[count++]];
                if (Math.Abs(sum + logCandidates[p[count]] - target) < Math.Abs(sum - target))
                    sum += logCandidates[p[count++]];
                var e = Math.Abs(sum - target);
                if (permuation == null || e < error)
                {
                    permuation = p;
                    s = count;
                    error = e;
                }
            }

            // Allocate and initialize one-time memory.
            if (siqs == null)
            {
                siqs = new Siqs
                {
                    IsQIndex = new bool[factorBaseSize],
                    Solution1 = new int[factorBaseSize],
                    Solution2 = new int[factorBaseSize],
                };
            }
            else
            {
                for (int i = 0; i < factorBaseSize; i++)
                    siqs.IsQIndex[i] = false;
            }
            if (siqs.Bainv2 == null || siqs.S != s)
                siqs.Bainv2 = new int[s, factorBaseSize];

            var qIndices = Enumerable.Range(0, s)
                .Select(index => candidates[permuation[index]])
                .OrderBy(index => index)
                .ToArray();
            var q = qIndices.Select(index => factorBase[index].P).ToArray();
            var a = q.Select(p => (BigInteger)p).Product();
            var capB = new BigInteger[s];
            var b = BigInteger.Zero;
            for (int l = 0; l < s; l++)
            {
                var j = qIndices[l];
                siqs.IsQIndex[j] = true;
                var r = q.Where(p => p != q[l]).ProductModulo(q[l]);
                var rInv = IntegerMath.ModularInverse(r, q[l]);
                Debug.Assert(r * rInv % q[l] == 1);
                var tSqrt = factorBase[j].Root;
                var gamma = tSqrt * rInv % q[l];
                if (gamma > q[l] / 2)
                    gamma = q[l] - gamma;
                capB[l] = q.Where(p => p != q[l]).Select(p => (BigInteger)p).Product() * gamma;
                Debug.Assert((capB[l] * capB[l] - n) % q[l] == 0);
                b += capB[l];
            }
            b %= a;
            Debug.Assert((b * b - n) % a == 0);
            var polynomial = new Polynomial { A = a, B = b };


            var bainv2 = siqs.Bainv2;
            var soln1 = siqs.Solution1;
            var soln2 = siqs.Solution2;
            for (int i = 0; i < factorBaseSize; i++)
            {
                if (siqs.IsQIndex[i])
                    continue;
                var entry = factorBase[i];
                var p = entry.P;
                var aInv = (long)IntegerMath.ModularInverse(a, p);
                Debug.Assert(a * aInv % p == 1);
                for (int l = 0; l < s; l++)
                    bainv2[l, i] = (int)(2 * (long)(capB[l] % p) * aInv % p);
                var root1 = (int)((entry.Root - b) % p);
                var root2 = root1 + entry.RootDiff;
                soln1[i] = (int)(aInv * root1 % p);
                soln2[i] = (int)(aInv * root2 % p);
                Debug.Assert(EvaluatePolynomial(polynomial, soln1[i]) % p == 0);
                Debug.Assert(EvaluatePolynomial(polynomial, soln2[i]) % p == 0);
                Debug.Assert(i == 0 || soln1[i] != soln2[i]);
            }

            siqs.Index = 0;
            siqs.QIndicies = qIndices;
            siqs.S = s;
            siqs.CapB = capB;
            siqs.Polynomial = polynomial;

            if ((diag & Diag.Polynomials) != 0)
                Console.WriteLine("first polynomial ({0} factors)", s);

            return siqs;
        }

        private Siqs ChangePolynomial(Siqs siqs)
        {
            if (siqs == null || siqs.Index == (1 << (siqs.S - 1)) - 1)
                return FirstPolynomial(siqs);

            int index = siqs.Index;
            var a = siqs.Polynomial.A;
            var b = siqs.Polynomial.B;
            var capB = siqs.CapB;
            var bainv2 = siqs.Bainv2;

            // Advance index; calculate v & e.
            var v = 0;
            int ii = index + 1;
            while ((ii & 1) == 0)
            {
                ++v;
                ii >>= 1;
            }
            var e = (ii & 2) == 0 ? -1 : 1;

            // Advance b and record new polynomial.
            b += 2 * e * capB[v];
            var polynomial = new Polynomial { A = a, B = b };
            Debug.Assert((b * b - n) % a == 0);

            // Calculate new offsets.
            var soln1 = siqs.Solution1;
            var soln2 = siqs.Solution2;
            for (int i = 0; i < factorBaseSize; i++)
            {
                if (siqs.IsQIndex[i])
                    continue;
                var p = primes[i];
                var step = e * bainv2[v, i] % p;
                soln1[i] = (soln1[i] - step) % p;
                soln2[i] = (soln2[i] - step) % p;
                Debug.Assert(EvaluatePolynomial(polynomial, soln1[i]) % p == 0);
                Debug.Assert(EvaluatePolynomial(polynomial, soln2[i]) % p == 0);
                Debug.Assert(i == 0 || soln1[i] != soln2[i]);
            }

            // Update siqs.
            siqs.Index = index + 1;
            siqs.Polynomial = polynomial;

            if ((diag & Diag.Polynomials) != 0)
                Console.WriteLine("change polynomial: index = {0})", siqs.Index);

            return siqs;
        }

        private BigInteger Solve()
        {
            if ((diag & Diag.SaveMatrix) != 0)
            {
                using (var stream = new StreamWriter(File.OpenWrite("matrix.txt")))
                {
                    stream.WriteLine("{0} {1}", matrix.Rows, matrix.Cols);
                    for (int i = 0; i < matrix.Rows; i++)
                        stream.WriteLine(string.Join(" ", matrix.GetNonZeroCols(i)));
                }
            }

            if ((diag & Diag.Solving) == 0)
            {
                return solver.Solve(matrix)
                    .Select(v => ComputeFactor(v))
                    .Where(factor => !factor.IsZero)
                    .FirstOrDefault();
            }

            var timer = new Stopwatch();
            timer.Start();
            var solutions = solver.Solve(matrix).GetEnumerator();
            var next = solutions.MoveNext();
            var elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000;
            Console.WriteLine("first solution: {0:F3} msec", elapsed);
            if (!next)
            {
                Console.WriteLine("no solutions!");
                return BigInteger.Zero;
            }
            do
            {
                var v = solutions.Current;
                if ((diag & Diag.Solutions) != 0)
                    Console.WriteLine("v = {0}", string.Join(", ", v.GetNonZeroIndices().ToArray()));
                int numberOfIndices = v.GetNonZeroIndices().Count();
                timer.Restart();
                var factor = ComputeFactor(v);
                elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000;
                Console.WriteLine("compute factor: {0:F3} msec ({1} indices)", elapsed, numberOfIndices);
                if (!factor.IsZero)
                    return factor;
            } while (solutions.MoveNext());
            Console.WriteLine("failed!");
            return BigInteger.Zero;
        }

        private void CalculateNumberOfThreads()
        {
            threads = config.Threads != 0 ? config.Threads : 1;
            if (n <= BigInteger.Pow(10, 10))
                threads = 1;
        }

        private int CalculateFactorBaseSize()
        {
            if (config.FactorBaseSize != 0)
                return config.FactorBaseSize;
            for (int i = 0; i < sizePairs.Length - 1; i++)
            {
                var pair = sizePairs[i];
                if (digits >= sizePairs[i].Item1 && digits <= sizePairs[i + 1].Item1)
                {
                    // Interpolate.
                    double x0 = sizePairs[i].Item1;
                    double y0 = sizePairs[i].Item2;
                    double x1 = sizePairs[i + 1].Item1;
                    double y1 = sizePairs[i + 1].Item2;
                    double x = y0 + (digits - x0) * (y1 - y0) / (x1 - x0);
                    return (int)Math.Ceiling(x);
                }
            }
            throw new InvalidOperationException("table entry not found");
        }

        private CountInt CalculateLowerBound(Polynomial polynomial, long x)
        {
            var y = EvaluatePolynomial(polynomial, x);
            return (CountInt)(LogScale(y) - (logMaximumDivisorSquared * (200 - lowerBoundPercent) / 200));
        }

        private BigInteger ComputeFactor(IBitArray v)
        {
            var indices = v.GetNonZeroIndices().ToArray();
            Debug.Assert(indices
                .Select(index => relations[index])
                .All(relation => (relation.X * relation.X - MultiplyFactors(relation)) % n == 0));
            var xPrime = indices
                .Select(index => relations[index].X)
                .ProductModulo(n);
            var exponents = SumExponents(indices);
            var yFactorBase = new[] { -1 }
                .Concat(factorBase.Select(entry => entry.P))
                .Zip(exponents, (p, exponent) => BigInteger.ModPow(p, exponent, n));
            var yCofactors = indices
                .Select(index => (BigInteger)relations[index].Cofactor)
                .Where(cofactor => cofactor != 1);
            var yPrime = yFactorBase
                .Concat(yCofactors)
                .ProductModulo(n);
            var factor = BigInteger.GreatestCommonDivisor(xPrime + yPrime, n);
            foreach (var multiplierFactor in multiplierFactors)
            {
                if (factor % multiplierFactor == 0)
                    factor /= multiplierFactor;
            }
            return !factor.IsOne && factor != nOrig ? factor : BigInteger.Zero;
        }

        private int[] SumExponents(IEnumerable<int> indices)
        {
            var results = new int[factorBaseSize + 1];
            foreach (int index in indices)
            {
                foreach (var entry in relations[index].Entries)
                    results[entry.Row] += entry.Exponent;
            }
            Debug.Assert(results.All(exponent => exponent % 2 == 0));
            for (int i = 0; i < results.Length; i++)
                results[i] /= 2;
            return results;
        }

        private static CountInt LogScale(BigInteger n)
        {
            return (CountInt)Math.Ceiling(BigInteger.Log(BigInteger.Abs(n), 2));
        }

        private static CountInt LogScale(long n)
        {
            return (CountInt)Math.Ceiling(Math.Log(Math.Abs(n), 2));
        }

        private void Sieve()
        {
            relationBuffer = new BlockingCollection<Relation>(desired);
            partialRelations = new Dictionary<long, PartialRelation>();

            if (threads == 1)
                SieveTask();
            else
            {
                var tasks = new Task[threads];
                for (int i = 0; i < threads; i++)
                    tasks[i] = Task.Factory.StartNew(SieveTask);
                WaitForTasks(tasks);
            }

            relations = relationBuffer.ToArray();
            relationBuffer = null;
            partialRelations = null;
        }

        private void SieveTask()
        {
            var interval = CreateInterval();
            while (relationBuffer.Count < relationBuffer.BoundedCapacity)
            {
                Sieve(GetNextInterval(interval));
                Interlocked.Increment(ref intervalsProcessed);
            }
        }

        private void WaitForTasks(Task[] tasks)
        {
            if ((diag & Diag.Sieve) == 0)
            {
                Task.WaitAll(tasks);
                return;
            }

            var timer = new Stopwatch();
            timer.Start();
            double percentCompleteSofar = 0;
            while (!Task.WaitAll(tasks, reportingInterval * 1000))
            {
                int current = relationBuffer.Count;
                double percentComplete = (double)current / desired * 100;
                double percentLatest = percentComplete - percentCompleteSofar;
                double percentRemaining = 100 - percentComplete;
                double percentRate = (double)percentLatest / reportingInterval;
                double timeRemainingSeconds = percentRate == 0 ? 0 : percentRemaining / percentRate;
                var timeRemaining = TimeSpan.FromSeconds(Math.Ceiling(timeRemainingSeconds));
                Console.WriteLine("{0:F2}% complete, rate = {1:F5} %/sec, sieve time remaining = {2}",
                    percentComplete, percentRate, timeRemaining);
                percentCompleteSofar = percentComplete;
            }
            double elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency;
            double overallPercentRate = 100 / elapsed;
            Console.WriteLine("overall rate = {0:F6} %/sec", overallPercentRate);
        }

        private Interval CreateInterval()
        {
            var interval = new Interval();
            interval.Exponents = new byte[factorBaseSize + 1];
            interval.Counts = new CountInt[subIntervalSize + 1];
            interval.Offsets1 = new int[factorBaseSize];
            interval.Offsets2 = new int[factorBaseSize];
            interval.OffsetsDiff = new int[factorBaseSize];
            return interval;
        }

        private void SetupIntervals()
        {
            if (config.IntervalSize != 0)
                intervalSize = config.IntervalSize;
            else if (algorithm == Algorithm.SelfInitializingQuadraticSieve)
            {
                intervalSize = 1024 * 1024;
                intervalSize = IntegerMath.MultipleOfCeiling(intervalSize, subIntervalSize);
            }
            else
            {
                intervalSize = (int)IntegerMath.Min((sqrtN + threads - 1) / threads, maximumIntervalSize);
                nextIntervalId = 0;
            }
        }

        private Interval GetNextInterval(Interval interval)
        {
            var offsets1 = interval.Offsets1;
            var offsets2 = interval.Offsets2;
            var offsetsDiff = interval.OffsetsDiff;
            if (algorithm == Algorithm.SelfInitializingQuadraticSieve)
            {
                var siqs = ChangePolynomial(interval.Siqs);
                interval.Siqs = siqs;
                var x = -intervalSize / 2;
                interval.X = x;
                interval.Polynomial = interval.Siqs.Polynomial;
                interval.Size = intervalSize;
                for (int i = 0; i < factorBaseSize; i++)
                {
                    var p = primes[i];
                    var o1 = offsets1[i] = ((siqs.Solution1[i] - x) % p + p) % p;
                    var o2 = offsets2[i] = ((siqs.Solution2[i] - x) % p + p) % p;
                    offsetsDiff[i] = ((o2 - o1) % p + p) % p;
                }
            }
            else
            {
                int intervalId = Interlocked.Increment(ref nextIntervalId) - 1;
                interval.Id = intervalId;
                int intervalNumber = intervalId % 2 == 0 ? intervalId / 2 : -(intervalId + 1) / 2;
                var x = (long)intervalNumber * intervalSize;
                interval.X = x;
                interval.Polynomial = new Polynomial { A = 1, B = sqrtN };
                interval.Size = intervalSize;
                for (int i = 0; i < factorBaseSize; i++)
                {
                    var entry = factorBase[i];
                    var p = entry.P;
                    offsets1[i] = ((int)((entry.Offset - x) % p) + p) % p;
                    offsets2[i] = (offsets1[i] + entry.RootDiff) % p;
                    offsetsDiff[i] = entry.RootDiff;
                }
            }
            interval.OffsetX = interval.X;
            return interval;
        }

        private void SetupSmallPrimeCycle()
        {
            int c = 1;
            int j = 0;
            while (j < factorBaseSize && c * factorBase[j].P < maximumCycleLenth)
            {
                var p = factorBase[j].P;
                c *= p;
                ++j;
            }
            mediumPrimeIndex = j;
            cInv = new int[mediumPrimeIndex];
            cProduct = new int[mediumPrimeIndex];
            c = 1;
            for (int i = 0; i < mediumPrimeIndex; i++)
            {
                var p = primes[i];
                cInv[i] = IntegerMath.ModularInverse(c, p);
                cProduct[i] = c;
                c *= p;
            }
            cycleLength = c;
            cycle = new CountInt[cycleLength];
            for (int i = 0; i < mediumPrimeIndex; i++)
            {
                var entry = factorBase[i];
                var p = entry.P;
                var p2 = entry.RootDiff;
                var logP = entry.LogP;
                for (int k = 0; k < cycleLength; k += p)
                {
                    cycle[k] += logP;
                    if (p2 != 0)
                        cycle[k + p2] += logP;
                }
            }
        }

        private void Sieve(Interval interval)
        {
#if false
            SieveTrialDivision(interval);
#else
            SieveQuadraticResidue(interval);
#endif
        }

        private void SieveTrialDivision(Interval interval)
        {
            for (int k = 0; k < interval.Size; k++)
            {
                if (!CheckValue(interval, k))
                    return;
            }
        }

        private void SieveQuadraticResidue(Interval interval)
        {
            int intervalSize = interval.Size;
            var xMin = interval.X;
            if (interval.X < 0 && interval.X + interval.Size > 0)
                xMin = 0;
            else if (interval.X < 0)
                xMin = interval.X + interval.Size;
            CountInt countLimit = CalculateLowerBound(interval.Polynomial, xMin);
            for (int k0 = 0; k0 < intervalSize; k0 += subIntervalSize)
            {
                int size = Math.Min(subIntervalSize, intervalSize - k0);
                SieveSmallPrimes(interval, size);
                SieveMediumPrimes(interval, size);
                SieveLargePrimes(interval, size);
                interval.OffsetX = interval.X + size;
                CheckForSmooth(interval, size, countLimit);
                if (relationBuffer.Count >= relationBuffer.BoundedCapacity)
                    break;
                interval.X += subIntervalSize;
            }
        }

        private void SieveSmallPrimes(Interval interval, int size)
        {
            var offsets1 = interval.Offsets1;
            var counts = interval.Counts;
            int k = 0;
            for (int i = 0; i < mediumPrimeIndex; i++)
                k += (offsets1[i] - k) * cInv[i] % primes[i] * cProduct[i];
            if (k < 0)
                k += cycleLength;
            Debug.Assert(k >= 0 && k < cycleLength);
            Debug.Assert(Enumerable.Range(0, mediumPrimeIndex).All(i => (k - offsets1[i]) % primes[i] == 0));

            Array.Copy(cycle, cycleLength - k, counts, 0, k);
            int kMax = size - cycleLength;
            while (k < kMax)
            {
                cycle.CopyTo(counts, k);
                k += cycleLength;
            }
            if (k < size)
            {
                Array.Copy(cycle, 0, counts, k, size - k);
                k += cycleLength;
            }

            Debug.Assert(k >= size);
            k -= size;
            for (int i = 0; i < mediumPrimeIndex; i++)
                offsets1[i] = k % primes[i];
        }

        private void SieveMediumPrimes(Interval interval, int size)
        {
            var siqs = interval.Siqs;
            var offsets1 = interval.Offsets1;
            var offsetsDiff = interval.OffsetsDiff;
            var counts = interval.Counts;
            int i = mediumPrimeIndex;
            if (primes[i] == 2)
            {
                var logP = factorBase[i].LogP;
                int k;
                for (k = offsets1[i]; k < size; k += 2)
                {
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k) % 2 == 0);
                    counts[k] += logP;
                }
                offsets1[i] = k - size;
                ++i;
            }
            while (i < largePrimeIndex)
            {
                if (siqs != null && siqs.IsQIndex[i])
                {
                    ++i;
                    continue;
                }
                var entry = factorBase[i];
                int p = entry.P;
                var logP = entry.LogP;
                int p1 = offsetsDiff[i];
                int p2 = p - p1;
                int k = offsets1[i];
                if (k >= p2 && k - p2 < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k - p2) % p == 0);
                    counts[k - p2] += logP;
                }
                int limit = size - p1;
                while (k < limit)
                {
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k) % p == 0);
                    counts[k] += logP;
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k + p1) % p == 0);
                    counts[k + p1] += logP;
                    k += p;
                }
                if (k < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k) % p == 0);
                    counts[k] += logP;
                    k += p;
                }
                offsets1[i++] = k - size;
            }
        }

        private void SieveLargePrimes(Interval interval, int size)
        {
            var offsets1 = interval.Offsets1;
            var offsets2 = interval.Offsets2;
            var counts = interval.Counts;
            for (int i = largePrimeIndex; i < factorBaseSize; i++)
            {
                int k1 = offsets1[i];
                if (k1 < size)
                {
                    var entry = factorBase[i];
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k1) % entry.P == 0);
                    counts[k1] += entry.LogP;
                    k1 += entry.P;
                }
                offsets1[i] = k1 - size;
                int k2 = offsets2[i];
                if (k2 < size)
                {
                    var entry = factorBase[i];
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k2) % entry.P == 0);
                    counts[k2] += entry.LogP;
                    k2 += entry.P;
                }
                offsets2[i] = k2 - size;
            }
        }

        private void CheckForSmooth(Interval interval, int size, CountInt countLimit)
        {
            var counts = interval.Counts;
            counts[size] = CountInt.MaxValue;
            int k = 0;
            if (digits >= 50)
            {
                while (k < size)
                {
                    if (counts[k] >= countLimit)
                        CheckValue(interval, k);
                    ++k;
                }
            }
            else
            {
                while (k < size)
                {
                    if (counts[k] >= countLimit)
                    {
                        if (!CheckValue(interval, k))
                            return;
                    }
                    ++k;
                }
            }
        }

        private bool CheckValue(Interval interval, int k)
        {
            ++valuesChecked;
            ClearExponents(interval);
            long cofactor = FactorOverBase(interval, interval.X + k);
            if (cofactor == 0)
                return true;
            if (cofactor == 1)
            {
                var relation = CreateRelation(
                    EvaluateMapping(interval.Polynomial, interval.X + k),
                    GetEntries(interval.Exponents),
                    1, 0);
                return relationBuffer.TryAdd(relation);
            }
            if (cofactor < maximumCofactorSize)
                return ProcessPartialRelation(interval, k, cofactor);
            return true;
        }

        private long FactorOverBase(Interval interval, long x)
        {
            var y = EvaluatePolynomial(interval.Polynomial, x);
            var exponents = interval.Exponents;
            var siqs = interval.Siqs;
            if (y < 0)
            {
                ++exponents[0];
                y = -y;
            }
            while (y.IsEven)
            {
                ++exponents[1];
                y >>= 1;
            }
            if (siqs != null)
            {
                for (int i = 0; i < siqs.S; i++)
                {
                    var j = siqs.QIndicies[i];
                    var q = factorBase[j].P;
                    ++exponents[j + 1];
                    while (y % q == 0)
                    {
                        ++exponents[j + 1];
                        y /= q;
                    }
                }
            }
            var delta = x - interval.OffsetX;
            if (delta >= int.MinValue + maximumDivisor && delta <= int.MaxValue - maximumDivisor)
                return FactorOverBase(interval, y, (int)delta);
            else
                return FactorOverBase(interval, y, delta);
        }

        private long FactorOverBase(Interval interval, BigInteger y, int delta)
        {
            var siqs = interval.Siqs;
            var offsets1 = interval.Offsets1;
            var offsetsDiff = interval.OffsetsDiff;
            var exponents = interval.Exponents;
            for (int i = 1; i < factorBaseSize; i++)
            {
                if (siqs != null && siqs.IsQIndex[i])
                    continue;
                var p = primes[i];
                var offset = (delta - offsets1[i]) % p;
                if (offset < 0)
                    offset += p;
                if (offset != 0 && offset != offsetsDiff[i])
                {
                    Debug.Assert(y % p != 0);
                    continue;
                }
                Debug.Assert(y % p == 0);
                while ((y % p).IsZero)
                {
                    ++exponents[i + 1];
                    y /= p;
                }
            }
            return y < long.MaxValue ? (long)y : 0;
        }

        private long FactorOverBase(Interval interval, BigInteger y, long delta)
        {
            var siqs = interval.Siqs;
            var offsets1 = interval.Offsets1;
            var offsetsDiff = interval.OffsetsDiff;
            var exponents = interval.Exponents;
            for (int i = 1; i < factorBaseSize; i++)
            {
                if (siqs != null && siqs.IsQIndex[i])
                    continue;
                var p = primes[i];
                var offset = (int)((delta - offsets1[i]) % p);
                if (offset < 0)
                    offset += p;
                if (offset != 0 && offset != offsetsDiff[i])
                {
                    Debug.Assert(y % p != 0);
                    continue;
                }
                Debug.Assert(y % p == 0);
                while ((y % p).IsZero)
                {
                    ++exponents[i + 1];
                    y /= p;
                }
            }
            return y < long.MaxValue ? (long)y : 0;
        }

        private long FactorOverBase(byte[] exponents, Polynomial polynomial, long x)
        {
            var y = polynomial.A * EvaluatePolynomial(polynomial, x);
            if (y < 0)
            {
                ++exponents[0];
                y = -y;
            }
            while (y.IsEven)
            {
                ++exponents[1];
                y >>= 1;
            }
            for (int i = 1; i < factorBaseSize; i++)
            {
                var p = primes[i];
                while (y % p == 0)
                {
                    ++exponents[i + 1];
                    y /= p;
                }
            }
            return y < long.MaxValue ? (long)y : 0;
        }


        private bool ProcessPartialRelation(Interval interval, int k, long cofactor)
        {
            ++partialRelationsProcessed;
            PartialRelation other;
            lock (partialRelations)
            {
                if (partialRelations.TryGetValue(cofactor, out other))
                    partialRelations.Remove(cofactor);
                else
                    partialRelations.Add(cofactor, new PartialRelation { X = interval.X + k, Polynomial = interval.Polynomial });
            }
            if (other.Polynomial == null)
                return true;
            ++partialRelationsConverted;
#if false
            var relation1 = CreateRelation(
                EvaluateMapping(interval.Polynomial, interval.X + k),
                GetEntries(interval.Exponents),
                cofactor, 1);
            var oldExponents = interval.Exponents;
            interval.Exponents = (CountInt[])oldExponents.Clone();
            ClearExponents(interval);
            var otherCofactor = FactorOverBase(interval.Exponents, other.Polynomial, other.X);
            if (otherCofactor != cofactor)
                Debugger.Break();
            var entries = GetEntries(interval.Exponents);
            var relation2 = CreateRelation(
                EvaluateMapping(other.Polynomial, other.X),
                entries,
                cofactor, 1);
            if ((BigInteger.Pow(relation1.X * relation2.X, 2) - MultiplyFactors(relation1) * MultiplyFactors(relation2)) % n != 0)
                Debugger.Break();
            interval.Exponents = oldExponents;
#endif
            FactorOverBase(interval.Exponents, other.Polynomial, other.X);
            var relation = CreateRelation(
                EvaluateMapping(interval.Polynomial, interval.X + k) * EvaluateMapping(other.Polynomial, other.X),
                GetEntries(interval.Exponents),
                cofactor, 2);
            return relationBuffer.TryAdd(relation);
        }

        private Relation CreateRelation(BigInteger x, ExponentEntry[] entries, long cofactor, int exponent)
        {
            var relation = new Relation
            {
                X = x,
                Entries = entries,
                Cofactor = cofactor,
                Exponent = exponent,
            };
            Debug.Assert((relation.X * relation.X - MultiplyFactors(relation)) % n == 0);
            return relation;
        }

        private void ProcessRelations()
        {
            Debug.Assert(relations.GroupBy(relation => relation.X).Count() == relations.Length);
            matrix = new BitMatrix(factorBaseSize + 1, relations.Length);
            for (int j = 0; j < relations.Length; j++)
            {
                var entries = relations[j].Entries;
                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    if (entry.Exponent % 2 != 0)
                        matrix[entry.Row, j] = true;
                }
            }
        }

        private void ClearExponents(Interval interval)
        {
            var exponents = interval.Exponents;
            for (int i = 0; i <= factorBaseSize; i++)
                exponents[i] = 0;
        }

        private ExponentEntry[] GetEntries(byte[] exponents)
        {
            int size = 16;
            var entries = new ExponentEntry[size];
            int k = 0;
            for (int i = 0; i < exponents.Length; i++)
            {
                if (exponents[i] != 0)
                {
                    entries[k++] = new ExponentEntry { Row = i, Exponent = exponents[i] };
                    if (k == size)
                    {
                        size *= 2;
                        Array.Resize(ref entries, size);
                    }
                }
            }
            Array.Resize(ref entries, k);
            return entries;
        }

        private BigInteger EvaluateMapping(Polynomial polynomial, long x)
        {
            return polynomial.A * x + polynomial.B;
        }

        private BigInteger EvaluatePolynomial(Polynomial polynomial, long x)
        {
            var xPrime = EvaluateMapping(polynomial, x);
            var yPrime = xPrime * xPrime - n;
            Debug.Assert(yPrime % polynomial.A == 0);
            return yPrime / polynomial.A;
        }

        private BigInteger MultiplyFactors(Relation relation)
        {
            var result = relation.Entries
                .Select(entry => BigInteger.Pow(entry.Row == 0 ? -1 : factorBase[entry.Row - 1].P, entry.Exponent))
                .ProductModulo(n);
            return result * BigInteger.ModPow(relation.Cofactor, relation.Exponent, n) % n;
        }
    }
}