using System.Numerics;

namespace Decompose.Numerics
{
    public class MillerRabin : IPrimalityAlgorithm<int>, IPrimalityAlgorithm<long>, IPrimalityAlgorithm<BigInteger>
    {
        private IRandomNumberAlgorithm<BigInteger> random = new MersenneTwisterBigInteger(0);
        private int k;

        public MillerRabin(int k)
        {
            this.k = k;
        }

        public bool IsPrime(int n)
        {
            return IsPrime(n, k);
        }

        public bool IsPrime(long n)
        {
            return IsPrime(n, k);
        }

        public bool IsPrime(BigInteger n)
        {
            return IsPrime(n, k);
        }

        private bool IsPrime(BigInteger n, int k)
        {
            if (n < BigIntegers.Two)
                return false;
            if (n != BigIntegers.Two && n.IsEven)
                return false;
            var s = n - 1;
            while (s.IsEven)
                s >>= 1;
            for (int i = 0; i < k; i++)
            {
                var a = random.Next(n - 4) + 2;
                var temp = s;
                var mod = BigInteger.ModPow(a, temp, n);
                var nMinusOne = n - 1;
                while (temp != nMinusOne && mod != 1 && mod != nMinusOne)
                {
                    mod = mod * mod % n;
                    temp = temp * BigIntegers.Two;
                }
                if (mod != nMinusOne && temp.IsEven)
                    return false;
            }
            return true;
        }
    }
}
