using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class OldMillerRabin : IPrimalityAlgorithm<int>, IPrimalityAlgorithm<uint>, IPrimalityAlgorithm<long>, IPrimalityAlgorithm<ulong>, IPrimalityAlgorithm<BigInteger>
    {
        private IRandomNumberGenerator generator = new MersenneTwister(0);
        private int k;

        public OldMillerRabin(int k)
        {
            this.k = k;
        }

        public bool IsPrime(int n)
        {
            return IsPrime(n, k);
        }

        public bool IsPrime(uint n)
        {
            return IsPrime(n, k);
        }

        public bool IsPrime(long n)
        {
            return IsPrime(n, k);
        }

        public bool IsPrime(ulong n)
        {
            return IsPrime(n, k);
        }

        public bool IsPrime(BigInteger n)
        {
            if (n <= long.MaxValue)
                return IsPrime((long)n, k);
            return IsPrime(n, k);
        }

        private bool IsPrime(long n, int k)
        {
            if (n < 2)
                return false;
            if (n != 2 && (n & 1) == 0)
                return false;
            var nMinusOne = n - 1;
            var s = nMinusOne;
            while ((s & 1) == 0)
                s >>= 1;
            var random = generator.Create<long>();
            for (int i = 0; i < k; i++)
            {
                var a = random.Next(n - 4) + 2;
                var temp = s;
                var mod = IntegerMath.ModularPower(a, temp, n);
                while (temp != nMinusOne && mod != 1 && mod != nMinusOne)
                {
                    mod = IntegerMath.ModularProduct(mod, mod, n);
                    temp <<= 1;
                }
                if (mod != nMinusOne && (temp & 1) == 0)
                    return false;
            }
            return true;
        }

        private bool IsPrime(BigInteger n, int k)
        {
            if (n < BigIntegers.Two)
                return false;
            if (n != BigIntegers.Two && n.IsEven)
                return false;
            var nMinusOne = n - 1;
            var s = nMinusOne;
            while (s.IsEven)
                s >>= 1;
            var random = generator.Create<BigInteger>();
            for (int i = 0; i < k; i++)
            {
                var a = random.Next(n - 4) + 2;
                var temp = s;
                var mod = BigInteger.ModPow(a, temp, n);
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
