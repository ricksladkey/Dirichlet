using System.Diagnostics;
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
            if ((n & 1) == 0)
                return false;
            if (n <= uint.MaxValue)
                return IsProbablePrime((uint)n);
#if true
            return ModularPower(2, n - 1, n) == 1;
#else
            return ModularPowerOfTwo(n - 1, n) == 1;
#endif
        }

        public static ulong ModularPowerOfTwo(ulong exponent, ulong modulus)
        {
            var exponentOrig = exponent;
            if (exponent < 64)
                return ((ulong)1 << (int)exponent) % modulus;
            var value = ulong.MaxValue % modulus + 1;
            var result = ((ulong)1 << (int)(exponent & 63)) % modulus;
            exponent >>= 6;
            return UInt128.ModularProduct(ModularPower(value, exponent, modulus), result, modulus);
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
