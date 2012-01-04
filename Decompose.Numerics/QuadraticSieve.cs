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

namespace Decompose.Numerics
{
    public class QuadraticSieve : IFactorizationAlgorithm<BigInteger>
    {
        public QuadraticSieve(int threads, int factorBaseSize, int lowerBoundPercent)
        {
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

        private class Candidate
        {
            public BigInteger X { get; set; }
            public int[] Exponents { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}", X);
            }
        }

        private class Interval
        {
            public BigInteger X { get; set; }
            public int Size { get; set; }
            public int[] Exponents { get; set; }
            public ushort[] Counts { get; set; }
            public int[] Offsets { get; set; }
            public Word32Integer XRep { get; set; }
            public Word32Integer Reg1 { get; set; }
            public Word32Integer Reg2 { get; set; }
            public Word32Integer Reg3 { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}, Size = {1}", X, Size);
            }
        }

        private const int maximumIntervalSize = 200000;
        private const int lowerBoundPercentDefault = 85;
        private const int surplusCandidates = 10;
        private readonly BigInteger smallFactorCutoff = (BigInteger)int.MaxValue;

        private int threadsOverride;
        private int factorBaseSizeOverride;
        private int lowerBoundPercentOverride;
        private IFactorizationAlgorithm<int> smallFactorizationAlgorithm;
        private IEnumerable<int> primes;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> nullSpaceAlgorithm;

        private BigInteger xPos;
        private BigInteger xNeg;
        private int[] offsetsPos;
        private int[] offsetsNeg;
        private int nextInterval;
        private int intervalSize;

        private Tuple<int, int>[] sizePairs =
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

        private BigInteger n;
        private BigInteger sqrtN;
        private int factorBaseSize;
        private int[] factorBase;
        private ushort[] logFactorBase;
        private int[] root1;
        private int[] root2;
        private int[] rootDiff;
        private Word32Integer nRep;

        private int threads;
        private CancellationToken token;
        private BlockingCollection<Candidate> candidateBuffer;
        private BlockingCollection<Interval> newIntervalBuffer;
        private BlockingCollection<Interval> oldIntervalBuffer;
        private List<Candidate> candidates;
        private IBitMatrix matrix;

