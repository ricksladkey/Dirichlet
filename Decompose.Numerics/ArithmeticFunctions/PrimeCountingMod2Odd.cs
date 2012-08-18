using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class PrimeCountingMod2Odd
    {
        private static int maximumBatchSize = 1 << 24;
        private const long C1 = 1;
        private const long C2 = 1;

        private int threads;
        private BigInteger n;
        private long sqrtn;
        private long kmax;
        private long imax;
        private long xmax;
        private MobiusOddRange mobius;
        private long[] xi;
        private long[] mx;
        private long m0;
        private sbyte[] values;
        private long[] m;

        private IDivisorSummatoryFunction<BigInteger>[] hyperbolicSum;

        public PrimeCountingMod2Odd(int threads)
        {
            this.threads = threads;
            var count = Math.Max(threads, 1);
            hyperbolicSum = new DivisorSummatoryFunctionOddUInt128[count];
            for (var i = 0; i < count; i++)
                hyperbolicSum[i] = new DivisorSummatoryFunctionOddUInt128(0);
        }

        public int Evaluate(BigInteger n)
        {
            this.n = n;
            var sum = 0;
            sqrtn = (long)IntegerMath.FloorSquareRoot(n);
            kmax = (int)IntegerMath.FloorLog(n, 2);
            imax = (long)IntegerMath.FloorPower(n, 1, 5) * C1 / C2;
            xmax = DownToOdd(imax != 0 ? Xi(imax) : sqrtn);
            mobius = new MobiusOddRange(xmax + 1, threads);
            xi = new long[imax + 1];
            mx = new long[imax + 1];

            // Initialize xi.
            for (var i = 1; i <= imax; i++)
                xi[i] = Xi(i);

            // Process ranges of Mobius values.
            values = new sbyte[maximumBatchSize >> 1];
            m = new long[maximumBatchSize >> 1];
            m0 = (long)0;
            for (var x = (long)1; x <= xmax; x += maximumBatchSize)
            {
                var xfirst = x;
                var xlast = Math.Min(xmax, xfirst + maximumBatchSize - 1);
                mobius.GetValues(xfirst, xlast + 2, values, xfirst, m, m0);
                sum += Pi2(xfirst, xlast);
                UpdateMx(xfirst, xlast, 1, 1);
                m0 = m[(xlast - xfirst) >> 1];
            }

            // Evaluate the tail.
            sum += EvaluateTail();

            // Adjust for final parity of F2.
            for (var k = 1; k <= kmax; k++)
                sum -= IntegerMath.Mobius(k);

            // Compute final result.
            sum &= 3;
            sum >>= 1;
            return (sum + (n >= 2 ? 1 : 0)) % 2;
        }

        private int EvaluateTail()
        {
            // Finialize mx.
            ComputeMx();

            // Compute tail.
            var s = (BigInteger)0;
            for (var i = 1; i < imax; i++)
                s += T2(i) * (mx[i] - mx[i + 1]);
            return (int)(s & 3);
        }

        private int Pi2(long x1, long x2)
        {
            var s = 0;
            for (var k = 1; k <= kmax; k++)
            {
                var mu = IntegerMath.Mobius(k);
                if (mu != 0)
                    s += mu * F2(IntegerMath.FloorRoot(n, k), x1, x2);
            }
            return s;
        }

        private int F2(BigInteger n, long x1, long x2)
        {
            var xmin = UpToOdd(Math.Max(1, x1));
            var xmax = DownToOdd(Math.Min((long)IntegerMath.FloorSquareRoot(n), x2));
            var s = 0;
            var x = xmax;
            var beta = (long)(n / (x + 2));
            var eps = (long)(n % (x + 2));
            var delta = (long)(n / x - (ulong)beta);
            var gamma = 2 * beta - x * delta;
            var alpha = beta / (x + 2);
            var alphax = (alpha + 1) * (x + 2);
            var lastalpha = (long)-1;
            var count = 0;
            while (x >= xmin)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += 4 * delta;
                beta += delta;
                alphax -= 2 * alpha + 2;
                if (alphax <= beta)
                {
                    ++alpha;
                    alphax += x;
                    if (alphax <= beta)
                    {
                        ++alpha;
                        alphax += x;
                        if (alphax <= beta)
                            break;
                    }
                }

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 2));
                Debug.Assert(gamma == 2 * beta - (BigInteger)(x - 2) * delta);
                Debug.Assert(alpha == n / ((BigInteger)x * x));

                var mu = values[(x - x1) >> 1];
                if (mu != 0)
                {
                    if (alpha != lastalpha)
                    {
                        count &= 3;
                        if (count != 0)
                        {
                            s += count * T2(lastalpha);
                            count = 0;
                        }
                        lastalpha = alpha;
                    }
                    count += mu;
                }
                x -= 2;
            }
            count &= 3;
            if (count != 0)
                s += count * T2(lastalpha);
            var xx = (ulong)x * (ulong)x;
            var dx = 4 * (ulong)x - 4;
            while (x >= xmin)
            {
                Debug.Assert(xx == (ulong)x * (ulong)x);
                var mu = values[(x - x1) >> 1];
                if (mu > 0)
                    s += T2(n / xx);
                else if (mu < 0)
                    s -= T2(n / xx);
                xx -= dx;
                dx -= 8;
                x -= 2;
            }
            return s & 3;
        }

        private int T2(BigInteger n)
        {
            var sqrt = (long)IntegerMath.FloorSquareRoot(n);
            var result = 2 * S1(n, 1, sqrt) + 3 * (int)(T1Odd(sqrt) & 1);
            Debug.Assert(result % 4 == new DivisionFreeDivisorSummatoryFunction(0, false, true).Evaluate(n) % 4);
            return result;
        }

        private const long nMaxSimple = (long)1 << 50;

        private int S1(BigInteger n, long x1, long x2)
        {
            if (n <= nMaxSimple)
                return S1((long)n, (int)x1, (int)x2);

#if false
            var s = (long)0;
            var x = DownToOdd(x2);
            var beta = (long)(n / (x + 2));
            var eps = (long)(n % (x + 2));
            var delta = (long)(n / x - beta);
            var gamma = 2 * beta - x * delta;
            while (x >= x1)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += 4 * delta;
                beta += delta;
                beta &= 3;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == ((n / x) & 3));
                Debug.Assert(delta == (n / x) - n / (x + 2));
                Debug.Assert(gamma == 2 * (n / x) - (BigInteger)(x - 2) * delta);

                s += beta + (beta & 1);
                x -= 2;
            }
            var nRep = (UInt128)n;
            while (x >= x1)
            {
                beta = (long)((nRep / (ulong)x) & 3);
                s += beta + (beta & 1);
                x -= 2;
            }
            return (int)((s >> 1) & 1);
