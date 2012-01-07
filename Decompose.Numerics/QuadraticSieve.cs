using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using BitArray = Decompose.Numerics.Word64BitArray;
using BitMatrix = Decompose.Numerics.Word64BitMatrix;
using CountInt = System.UInt16;

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
            nullSpaceAlgorithm = new GaussianElimination<BitArray>(config.Threads);
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            if (n <= smallFactorCutoff)
                return smallFactorizationAlgorithm.Factor((int)n).Select(factor => (BigInteger)factor);
            var factors = new List<BigInteger>();
            FactorCore(n, factors);
            return factors;
        }

        private class Relation
        {
            public BigInteger X { get; set; }
            public int[] Exponents { get; set; }
            public long Cofactor { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}", X);
            }
        }

        private class Interval
        {
            public long X { get; set; }
            public long OffsetRef { get; set; }
            public int Size { get; set; }
            public int[] Exponents { get; set; }
            public CountInt[] Counts { get; set; }
            public int[] Offsets { get; set; }
            public Word32Integer Reg1 { get; set; }
            public Word32Integer Reg2 { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}, Size = {1}", X, Size);
            }
        }

        private const int maximumIntervalSize = 1000000;
        private const int subIntervalSize = 200000;
        private const int lowerBoundPercentDefaultNoCofactors = 85;
        private const int lowerBoundPercentDefaultCofactors = 95;
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
            Tuple.Create(50, 5000),
            Tuple.Create(60, 12000),
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
        private int factorBaseSize;
        private int[] factorBase;
        private CountInt[] logFactorBase;
        private int[] roots;
        private int[] sqrtNOffsets;
        private int[] rootsDiff;
        private long maximumDivisorSquared;
        private CountInt logMaximumDivisorSquared;
        private Word32Integer nRep;
        private Word32Integer sqrtNRep;
        private bool processPartialRelations;

        private int threads;
        private BlockingCollection<Relation> relationBuffer;
        private List<Relation> relations;
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
            var smallPrimes = primes.Take(5).ToArray();
            multiplier = multiplierOverride != 0 ? multiplierOverride : 1;
            multiplierFactors = smallFactorizationAlgorithm.Factor(multiplier).ToArray();
            this.nOrig = nOrig;
            n = nOrig * multiplier;
            sqrtN = IntegerMath.Sqrt(this.n);
            factorBaseSize = CalculateFactorBaseSize(n);
            factorBase = primes
                .Where(p => IntegerMath.JacobiSymbol(n, p) == 1)
                .Take(factorBaseSize)
                .ToArray();
            logFactorBase = factorBase
                .Select(factor => LogScale(factor))
                .ToArray();
            roots = factorBase
                .Select(root => (int)IntegerMath.ModularSquareRoot(n, root))
                .ToArray();
            rootsDiff = factorBase
                .Zip(roots, (p, root) => p - 2 * root)
                .ToArray();
            sqrtNOffsets = factorBase
                .Zip(roots, (p, root) => ((int)((root - sqrtN) % p) + p) % p)
                .ToArray();
            int desired = factorBaseSize + 1 + surplusRelations;
            long maximumDivisor = factorBase[factorBaseSize - 1];
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

            Sieve(desired, SieveQuadraticResidue);

            if ((diag & Diag.Summary) != 0)
            {
                Console.WriteLine("digits = {0}, factorBaseSize = {1}, desired = {2}", digits, factorBaseSize, desired);
                Console.WriteLine("values checked = {0}", valuesChecked);
                Console.WriteLine("intervals processed = {0}, relations processed = {1}, converted = {2}", intervalsProcessed, partialRelationsProcessed, partialRelationsConverted);
                Console.WriteLine("first few factors: {0}", string.Join(", ", factorBase.Take(10).ToArray()));
            }
            if ((diag & Diag.Timing) != 0)
            {
                var elapsed1 = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Sieving: {0:F3} msec", 1000.0 * elapsed1 / Stopwatch.Frequency);
            }

            var result = nullSpaceAlgorithm.Solve(matrix)
                .Select(v => ComputeFactor(v))
                .Where(factor => !factor.IsZero)
                .Take(1)
                .FirstOrDefault();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed2 = timer.ElapsedTicks;
                timer.Stop();
                Console.WriteLine("Gaussian elimination: {0:F3}", 1000.0 * elapsed2 / Stopwatch.Frequency);
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
#if true
            var y = EvaluatePolynomial(x);
            if (processPartialRelations)
            {
                int percent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefaultCofactors;
                return (CountInt)((LogScale(y) - logMaximumDivisorSquared / 2) * percent / 100);
            }
            else
            {
                int percent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefaultNoCofactors;
                return (CountInt)(LogScale(y) * percent / 100);
            }
