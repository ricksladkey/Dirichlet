using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Decompose.Numerics
{
    public static class MathUtils
    {
        const int threadsPollard = 4;
        const int iterationsPollard = 100;

        private static BigInteger two = (BigInteger)2;
        private static MersenneTwister32 randomMillerRabin = new MersenneTwister32(0);
        private static MersenneTwister32 randomPollard = new MersenneTwister32(0);

        public static BigInteger Sqrt(BigInteger n)
        {
            return SqrtNewtonsMethod(n);
        }

        public static BigInteger SqrtNewtonsMethod(BigInteger n)
        {
            if (n < two)
                return n;
            var x0 = n;
            var x1 = x0;
            do
            {
                x0 = x1;
                x1 = (x0 + n / x0) / two;
            } while (x1 < x0);
            return x0;
        }

        public static BigInteger SqrtBinarySearch(BigInteger n)
        {
            if (n < 2)
                return n;
            var m0 = (BigInteger)1;
            var m2 = n / 2 + 1;
            var n0 = m0 * m0;
            var n2 = m2 * m2;
            while (m2 - m0 > 1)
            {
                var m1 = (m0 + m2) / 2;
                var n1 = m1 * m1;
                if (n1 == n)
                    return m1;
                else if (n1 > n)
                {
                    m2 = m1;
                    n2 = n1;
                }
                else if (n1 < n)
                {
                    m0 = m1;
                    n0 = n1;
                }
            }
            return m0;
        }

        public static bool IsPrimeMillerRabin(BigInteger n, int k)
        {
            if (n < two)
                return false;
            if (n != two && n.IsEven)
                return false;
            var s = n - 1;
            while (s.IsEven)
                s >>= 1;
            for (int i = 0; i < k; i++)
            {
                var a = randomMillerRabin.Next(n - 4) + 2;
                var temp = s;
                var mod = BigInteger.ModPow(a, temp, n);
                var nMinusOne = n - 1;
                while (temp != nMinusOne && mod != 1 && mod != nMinusOne)
                {
                    mod = mod * mod % n;
                    temp = temp * two;
                }
                if (mod != nMinusOne && temp.IsEven)
                    return false;
            }
            return true;
        }

        public static BigInteger ModPow(BigInteger b, BigInteger e, BigInteger m)
        {
            return ModPowInternal(b, e, 1, m);
        }

        private static BigInteger ModPowInternal(BigInteger b, BigInteger e, BigInteger p, BigInteger modulus)
        {
            if (e == 0)
                return p;
            if (e % 2 == 0)
                return ModPowInternal(b * b % modulus, e / 2, p, modulus);
            return ModPowInternal(b, e - 1, b * p % modulus, modulus);
        }

        public static IEnumerable<BigInteger> FactorPollard(BigInteger n)
        {
            var factors = new ConcurrentQueue<BigInteger>();
            FactorPollardInternal(n, factors);
            return factors;
        }

        private static void FactorPollardInternal(BigInteger n, ConcurrentQueue<BigInteger> factors)
        {
            if (n == 1)
                return;
            if (IsPrimeMillerRabin(n, 16))
            {
                factors.Enqueue(n);
                return;
            }
#if false
            var divisor = RhoParallel(n, RhoPollard);
#else
            var divisor = RhoParallel(n, RhoBrent);
#endif
            FactorPollardInternal(divisor, factors);
            FactorPollardInternal(n / divisor, factors);
        }

        private static BigInteger RhoParallel(BigInteger n, Func<BigInteger, CancellationToken, BigInteger> rho)
        {
            if (threadsPollard == 1)
                return rho(n, CancellationToken.None);
            var cancellationToken = new CancellationTokenSource();
            var tasks = new Task<BigInteger>[threadsPollard];
            for (int i = 0; i < threadsPollard; i++)
                tasks[i] = Task.Factory.StartNew(() => rho(n, cancellationToken.Token));
            var index = Task.WaitAny(tasks);
            cancellationToken.Cancel();
            return tasks[index].Result;
        }

        private static BigInteger RhoPollard(BigInteger n, CancellationToken cancellationToken)
        {
            if (n % two == 0) return two;

            var c = randomPollard.Next(n - BigInteger.One) + BigInteger.One;
            var x = randomPollard.Next(n);
            var y = x;

            var divisor = BigInteger.One;
            var x0 = x;
            var xx0 = y;
            do
            {
                var z = BigInteger.One;
                x0 = x;
                xx0 = y;
                for (int i = 0; i < iterationsPollard; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return BigInteger.Zero;
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    z = z * (x - y) % n;
                }
                divisor = BigInteger.GreatestCommonDivisor(z, n);
            } while (divisor.IsOne);

            if (divisor == n)
            {
                x = x0;
                y = xx0;
                do
                {
                    if (cancellationToken.IsCancellationRequested)
                        return BigInteger.Zero;
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    divisor = BigInteger.GreatestCommonDivisor(y - x, n);
                } while (divisor.IsOne);
            }

            return divisor;
        }

        private static BigInteger RhoBrent(BigInteger n, CancellationToken cancellationToken)
        {
            if (n % two == 0) return two;

            var c = randomPollard.Next(n - BigInteger.One) + BigInteger.One;
            var x = randomPollard.Next(n);
            var y = x;
            var ys = y;

            var g = BigInteger.One;
            var q = BigInteger.One;
            var r = 1;
            var m = 100;
            do
            {
                x = y;
                for (int i = 0; i < r; i++)
                    y = (y * y + c) % n;
                var k = 0;
                while (k < r && g == 1)
                {
                    ys = y;
                    var limit = Math.Min(m, r - k);
                    for (int i = 0; i < limit; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return BigInteger.Zero;
                        y = (y * y + c) % n;
                        q = q * (x - y) % n;
                    }
                    g = BigInteger.GreatestCommonDivisor(q, n);
                    k += m;
                }
                r <<= 1;
            } while (g.IsOne);

            if (g == n)
            {
                do
                {
                    if (cancellationToken.IsCancellationRequested)
                        return BigInteger.Zero;
                    ys = (ys * ys + c) % n;
                    g = BigInteger.GreatestCommonDivisor(x - ys, n);
                } while (g.IsOne);
            }

#if false
            Console.WriteLine("n = {0}, count = {1}", n, count);
#endif
            return g;
        }
    }
}
