using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class PrimeCountingMod2Odd
    {
        private const int maximumBatchSize = 1 << 20;
        private const int divisorBatchSize = 1 << 18;
        private const long nMaxSimple = (long)1 << 40;
        private const long C1 = 1;
        private const long C2 = 1;
        private const long C3 = 1;
        private const long C4 = 1;

        private int threads;
        private BigInteger n;
        private long sqrtn;
        private long kmax;
        private long imax;
        private long xmed;
        private long xmax;
        private MobiusOddRange mobius;
        private DivisorOddRange divisors;
        private long[] xi;
        private long[] mx;
        private long m0;
        private sbyte[] values;
        private long[] m;
        private bool sieveDivisors;
        private long d1;
        private long d2;
        private long[] dsums;

        private IDivisorSummatoryFunction<BigInteger>[] hyperbolicSum;

        public PrimeCountingMod2Odd(int threads)
        {
            this.threads = threads;
            var count = Math.Max(threads, 1);
            hyperbolicSum = new IDivisorSummatoryFunction<BigInteger>[count];
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
            xmax = imax != 0 ? Xi(imax) : sqrtn;
            xmed = DownToOdd(Math.Min((long)(IntegerMath.FloorPower(n, 2, 7) * C3 / C4), xmax));
            var dmax = (long)IntegerMath.Min(n / ((BigInteger)xmed * xmed) + 1, n);
            mobius = new MobiusOddRange((xmax + 2) | 1, 0);
            divisors = new DivisorOddRange((dmax + 2) | 1, 0);
            xi = new long[imax + 1];
            mx = new long[imax + 1];

            // Initialize xi.
            for (var i = 1; i <= imax; i++)
                xi[i] = Xi(i);

            values = new sbyte[maximumBatchSize >> 1];
            m = new long[maximumBatchSize >> 1];
            m0 = (long)0;
            dsums = new long[maximumBatchSize];

            // Process small values.
            sieveDivisors = false;
            for (var x = (long)1; x <= xmed; x += maximumBatchSize)
            {
                var xfirst = x;
                var xlast = Math.Min(xmed, xfirst + maximumBatchSize - 2);
                m0 = mobius.GetValuesAndSums(xfirst, xlast + 2, values, m, m0);
                sum += Pi2Small(xfirst, xlast);
                UpdateMx(xfirst, xlast, 1, 1);
            }

            // Process medium values.
            sieveDivisors = true;
            d1 = d2 = 1;
            var xmaxodd = DownToOdd(xmax);
            for (var x = xmed + 2; x <= xmaxodd; x += maximumBatchSize)
            {
                var xfirst = x;
                var xlast = Math.Min(xmaxodd, xfirst + maximumBatchSize - 2);
                m0 = mobius.GetValuesAndSums(xfirst, xlast + 2, values, m, m0);
                sum += Pi2Medium(xfirst, xlast);
                UpdateMx(xfirst, xlast, 1, 1);
            }

            // Process large values.
            sum += Pi2Large();

            // Adjust for final parity of F2.
            sum -= IntegerMath.Mertens(kmax);

            // Compute final result.
            sum &= 3;
            Debug.Assert((sum & 1) == 0);
            sum >>= 1;
            return (sum + (n >= 2 ? 1 : 0)) % 2;
        }

        private int Pi2Small(long x1, long x2)
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

        private int Pi2Medium(long x1, long x2)
        {
            Debug.Assert(x1 > IntegerMath.FloorRoot(n, 4));
            return F2(n, x1, x2);
        }

        private int Pi2Large()
        {
            // Finialize mx.
            ComputeMx();

            // Compute tail.
            var s = (BigInteger)0;
            for (var i = 1; i < imax; i++)
                s += T2(i) * (mx[i] - mx[i + 1]);
            return (int)(s & 3);
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
            return sieveDivisors ? T2Sequential(n) : T2Isolated(n);
        }

        private int T2Isolated(BigInteger n)
        {
            var sqrt = (long)IntegerMath.FloorSquareRoot(n);
            var result = 2 * S1(n, 1, sqrt) + 3 * (int)(T1Odd(sqrt) & 1);
            Debug.Assert(result % 4 == new DivisionFreeDivisorSummatoryFunction(0, false, true).Evaluate(n) % 4);
            return result & 3;
        }

        private int T2Sequential(BigInteger n)
        {
            if (n < d1 || n >= d2)
            {
                var sum0 = (long)0;
                if (n >= d2 && n < d2 + divisorBatchSize)
                {
                    sum0 = d2 == 1 ? 0 : dsums[(d2 - d1 - 2) >> 1];
                    d1 = d2;
                }
                else if (n < d1 && n >= d1 - divisorBatchSize)
                {
                    // Could avoid an isolated computation if we supported summing down.
                    d1 = Math.Max(1, d1 - divisorBatchSize);
                    sum0 = d1 == 1 ? 0 : T2Isolated(d1 - 2);
                }
                else
                {
                    d1 = DownToOdd((long)n);
                    sum0 = d1 == 1 ? 0 : T2Isolated(d1 - 2);
                }
                d2 = DownToOdd(Math.Min(d1 + divisorBatchSize, Math.Max(divisors.Size, d1)));
                divisors.GetSums(d1, d2, dsums, sum0);
            }
            Debug.Assert(dsums[(int)(n - d1) >> 1] % 4 == new DivisionFreeDivisorSummatoryFunction(0, false, true).Evaluate(n) % 4);
            return (int)(dsums[(int)(n - d1) >> 1] & 3);
        }

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
                beta += delta;
                gamma += delta << 2;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 2));
                Debug.Assert(gamma == 2 * beta - (x - 2) * delta);

                s ^= beta + 1;
                x -= 2;
            }
            while (x >= x1)
            {
                beta = (int)((n / x) & 3);
                s ^= beta + 1;
                x -= 2;
            }
            return (s >> 1) & 1;
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

                var jmin = UpToOdd(Math.Max(3, x / (x2 + 2) + 1));
                var jmax = DownToOdd(Math.Min(sqrt, x / x1));
                s += JSum(x, jmin, ref jmax, x1);
                for (var j = jmin; j <= jmax; j += 2)
                    s += m[(x / j - x1) >> 1];

                var kmin = Math.Max(1, x1);
                var kmax = Math.Min(x / sqrt - 1, x2 + 2);
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
