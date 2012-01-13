using System;
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
using BitMatrix = Decompose.Numerics.HashSetBitMatrix;
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
            Verbose = Summary | Sieve | Timing | Solving,
        }

        public class Config
        {
            public Diag Diagnostics { get; set; }
            public int Threads { get; set; }
            public int FactorBaseSize { get; set; }
            public int LowerBoundPercent { get; set; }
            public int Multiplier { get; set; }
            public int ReportingInterval { get; set; }
        }

        public QuadraticSieve(Config config)
        {
            diag = config.Diagnostics;
            threadsOverride = config.Threads;
            factorBaseSizeOverride = config.FactorBaseSize;
            lowerBoundPercentOverride = config.LowerBoundPercent;
            multiplierOverride = config.Multiplier;
            reportingIntervalOverride = config.ReportingInterval;
            smallIntegerFactorer = new TrialDivisionFactorization();
            primes = new SieveOfErostothones();
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
            public int Index { get; set; }
            public int Exponent { get; set; }
            public override string ToString()
            {
                return string.Format("Factor[{0}] ^ {1}", Index, Exponent);
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

        private class Relation
        {
            public BigInteger X { get; set; }
            public ExponentEntry[] Entries { get; set; }
            public long Cofactor { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}", X);
            }
        }

        private class Interval
        {
            public long X { get; set; }
            public long OffsetX { get; set; }
            public int Size { get; set; }
            public byte[] Exponents { get; set; }
            public CountInt[] Counts { get; set; }
            public int[] Offsets1 { get; set; }
            public int[] Offsets2 { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}, Size = {1}", X, Size);
            }
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

        private int threadsOverride;
        private int factorBaseSizeOverride;
        private int lowerBoundPercentOverride;
        private int multiplierOverride;
        private int reportingIntervalOverride;
        private IFactorizationAlgorithm<int> smallIntegerFactorer;
        private IEnumerable<int> primes;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> solver;

        private int intervalSize;
        private int nextIntervalId;

        private Diag diag;
        private int multiplier;
        private int[] multiplierFactors;
        private BigInteger nOrig;
        private BigInteger n;
        private BigInteger sqrtN;
        private int factorBaseSize;
        private int desired;
        private int digits;
        private FactorBaseEntry[] factorBase;
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
        private Dictionary<long, long> partialRelations;
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

            Sieve();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Sieving: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            if ((diag & Diag.Summary) != 0)
            {
                Console.WriteLine("digits = {0}, factorBaseSize = {1}, desired = {2}", digits, factorBaseSize, desired);
                Console.WriteLine("threads = {0}, lowerBoundPercent = {1}", threads, lowerBoundPercent);
                Console.WriteLine("intervals processsed = {0}, values checked = {1}", intervalsProcessed, valuesChecked);
                Console.WriteLine("partial relations processed = {0}, converted = {1}", partialRelationsProcessed, partialRelationsConverted);
                Console.WriteLine("first few factors: {0}", string.Join(", ", factorBase.Select(entry => entry.P).Take(10).ToArray()));
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
            if (multiplierOverride != 0)
                multiplier = multiplierOverride;
            else
                multiplier = 1;
            multiplierFactors = smallIntegerFactorer.Factor(multiplier).ToArray();
            this.nOrig = nOrig;
            n = nOrig * multiplier;
            sqrtN = IntegerMath.Sqrt(this.n);
            digits = (int)Math.Ceiling(BigInteger.Log(n, 10));
            factorBaseSize = CalculateFactorBaseSize();
            factorBase = primes
                .Where(p => IntegerMath.JacobiSymbol(n, p) == 1)
                .Take(factorBaseSize)
                .Select(p => new FactorBaseEntry(p, n, sqrtN))
                .ToArray();
            desired = factorBaseSize + 1 + surplusRelations;
            maximumDivisor = factorBase[factorBaseSize - 1].P;
            long maximumDivisorSquared = (long)maximumDivisor * maximumDivisor;
            logMaximumDivisorSquared = LogScale(maximumDivisorSquared);
            largePrimeIndex = Enumerable.Range(0, factorBaseSize + 1)
                .Where(index => index == factorBaseSize || factorBase[index].P >= subIntervalSize)
                .First();
            maximumCofactorSize = Math.Min((long)maximumDivisor * cofactorScaleFactor, maximumDivisorSquared);
            lowerBoundPercent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefault;
            reportingInterval = reportingIntervalOverride != 0 ? reportingIntervalOverride : reportingIntervalDefault;

            intervalsProcessed = 0;
            valuesChecked = 0;
            partialRelationsProcessed = 0;
            partialRelationsConverted = 0;
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
            threads = threadsOverride != 0 ? threadsOverride : 1;
            if (n <= BigInteger.Pow(10, 10))
                threads = 1;
        }

        private int CalculateFactorBaseSize()
        {
            if (factorBaseSizeOverride != 0)
                return factorBaseSizeOverride;
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

        private CountInt CalculateLowerBound(long x)
        {
            var y = EvaluatePolynomial(x);
            return (CountInt)(LogScale(y) - (logMaximumDivisorSquared * (200 - lowerBoundPercent) / 200));
        }

        private BigInteger ComputeFactor(IBitArray v)
        {
            var indices = v.GetNonZeroIndices().ToArray();
            var xPrime = indices
                .Select(index => relations[index].X)
                .ProductModulo(n);
            var exponents = SumExponents(indices);
            var yFactorBase = new[] { -1 }
                .Concat(factorBase.Select(entry => entry.P))
                .Zip(exponents, (p, exponent) => BigInteger.Pow(p, exponent / 2));
            var yCofactors = indices
                .Select(index => (BigInteger)relations[index].Cofactor)
                .Where(cofactor => cofactor != 0);
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
                    results[entry.Index] += entry.Exponent;
            }
            Debug.Assert(results.All(exponent => exponent % 2 == 0));
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
            CalculateNumberOfThreads();
            SetupIntervals();
            SetupSmallPrimeCycle();
            relationBuffer = new BlockingCollection<Relation>(desired);
            partialRelations = new Dictionary<long, long>();

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
            return interval;
        }

        private void SetupIntervals()
        {
            intervalSize = (int)IntegerMath.Min((sqrtN + threads - 1) / threads, maximumIntervalSize);
            nextIntervalId = 0;
        }

        private Interval GetNextInterval(Interval interval)
        {
            int intervalId = Interlocked.Increment(ref nextIntervalId) - 1;
            int intervalNumber = intervalId % 2 == 0 ? intervalId / 2 : -(intervalId + 1) / 2;
            var x = (long)intervalNumber * intervalSize;
            interval.X = x;
            interval.Size = intervalSize;
            var offsets1 = interval.Offsets1;
            var offsets2 = interval.Offsets2;
            for (int i = 0; i < factorBaseSize; i++)
            {
                var entry = factorBase[i];
                var p = entry.P;
                offsets1[i] = ((int)((entry.Offset - x) % p) + p) % p;
                offsets2[i] = (offsets1[i] + entry.RootDiff) % p;
            }
            interval.OffsetX = x;
            return interval;
        }

        private void SetupSmallPrimeCycle()
        {
            int c = 1;
            int j = 0;
            while (j < factorBaseSize && c * factorBase[j].P < maximumCycleLenth)
            {
                int p = factorBase[j].P;
                c *= p;
                ++j;
            }
            mediumPrimeIndex = j;
            cInv = new int[mediumPrimeIndex];
            cProduct = new int[mediumPrimeIndex];
            c = 1;
            for (int i = 0; i < mediumPrimeIndex; i++)
            {
                int p = factorBase[i].P;
                cInv[i] = IntegerMath.ModularInverse(c, p);
                cProduct[i] = c;
                c *= p;
            }
            cycleLength = c;
            cycle = new CountInt[cycleLength];
            for (int i = 0; i < mediumPrimeIndex; i++)
            {
                var entry = factorBase[i];
                int p = entry.P;
                int p2 = entry.RootDiff;
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
            var xMin = interval.X > 0 ? interval.X : interval.X + interval.Size;
            CountInt countLimit = CalculateLowerBound(xMin);
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
                k += (offsets1[i] - k) * cInv[i] % factorBase[i].P * cProduct[i];
            if (k < 0)
                k += cycleLength;
            Debug.Assert(k >= 0 && k < cycleLength);
            Debug.Assert(Enumerable.Range(0, mediumPrimeIndex).All(i => (k - offsets1[i]) % factorBase[i].P == 0));

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
                offsets1[i] = k % factorBase[i].P;
        }

        private void SieveMediumPrimes(Interval interval, int size)
        {
            var offsets1 = interval.Offsets1;
            var counts = interval.Counts;
            int i = mediumPrimeIndex;
            if (factorBase[i].P == 2)
            {
                var logP = factorBase[i].LogP;
                int k;
                for (k = offsets1[i]; k < size; k += 2)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % 2 == 0);
                    counts[k] += logP;
                }
                offsets1[i] = k - size;
                ++i;
            }
            while (i < largePrimeIndex)
            {
                var entry = factorBase[i];
                int p = entry.P;
                var logP = entry.LogP;
                int p1 = entry.RootDiff;
                int p2 = p - p1;
                int k = offsets1[i];
                if (k >= p2 && k - p2 < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k - p2) % p == 0);
                    counts[k - p2] += logP;
                }
                int limit = size - p1;
                while (k < limit)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % p == 0);
                    counts[k] += logP;
                    Debug.Assert(EvaluatePolynomial(interval.X + k + p1) % p == 0);
                    counts[k + p1] += logP;
                    k += p;
                }
                if (k < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % p == 0);
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
                    Debug.Assert(EvaluatePolynomial(interval.X + k1) % entry.P == 0);
                    counts[k1] += entry.LogP;
                    k1 += entry.P;
                }
                offsets1[i] = k1 - size;
                int k2 = offsets2[i];
                if (k2 < size)
                {
                    var entry = factorBase[i];
                    Debug.Assert(EvaluatePolynomial(interval.X + k2) % entry.P == 0);
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
                var relation = new Relation
                {
                    X = sqrtN + interval.X + k,
                    Entries = GetEntries(interval.Exponents),
                };
                return relationBuffer.TryAdd(relation);
            }
            if (cofactor < maximumCofactorSize)
                return ProcessPartialRelation(interval, k, cofactor);
            return true;
        }

        private long FactorOverBase(Interval interval, long x)
        {
            var y = EvaluatePolynomial(x);
            if (y < 0)
            {
                ++interval.Exponents[0];
                y = -y;
            }
            var delta = x - interval.OffsetX;
            if (delta >= int.MinValue + maximumDivisor && delta <= int.MaxValue - maximumDivisor)
                return FactorOverBase(interval, y, (int)delta);
            else
                return FactorOverBase(interval, y, delta);
        }

        private long FactorOverBase(Interval interval, BigInteger y, int delta)
        {
            var offsets1 = interval.Offsets1;
            var exponents = interval.Exponents;
            for (int i = 0; i < factorBaseSize; i++)
            {
                var entry = factorBase[i];
                var p = entry.P;
                var offset = (delta - offsets1[i]) % p;
                if (offset < 0)
                    offset += p;
                if (offset != 0 && offset != entry.RootDiff)
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
            var offsets1 = interval.Offsets1;
            var exponents = interval.Exponents;
            for (int i = 0; i < factorBaseSize; i++)
            {
                var entry = factorBase[i];
                var p = entry.P;
                var offset = (int)((delta - offsets1[i]) % p);
                if (offset < 0)
                    offset += p;
                if (offset != 0 && offset != entry.RootDiff)
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

        private bool ProcessPartialRelation(Interval interval, int k, long cofactor)
        {
            ++partialRelationsProcessed;
            long other;
            lock (partialRelations)
            {
                if (partialRelations.TryGetValue(cofactor, out other))
                    partialRelations.Remove(cofactor);
                else
                    partialRelations.Add(cofactor, interval.X + k);
            }
            if (other == 0)
                return true;
            ++partialRelationsConverted;
            FactorOverBase(interval, other);
            var relation = new Relation
            {
                X = (sqrtN + interval.X + k) * (sqrtN + other),
                Entries = GetEntries(interval.Exponents),
                Cofactor = cofactor,
            };
            return relationBuffer.TryAdd(relation);
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
                        matrix[entry.Index, j] = true;
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
                    entries[k++] = new ExponentEntry { Index = i, Exponent = exponents[i] };
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

        private BigInteger EvaluatePolynomial(long x)
        {
            var xPrime = sqrtN + x;
            return xPrime * xPrime - n;
        }
    }
}
