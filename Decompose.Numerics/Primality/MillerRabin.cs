using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class MillerRabin : IPrimalityAlgorithm<int>, IPrimalityAlgorithm<long>, IPrimalityAlgorithm<BigInteger>
    {
        private IRandomNumberGenerator<ulong> randomLong = new MersenneTwister(0).CreateInstance<ulong>();
        private IRandomNumberGenerator<BigInteger> randomBigInteger = new MersenneTwister(0).CreateInstance<BigInteger>();
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
            for (int i = 0; i < k; i++)
            {
                var a = (long)randomLong.Next((ulong)n - 4) + 2;
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
            for (int i = 0; i < k; i++)
            {
                var a = randomBigInteger.Next(n - 4) + 2;
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

        private bool IsPrime<T>(T n, int k, IReductionAlgorithm<T> ops)
        {
            var one = ops.Convert(1);
            var two = ops.Convert(2);
            var four = ops.Convert(4);
            if (ops.Compare(n, two) < 0)
                return false;
            if (!ops.Equals(n, two) && ops.IsEven(n))
                return false;
            var nMinusOne = ops.Subtract(n, one);
            var s = nMinusOne;
            while (ops.IsEven(s))
                s = ops.RightShift(s, 1);
            for (int i = 0; i < k; i++)
            {
                var a = ops.Add(ops.Random.Next(ops.Subtract(n, four)), two);
                var temp = s;
                var mod = ops.ModularPower(a, temp, n);
                while (!ops.Equals(temp, nMinusOne) && !ops.Equals(mod, one) && !ops.Equals(mod, nMinusOne))
                {
                    mod = ops.ModularProduct(mod, mod, n);
                    temp = ops.LeftShift(temp, 1);
                }
                if (!ops.Equals(mod, nMinusOne) && ops.IsEven(temp))
                    return false;
            }
            return true;
        }
    }
}