        private int intervalsProduced;
        private int intervalsProcessed;

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
            var words = IntegerMath.QuotientCeiling(n.GetBitLength(), Word32Integer.WordLength);
            nRep = new Word32Integer(words * 2 + 1).Set(n);
            sqrtN = IntegerMath.Sqrt(n);
            factorBaseSize = CalculateFactorBaseSize(n);
            factorBase = primes
                .Where(p => IntegerMath.JacobiSymbol(n, p) == 1)
                .Take(factorBaseSize)
                .ToArray();
            logFactorBase = factorBase
                .Select(factor => LogScale(factor))
                .ToArray();
            root1 = factorBase
                .Select(root => (int)IntegerMath.ModularSquareRoot(n, root))
                .ToArray();
            root2 = factorBase.Zip(root1, (p, root) => p - root).ToArray();
            rootDiff = root1.Zip(root2, (r1, r2) => r2 - r1).ToArray();
            int desired = factorBaseSize + 1 + surplusCandidates;
#if false
            Sieve(desired, SieveTrialDivision);
#else
            Sieve(desired, SieveQuadraticResidue);
#endif
            foreach (var v in nullSpaceAlgorithm.Solve(matrix))
            {
                var factor = ComputeFactor(v);
                if (!factor.IsZero)
                    return factor;
            }
            return BigInteger.Zero;
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
            return (digits - 5) * 5 + digits + 1;
        }

        private ushort CalculateLowerBound(BigInteger y)
        {
            int percent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefault;
            return (ushort)(LogScale(y) * percent / 100);
        }

        private BigInteger ComputeFactor(IBitArray v)
        {
            var indices = v
                .Select((selected, index) => new { Index = index, Selected = selected })
                .Where(pair => pair.Selected)
                .Select(pair => pair.Index)
                .ToArray();
#if false
            Console.WriteLine("v = {0}", string.Join(", ", indices));
#endif
            return ComputeFactorWithExponents(indices);
        }

        private BigInteger ComputeFactorWithExponents(IEnumerable<int> indices)
        {
            var xPrime = indices.Select(index => candidates[index].X).Product(n);
            var exponents = SumExponents(indices);
            var yPrime = new[] { -1 }.Concat(factorBase).Zip(exponents,
                (p, exponent) => BigInteger.Pow(p, exponent / 2)).Product(n);
            var factor = BigInteger.GreatestCommonDivisor(xPrime + yPrime, n);
            if (!factor.IsOne && factor != n)
                return factor;
            return BigInteger.Zero;
        }

        private BigInteger ComputeFactorWithoutExponents(IEnumerable<int> indices)
        {
            var xSet = indices.Select(index => candidates[index].X).ToArray();
            var xPrime = xSet
                .Aggregate((sofar, current) => sofar * current) % n;
            var yPrimeSquared = xSet
                .Aggregate(BigInteger.One, (sofar, current) => sofar * (current * current - n));
            var yPrime = IntegerMath.Sqrt(yPrimeSquared) % n;
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

        private static ushort LogScale(BigInteger n)
        {
            return (ushort)Math.Ceiling(10 * BigInteger.Log(BigInteger.Abs(n)));
        }

        private void Sieve(int desired, Action<Interval, Func<Candidate, bool>> sieveCore)
        {
            CalculateNumberOfThreads();
            SetupIntervals();

            if (threads == 1)
            {
                candidates = new List<Candidate>();
                token = CancellationToken.None;
                var interval = new Interval();
                while (candidates.Count < desired)
                {
                    sieveCore(GetInterval(interval), candidate =>
                        {
                            if (candidates.Count < desired)
                            {
                                candidates.Add(candidate);
                                return true;
                            }
                            return false;
                        });
                    ++intervalsProcessed;
                }
                candidates.RemoveRange(desired, candidates.Count - desired);
            }
            else
            {
                candidateBuffer = new BlockingCollection<Candidate>(desired);
                newIntervalBuffer = new BlockingCollection<Interval>();
                oldIntervalBuffer = new BlockingCollection<Interval>();
                var tokenSource = new CancellationTokenSource();
                for (int i = 0; i <= threads; i++)
                    oldIntervalBuffer.Add(new Interval());
                token = tokenSource.Token;
                var producer = Task.Factory.StartNew(ProduceIntervals);
                var tasks = new Task[threads];
                for (int i = 0; i < threads; i++)
                    tasks[i] = Task.Factory.StartNew(() => SieveParallel(sieveCore));
                producer.Wait();
                candidates = candidateBuffer.ToList();
                tokenSource.Cancel();
                for (int i = 0; i <= threads; i++)
                    newIntervalBuffer.Add(null);
                Task.WaitAll(tasks);
            }
            ProcessCandidates();
#if true
            Console.WriteLine("desired = {0}", desired);
            Console.WriteLine("intervals produced = {0}", intervalsProduced);
            Console.WriteLine("intervals processed = {0}", intervalsProcessed);
#endif
        }

        private void ProduceIntervals()
        {
            while (candidateBuffer.Count < candidateBuffer.BoundedCapacity)
                newIntervalBuffer.Add(GetInterval(oldIntervalBuffer.Take()));
        }

        private void SieveParallel(Action<Interval, Func<Candidate, bool>> sieveCore)
        {
            var callback = new Func<Candidate, bool>(ParallelCallback);
            while (candidateBuffer.Count < candidateBuffer.BoundedCapacity)
            {
                var interval = newIntervalBuffer.Take();
                if (interval == null)
                    break;
                sieveCore(interval, callback);
                oldIntervalBuffer.Add(interval);
                Interlocked.Increment(ref intervalsProcessed);
            }
        }

        private bool ParallelCallback(Candidate candidate)
        {
            return candidateBuffer.TryAdd(candidate);
        }

        private void SetupIntervals()
        {
            intervalsProduced = 0;
            intervalsProcessed = 0;
            xPos = sqrtN;
            offsetsPos = new int[factorBaseSize];
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                var offset = ((int)((root1[i] - xPos) % p) + p) % p;
                offsetsPos[i] = offset;
            }
            nextInterval = -1;
            intervalSize = (int)IntegerMath.Min((sqrtN + threads - 1) / threads, maximumIntervalSize);
            xNeg = xPos - intervalSize;
            offsetsNeg = (int[])offsetsPos.Clone();
            ShiftOffsets(offsetsNeg, -intervalSize);
        }

        private Interval GetInterval(Interval interval)
        {
            ++intervalsProduced;
            if (nextInterval == 1)
            {
                SetupInterval(interval, xPos, intervalSize, offsetsPos);
                ShiftOffsets(offsetsPos, intervalSize);
                xPos += intervalSize;
                nextInterval = -nextInterval;
                return interval;
            }
            else
            {
                SetupInterval(interval, xNeg, intervalSize, offsetsNeg);
                ShiftOffsets(offsetsNeg, -intervalSize);
                xNeg -= intervalSize;
                nextInterval = -nextInterval;
                return interval;
            }
        }

        private Interval SetupInterval(Interval interval, BigInteger x, int size, int[] offsets)
        {
            interval.X = x;
            interval.Size = size;
            if (interval.Offsets == null)
                interval.Offsets = new int[factorBaseSize];
            offsets.CopyTo(interval.Offsets, 0);
            if (interval.XRep == null)
            {
                interval.XRep = nRep.Copy();
                interval.Reg1 = nRep.Copy();
                interval.Reg2 = nRep.Copy();
                interval.Reg3 = nRep.Copy();
            }
            interval.XRep.Clear();
            return interval;
        }

        private void ShiftOffsets(int[] offsets, int window)
        {
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                offsets[i] = ((offsets[i] - window) % p + p) % p;
            }
        }

        private void SieveTrialDivision(Interval interval, Func<Candidate, bool> candidateCallback)
        {
            if (interval.Exponents == null)
                interval.Exponents = new int[factorBaseSize + 1];
            var exponents = interval.Exponents;
            for (int i = 0; i < interval.Size; i++)
            {
                if (ValueIsSmooth(i, interval))
                {
                    var candidate = new Candidate
                    {
                        X = interval.X + i,
                        Exponents = (int[])exponents.Clone(),
                    };
                    if (!candidateCallback(candidate))
                        break;
                }
            }
        }

        private void SieveQuadraticResidue(Interval interval, Func<Candidate, bool> candidateCallback)
        {
            var x0 = interval.X;
            var y0 = x0 * x0 - n;
            int size = interval.Size;
            var offsets = interval.Offsets;
            if (interval.Exponents == null)
                interval.Exponents = new int[factorBaseSize + 1];
            var exponents = interval.Exponents;
            if (interval.Counts == null)
                interval.Counts = new ushort[size];
            var counts = interval.Counts;
            int i0 = 0;
            if (factorBase[0] == 2)
            {
                var p = factorBase[0];
                var logP = logFactorBase[0];
                for (int k = offsets[0]; k < size; k += p)
                {
                    Debug.Assert((BigInteger.Pow(x0 + k, 2) - n) % p == 0);
                    counts[k] += logP;
                }
                ++i0;
            }
            for (int i = i0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                var logP = logFactorBase[i];
                int k = offsets[i];
                int p1 = rootDiff[i];
                int p2 = p - p1;
                if (k >= p2 && k - p2 < size)
                {
                    Debug.Assert((BigInteger.Pow(x0 + k - p2, 2) - n) % p == 0);
                    counts[k - p2] += logP;
                }
                while (true)
                {
                    if (k >= size)
                        break;
                    Debug.Assert((BigInteger.Pow(x0 + k, 2) - n) % p == 0);
                    counts[k] += logP;
                    k += p1;
                    if (k >= size)
                        break;
                    Debug.Assert((BigInteger.Pow(x0 + k, 2) - n) % p == 0);
                    counts[k] += logP;
                    k += p2;
                }
                if (token.IsCancellationRequested)
                    return;
            }
            ushort limit = CalculateLowerBound(y0);
            for (int k = 0; k < size; k++)
            {
                if (counts[k] >= limit)
                {
                    var x = x0 + k;
                    if (ValueIsSmooth(k, interval))
                    {
                        var candidate = new Candidate
                        {
                            X = x,
                            Exponents = (int[])exponents.Clone(),
                        };
                        if (!candidateCallback(candidate))
                            return;
                    }
                }
                counts[k] = 0;
            }
        }

        private bool ValueIsSmooth(int k, Interval interval)
        {
            var exponents = interval.Exponents;
            for (int i = 0; i <= factorBaseSize; i++)
                exponents[i] = 0;
            if (interval.XRep.IsZero)
                interval.XRep.Set(interval.X);
            var xRep = interval.Reg1.SetSum(interval.XRep, (uint)k);
            var yRep = interval.Reg2.SetSquare(xRep);
            var zRep = interval.Reg3;
            if (yRep < nRep)
            {
                yRep.SetDifference(nRep, yRep);
                exponents[0] = 1;
            }
            else
                yRep.Subtract(nRep);
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                while (zRep.SetRemainder(yRep, (uint)p).IsZero)
                {
                    ++exponents[i + 1];
                    yRep.Divide((uint)p, zRep);
                }
            }
            return yRep.IsOne;
        }

        private bool ValueIsSmoothBigInteger(int k, Interval interval)
        {
            var exponents = interval.Exponents;
            var x = interval.X + k;
            var y = x * x - n;
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
            return y.IsOne;
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
    }
}
