using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System;

namespace Decompose.Numerics
{
    public class QuadraticSieve : IFactorizationAlgorithm<BigInteger>
    {
        protected MersenneTwister32 random = new MersenneTwister32(0);
        protected int threads;

        public QuadraticSieve(int threads)
        {
            this.threads = threads;
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            var factors = new List<BigInteger>();
            FactorCore(n, factors);
            return factors;
        }

        private void FactorCore(BigInteger n, List<BigInteger> factors)
        {
            if (n == 1)
                return;
            if (BigIntegerUtils.IsPrime(n))
            {
                factors.Add(n);
                return;
            }
            var divisor = GetDivisor(n);
            if (!divisor.IsZero)
            {
                FactorCore(divisor, factors);
                FactorCore(n / divisor, factors);
            }
        }

        private BigInteger GetDivisor(BigInteger n)
        {
            if (n.IsEven)
                return BigIntegerUtils.Two;
            var sqrtn = BigIntegerUtils.Sqrt(n);
            var factorBaseCandidates = new List<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };
            var factorBase = factorBaseCandidates.Where(factor => BigIntegerUtils.JacobiSymbol(n, factor) == 1).ToList();
                return BigInteger.Zero;
        }
    }
}
