using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class MertensRange
    {
        private long nmax;
        private MobiusRange mobius;
        private long u;
        private long[] m;

        public MertensRange(MobiusRange mobius, long nmax)
        {
            this.mobius = mobius;
            this.nmax = nmax;

            u = (long)IntegerMath.FloorPower((BigInteger)nmax, 2, 3);
            m = new long[u + 1];
            var values = new sbyte[u + 1];
            mobius.GetValues(0, u + 1, values);
            var t = (long)0;
            for (var i = 0; i <= u; i++)
            {
                t += values[i];
                m[i] = t;
            }
        }

        public long Evaluate(long n)
        {
            if (n <= 0)
                return 0;
            if (n > nmax)
                throw new ArgumentException("n");
            var imax = Math.Max(1, n / u);
            var mx = new long[imax + 1];
            var threads = mobius.Threads;
            if (threads <= 1)
            {
                for (var i = 1; i <= imax; i += 2)
                    UpdateMx(mx, n, imax, i);
            }
            else
            {
                var tasks = new Task[threads];
                for (var thread = 0; thread < threads; thread++)
                {
                    var offset = 2 * thread + 1;
                    var increment = 2 * threads;
                    tasks[thread] = Task.Factory.StartNew(() =>
                        {
                            for (var i = offset; i <= imax; i += increment)
                                UpdateMx(mx, n, imax, i);
                        });
                }
                Task.WaitAll(tasks);
            }
            ComputeMx(mx, imax);
            return mx[1];
        }

        private void UpdateMx(long[] mx, long n, long imax, long i)
        {
            var ni = n / i;
            var sqrt = IntegerMath.FloorSquareRoot(ni);
            var s = (long)0;

            var jmin = UpToOdd(imax / i + 1);
            var jmax = DownToOdd(sqrt);
            s += JSum(ni, jmin, ref jmax);
            for (var j = jmin; j <= jmax; j += 2)
                s += m[ni / j];

            var kmax = ni / sqrt - 1;
            s += KSum(ni, 1, ref kmax);
            var current = T1Odd(ni);
            for (var k = 1; k <= kmax; k++)
            {
                var next = T1Odd(ni / (k + 1));
                s += (current - next) * m[k];
                current = next;
            }

            mx[i] = -s;
        }

        private long JSum(long n, long j1, ref long j)
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

                s += m[beta];
                j -= 2;
            }
            return s;
        }

        private long KSum(long n, long k1, ref long k)
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
                s += ((delta + (beta & 1)) >> 1) * m[k];
                --k;
            }
            return s;
        }

        private void ComputeMx(long[] mx, long imax)
        {
            for (var i = DownToOdd(imax); i >= 1; i -= 2)
            {
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
            return a - (~a & 1);
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
        };

        public static BigInteger PowerOfTen(int i)
        {
            return data10[i];
        }
    }
}
