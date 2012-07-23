using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class PrimeCountingMod2
    {
        private int threads;
        private MobiusCollection mobius;
        private Dictionary<BigInteger, BigInteger> t2Map;
        private DivisionFreeDivisorSummatoryFunction[] hyperbolicSum;

        public PrimeCountingMod2(int threads)
        {
            this.threads = threads;
            t2Map = new Dictionary<BigInteger, BigInteger>();
            var count = Math.Max(threads, 1);
            hyperbolicSum = new DivisionFreeDivisorSummatoryFunction[count];
            for (var i = 0; i < count; i++)
                hyperbolicSum[i] = new DivisionFreeDivisorSummatoryFunction(0, false, false);
        }

        public BigInteger Evaluate(BigInteger n)
        {
            t2Map.Clear();
            var jmax = IntegerMath.FloorLog(n, 2);
            var dmax = IntegerMath.FloorRoot(n, 2);
            mobius = new MobiusCollection((int)(IntegerMath.Max(jmax, dmax) + 1), 0);
            return Pi2(n);
        }

        public BigInteger Pi2(BigInteger n)
        {
            var kmax = IntegerMath.FloorLog(n, 2);
            var sum = (BigInteger)0;
            for (var k = 1; k <= kmax; k++)
            {
                if (k % 3 != 0 && mobius[k] != 0)
                    sum += mobius[k] * F2(IntegerMath.FloorRoot(n, k));
            }
            return sum % 2;
        }

        public BigInteger F2(BigInteger n)
        {
            var s = (BigInteger)0;
            var dmax = IntegerMath.FloorRoot(n, 2);
            for (var d = 1; d <= dmax; d++)
            {
                var md = mobius[d];
                if (md == 0)
                    continue;
                var term = T2(n / IntegerMath.Power((BigInteger)d, 2));
                s += md * term;
            }
            Debug.Assert((s - 1) % 2 == 0);
            return (s - 1) / 2;
        }

        public BigInteger T2(BigInteger n)
        {
            BigInteger value;
            if (t2Map.TryGetValue(n, out value))
                return value;
            return t2Map[n] = hyperbolicSum[0].Evaluate(n);
        }
    }
}
