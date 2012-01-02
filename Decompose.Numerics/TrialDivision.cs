using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class TrialDivision : IFactorizationAlgorithm<int>
    {
        private SieveOfErostothones primes = new SieveOfErostothones();

        public IEnumerable<int> Factor(int n)
        {
            if (n < 2)
            {
                yield return n;
                yield break;
            }
            foreach (var p in primes)
            {
                while (n % p == 0)
                {
                    yield return p;
                    n /= p;
                    if (n == 1)
                        yield break;
                }
                if (p * p > n)
                {
                    yield return n;
                    yield break;
                }
            }
        }
    }
}
