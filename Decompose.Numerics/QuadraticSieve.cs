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
        public QuadraticSieve(int threads, int factorBaseSize, int lowerBoundPercent)
        {
            diag = Diag.Summary | Diag.Sieve;
            threadsOverride = threads;
            factorBaseSizeOverride = factorBaseSize;
            lowerBoundPercentOverride = lowerBoundPercent;
            smallFactorizationAlgorithm = new TrialDivision();
            primes = new SieveOfErostothones();
            nullSpaceAlgorithm = new GaussianElimination<BitArray>(threads);
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            if (n <= smallFactorCutoff)
                return smallFactorizationAlgorithm.Factor((int)n).Select(factor => (BigInteger)factor);
            var factors = new List<BigInteger>();
            FactorCore(n, factors);
            return factors;
        }

        [Flags]
        private enum Diag
        {
            None = 0,
            Summary = 1,
            Solutions = 2,
            Sieve = 4,
        }

        private class Candidate
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
        private const int surplusCandidates = 10;
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
        private IFactorizationAlgorithm<int> smallFactorizationAlgorithm;
        private IEnumerable<int> primes;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> nullSpaceAlgorithm;

        private int intervalSize;
        private int nextIntervalId;

        private Diag diag;
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
        private bool detectPrimeCofactors;

        private int threads;
        private BlockingCollection<Candidate> candidateBuffer;
        private List<Candidate> candidates;
        private Dictionary<long, long> relations;
        private IBitMatrix matrix;

        private int intervalsProcessed;
        private int relationsProcessed;
        private int relationsConverted;

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

        private BigInteger GetDivisor(BigInteger n)
        {
            if (n.IsEven)
                return BigIntegers.Two;
            this.n = n;
            sqrtN = IntegerMath.Sqrt(n);
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
            rootsDiff = factorBase.Zip(roots, (p, root) => p - 2 * root).ToArray();
            sqrtNOffsets = factorBase.Zip(roots, (p, root) => ((int)((root - sqrtN) % p) + p) % p).ToArray();
            int desired = factorBaseSize + 1 + surplusCandidates;
            long maximumDivisor = factorBase[factorBaseSize - 1];
            maximumDivisorSquared = maximumDivisor * maximumDivisor;
            logMaximumDivisorSquared = LogScale(maximumDivisorSquared);
            int digits = (int)Math.Ceiling(BigInteger.Log(n, 10));
            detectPrimeCofactors = digits > 50;
            var words = IntegerMath.QuotientCeiling(n.GetBitLength(), Word32Integer.WordLength);
            nRep = new Word32Integer(words * 2 + 1).Set(n);
            sqrtNRep = nRep.Copy().Set(sqrtN);

            intervalsProcessed = 0;
            relationsProcessed = 0;
            relationsConverted = 0;

            Sieve(desired, SieveQuadraticResidue);
            var result = nullSpaceAlgorithm.Solve(matrix)
                .Select(v => ComputeFactor(v))
                .Where(factor => !factor.IsZero)
                .Take(1)
                .FirstOrDefault();

            if ((diag & Diag.Summary) != 0)
            {
                Console.WriteLine("threads = {0}, lowerBound = {1}", threadsOverride, lowerBoundPercentOverride);
                Console.WriteLine("digits = {0}, factorBaseSize = {1}, desired = {2}", digits, factorBaseSize, desired);
                Console.WriteLine("intervals processed = {0}", intervalsProcessed);
                Console.WriteLine("relations processed = {0}, converted = {1}", relationsProcessed, relationsConverted);
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
            int digits = (int)Math.Ceiling(BigInteger.Log(n) / Math.Log(10));
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

        private CountInt CalculateLowerBound(BigInteger y)
        {
            if (detectPrimeCofactors)
            {
                int percent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefaultCofactors;
                return (CountInt)((LogScale(y) - logMaximumDivisorSquared / 2) * percent / 100);
            }
            else
            {
                int percent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefaultNoCofactors;
                return (CountInt)(LogScale(y) * percent / 100);
            }
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
            var xPrime = indices.Select(index => candidates[index].X).ProductModulo(n);
            var exponents = SumExponents(indices);
            var yFactorBase = new[] { -1 }.Concat(factorBase).Zip(exponents,
                (p, exponent) => BigInteger.Pow(p, exponent / 2));
            var yCofactors = indices
                .Select(index => (BigInteger)candidates[index].Cofactor)
                .Where(cofactor => cofactor != 0);
            var yPrime = yFactorBase.Concat(yCofactors).ProductModulo(n);
            var factor = BigInteger.GreatestCommonDivisor(xPrime + yPrime, n);
            if (!factor.IsOne && factor != n)
                return factor;
            return BigInteger.Zero;
        }

        private int[] SumExponents(IEnumerable<int> indices)
        {
            var results = new int[factorBaseSize + 1];
            foreach (int index in indices)
            {
                var exponents = candidates[index].Exponents;
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
            candidateBuffer = new BlockingCollection<Candidate>(desired);
            relations = new Dictionary<long, long>();

            if (threads == 1)
            {
                candidates = new List<Candidate>();
                var interval = CreateInterval();
                while (candidates.Count < desired)
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

            candidates = candidateBuffer.ToList();
            candidateBuffer = null;
            relations = null;
            ProcessCandidates();
        }

        private void SieveThread(Func<Interval, int> sieveCore)
        {
            int count = 0;
            int intervals = 0;
            var interval = CreateInterval();
            while (candidateBuffer.Count < candidateBuffer.BoundedCapacity)
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
            var exponents = interval.Exponents;
            for (int k = 0; k < interval.Size; k++)
            {
                if (IsSmooth(interval, k))
                {
                    var candidate = new Candidate
                    {
                        X = sqrtN + interval.X + k,
                        Exponents = (int[])exponents.Clone(),
                    };
                    ++count;
                    if (!candidateBuffer.TryAdd(candidate))
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
            var min = interval.X > 0 ? interval.X : interval.X + interval.Size;
            CountInt countLimit = CalculateLowerBound(EvaluatePolynomial(min));
            for (int k0 = 0; k0 < intervalSize; k0 += subIntervalSize)
            {
                int size = Math.Min(subIntervalSize, intervalSize - k0);
                SieveInterval(interval, k0, size);
                count += CheckForSmooth(interval, k0, size, countLimit);
            }
            return count;
        }

        private void SieveInterval(Interval interval, int k0, int size)
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
                    Debug.Assert(EvaluatePolynomial(interval.X + k0 + k) % 2 == 0);
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
                    Debug.Assert(EvaluatePolynomial(interval.X + k0 + k - p2) % p == 0);
                    counts[k - p2] += logP;
                }
                while (k < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.X + k0 + k) % p == 0);
                    counts[k] += logP;
                    k += p1;
                    Debug.Assert(EvaluatePolynomial(interval.X + k0 + k) % p == 0);
                    counts[k] += logP;
                    k += p2;
                }
                offsets[i] = k - size;

            }
        }

        private int CheckForSmooth(Interval interval, int k0, int size, CountInt countLimit)
        {
            int count = 0;
            var counts = interval.Counts;
            for (int k = 0; k < size; k++)
            {
                if (counts[k] >= countLimit)
                {
                    if (IsSmooth(interval, k0 + k))
                    {
                        var candidate = new Candidate
                        {
                            X = sqrtN + interval.X + k0 + k,
                            Exponents = (int[])interval.Exponents.Clone(),
                        };
                        ++count;
                        if (!candidateBuffer.TryAdd(candidate))
                            return count;
                    }
                }
                counts[k] = 0;
            }
            return count;
        }

        private bool IsSmooth(Interval interval, int k)
        {
            return IsSmoothWord32Integer(interval, k);
        }

        private bool IsSmoothWord32Integer(Interval interval, int k)
        {
            long y = FactorOverBase(interval, interval.X + k);
            if (detectPrimeCofactors && y != 1 && y < maximumDivisorSquared)
                ProcessRelation(interval, k, y);
            return y == 1;
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
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                while (y.GetRemainder((uint)p) == 0)
                {
                    ++exponents[i + 1];
                    y.Divide((uint)p, z);
                }
            }
            return y < (ulong)long.MaxValue ? (long)(ulong)y : 0;
        }

        private bool IsSmoothBigInteger(Interval interval, int k)
        {
            var exponents = interval.Exponents;
            var y = EvaluatePolynomial(interval.X + k);
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
            if (detectPrimeCofactors && !y.IsOne && y < maximumDivisorSquared)
                ProcessRelation(interval, k, (long)y);
            return y.IsOne;
        }

        private bool ProcessRelation(Interval interval, int k, long cofactor)
        {
            long other;
            lock (relations)
            {
                if (relations.TryGetValue(cofactor, out other))
                    relations.Remove(cofactor);
                else
                    relations.Add(cofactor, interval.X + k);
            }
            if (other != 0)
            {
                var candidate = new Candidate
                {
                    X = (sqrtN + interval.X + k) * (sqrtN + other),
                    Exponents = (int[])interval.Exponents.Clone(),
                    Cofactor = cofactor,
                };
                ++relationsConverted;
                var y = FactorOverBase(interval, other);
                for (int i = 0; i <= factorBaseSize; i++)
                    candidate.Exponents[i] += interval.Exponents[i];
                return candidateBuffer.TryAdd(candidate);
            }
            return true;
        }

        private void ProcessCandidates()
        {
            Debug.Assert(candidates.GroupBy(candidate => candidate.X).Count() == candidates.Count);
            matrix = new BitMatrix(factorBaseSize + 1, candidates.Count);
            int cacheSize = matrix.WordLength;
            var cache = new BitMatrix(matrix.Rows, cacheSize);
            for (int j = 0; j < candidates.Count; j += cacheSize)
            {
                int limit = Math.Min(cacheSize, candidates.Count - j);
                for (int k = 0; k < limit; k++)
                {
                    var exponents = candidates[j + k].Exponents;
                    for (int i = 0; i < exponents.Length; i++)
                        cache[i, k] = exponents[i] % 2 != 0;
                }
                matrix.CopySubMatrix(cache, 0, j);
                cache.Clear();
            }
        }

        private void ProcessCandidatesSimple()
        {
            Debug.Assert(candidates.GroupBy(candidate => candidate.X).Count() == candidates.Count);
            matrix = new BitMatrix(factorBaseSize + 1, candidates.Count);
            for (int j = 0; j < candidates.Count; j++)
            {
                var exponents = candidates[j].Exponents;
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
