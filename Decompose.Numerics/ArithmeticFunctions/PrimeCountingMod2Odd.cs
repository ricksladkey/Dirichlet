using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class PrimeCountingMod2Odd
    {
        private static int maximumBatchSize = 1 << 20;
        private int threads;
        private long kmax;
        private sbyte[] mobius;
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
            var sum = 0;
            kmax = (int)IntegerMath.FloorLog(n, 2);
            var xmax = (long)IntegerMath.FloorRoot(n, 2);
            var range = new MobiusOddRange(xmax + 1, 0);
            mobius = new sbyte[maximumBatchSize >> 1];
            for (var x = (long)1; x <= xmax; x += maximumBatchSize)
            {
                var xfirst = x;
                var xlast = Math.Min(xmax, xfirst + maximumBatchSize - 1);
                range.GetValues(xfirst, xlast + 2, mobius);
                sum += Pi2(n, xfirst, xlast);
            }
            for (var k = 1; k <= kmax; k++)
                sum -= IntegerMath.Mobius(k);
            sum &= 3;
            sum >>= 1;
            return (sum + (n >= 2 ? 1 : 0)) % 2;
        }

        private int Pi2(BigInteger n, long x1, long x2)
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
            var xmin = (Math.Max(1, x1) - 1) | 1;
#if true
            var xmax = (Math.Min((long)IntegerMath.FloorSquareRoot(n), x2) - 1) | 1;
#else
            var xmax = long)IntegerMath.FloorPower(n, 2, 7);
#endif
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

                var mu = mobius[(x - x1) >> 1];
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
                var mu = mobius[(x - x1) >> 1];
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

        public int T2(BigInteger n)
        {
            var sqrt = (long)IntegerMath.FloorSquareRoot(n);
            var result = 2 * S1(n, 1, sqrt) + 3 * (int)(((sqrt + (sqrt & 1)) >> 1) & 1);
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
            var x = (x2 - 1) | 1;
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
            var x = (x2 - 1) | 1;
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
    }
}
