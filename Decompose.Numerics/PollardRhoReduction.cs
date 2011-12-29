using System;
using System.Numerics;
using System.Threading;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class PollardRhoReduction : PollardRhoBase
    {
        const int iterations = 100;
        IReductionAlgorithm reduction;

        public PollardRhoReduction(int threads, IReductionAlgorithm reduction)
            : base(threads)
        {
            this.reduction = reduction;
        }

        protected override BigInteger Rho(BigInteger n, BigInteger xInit, BigInteger c, CancellationToken cancellationToken)
        {
            if (n.IsEven)
                return BigIntegerUtils.Two;

            var reducer = reduction.GetReducer(n);
            var x = reducer.ToResidue(xInit);
            var y = x.Copy();
            var ys = x.Copy();
            var r = 1;
            var m = iterations;
            var cPrime = reducer.ToResidue(c);
            var one = reducer.ToResidue(BigInteger.One);
            var diff = one.Copy();
            var q = one.Copy();
            var g = BigInteger.One;

            do
            {
                x.Set(y);
                for (int i = 0; i < r; i++)
                    AdvanceF(y, cPrime);
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
                        AdvanceF(y, cPrime);
                        q.Multiply(AbsDiff(diff, x, y));
                    }
                    g = BigInteger.GreatestCommonDivisor(q.ToBigInteger(), n);
                    k += limit;
                }
                r <<= 1;
            } while (g.IsOne);

            if (g.CompareTo(n) == 0)
            {
                // Backtrack.
                do
                {
                    if (cancellationToken.IsCancellationRequested)
                        return BigInteger.Zero;
                    AdvanceF(ys, cPrime);
                    AbsDiff(diff, ys, x);
                    g = BigInteger.GreatestCommonDivisor(diff.ToBigInteger(), n);
                } while (g.IsOne);
            }

            if (g.CompareTo(n) == 0)
                return BigInteger.Zero;

            return g;
        }

        private static void AdvanceF(IResidue x, IResidue c)
        {
            x.Multiply(x).Add(c);
        }

        private static IResidue AbsDiff(IResidue diff, IResidue x, IResidue y)
        {
            if (x.CompareTo(y) <= 0)
                diff.Set(y).Subtract(x);
            else
                diff.Set(x).Subtract(y);
            return diff;
        }
    }
}
