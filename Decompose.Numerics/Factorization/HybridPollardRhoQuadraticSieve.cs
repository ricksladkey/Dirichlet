using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public class HybridPollardRhoQuadraticSieve : IFactorizationAlgorithm<BigInteger>
    {
        IFactorizationAlgorithm<BigInteger> pollard;
        IFactorizationAlgorithm<BigInteger> quadraticSieve;

        public HybridPollardRhoQuadraticSieve(int threads, int iterations, QuadraticSieve.Config config)
        {
            pollard = new PollardRhoReduction(threads, iterations, new MontgomeryReduction());
            quadraticSieve = new QuadraticSieve(config);
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            var smallFactors = pollard.Factor(n);
            var c = n / smallFactors.Product();
            if (c.IsOne)
                return smallFactors;
            var largeFactors = quadraticSieve.Factor(c);
            return smallFactors.Concat(largeFactors);
        }
    }
}
