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
            public BigInteger Max { get { return X + Size; } }
            public override string ToString()
            {
                return string.Format("[{0}, {1})", X, Max);
            }
        }

        private const int windowSize = 100000;
        private const int lowerBoundPercentDefault = 85;
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
            Tuple.Create(100, 80000), // http://www.mersenneforum.org/showthread.php?t=4013
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

        private int CalculateNumberOfThreads()
        {
            int threads = threadsOverride != 0 ? threadsOverride : 1;
            if (n <= BigInteger.Pow(10, 10))
                return 1;
            return threads;
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
                    // Interopolate.
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
            int desired = factorBase.Length + 10;
#if false
            var candidates = Sieve(desired, SieveTrialDivision);
#else
            var candidates = Sieve(desired, SieveQuadraticResidue);
#endif
            var matrix = new List<BitArray>();
            for (int i = 0; i <= factorBaseSize; i++)
                matrix.Add(new BitArray(candidates.Count));
            for (int j = 0; j < candidates.Count; j++)
            {
                var exponents = candidates[j].Exponents;
                for (int i = 0; i < exponents.Length; i++)
                    matrix[i][j] = exponents[i] % 2 != 0;
            }
            foreach (var v in Solve(matrix))
            {
                var vbool = v.Cast<bool>();
#if false
                Console.WriteLine("v = {0}", string.Join("", vbool.Select(bit => bit ? 1 : 0).ToArray()));
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
            }
            return BigInteger.Zero;
        }

        private static int LogScale(BigInteger n)
        {
            return (int)Math.Floor(1000 * BigInteger.Log(BigInteger.Abs(n)));
        }

        private CancellationToken token;
        private BlockingCollection<Candidate> candidateBuffer;
        private BlockingCollection<Interval> intervalBuffer;

        private List<Candidate> Sieve(int desired, Action<Interval, Action<Candidate>> sieveCore)
        {
            candidateBuffer = new BlockingCollection<Candidate>();
            intervalBuffer = new BlockingCollection<Interval>();
            var candidates = new List<Candidate>();
            var threads = CalculateNumberOfThreads();
            if (threads == 1)
            {
                token = CancellationToken.None;
                foreach (var interval in GetIntervals())
                {
                    var left = desired - candidates.Count;
                    sieveCore(interval, candidate => candidates.Add(candidate));
                    if (candidates.Count >= desired)
                    {
                        candidates.RemoveRange(desired, desired - candidates.Count);
                        break;
                    }
                }
            }
            else
            {
                var tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                var tasks = new Task[threads + 1];
                var ranges = GetIntervals().GetEnumerator();
                for (int i = 0; i < threads; i++)
                {
                    ranges.MoveNext();
                    tasks[i] = StartNew(ranges.Current, sieveCore);
                }
                tasks[threads] = Task.Factory.StartNew(() => ReadCandidates(candidates, desired));
                while (true)
                {
                    var index = Task.WaitAny(tasks);
                    if (index == threads)
                    {
                        tokenSource.Cancel();
                        break;
                    }
                    ranges.MoveNext();
                    tasks[index] = StartNew(ranges.Current, sieveCore);
                }
            }
            return candidates;
        }

        private Task StartNew(Interval interval, Action<Interval, Action<Candidate>> sieveCore)
        {
            return Task.Factory.StartNew(() => sieveCore(interval, ParallelCallback));
        }

        private void ReadCandidates(List<Candidate> list, int desired)
        {
            while (list.Count < desired)
            {
                var candidate = null as Candidate;
                candidateBuffer.TryTake(out candidate, Timeout.Infinite);
                list.Add(candidate);
            }
        }

        private void ParallelCallback(Candidate candidate)
        {
            candidateBuffer.Add(candidate);
        }

        private IEnumerable<Interval> GetIntervals()
        {
            var xPos = sqrtN;
            int threads = CalculateNumberOfThreads();
            var size = Math.Max((int)IntegerMath.Min(sqrtN / threads, windowSize), 1);
            var offsetsPos = new int[factorBaseSize];
            for (int i = 0; i < factorBaseSize; i++)
            {
                var p = factorBase[i];
                var offset = ((int)((roots[i].Item1 - xPos) % p) + p) % p;
                offsetsPos[i] = offset;
            }
            var xNeg = xPos - size;
            var offsetsNeg = (int[])offsetsPos.Clone();
            ShiftOffsets(offsetsNeg, -size);
            while (true)
            {
                yield return RecycleInterval(xNeg, size, offsetsNeg);
                ShiftOffsets(offsetsNeg, -size);
                xNeg -= size;
                yield return RecycleInterval(xPos, size, offsetsPos);
                ShiftOffsets(offsetsPos, size);
                xPos += size;
            }
        }

        private Interval RecycleInterval(BigInteger x, int size, int[] offsets)
        {
            var interval = null as Interval;
            if (!intervalBuffer.TryTake(out interval))
                interval = new Interval();
            interval.X = x;
            interval.Size = size;
            if (interval.Offsets == null)
                interval.Offsets = new int[factorBaseSize];
            offsets.CopyTo(interval.Offsets, 0);
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
            var exponents = new int[factorBaseSize + 1];
            for (var k = interval.X; k < interval.Max; k++)
            {
                var x = sqrtN + k;
                if (ValueIsSmooth(x, exponents))
                {
                    var candidate = new Candidate
                    {
                        X = x,
                        Exponents = (int[])exponents.Clone(),
                    };
                    candidateCallback(candidate);
                    if (token.IsCancellationRequested)
                        break;
                }
            }
            intervalBuffer.Add(interval);
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
                var offset = offsets[i];
                var j0 = offset;
                for (int root = 0; root < 2; root++)
                {
                    if (root == 1 && p == 2)
                        continue;
                    for (int j = j0; j < size; j += p)
                    {
                        Debug.Assert((BigInteger.Pow(x0 + j, 2) - n) % p == 0);
                        counts[j] += logP;
                    }
                    j0 += roots[i].Item2 - roots[i].Item1;
                }
            }
            int limit = CalculateLowerBound(y0);
            for (int j = 0; j < size; j++)
            {
                if (counts[j] >= limit)
                {
                    var x = x0 + j;
                    if (ValueIsSmooth(x, exponents))
                    {
                        var candidate = new Candidate
                        {
                            X = x,
                            Exponents = (int[])exponents.Clone(),
                        };
                        candidateCallback(candidate);
                        if (token.IsCancellationRequested)
                            break;
                    }
                }
                counts[j] = 0;
            }
            intervalBuffer.Add(interval);
        }

        private bool ValueIsSmooth(BigInteger x, int[] exponents)
        {
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
            for (int i = 0; i < cols; i++)
                c.Add(-1);
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
                    for (int i = 0; i < rows; i++)
                    {
                        if (i == j || !matrix[i][k])
                            continue;
                        matrix[i].Xor(matrix[j]);
                    }
                    c[j] = k;
                }
                else
                {
                    var v = new BitArray(c.Count);
                    for (int jj = 0; jj < c.Count; jj++)
                    {
                        int js = -1;
                        for (int s = 0; s < c.Count; s++)
                        {
                            if (c[s] == jj)
                            {
                                js = s;
                                break;
                            }
                        }
                        if (js != -1)
                            v[jj] = matrix[js][k];
                        else if (jj == k)
                            v[jj] = true;
                        else
                            v[jj] = false;
                    }
                    if (VerifySolution(matrix, v))
                        yield return v;
                }
#if false
                PrintMatrix(string.Format("k = {0}", k), matrix);
#endif
            }
        }

        private bool VerifySolution(List<BitArray> matrix, BitArray solution)
        {
            int rows = matrix.Count;
            int cols = matrix[0].Count;
            for (int i = 0; i < rows; i++)
            {
                bool row = false;
                for (int j = 0; j < cols; j++)
                {
                    row ^= solution[j] & matrix[i][j];
                }
                if (row)
                {
                    return false;
                }
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

