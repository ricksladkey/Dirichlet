using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class QuadraticSieve : IFactorizationAlgorithm<BigInteger>
    {
        private class Candidate
        {
            public BigInteger X { get; set; }
            public int[] Exponents { get; set; }
        }

        private class Interval
        {
            public BigInteger X { get; set; }
            public int Size { get; set; }
            public int[] Exponents { get; set; }
            public int[] Counts { get; set; }
            public int[] Offsets { get; set; }
            public Radix32Integer XRep { get; set; }
            public Radix32Integer Reg1 { get; set; }
            public Radix32Integer Reg2 { get; set; }
            public Radix32Integer Reg3 { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}, Size = {1}", X, Size);
            }
        }

        private const int intervalSize = 100000;
        private const int lowerBoundPercentDefault = 85;
        private const int surplusCandidates = 10;
        private readonly BigInteger smallFactorCutoff = (BigInteger)int.MaxValue;

        private int threadsOverride;
        private int factorBaseSizeOverride;
        private int lowerBoundPercentOverride;
        private IFactorizationAlgorithm<int> smallFactorer;
        private IEnumerable<int> primes;

        private Tuple<int, int>[] sizePairs =
        {
            Tuple.Create(1, 2),
            Tuple.Create(6, 5),
            Tuple.Create(10, 30),
            Tuple.Create(20, 60),
            Tuple.Create(30, 500),
            Tuple.Create(40, 1200),
            Tuple.Create(50, 5000),
            Tuple.Create(90, 60000), // http://www.mersenneforum.org/showthread.php?t=4013
        };

        public QuadraticSieve(int threads, int factorBaseSize, int lowerBoundPercent)
        {
            threadsOverride = threads;
            factorBaseSizeOverride = factorBaseSize;
            lowerBoundPercentOverride = lowerBoundPercent;
            smallFactorer = new TrialDivision();
            primes = new SieveOfErostothones();
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            if (n <= smallFactorCutoff)
                return smallFactorer.Factor((int)n).Select(factor => (BigInteger)factor);
            var factors = new List<BigInteger>();
            FactorCore(n, factors);
            return factors;
        }

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

        private BigInteger n;
        private BigInteger sqrtN;
        private int factorBaseSize;
        private int[] factorBase;
        private int[] logFactorBase;
        private Tuple<int, int>[] roots;
        private Radix32Integer nRep;

        private int threads;
        private CancellationToken token;
        private BlockingCollection<Candidate> candidateBuffer;
        private BlockingCollection<Interval> newIntervalBuffer;
        private BlockingCollection<Interval> oldIntervalBuffer;
        private List<Candidate> candidates;
        private List<BitArray> matrix;
        private int matrixColumn;

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
                    return (int)Math.Ceiling(y0 + (digits - x0) * (y1 - y0) / (x1 - x0));
                }
            }
            return (digits - 5) * 5 + digits + 1;
        }

        private int CalculateLowerBound(BigInteger y)
        {
            int percent = lowerBoundPercentOverride != 0 ? lowerBoundPercentOverride : lowerBoundPercentDefault;
            return LogScale(y) * percent / 100;
        }

        private BigInteger GetDivisor(BigInteger n)
        {
            if (n.IsEven)
                return BigIntegers.Two;
            this.n = n;
            nRep = new Radix32Integer(n.GetBitLength() / Radix32Integer.WordLength * 2 + 1);
            nRep.Set(n);
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
                .Select(factor =>
                    {
                        var root = (int)IntegerMath.ModularSquareRoot(n, factor);
                        return Tuple.Create(root, factor - root);
                    })
                .ToArray();
            int desired = factorBase.Length + surplusCandidates;
#if false
            var matrix = Sieve(desired, SieveTrialDivision);
#else
            var matrix = Sieve(desired, SieveQuadraticResidue);
#endif
            foreach (var v in Solve(matrix))
            {
                var factor = ComputeFactor(candidates, v);
                if (!factor.IsZero)
                    return factor;
            }
            return BigInteger.Zero;
        }

        private BigInteger ComputeFactor(List<Candidate> candidates, BitArray v)
        {
            var vbool = v.Cast<bool>();
#if false
                Console.WriteLine("v = {0}", string.Join(", ", vbool
                    .Select((selected, index) => new { Index = index, Selected = selected })
                    .Where(pair => pair.Selected)
                    .Select(pair => pair.Index)
                    .ToArray()));
#endif
            var xSet = candidates
                .Zip(vbool, (candidate, selected) => new { X = candidate.X, Selected = selected })
                .Where(pair => pair.Selected)
                .Select(pair => pair.X)
                .ToArray();
            var xPrime = xSet.Aggregate((sofar, current) => sofar * current) % n;
            var yPrime = IntegerMath.Sqrt(xSet
                .Aggregate(BigInteger.One, (sofar, current) => sofar * (current * current - n))) % n;
            var factor = BigInteger.GreatestCommonDivisor(xPrime + yPrime, n);
            if (!factor.IsOne && factor != n)
                return factor;
            return BigInteger.Zero;
        }

        private static int LogScale(BigInteger n)
        {
            return (int)Math.Floor(1000 * BigInteger.Log(BigInteger.Abs(n)));
        }

        private List<BitArray> Sieve(int desired, Action<Interval, Action<Candidate>> sieveCore)
        {
            CalculateNumberOfThreads();
            candidateBuffer = new BlockingCollection<Candidate>();
            newIntervalBuffer = new BlockingCollection<Interval>();
            oldIntervalBuffer = new BlockingCollection<Interval>();
            candidates = new List<Candidate>();
            matrix = CreateMatrix(desired);
            SetupIntervals();

            if (threads == 1)
            {
                token = CancellationToken.None;
                while (candidates.Count < desired)
                {
                    var left = desired - candidates.Count;
                    var interval = newIntervalBuffer.Take();
                    sieveCore(interval, candidate => candidates.Add(candidate));
                    oldIntervalBuffer.Add(interval);
                }
            }
            else
            {
                var tokenSource = new CancellationTokenSource();
                for (int i = 0; i <= threads; i++)
                    oldIntervalBuffer.Add(new Interval());
                token = tokenSource.Token;
                Task.Factory.StartNew(ProduceIntervals);
                for (int i = 0; i < threads; i++)
                    Task.Factory.StartNew(() => SieveParallel(sieveCore));
                while (candidates.Count < desired)
                {
                    var candidate = candidateBuffer.Take();
                    candidates.Add(candidate);
                }
                tokenSource.Cancel();
            }
            ProcessCandidates();
            return matrix;
        }

        private void ProduceIntervals()
        {
            while (!token.IsCancellationRequested)
            {
                var interval = oldIntervalBuffer.Take();
                newIntervalBuffer.Add(GetInterval(interval));
            }
        }

        private List<BitArray> CreateMatrix(int columns)
        {
            var matrix = new List<BitArray>();
            for (int i = 0; i <= factorBaseSize; i++)
                matrix.Add(new BitArray(columns));
            matrixColumn = 0;
            return matrix;
        }

        private void ProcessCandidates()
        {
#if false
            for (int i = 0; i < candidates.Count; i++)
                ProcessCandidate(candidates[i]);
#else
            int rows = matrix.Count;
            int cols = matrix[0].Length;
            var bits = new bool[cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    bits[j] = candidates[j].Exponents[i] % 2 != 0;
                matrix[i] = new BitArray(bits);
            }
#endif
        }

        private void ProcessCandidate(Candidate candidate)
        {
            // Doing this in parallel with sieving interferes with the cache.
            int j = matrixColumn++;
            var exponents = candidate.Exponents;
            for (int i = 0; i < exponents.Length; i++)
                matrix[i][j] = exponents[i] % 2 != 0;
        }

        private void SieveParallel(Action<Interval, Action<Candidate>> sieveCore)
        {
            var callback = new Action<Candidate>(ParallelCallback);
            while (!token.IsCancellationRequested)
            {
                var interval = newIntervalBuffer.Take();
                sieveCore(interval, callback);
                oldIntervalBuffer.Add(interval);
            }
        }

        private void ParallelCallback(Candidate candidate)
        {
            candidateBuffer.Add(candidate);
        }

        private object intervalSync = new object();
        private BigInteger xPos;
        private BigInteger xNeg;
        private int[] offsetsPos;
        private int[] offsetsNeg;
        private int nextInterval = -1;
        private int size;

        private void SetupIntervals()
        {
            xPos = sqrtN;
            offsetsPos = new int[factorBaseSize];
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                var offset = ((int)((roots[i].Item1 - xPos) % p) + p) % p;
                offsetsPos[i] = offset;
            }
            xNeg = xPos - size;
            offsetsNeg = (int[])offsetsPos.Clone();
            ShiftOffsets(offsetsNeg, -size);
            size = (int)IntegerMath.Min((sqrtN + threads - 1) / threads, intervalSize);
        }

        private Interval GetInterval(Interval interval)
        {
            if (nextInterval == 1)
            {
                SetupInterval(interval, xPos, size, offsetsPos);
                ShiftOffsets(offsetsPos, size);
                xPos += size;
                nextInterval = -nextInterval;
                return interval;
            }
            else
            {
                SetupInterval(interval, xNeg, size, offsetsNeg);
                ShiftOffsets(offsetsNeg, -size);
                xNeg -= size;
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

        private void SieveTrialDivision(Interval interval, Action<Candidate> candidateCallback)
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
                    candidateCallback(candidate);
                    if (token.IsCancellationRequested)
                        break;
                }
            }
        }

        private void SieveQuadraticResidue(Interval interval, Action<Candidate> candidateCallback)
        {
            var x0 = interval.X;
            var y0 = x0 * x0 - n;
            int size = interval.Size;
            var offsets = interval.Offsets;
            if (interval.Exponents == null)
                interval.Exponents = new int[factorBaseSize + 1];
            var exponents = interval.Exponents;
            if (interval.Counts == null)
                interval.Counts = new int[size];
            var counts = interval.Counts;
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                var logP = logFactorBase[i];
                var k0 = offsets[i];
                for (int root = 0; root < 2; root++)
                {
                    if (root == 1 && p == 2)
                        continue;
                    for (int k = k0; k < size; k += p)
                    {
                        Debug.Assert((BigInteger.Pow(x0 + k, 2) - n) % p == 0);
                        counts[k] += logP;
                    }
                    k0 += roots[i].Item2 - roots[i].Item1;
                }
                if (token.IsCancellationRequested)
                    return;
            }
            if (token.IsCancellationRequested)
                return;
            int limit = CalculateLowerBound(y0);
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
                        candidateCallback(candidate);
                        if (token.IsCancellationRequested)
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

        private IEnumerable<BitArray> Solve(List<BitArray> matrix)
        {
#if false
            PrintMatrix("initial:", matrix);
#endif
            int rows = Math.Min(matrix.Count, matrix[0].Count);
            int cols = matrix[0].Count;
            var c = new List<int>();
            var cInv = new List<int>();
            for (int i = 0; i < cols; i++)
            {
                c.Add(-1);
                cInv.Add(-1);
            }
            for (int k = 0; k < cols; k++)
            {
                int j = -1;
                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i][k] && c[i] < 0)
                    {
                        j = i;
                        break;
                    }
                }
                if (j != -1)
                {
                    ZeroColumn(matrix, rows, j, k);
                    c[j] = k;
                    cInv[k] = j;
                }
                else
                {
                    var v = new BitArray(c.Count);
                    int ones = 0;
                    for (int jj = 0; jj < c.Count; jj++)
                    {
                        int s = cInv[jj];
                        bool value;
                        if (s != -1)
                            value = matrix[s][k];
                        else if (jj == k)
                            value = true;
                        else
                            value = false;
                        v[jj] = value;
                        if (value)
                            ++ones;
                    }
#if DEBUG
                    Debug.Assert(VerifySolution(matrix, 0, matrix.Count, v));

#endif
                    if (VerifySolution(matrix, rows, matrix.Count, v))
                        yield return v;
                }
