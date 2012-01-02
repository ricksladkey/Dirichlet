using System.Collections.Generic;

namespace Decompose.Numerics
{
    public class TrialDivision : IPrimalityAlgorithm<int>, IFactorizationAlgorithm<int>
    {
        public bool IsPrime(int n)
        {
            if (n < 2)
                return false;
            if (n <= 3)
                return true;
            if ((n & 1) == 0)
                return false;
            if (n % 3 == 0)
                return false;
            int p = 5;
            int i = 2;
            while (p * p <= n)
            {
                if (n % p == 0)
                    return false;
                p += i;
                i = 6 - i;
            }
            return true;
        }

        public IEnumerable<int> Factor(int n)
        {
            if (n < 2)
            {
                yield return n;
                yield break;
            }
            while ((n & 1) == 0)
            {
                yield return 2;
                n >>= 1;
            }
            while (n % 3 == 0)
            {
                yield return 3;
                n /= 3;
            }
            if (n == 1)
                yield break;
            int p = 5;
            int i = 2;
            while (true)
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
                p += i;
                i = 6 - i;
            }
        }
    }
}