#else
            return hyperbolicSum[0].Evaluate(n, x1, x2).IsEven ? 0 : 1;
#endif
        }

        private int S1(long n, int x1, int x2)
        {
            var s = (int)0;
            var x = (int)DownToOdd(x2);
            var beta = (int)(n / (x + 2));
            var eps = (int)(n % (x + 2));
            var delta = (int)(n / x - beta);
            var gamma = 2 * beta - x * delta;
            while (x >= x1)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += 4 * delta;
                beta += delta;
                beta &= 3;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == ((n / x) & 3));
                Debug.Assert(delta == (n / x) - n / (x + 2));
                Debug.Assert(gamma == 2 * (n / x) - (BigInteger)(x - 2) * delta);

                s += beta + (beta & 1);
                x -= 2;
            }
            while (x >= x1)
            {
                beta = (int)((n / x) & 3);
                s += beta + (beta & 1);
                x -= 2;
            }
            return (int)((s >> 1) & 1);
        }

        private long Xi(long i)
        {
            return (long)IntegerMath.FloorSquareRoot(n / i);
        }

        private void UpdateMx(long x1, long x2, long offset, long increment)
        {
            // Add the contributions to each mx from all the small m values.
            for (var i = offset; i <= imax; i += increment)
            {
                var x = xi[i];
                var sqrt = IntegerMath.FloorSquareRoot(x);
                var s = (long)0;

                var jmin = UpToOdd(Math.Max(3, x / (x2 + 1) + 1));
                var jmax = DownToOdd(Math.Min(sqrt, x / x1));
                s += JSum(x, jmin, ref jmax, x1);
                for (var j = jmin; j <= jmax; j += 2)
                    s += m[(x / j - x1) >> 1];

                var kmin = Math.Max(1, x1);
                var kmax = Math.Min(x / sqrt - 1, x2);
                s += KSum(x, kmin, ref kmax, x1);
                var current = T1Odd(x / kmin);
                for (var k = kmin; k <= kmax; k++)
                {
                    var next = T1Odd(x / (k + 1));
                    s += (current - next) * m[(k - x1) >> 1];
                    current = next;
                }

                mx[i] -= s;
            }
        }

        private long JSum(long n, long j1, ref long j, long x1)
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
                gamma += delta;
                gamma += delta;
                gamma += delta;
                gamma += delta;
                beta += delta;

                Debug.Assert(eps == n % j);
                Debug.Assert(beta == n / j);
                Debug.Assert(delta == beta - n / (j + 2));
                Debug.Assert(gamma == 2 * beta - (BigInteger)(j - 2) * delta);

                s += m[(beta - x1) >> 1];
                j -= 2;
            }
            return s;
        }

        private long KSum(long n, long k1, ref long k, long x1)
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
                gamma += delta;
                gamma += delta;
                beta += delta;

                Debug.Assert(eps == n % k);
                Debug.Assert(beta == n / k);
                Debug.Assert(delta == beta - n / (k + 1));
                Debug.Assert(gamma == beta - (BigInteger)(k - 1) * delta);

                // Equivalent to:
                // s += (T1Odd(beta) - T1Odd(beta - delta)) * m[k];
                s += ((delta + (beta & 1)) >> 1) * m[(k - x1) >> 1];
                --k;
            }
            return s;
        }

        private void ComputeMx()
        {
            // Add the remaining contributions to each mx from other mx values.
            for (var i = imax; i >= 1; i--)
            {
                var jmax = DownToOdd(xi[i] / (xmax + 1));
                var s = (long)0;
                for (var j = (long)3; j <= jmax; j += 2)
                    s += mx[j * j * i];
                mx[i] += 1 - s;
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

        private long T1Odd(long a)
        {
            return (a + (a & 1)) >> 1;
        }
    }
}
