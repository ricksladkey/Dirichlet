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
#else
using BitMatrix = Decompose.Numerics.Word64BitMatrix;
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
            Verbose = Summary | Sieve | Timing,
        }

        public class Config
        {
            public Diag Diagnostics { get; set; }
            public int Threads { get; set; }
            public int FactorBaseSize { get; set; }
            public int LowerBoundPercent { get; set; }
            public bool? ProcessPartialRelations { get; set; }
            public int Multiplier { get; set; }
        }

        public QuadraticSieve(Config config)
        {
            diag = config.Diagnostics;
            threadsOverride = config.Threads;
            factorBaseSizeOverride = config.FactorBaseSize;
            lowerBoundPercentOverride = config.LowerBoundPercent;
            processPartialRelationsOverride = config.ProcessPartialRelations;
            multiplierOverride = config.Multiplier;
            smallFactorizationAlgorithm = new TrialDivision();
            primes = new SieveOfErostothones();
            nullSpaceAlgorithm = new Solver(config.Threads);
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            if (n <= smallFactorCutoff)
                return smallFactorizationAlgorithm.Factor((int)n).Select(factor => (BigInteger)factor);
            var factors = new List<BigInteger>();
            FactorCore(n, factors);
            return factors;
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
            public int[] Offsets { get; set; }
            public Word32Integer Reg1 { get; set; }
            public Word32Integer Reg2 { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}, Size = {1}", X, Size);
            }
        }

        private const int subIntervalSize = 256 * 1024;
        private const int maximumIntervalSize = subIntervalSize * 8;
        private const int lowerBoundPercentDefaultNoCofactors = 85;
        private const int lowerBoundPercentDefaultCofactors = 75;
        private const int surplusRelations = 10;
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
            Tuple.Create(90, 60000), // http://www.mersenneforum.org/showthread.php?t=4013
        };

        private int threadsOverride;
        private int factorBaseSizeOverride;
        private int lowerBoundPercentOverride;
        private bool? processPartialRelationsOverride;
        private int multiplierOverride;
        private IFactorizationAlgorithm<int> smallFactorizationAlgorithm;
        private IEnumerable<int> primes;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> nullSpaceAlgorithm;

        private int intervalSize;
        private int nextIntervalId;

        private Diag diag;
        private int multiplier;
        private int[] multiplierFactors;
        private BigInteger nOrig;
        private BigInteger n;
        private BigInteger sqrtN;
        private CountInt logSqrtN;
        private int factorBaseSize;
        private FactorBaseEntry[] factorBase;
        private long maximumDivisorSquared;
        private CountInt logMaximumDivisorSquared;
        private Word32Integer nRep;
        private Word32Integer sqrtNRep;
        private bool processPartialRelations;

        private int threads;
        private BlockingCollection<Relation> relationBuffer;
        private Relation[] relations;
        private Dictionary<long, long> partialRelations;
        private IBitMatrix matrix;
        private Stopwatch timer;

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
            if (multiplierOverride != 0)
                multiplier = multiplierOverride;
            else
            {
                multiplier = 1;
#if false
                if (IntegerMath.JacobiSymbol(nOrig, 2) == 1)
                {
                    switch ((int)(nOrig % 8))
                    {
                        case 3:
                            multiplier = 5;
                            break;
                        case 5:
                            multiplier = 3;
                            break;
                        case 7:
                            multiplier = 7;
                            break;
                    }
                }
                Console.WriteLine("multiplier = {0}, J({1} * n) = {2})", multiplier, multiplier, IntegerMath.JacobiSymbol(nOrig * multiplier, 2));
#endif
            }
            multiplierFactors = smallFactorizationAlgorithm.Factor(multiplier).ToArray();
            this.nOrig = nOrig;
            n = nOrig * multiplier;
            sqrtN = IntegerMath.Sqrt(this.n);
            logSqrtN = LogScale(sqrtN);
            factorBaseSize = CalculateFactorBaseSize(nOrig);
            factorBase = primes
                .Where(p => IntegerMath.JacobiSymbol(n, p) == 1)
                .Take(factorBaseSize)
                .Select(p => new FactorBaseEntry(p, n, sqrtN))
                .ToArray();
            int desired = factorBaseSize + 1 + surplusRelations;
            long maximumDivisor = factorBase[factorBaseSize - 1].P;
            maximumDivisorSquared = maximumDivisor * maximumDivisor;
            logMaximumDivisorSquared = LogScale(maximumDivisorSquared);
            int digits = (int)Math.Ceiling(BigInteger.Log(n, 10));
            processPartialRelations = processPartialRelationsOverride.HasValue ? processPartialRelationsOverride.Value : true;
            var words = IntegerMath.QuotientCeiling(n.GetBitLength(), Word32Integer.WordLength);
            nRep = new Word32Integer(words * 2 + 1).Set(n);
            sqrtNRep = nRep.Copy().Set(sqrtN);

            intervalsProcessed = 0;
            valuesChecked = 0;
            partialRelationsProcessed = 0;
            partialRelationsConverted = 0;

            if ((diag & Diag.Timing) != 0)
            {
                timer = new Stopwatch();
                timer.Start();
            }

            Sieve(desired);

            if ((diag & Diag.Summary) != 0)
            {
                Console.WriteLine("digits = {0}, factorBaseSize = {1}, desired = {2}", digits, factorBaseSize, desired);
                Console.WriteLine("intervals processsed = {0}, values checked = {1}", intervalsProcessed, valuesChecked);
                Console.WriteLine("partial relations processed = {0}, converted = {1}", partialRelationsProcessed, partialRelationsConverted);
                Console.WriteLine("first few factors: {0}", string.Join(", ", factorBase.Select(entry => entry.P).Take(10).ToArray()));
            }
            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Sieving: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            ProcessRelations();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Processing relations: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            var result = nullSpaceAlgorithm.Solve(matrix)
                .Select(v => ComputeFactor(v))
                .Where(factor => !factor.IsZero)
                .Take(1)
                .FirstOrDefault();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Stop();
                Console.WriteLine("Gaussian elimination: {0:F3}", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            return result;
        }

        private void CalculateNumberOfThreads()
        {
            threads = threadsOverride != 0 ? threadsOverride : 1;
            if (n <= BigInteger.Pow(10, 10))
                threads = 1;
        }

        private int CalculateFactorBaseSize(BigInteger n)
        {
            if (factorBaseSizeOverride != 0)
                return factorBaseSizeOverride;
            int digits = (int)Math.Ceiling(BigInteger.Log(nOrig, 10));
            for (int i = 0; i < sizePairs.Length - 1; i++)
            {
                var pair = sizePairs[i];
                if (digits >= sizePairs[i].Item1 && digits <= sizePairs[i + 1].Item1)
                {
                    // Interpolate.
                    double x0 = sizePairs[i].Item1;
                    double x1 = sizePairs[i + 1].Item1;
                    double y0 = sizePairs[i].Item2;
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
            if (processPartialRelations)
            {
                int percent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefaultCofactors;
                return (CountInt)(LogScale(y) - (logMaximumDivisorSquared * (200 - percent) / 200));
            }
            else
            {
                int percent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefaultNoCofactors;
                return (CountInt)(LogScale(y) * percent / 100);
            }
        }

        private BigInteger ComputeFactor(IBitArray v)
        {
            if ((diag & Diag.Solutions) != 0)
                Console.WriteLine("v = {0}", string.Join(", ", v.GetNonZeroIndices().ToArray()));

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
            if (!factor.IsOne && factor != nOrig)
                return factor;
            return BigInteger.Zero;
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

        private void Sieve(int desired)
        {
            CalculateNumberOfThreads();
            SetupIntervals();
            relationBuffer = new BlockingCollection<Relation>(desired);
            partialRelations = new Dictionary<long, long>();

            if (threads == 1)
                SieveThread();
            else
            {
                var tasks = new Task[threads];
                for (int i = 0; i < threads; i++)
                    tasks[i] = Task.Factory.StartNew(SieveThread);
                Task.WaitAll(tasks);
            }

            relations = relationBuffer.ToArray();
            relationBuffer = null;
            partialRelations = null;
        }

        private void SieveThread()
        {
            int count = 0;
            int intervals = 0;
            var interval = CreateInterval();
            while (relationBuffer.Count < relationBuffer.BoundedCapacity)
            {
                count += Sieve(GetNextInterval(interval));
                if ((diag & Diag.Sieve) != 0)
                {
                    if (++intervals % 500 == 0)
                    {
                        Console.WriteLine("count = {0}", count);
                        count = 0;
                    }
                }
                Interlocked.Increment(ref intervalsProcessed);
            }
        }

        private Interval CreateInterval()
        {
            var interval = new Interval();
            interval.Exponents = new byte[factorBaseSize + 1];
            interval.Counts = new CountInt[subIntervalSize];
            interval.Offsets = new int[factorBaseSize];
            interval.Reg1 = nRep.Copy();
            interval.Reg2 = nRep.Copy();
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
            var offsets = interval.Offsets;
            for (int i = 0; i < factorBaseSize; i++)
            {
                var entry = factorBase[i];
                var p = entry.P;
                offsets[i] = ((int)((entry.Offset - x) % p) + p) % p;
            }
            interval.OffsetX = x;
            return interval;
        }

        private int Sieve(Interval interval)
        {
#if false
            return SieveTrialDivision(interval);
#else
            return SieveQuadraticResidue(interval);
#endif
        }

        private int SieveTrialDivision(Interval interval)
        {
            int count = 0;
            for (int k = 0; k < interval.Size; k++)
            {
                ++valuesChecked;
                var relation = GetRelation(interval, k);
                if (relation != null)
                {
                    ++count;
                    if (!relationBuffer.TryAdd(relation))
                        break;
                }
            }
            return count;
        }

        private int SieveQuadraticResidue(Interval interval)
        {
            int count = 0;
            int intervalSize = interval.Size;
            var xMin = interval.X > 0 ? interval.X : interval.X + interval.Size;
            CountInt countLimit = CalculateLowerBound(xMin);
            for (int k0 = 0; k0 < intervalSize; k0 += subIntervalSize)
            {
                int size = Math.Min(subIntervalSize, intervalSize - k0);
                SieveInterval(interval, size);
                count += CheckForSmooth(interval, size, countLimit);
                if (relationBuffer.Count >= relationBuffer.BoundedCapacity)
                    break;
                interval.X += subIntervalSize;
            }
            return count;
        }

        private void SieveInterval(Interval interval, int size)
        {
            var offsets = interval.Offsets;
            var counts = interval.Counts;
            int i = 0;
            if (factorBase[0].P == 2)
            {
#if false
                var logP = logFactorBase[0];
                int k;
                for (k = offsets[0]; k < size; k += 2)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % 2 == 0);
                    counts[k] += logP;
                }
                offsets[0] = k - size;
#else
                if ((size & 1) == 1)
                    offsets[0] = 1 - offsets[0];
#endif
                ++i;
            }
            while (i < factorBaseSize)
            {
                var entry = factorBase[i];
                int p = entry.P;
                if (p >= size)
                    break;
                var logP = entry.LogP;
                int p1 = entry.RootDiff;
                int p2 = p - p1;
                int k = offsets[i];
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
                    k += p1;
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % p == 0);
                    counts[k] += logP;
                    k += p2;
                }
                if (k < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k + p1) % p == 0);
                    counts[k] += logP;
                    k += p;
                }
                offsets[i++] = k - size;
            }
            while (i < factorBaseSize)
            {
                var entry = factorBase[i];
                int p = entry.P;
                int p1 = entry.RootDiff;
                int p2 = p - p1;
                int k = offsets[i];
                if (k >= p2 && k - p2 < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k - p2) % p == 0);
                    counts[k - p2] += entry.LogP;
                }
                else if (k + p1 < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k + p1) % p == 0);
                    counts[k + p1] += entry.LogP;
                }
                if (k < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % p == 0);
                    counts[k] += entry.LogP;
                    k += p;
                }
                offsets[i++] = k - size;
            }
            interval.OffsetX = interval.X + size;
        }

        private int CheckForSmooth(Interval interval, int size, CountInt countLimit)
        {
            int count = 0;
            var counts = interval.Counts;
            for (int k = 0; k < size; k++)
            {
                if (counts[k] >= countLimit)
                {
                    ++valuesChecked;
                    var relation = GetRelation(interval, k);
                    if (relation != null)
                    {
                        ++count;
                        if (!relationBuffer.TryAdd(relation))
                            break;
                    }
                }
                counts[k] = 0;
            }
            return count;
        }

        private Relation GetRelation(Interval interval, int k)
        {
            long y = FactorOverBase(interval, interval.X + k);
            if (y == 0)
                return null;
            if (y == 1)
            {
                return new Relation
                {
                    X = sqrtN + interval.X + k,
                    Entries = GetEntries(interval.Exponents),
                };
            }
            if (processPartialRelations && y < maximumDivisorSquared)
                return ProcessPartialRelation(interval, k, y);
            return null;
        }

        private long FactorOverBase(Interval interval, long x)
        {
#if false
            return FactorOverBaseBigInteger(interval, x);
#else
            return FactorOverBaseWord32Integer(interval, x);
#endif
        }

        private long FactorOverBaseBigInteger(Interval interval, long x)
        {
            var y = EvaluatePolynomial(x);
            var exponents = interval.Exponents;
            for (int i = 0; i <= factorBaseSize; i++)
                exponents[i] = 0;
            if (y < 0)
            {
                exponents[0] = 1;
                y = -y;
            }
            var delta = x - interval.OffsetX;
            var offsets = interval.Offsets;
            for (int i = 0; i < factorBaseSize; i++)
            {
                var entry = factorBase[i];
                var p = entry.P;
                var offset = (int)((delta - offsets[i]) % p);
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

        private long FactorOverBaseWord32Integer(Interval interval, long x)
        {
            var y = interval.Reg1;
            var z = interval.Reg2;
            bool negative;
            EvaluatePolynomial(x, y, z, out negative);
            var exponents = interval.Exponents;
            for (int i = 0; i <= factorBaseSize; i++)
                exponents[i] = 0;
            if (negative)
                exponents[0] = 1;
            var delta = x - interval.OffsetX;
            var offsets = interval.Offsets;
            for (int i = 0; i < factorBaseSize; i++)
            {
                var entry = factorBase[i];
                var p = entry.P;
                var offset = (int)((delta - offsets[i]) % p);
                if (offset < 0)
                    offset += p;
                if (offset != 0 && offset != entry.RootDiff)
                {
                    Debug.Assert(y.ToBigInteger() % p != 0);
                    continue;
                }
                Debug.Assert(y.ToBigInteger() % p == 0);
                while (y.GetRemainder((uint)p) == 0)
                {
                    ++exponents[i + 1];
                    y.Divide((uint)p, z);
                }
            }
            return y < (ulong)long.MaxValue ? (long)(ulong)y : 0;
        }

        private Relation ProcessPartialRelation(Interval interval, int k, long cofactor)
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
                return null;
            ++partialRelationsConverted;
            var relation = new Relation
            {
                X = (sqrtN + interval.X + k) * (sqrtN + other),
                Entries = GetEntries(interval.Exponents),
                Cofactor = cofactor,
            };
            FactorOverBase(interval, other);
            relation.Entries = CombineEntries(relation.Entries, GetEntries(interval.Exponents));
            return relation;
        }

        private void ProcessRelations()
        {
            ProcessRelationsSimple();
#if false
            using (var stream = new StreamWriter("matrix.txt"))
            {
                for (int i = 0; i < matrix.Rows; i++)
                    stream.WriteLine(string.Concat(matrix.GetRow(i).Select(bit => bit ? 1 : 0).ToArray()));
            }
#endif
        }

        private void ProcessRelationsSimple()
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

        private void ProcessRelationsWithCache()
        {
            Debug.Assert(relations.GroupBy(relation => relation.X).Count() == relations.Length);
            matrix = new BitMatrix(factorBaseSize + 1, relations.Length);
            int cacheSize = matrix.WordLength;
            var cache = new BitMatrix(matrix.Rows, cacheSize);
            for (int j = 0; j < relations.Length; j += cacheSize)
            {
                int limit = Math.Min(cacheSize, relations.Length - j);
                for (int k = 0; k < limit; k++)
                {
                    var entries = relations[j + k].Entries;
                    for (int i = 0; i < entries.Length; i++)
                    {
                        var entry = entries[i];
                        cache[entry.Index, k] = entry.Exponent % 2 != 0;
                    }
                }
                matrix.CopySubMatrix(cache, 0, j);
                cache.Clear();
            }
        }

        private ExponentEntry[] GetEntries(byte[] exponents)
        {
            return exponents
                .Select((exponent, index) => new ExponentEntry { Index = index, Exponent = exponent })
                .Where(entry => entry.Exponent != 0)
                .ToArray();
        }

        private ExponentEntry[] CombineEntries(ExponentEntry[] a, ExponentEntry[] b)
        {
            return a.Concat(b)
                .OrderBy(entry => entry.Index)
                .GroupBy(entry => entry.Index)
                .Select(grouping =>
                    new ExponentEntry
                    {
                        Index = grouping.Key,
                        Exponent = grouping.Sum(entry => entry.Exponent),
                    })
                .ToArray();
        }

        private BigInteger EvaluatePolynomial(long x)
        {
            var xPrime = sqrtN + x;
            return xPrime * xPrime - n;
        }

        private Word32Integer EvaluatePolynomial(long x, Word32Integer y, Word32Integer reg1, out bool negative)
        {
            reg1.Set(sqrtNRep);
            if (x < 0)
                reg1.Subtract(y.Set((ulong)-x));
            else
                reg1.Add(y.Set((ulong)x));
            y.SetSquare(reg1);
            if (y < nRep)
            {
                y.SetDifference(nRep, y);
                negative = true;
            }
            else
            {
                y.Subtract(nRep);
                negative = false;
            }
            return y;
        }
    }
}
