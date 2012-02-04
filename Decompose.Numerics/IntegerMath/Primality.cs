using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        private static IPrimalityAlgorithm<uint> primalityInt = new TrialDivisionPrimality();

        public static bool IsPrime(int n)
        {
            return IsPrime((uint)n);
        }

        public static bool IsPrime(uint n)
        {
            return primalityInt.IsPrime(n);
        }

        private static IPrimalityAlgorithm<ulong> primalityLong = MillerRabin.Create(16, new UInt64MontgomeryReduction());

        public static bool IsPrime(long n)
        {
            return IsPrime((ulong)n);
        }

        public static bool IsPrime(ulong n)
        {
            if (n <= uint.MaxValue)
                return IsPrime((uint)n);
            return primalityLong.IsPrime(n);
        }

        private static IPrimalityAlgorithm<BigInteger> primalityBigInteger = MillerRabin.Create(16, new BigIntegerMontgomeryReduction());

        public static bool IsPrime(BigInteger n)
        {
            if (n <= ulong.MaxValue)
                return IsPrime((ulong)n);
            return primalityBigInteger.IsPrime(n);
        }

        public static bool IsProbablePrime(int n)
        {
            return IsProbablePrime((uint)n);
        }

        public static bool IsProbablePrime(uint n)
        {
            return IntegerMath.ModularPower(2, n - 1, n) == 1;
        }

        public static bool IsProbablePrime(long n)
        {
            return IsProbablePrime((ulong)n);
        }

        public static bool IsProbablePrime(ulong n)
        {
            if (n <= uint.MaxValue)
                return IsProbablePrime((uint)n);
            return ModularPower(2, n - 1, n) == 1;
        }

        public static bool IsProbablePrime(BigInteger n)
        {
            return ModularPower(BigIntegers.Two, n - BigInteger.One, n).IsOne;
        }

        public static BigInteger NextPrime(BigInteger n)
        {
            while (!IsPrime(n))
                ++n;
            return n;
        }
    }
}
