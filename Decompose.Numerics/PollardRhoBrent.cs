using System;
using System.Numerics;
using System.Threading;

namespace Decompose.Numerics
{
    public class PollardRhoBrent : PollardRhoBase
    {
        const int iterations = 100;

        public PollardRhoBrent(int threads)
            : base(threads)
        {
        }

        protected override BigInteger Rho(BigInteger n, CancellationToken cancellationToken)
        {
            if (n.IsEven)
                return BigIntegerUtils.Two;

            var c = random.Next(n - BigInteger.One) + BigInteger.One;
            var x = random.Next(n);
            var y = x;
            var ys = y;
            var r = 1;
            var m = iterations;
            var g = BigInteger.One;

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
                    var q = BigInteger.One;
                    for (int i = 0; i < limit; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return BigInteger.Zero;
                        y = (y * y + c) % n;
                        q = q * (x - y) % n;
                    }
                    g = BigInteger.GreatestCommonDivisor(q, n);
                    k += limit;
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

            return g;
        }
    }
}