#if false
                PrintMatrix(string.Format("k = {0}", k), matrix);
#endif
            }
        }

        private void ZeroColumn(List<BitArray> matrix, int rows, int j, int k)
        {
            if (rows < 256)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (i == j || !matrix[i][k])
                        continue;
                    matrix[i].Xor(matrix[j]);
                }
            }
            else
            {
                int range = (rows + threads - 1) / threads;
                Parallel.For(0, threads, thread =>
                {
                    int beg = thread * range;
                    int end = Math.Min(beg + range, rows);
                    for (int i = beg; i < end; i++)
                    {
                        if (i != j && matrix[i][k])
                            matrix[i].Xor(matrix[j]);
                    }
                });
            }
        }

        private bool VerifySolution(List<BitArray> matrix, int rowMin, int rowMax, BitArray solution)
        {
            int cols = matrix[0].Count;
            for (int i = rowMin; i < rowMax; i++)
            {
                bool row = false;
                for (int j = 0; j < cols; j++)
                {
                    row ^= solution[j] & matrix[i][j];
                }
                if (row)
                    return false;
            }
            return true;
        }

        private void PrintMatrix(string label, List<List<int>> matrix)
        {
            Console.WriteLine(label);
            for (int i = 0; i < matrix.Count; i++)
                Console.WriteLine(string.Join(" ", matrix[i].ToArray()));
        }
    }
}

