using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class DivisionFreeDivisorSummatoryFunction
    {
        private BigInteger n;

        public BigInteger Evaluate(BigInteger n)
        {
            this.n = n;
            return S1(1, (long)IntegerMath.FloorSquareRoot(n));
        }

        private const ulong smax = (ulong)1 << 63;

        public BigInteger S1(long x1, long x2)
        {
            var s = (UInt128)0;
            var s2 = (ulong)s;
            var x = x2;
#if true
            var beta = (long)(n / (x + 1));
            var eps = (long)(n - (x + 1) * beta);
            var delta = (long)(n / x - beta);
            var gamma = beta - x * delta;
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
                gamma += 2 * delta;
                beta += delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 1));
                Debug.Assert(gamma == beta - (x - 1) * delta);

                s2 += (ulong)beta;
                if (s2 > smax)
                {
                    s += s2;
                    s2 = 0;
                }
                --x;
            }
            s += s2;
            s2 = 0;
#endif
#if true
            eps = (long)(n - (x + 1) * beta);
            delta = (long)(n / x - beta);
            gamma = beta - x * delta;
            var betaRep = (UInt128)beta;
            var xmin = IntegerMath.FloorRoot(n, 6);
            while (x >= xmin)
            {
                eps += gamma;
                var delta2 = eps >= 0 ? eps / x : (eps - x + 1) / x;
                delta += delta2;
                var a = x * delta2;
                eps -= a;
                gamma += 2 * delta - a;
                betaRep += (ulong)delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(betaRep == n / x);
                Debug.Assert(delta == betaRep - n / (x + 1));
                Debug.Assert(gamma == (BigInteger)betaRep - (x - 1) * delta);

                s += betaRep;
                --x;
            }
#endif
            var nRep = (UInt128)n;
            while (x >= x1)
            {
                s += nRep / (ulong)x;
                --x;
            }
            return s;
        }
    }
}
