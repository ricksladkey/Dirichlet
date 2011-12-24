using System;
using System.Numerics;
using System.Threading;

namespace Decompose.Numerics
{
    public class PollardRhoMontgomery : PollardRhoBase
    {
        const int iterations = 100;

        public PollardRhoMontgomery(int threads)
            : base(threads)
        {
        }

        protected override BigInteger Rho(BigInteger n, BigInteger xInit, BigInteger c, CancellationToken cancellationToken)
        {
            if (n.IsEven)
                return BigIntegerUtils.Two;

            var x = xInit;
            var y = xInit;
            var ys = y;
            var r = 1;
            var m = iterations;
            var g = BigInteger.One;
            var reduction = new MontgomeryReductionBigInteger(n);
            var cPrime = reduction.ToResidue(c);

            do
            {
                x = y;
                var yPrime = reduction.ToResidue(y);
                for (int i = 0; i < r; i++)
                    yPrime.Multiply(yPrime).Add(cPrime);
                y = yPrime.ToBigInteger();
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
                        y = F(y, c, n);
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
                    ys = F(ys, c, n);
                    g = BigInteger.GreatestCommonDivisor(x - ys, n);
                } while (g.IsOne);
            }

            return g;
        }
    }
}
