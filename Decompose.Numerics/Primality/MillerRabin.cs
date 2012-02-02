using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class MillerRabin
    {
        private class PrimalityAlgorithm<T> : IPrimalityAlgorithm<T>
        {
            private IReductionAlgorithm<T> reduction;
            private IRandomNumberGenerator generator = new MersenneTwister(0);
            private int k;

            public PrimalityAlgorithm(int k, IReductionAlgorithm<T> reduction)
            {
                this.k = k;
                this.reduction = reduction;
            }

            public bool IsPrime(T n)
            {
                if (reduction.Compare(n, reduction.Two) < 0)
                    return false;
                if (reduction.Equals(n, reduction.Two) || reduction.Equals(n, reduction.Convert(3)))
                    return true;
                if (reduction.IsEven(n))
                    return false;
                var random = generator.Create<T>();
                var reducer = reduction.GetReducer(n);
                var four = reduction.Convert(4);
                var s = 0;
                var d = reduction.Subtract(n, reduction.One);
                while (reduction.IsEven(d))
                {
                    d = reduction.RightShift(d, 1);
                    ++s;
                }
                var nMinusOne = reducer.ToResidue(reduction.Subtract(n, reduction.One));
                var x = reducer.ToResidue(reduction.Zero);
                for (int i = 0; i < k; i++)
                {
                    var a = reduction.Add(random.Next(reduction.Subtract(n, four)), reduction.Two);
                    x.Set(a).Power(d);
                    if (x.IsOne || x.Equals(nMinusOne))
                        continue;
                    for (int r = 1; r < s; r++)
                    {
                        x.Multiply(x);
                        if (x.IsOne)
                            return false;
                        if (x.Equals(nMinusOne))
                            break;
                    }
                    if (!x.Equals(nMinusOne))
                        return false;
                }
                return true;
            }
        }

        public static IPrimalityAlgorithm<T> Create<T>(int k, IReductionAlgorithm<T> reduction)
        {
            return new PrimalityAlgorithm<T>(k, reduction);
        }
    }
}
