using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public class PrimeCountingMod3
    {
        private MobiusCollection mobius;

        public BigInteger Evaluate(BigInteger n)
        {
            var jmax = IntegerMath.FloorLog(n, 2);
            var dmax = IntegerMath.FloorRoot(n, 3);
            mobius = new MobiusCollection((int)(IntegerMath.Max(jmax, dmax) + 1), 0);
            var sum = SMT(n) / 3;
            for (var j = 2; j <= jmax; j++)
                sum -= (j % 3) * Evaluate(IntegerMath.FloorRoot(n, j));
            return sum % 3;
        }

        public BigInteger SMT(BigInteger n)
        {
            var s = (BigInteger)0;
            var dmax = IntegerMath.FloorRoot(n, 3);
            for (var d = 1; d <= dmax; d++)
            {
                var md = mobius[d];
                if (md == 0)
                    continue;
                var term = T3(n / IntegerMath.Power(d, 3));
                s += md * term;
            }
            return s;
        }

        public BigInteger T3(BigInteger n)
        {
            var sum = (BigInteger)0;
            var root3 = IntegerMath.FloorRoot(n, 3);
            for (var z = (BigInteger)1; z <= root3; z++)
            {
                var nz = n / z;
                var sqrtnz = IntegerMath.FloorSquareRoot(nz);
                var t = (BigInteger)0;
                for (var x = z + 1; x <= sqrtnz; x++)
                    t += nz / x;
                sum += 2 * t - sqrtnz * sqrtnz + nz / z;
            }
            sum = 3 * sum + root3 * root3 * root3;
            return sum;
        }
    }
}
