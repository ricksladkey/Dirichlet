using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class QuadraticSieve : IFactorizationAlgorithm<BigInteger>
    {
        private class Candidate
        {
            public BigInteger X { get; set; }
            public int[] Exponents { get; set; }
        }

        private struct Range
        {
            public BigInteger Min { get; set; }
            public BigInteger Max { get; set; }
        }

        protected int threads;

        public QuadraticSieve(int threads)
        {
            this.threads = threads;
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            var factors = new List<BigInteger>();
            FactorCore(n, factors);
            return factors;
        }

        private void FactorCore(BigInteger n, List<BigInteger> factors)
        {
            if (n == 1)
                return;
            if (BigIntegerUtils.IsPrime(n))
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
        private BigInteger sqrtn;
        private int[] factorBase;

        private BigInteger GetDivisor(BigInteger n)
        {
            if (n.IsEven)
                return BigIntegerUtils.Two;
            this.n = n;
            sqrtn = BigIntegerUtils.Sqrt(n);
            var digits = BigInteger.Log(n) / Math.Log(10);
            int factorBaseSize = (int)Math.Ceiling((digits - 5) * 5 + digits) + 1;
            factorBase = new SieveOfErostothones()
                .Where(p => BigIntegerUtils.JacobiSymbol(n, p) == 1)
                .Take(factorBaseSize)
                .ToArray();
            int desired = factorBase.Length + 1 + (int)Math.Ceiling(digits);
            var candidates = Sieve(desired);
            var matrix = new List<List<int>>();
            for (int i = 0; i <= factorBaseSize; i++)
                matrix.Add(new List<int>());
            foreach (var candidate in candidates)
            {
                var exponents = candidate.Exponents;
                for (int i = 0; i < exponents.Length; i++)
                    matrix[i].Add(exponents[i] % 2);
            }
            foreach (var v in Solve(matrix))
            {
#if false
                Console.WriteLine("v = {0}", string.Join(" ", v.ToArray()));
#endif
                var xSet = candidates
                    .Zip(v, (candidate, selected) => new { X = candidate.X, Selected = selected })
                    .Where(pair => pair.Selected == 1)
                    .Select(pair => pair.X)
                    .ToArray();
                var xPrime = xSet.Aggregate((sofar, current) => sofar * current) % n;
                var yPrime = BigIntegerUtils.Sqrt(xSet
                    .Aggregate(BigInteger.One, (sofar, current) => sofar * (current * current - n))) % n;
                var factor = BigInteger.GreatestCommonDivisor(xPrime + yPrime, n);
                if (!factor.IsOne && factor != n)
                    return factor;
            }
            return BigInteger.Zero;
        }

        private List<Candidate> Sieve(int desired)
        {
            var candidates = new List<Candidate>();
            if (threads == 1)
            {
                foreach (var range in Ranges)
                {
                    var left = desired - candidates.Count;
                    candidates.AddRange(SieveTrialDivision(range.Min, range.Max).Take(left));
                    if (candidates.Count == desired)
                        break;
                }
            }
            else
            {
                var queue = new ConcurrentQueue<Candidate>();
                var cancellationTokenSource = new CancellationTokenSource();
                var tasks = new Task[threads];
                var ranges = Ranges.GetEnumerator();
                for (int i = 0; i < threads; i++)
                {
                    ranges.MoveNext();
                    tasks[i] = Task.Factory.StartNew(() => SieveParallel(ranges.Current, queue, cancellationTokenSource.Token));
                }
                while (true)
                {
                    var index = Task.WaitAny(tasks);
                    var candidate = null as Candidate;
                    while (candidates.Count < desired && queue.TryDequeue(out candidate))
                        candidates.Add(candidate);
                    if (candidates.Count == desired)
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                    ranges.MoveNext();
                    tasks[index] = Task.Factory.StartNew(() => SieveParallel(ranges.Current, queue, cancellationTokenSource.Token));
                }
            }
            return candidates;
        }

        private void SieveParallel(Range range, ConcurrentQueue<Candidate> candidates, CancellationToken cancellationToken)
        {
            foreach (var candidate in SieveTrialDivision(range.Min, range.Max))
            {
                candidates.Enqueue(candidate);
                if (cancellationToken.IsCancellationRequested)
                    return;
            }
        }

        private IEnumerable<Range> Ranges
        {
            get
            {
                var k = BigInteger.Zero;
                var window = BigIntegerUtils.Min(sqrtn, 1000);
                while (true)
                {
                    yield return new Range { Min = k, Max = k + window };
                    yield return new Range { Min = -k - window, Max = -k };
                    k += window;
                }
            }
        }

        private IEnumerable<Candidate> SieveTrialDivision(BigInteger kmin, BigInteger kmax)
        {
            int factorBaseSize = factorBase.Length;
            var exponents = new int[factorBaseSize + 1];
            for (var k = kmin; k < kmax; k++)
            {
                for (int i = 0; i <= factorBaseSize; i++)
                    exponents[i] = 0;
                var x = sqrtn + k;
                var y = x * x - n;
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
                if (y.IsOne)
                {
                    yield return new Candidate
                    {
                        X = x,
                        Exponents = (int[])exponents.Clone(),
                    };
                }
            }
        }

        private IEnumerable<List<int>> Solve(List<List<int>> matrix)
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
                    if (matrix[i][k] != 0 && c[i] < 0)
                    {
                        j = i;
                        break;
                    }
                }
                if (j != -1)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        if (i == j || matrix[i][k] == 0)
                            continue;
                        for (int l = 0; l < cols; l++)
                            matrix[i][l] ^= matrix[j][l];
                    }
                    c[j] = k;
                }
                else
                {
                    var v = new List<int>();
                    for (int i = 0; i < c.Count; i++)
                        v.Add(0);
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
                            v[jj] = 1;
                        else
                            v[jj] = 0;
                    }
                    if (VerifySolution(matrix, v))
                        yield return v;
                }
#if false
                PrintMatrix(string.Format("k = {0}", k), matrix);
#endif
            }
        }

        private bool VerifySolution(List<List<int>> matrix, List<int> solution)
        {
            int rows = matrix.Count;
            int cols = matrix[0].Count;
            for (int ii = 0; ii < rows; ii++)
            {
                int row = 0;
                for (int jj = 0; jj < cols; jj++)
                {
                    row ^= solution[jj] * matrix[ii][jj];
                }
                if (row != 0)
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
