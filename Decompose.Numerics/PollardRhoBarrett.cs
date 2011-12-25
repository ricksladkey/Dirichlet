using System;
using System.Numerics;
using System.Threading;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class PollardRhoBarrett : PollardRhoBase
    {
        const int iterations = 100;

        public PollardRhoBarrett(int threads)
            : base(threads)
        {
        }

        protected override BigInteger Rho(BigInteger n, BigInteger xInit, BigInteger c, CancellationToken cancellationToken)
        {
            if (n.IsEven)
                return BigIntegerUtils.Two;

            var reduction = new BarrettReduction(n);
            var x = reduction.ToResidue(xInit);
            var y = x.Copy();
            var ys = x.Copy();
            var r = 1;
            var m = iterations;
            var nPrime = reduction.ToResidue(n);
            var cPrime = reduction.ToResidue(c);
            var one = reduction.ToResidue(BigInteger.One);
            var diff = one.Copy();
            var q = one.Copy();
            var g = one.Copy();

            do
            {
                x.Set(y);
                for (int i = 0; i < r; i++)
                    NextF(y, cPrime);
                var k = 0;
                while (k < r && g.IsOne)
                {
                    ys.Set(y);
                    var limit = Math.Min(m, r - k);
                    q.Set(one);
                    for (int i = 0; i < limit; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return BigInteger.Zero;
                        NextF(y, cPrime);
                        
                        if (x.CompareTo(y) <= 0)
                            diff.Set(y).Subtract(x);
                        else
                            diff.Set(x).Subtract(y);
                        q.Multiply(diff);
                    }
                    g.SetGreatestCommonDivisor(q, nPrime);
                    k += limit;
                }
                r <<= 1;
            } while (g.IsOne);

            if (g.CompareTo(nPrime) == 0)
            {
                do
                {
                    if (cancellationToken.IsCancellationRequested)
                        return BigInteger.Zero;
                    NextF(ys, cPrime);
                    if (x.CompareTo(ys) <= 0)
                        diff.Set(ys).Subtract(x);
                    else
                        diff.Set(x).Subtract(ys);
                    g.SetGreatestCommonDivisor(diff, nPrime);
                } while (g.IsOne);
            }

            if (g.CompareTo(nPrime) == 0)
                throw new InvalidOperationException("failed");

            return g.ToBigInteger();
        }

        private static void NextF(IResidue x, IResidue c)
        {
            x.Multiply(x).Add(c);
        }
    }
}
