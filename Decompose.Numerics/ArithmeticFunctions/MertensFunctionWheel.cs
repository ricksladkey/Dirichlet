using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class MertensFunctionWheel
    {
        private const long maximumBatchSize = (long)1 << 26;
        private const long C1 = 1;
        private const long C2 = 2;
        private const long C3 = 2;
        private const long C4 = 1;

        private int threads;
        private MobiusRange mobius;
        private long n;
        private long u;
        private long imax;
        private long[] m;
        private long[] mx;
        private int[] r;
        private int lmax;

#if false
        private const int wheelSize = 2;
        private const int wheelCount = 1;
#endif
#if false
        private const int wheelSize = 6;
        private const int wheelCount = 2;
#endif
#if false
        private const int wheelSize = 30;
        private const int wheelCount = 8;
#endif
#if true
        private const int wheelSize = 210;
        private const int wheelCount = 48;
#endif

        private int[] wheelSubtotal;
        private bool[] wheelInclude;


        public MertensFunctionWheel(int threads)
        {
            this.threads = threads;
            var subtotal = new List<int>();
            var total = 0;
            for (var i = 0; i < wheelSize; i++)
            {
                if (IntegerMath.GreatestCommonDivisor(i, wheelSize) == 1)
                    ++total;
                subtotal.Add(total);
            }
            wheelSubtotal = subtotal.ToArray();
            var include = new List<bool>();
            for (var i = 0; i < wheelSize; i++)
                include.Add(IntegerMath.GreatestCommonDivisor(i, wheelSize) == 1);
            wheelInclude = include.ToArray();
        }

        public long Evaluate(long n)
        {
            if (n <= 0)
                return 0;

            this.n = n;
            u = Math.Max((long)IntegerMath.FloorPower((BigInteger)n, 2, 3) * C1 / C2, IntegerMath.CeilingSquareRoot(n));

            if (u <= wheelSize)
            {
                mobius = new MobiusRange(n + 1, threads);
                m = new long[n];
                mobius.GetValues(1, n + 1, null, 1, m, 0);
                return m[n - 1];
            }

            imax = n / u;
            mobius = new MobiusRange(u + 1, threads);
            var batchSize = Math.Min(u, maximumBatchSize);
            m = new long[batchSize];
            mx = new long[imax + 1];
            r = new int[imax + 1];

            lmax = 0;
            for (var i = 1; i <= imax; i += 2)
            {
                if (wheelInclude[i % wheelSize])
                    r[lmax++] = i;
            }

            var m0 = (long)0;
            for (var x = (long)1; x <= u; x += maximumBatchSize)
            {
                var xstart = x;
                var xend = Math.Min(xstart + maximumBatchSize - 1, u);
                mobius.GetValues(xstart, xend + 1, null, xstart, m, m0);
                m0 = m[xend - xstart];
                ProcessBatch(xstart, xend);
            }
            ComputeMx();
            return mx[1];
        }

        private void ProcessBatch(long x1, long x2)
        {
            if (threads <= 1)
                UpdateMx(x1, x2, 0, 1);
            else
            {
                var tasks = new Task[threads];
                for (var thread = 0; thread < threads; thread++)
                {
                    var min = thread;
                    var increment = threads;
                    tasks[thread] = Task.Factory.StartNew(() => UpdateMx(x1, x2, min, increment));
                }
                Task.WaitAll(tasks);
            }
        }

        private void UpdateMx(long x1, long x2, long min, long increment)
        {
            for (var l = min; l < lmax; l += increment)
            {
                var i = r[l];
                var x = n / i;
                var sqrt = IntegerMath.FloorSquareRoot(x);
                var xover = Math.Min(sqrt * C3 / C4, x);
                xover = x / (x / xover);
                var s = (long)0;

                var jmin = UpToOdd(Math.Max(imax / i + 1, x / (x2 + 1) + 1));
                var jmax = DownToOdd(Math.Min(xover, x / x1));
                s += JSum1(x, jmin, ref jmax, x1);
                s += JSum2(x, jmin, jmax, x1);

                var kmin = Math.Max(1, x1);
                var kmax = Math.Min(x / xover - 1, x2);
                s += KSum1(x, kmin, ref kmax, x1);
                s += KSum2(x, kmin, kmax, x1);

                mx[i] -= s;
            }
        }

        private long JSum2(long x, long jmin, long jmax, long x1)
        {
            var s = (long)0;
            for (var j = jmin; j <= jmax; j += 2)
            {
                if (wheelInclude[j % wheelSize])
                    s += m[x / j - x1];
            }
            return s;
        }

        private long KSum2(long x, long kmin, long kmax, long x1)
        {
            var s = (long)0;
            var current = T1Wheel(x);
            for (var k = kmin; k <= kmax; k++)
            {
                var next = T1Wheel(x / (k + 1));
                s += (current - next) * m[k - x1];
                current = next;
            }
            return s;
        }

        private long JSum1(long x, long j1, ref long j, long offset)
        {
            var s = (long)0;
            var beta = x / (j + 2);
            var eps = x % (j + 2);
            var delta = x / j - beta;
            var gamma = 2 * beta - j * delta;
            var mod = j % wheelSize;
            while (j >= j1)
            {
                eps += gamma;
                if (eps >= j)
                {
                    ++delta;
                    gamma -= j;
                    eps -= j;
                    if (eps >= j)
                    {
                        ++delta;
                        gamma -= j;
                        eps -= j;
                        if (eps >= j)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += j;
                    eps += j;
                }
                gamma += 4 * delta;
                beta += delta;

                Debug.Assert(eps == x % j);
                Debug.Assert(beta == x / j);
                Debug.Assert(delta == beta - x / (j + 2));
                Debug.Assert(gamma == 2 * beta - (BigInteger)(j - 2) * delta);

                if (wheelInclude[mod])
                    s += m[beta - offset];
                mod -= 2;
                if (mod < 0)
                    mod += wheelSize;
                j -= 2;
            }
            return s;
        }

        private long KSum1(long x, long k1, ref long k, long offset)
        {
            if (k == 0)
                return 0;
            var s = (long)0;
            var beta = x / (k + 1);
            var eps = x % (k + 1);
            var delta = x / k - beta;
            var gamma = beta - k * delta;
            var lastCount = T1Wheel(beta);
            while (k >= k1)
            {
                eps += gamma;
                if (eps >= k)
                {
                    ++delta;
                    gamma -= k;
                    eps -= k;
                    if (eps >= k)
                    {
                        ++delta;
                        gamma -= k;
                        eps -= k;
                        if (eps >= k)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += k;
                    eps += k;
                }
                gamma += 2 * delta;
                beta += delta;

                Debug.Assert(eps == x % k);
                Debug.Assert(beta == x / k);
                Debug.Assert(delta == beta - x / (k + 1));
                Debug.Assert(gamma == beta - (BigInteger)(k - 1) * delta);

                var count = T1Wheel(beta);
                s += (count - lastCount) * m[k - offset];
                lastCount = count;
                --k;
            }
            return s;
        }

        private void ComputeMx()
        {
            for (var l = lmax - 1; l >= 0; l--)
            {
                var i = r[l];
                var s = (long)0;
                var ijmax = imax / i * i;
                for (var ij = 2 * i; ij <= ijmax; ij += i)
                    s += mx[ij];
                mx[i] -= s;
            }
        }

        private long UpToOdd(long a)
        {
            return a | 1;
        }

        private long DownToOdd(long a)
        {
            return (a - 1) | 1;
        }
        private long T1Wheel(long a)
        {
            var b = a / wheelSize;
            var c = (int)(a - b * wheelSize);
            return wheelCount * b + wheelSubtotal[c];
        }
    }
}