#else
            return LogScale((2 * x) * sqrtN);
#endif
        }

        private BigInteger ComputeFactor(IBitArray v)
        {
            var indices = v
                .Select((selected, index) => new { Index = index, Selected = selected })
                .Where(pair => pair.Selected)
                .Select(pair => pair.Index)
                .ToArray();
            if ((diag & Diag.Solutions) != 0)
                Console.WriteLine("v = {0}", string.Join(", ", indices));
            var xPrime = indices.Select(index => relations[index].X).ProductModulo(n);
            var exponents = SumExponents(indices);
            var yFactorBase = new[] { -1 }.Concat(factorBase).Zip(exponents,
                (p, exponent) => BigInteger.Pow(p, exponent / 2));
            var yCofactors = indices
                .Select(index => (BigInteger)relations[index].Cofactor)
                .Where(cofactor => cofactor != 0);
            var yPrime = yFactorBase.Concat(yCofactors).ProductModulo(n);
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
                var exponents = relations[index].Exponents;
                for (int i = 0; i <= factorBaseSize; i++)
                    results[i] += exponents[i];
            }
            Debug.Assert(results.All(exponent => exponent % 2 == 0));
            return results;
        }

        private static CountInt LogScale(BigInteger n)
        {
            return (CountInt)Math.Ceiling(10 * BigInteger.Log(BigInteger.Abs(n)));
        }

        private void Sieve(int desired, Func<Interval, int> sieveCore)
        {
            CalculateNumberOfThreads();
            SetupIntervals();
            relationBuffer = new BlockingCollection<Relation>(desired);
            partialRelations = new Dictionary<long, long>();

            if (threads == 1)
            {
                relations = new List<Relation>();
                var interval = CreateInterval();
                while (relations.Count < desired)
                {
                    sieveCore(GetNextInterval(interval));
                    ++intervalsProcessed;
                }
            }
            else
            {
                var tasks = new Task[threads];
                for (int i = 0; i < threads; i++)
                    tasks[i] = Task.Factory.StartNew(() => SieveThread(sieveCore));
                Task.WaitAll(tasks);
            }

            relations = relationBuffer.ToList();
            relationBuffer = null;
            partialRelations = null;
            ProcessRelations();
        }

        private void SieveThread(Func<Interval, int> sieveCore)
        {
            int count = 0;
            int intervals = 0;
            var interval = CreateInterval();
            while (relationBuffer.Count < relationBuffer.BoundedCapacity)
            {
                count += sieveCore(GetNextInterval(interval));
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
            interval.Exponents = new int[factorBaseSize + 1];
            interval.Counts = new CountInt[subIntervalSize + factorBase[factorBaseSize - 1]];
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
            interval.X = (long)intervalNumber * intervalSize;
            interval.Size = intervalSize;
            return interval;
        }

        private int SieveTrialDivision(Interval interval)
        {
            int count = 0;
            for (int k = 0; k < interval.Size; k++)
            {
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
            var offsets = interval.Offsets;
            for (int i = 0; i < factorBaseSize; i++)
            {
                int p = factorBase[i];
                offsets[i] = ((int)((sqrtNOffsets[i] - interval.X) % p) + p) % p;
            }

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
            int i0 = 0;
            if (factorBase[0] == 2)
            {
                var logP = logFactorBase[0];
                int k;
                for (k = offsets[0]; k < size; k += 2)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % 2 == 0);
                    counts[k] += logP;
                }
                offsets[0] = k - size;
                ++i0;
            }
            for (int i = i0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                var logP = logFactorBase[i];
                int p1 = rootsDiff[i];
                int p2 = p - p1;
                int k = offsets[i];
                if (k >= p2 && k - p2 < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k - p2) % p == 0);
                    counts[k - p2] += logP;
                }
                while (k < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % p == 0);
                    counts[k] += logP;
                    k += p1;
                    Debug.Assert(EvaluatePolynomial(interval.X + k) % p == 0);
                    counts[k] += logP;
                    k += p2;
                }
                offsets[i] = k - size;

            }
            interval.OffsetRef = interval.X + size;
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
            return GetRelationWord32Integer(interval, k);
        }

        private Relation GetRelationWord32Integer(Interval interval, int k)
        {
            long y = FactorOverBase(interval, interval.X + k);
            if (y == 0)
                return null;
            if (y == 1)
            {
                return new Relation
                {
                    X = sqrtN + interval.X + k,
                    Exponents = (int[])interval.Exponents.Clone(),
                };
            }
            if (processPartialRelations && y < maximumDivisorSquared)
                return ProcessPartialRelation(interval, k, y);
            return null;
        }

        private long FactorOverBase(Interval interval, long x)
        {
            var exponents = interval.Exponents;
            var y = interval.Reg1;
            var z = interval.Reg2;
            for (int i = 0; i <= factorBaseSize; i++)
                exponents[i] = 0;
            bool negative;
            EvaluatePolynomial(x, y, z, out negative);
            if (negative)
                exponents[0] = 1;
            var delta = x - interval.OffsetRef;
            var offsets = interval.Offsets;
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
#if true
                var offset = (int)((delta - offsets[i]) % p);
                if (offset < 0)
                    offset += p;
                if (offset != 0 && offset != rootsDiff[i])
                    continue;
#endif
                while (y.GetRemainder((uint)p) == 0)
                {
                    ++exponents[i + 1];
                    y.Divide((uint)p, z);
                }
            }
            return y < (ulong)long.MaxValue ? (long)(ulong)y : 0;
        }

        private Relation GetRelationBigInteger(Interval interval, int k)
        {
            var y = EvaluatePolynomial(interval.X + k);
            var exponents = interval.Exponents;
            for (int i = 0; i <= factorBaseSize; i++)
                exponents[i] = 0;
            if (y < 0)
            {
                exponents[0] = 1;
                y = -y;
            }
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                while ((y % p).IsZero)
                {
                    ++exponents[i + 1];
                    y /= p;
                }
            }
            if (processPartialRelations && y.IsOne && y < maximumDivisorSquared)
                return ProcessPartialRelation(interval, k, (long)y);
            if (!y.IsOne)
                return null;
            return new Relation
            {
                X = sqrtN + interval.X + k,
                Exponents = (int[])interval.Exponents.Clone(),
            };
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
                Exponents = (int[])interval.Exponents.Clone(),
                Cofactor = cofactor,
            };
            FactorOverBase(interval, other);
            for (int i = 0; i <= factorBaseSize; i++)
                relation.Exponents[i] += interval.Exponents[i];
            return relation;
        }

        private void ProcessRelations()
        {
            Debug.Assert(relations.GroupBy(relation => relation.X).Count() == relations.Count);
            matrix = new BitMatrix(factorBaseSize + 1, relations.Count);
            int cacheSize = matrix.WordLength;
            var cache = new BitMatrix(matrix.Rows, cacheSize);
            for (int j = 0; j < relations.Count; j += cacheSize)
            {
                int limit = Math.Min(cacheSize, relations.Count - j);
                for (int k = 0; k < limit; k++)
                {
                    var exponents = relations[j + k].Exponents;
                    for (int i = 0; i < exponents.Length; i++)
                        cache[i, k] = exponents[i] % 2 != 0;
                }
                matrix.CopySubMatrix(cache, 0, j);
                cache.Clear();
            }
        }

        private void ProcessRelationsSimple()
        {
            Debug.Assert(relations.GroupBy(relation => relation.X).Count() == relations.Count);
            matrix = new BitMatrix(factorBaseSize + 1, relations.Count);
            for (int j = 0; j < relations.Count; j++)
            {
                var exponents = relations[j].Exponents;
                for (int i = 0; i < exponents.Length; i++)
                    matrix[i, j] = exponents[i] % 2 != 0;
            }
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
