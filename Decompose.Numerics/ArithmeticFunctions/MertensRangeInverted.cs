using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class MertensRangeInverted
    {
        private const long maximumBatchSize = (long)1 << 26;
        private const long minimumLowSize = (long)1 << 22;
        private const long C1 = 1;
        private const long C2 = 2;

        private int threads;
        private long nmax;
        private MobiusRange mobius;
        private long ulo;
        private long u;
        private sbyte[] values;
        private long[] mlo;
        private long sum2;
        private long sqrt;

        public MertensRangeInverted(long nmax, int threads)
            : this(new MobiusRange((long)IntegerMath.FloorPower((BigInteger)nmax, 2, 3) + 1, threads), nmax)
        {
        }

        public MertensRangeInverted(MobiusRange mobius, long nmax)
        {
            this.mobius = mobius;
            this.nmax = nmax;
            threads = mobius.Threads;

            sum2 = 0;
            var sqrt = IntegerMath.FloorSquareRoot(nmax);
            u = Math.Max((long)IntegerMath.FloorPower((BigInteger)nmax, 2, 3) * C1 / C2, sqrt + 1);
            ulo = Math.Max(Math.Min(u, maximumBatchSize), minimumLowSize);
            mlo = new long[ulo];
            values = new sbyte[ulo];
            mobius.GetValues(1, ulo + 1, values, 1, mlo, 0);
        }

        public long Evaluate(long n)
        {
            if (n <= 0)
                return 0;
            if (n > nmax)
                throw new ArgumentException("n");
            sqrt = IntegerMath.FloorSquareRoot(n);
            var imax = Math.Max(1, n / u);
            var mx = new long[imax + 1];
            ProcessBatch(mx, n, imax, mlo, 1, ulo);
            if (ulo < u)
            {
                var mhi = new long[maximumBatchSize];
                var m0 = mlo[ulo - 1];
                for (var x = ulo + 1; x <= u; x += maximumBatchSize)
                {
                    var xstart = x;
                    var xend = Math.Min(xstart + maximumBatchSize - 1, u);
                    mobius.GetValues(xstart, xend + 1, null, xstart, mhi, m0);
                    ProcessBatch(mx, n, imax, mhi, xstart, xend);
                    m0 = mhi[xend - xstart];
                }
            }
            return ComputeMx(mx, imax);
        }

        private void ProcessBatch(long[] mx, long n, long imax, long[] m, long x1, long x2)
        {
            if (threads <= 1)
                UpdateMx(mx, n, m, x1, x2, 1, imax, 2);
            else
            {
                var tasks = new Task[threads];
                for (var thread = 0; thread < threads; thread++)
                {
                    var imin = 2 * thread + 1;
                    var increment = 2 * threads;
                    tasks[thread] = Task.Factory.StartNew(() => UpdateMx(mx, n, m, x1, x2, imin, imax, increment));
                }
                Task.WaitAll(tasks);
            }
            UpdateMxSmall(mx, n, imax, m, x1, x2);
        }

        private void UpdateMx(long[] mx, long n, long[] m, long x1, long x2, long imin, long imax, long increment)
        {
            for (var i = imin; i <= imax; i += increment)
            {
                if (values[i - 1] == 0)
                    continue;

                var x = n / i;
                var sqrt = IntegerMath.FloorSquareRoot(x);
                var s = (long)0;

                var jmin = UpToOdd(Math.Max(imax / i + 1, x / (x2 + 1) + 1));
                var jmax = DownToOdd(Math.Min(sqrt, x / x1));
                s += JSum1(x, jmin, ref jmax, m, x1);
                s += JSum2(x, jmin, jmax, m, x1);

                mx[i] += s;
            }
        }

        private void UpdateMxSmall(long[] mx, long n, long imax, long[] m, long x1, long x2)
        {
            var kmin = Math.Max(1, x1);
            var kmax = Math.Min(sqrt, x2);
            for (var k = kmin; k <= kmax; k++)
            {
                var ilast = IntegerMath.Min(imax, n / (k * k));
                var nk1 = n / k;
                var nk2 = n / (k + 1);
                while (ilast > 0 && nk2 / ilast < IntegerMath.FloorSquareRoot(n / ilast))
                    --ilast;
                var s = (long)0;
                for (var i = 1; i <= ilast; i += 2)
                    s += values[i - 1] * (T1Odd(nk1 / i) - T1Odd(nk2 / i));
                sum2 += m[k - x1] * s;
            }
        }

        private long JSum2(long x, long jmin, long jmax, long[] m, long x1)
        {
            var s = (long)0;
            for (var j = jmin; j <= jmax; j += 2)
                s += m[x / j - x1];
            return s;
        }

        private long KSum2(long x, long kmin, long kmax, long[] m, long x1)
        {
            var s = (long)0;
            var current = T1Odd(x);
            for (var k = kmin; k <= kmax; k++)
            {
                var next = T1Odd(x / (k + 1));
                s += (current - next) * m[k - x1];
                current = next;
            }
            return s;
        }

        private long JSum1(long n, long j1, ref long j, long[] m, long offset)
        {
            var s = (long)0;
            var beta = n / (j + 2);
            var eps = n % (j + 2);
            var delta = n / j - beta;
            var gamma = 2 * beta - j * delta;
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

                Debug.Assert(eps == n % j);
                Debug.Assert(beta == n / j);
                Debug.Assert(delta == beta - n / (j + 2));
                Debug.Assert(gamma == 2 * beta - (BigInteger)(j - 2) * delta);

                s += m[beta - offset];
                j -= 2;
            }
            return s;
        }

        private long KSum1(long n, long k1, ref long k, long[] m, long offset)
        {
            if (k == 0)
                return 0;
            var s = (long)0;
            var beta = n / (k + 1);
            var eps = n % (k + 1);
            var delta = n / k - beta;
            var gamma = beta - k * delta;
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

                Debug.Assert(eps == n % k);
                Debug.Assert(beta == n / k);
                Debug.Assert(delta == beta - n / (k + 1));
                Debug.Assert(gamma == beta - (BigInteger)(k - 1) * delta);

                // Equivalent to:
                // s += (T1Odd(beta) - T1Odd(beta - delta)) * m[k];
                s += ((delta + (beta & 1)) >> 1) * m[k - offset];
                --k;
            }
            return s;
        }

        private long ComputeMx(long[] mx, long imax)
        {
            var s = (long)0;
            for (var i = 1; i <= imax; i += 2)
                s += values[i - 1] * mx[i];
            return -(s + sum2);
        }

        private long UpToOdd(long a)
        {
            return a | 1;
        }

        private long DownToOdd(long a)
        {
            return (a - 1) | 1;
        }

        private long T1Odd(long a)
        {
            return (a + (a & 1)) >> 1;
        }

        private static BigInteger[] data10 = new BigInteger[]
        {
            BigInteger.Parse("0"),
            BigInteger.Parse("-1"),
            BigInteger.Parse("1"),
            BigInteger.Parse("2"),
            BigInteger.Parse("-23"),
            BigInteger.Parse("-48"),
            BigInteger.Parse("212"),
            BigInteger.Parse("1037"),
            BigInteger.Parse("1928"),
            BigInteger.Parse("-222"),
            BigInteger.Parse("-33722"),
            BigInteger.Parse("-87856"),
            BigInteger.Parse("62366"),
            BigInteger.Parse("599582"),
            BigInteger.Parse("-875575"),
            BigInteger.Parse("-3216373"),
            BigInteger.Parse("-3195437"),
            BigInteger.Parse("-21830254"), // vs. -21830259 in http://oeis.org/A084237
            BigInteger.Parse("-46758740"),
        };

        public static BigInteger PowerOfTen(int i)
        {
            return data10[i];
        }
    }
}
