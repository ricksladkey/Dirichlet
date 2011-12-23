using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public static class BigIntegerUtils
    {
        public static BigInteger Two = (BigInteger)2;

        public static BigInteger AddMod(BigInteger a, BigInteger b, BigInteger n)
        {
            Debug.Assert(a >= BigInteger.Zero && a < n);
            Debug.Assert(b >= BigInteger.Zero && b < n);
            var sum = a + b;
            if (sum >= n)
                sum -= n;
            return sum;
        }

        public static BigInteger SubAbsMod(BigInteger a, BigInteger b, BigInteger n)
        {
            Debug.Assert(a >= BigInteger.Zero && a < n);
            Debug.Assert(b >= BigInteger.Zero && b < n);
            if (a > b)
                return a - b;
            return b - a;
        }

        public static BigInteger MulMod(BigInteger a, BigInteger b, BigInteger n)
        {
            Debug.Assert(a >= BigInteger.Zero && a < n);
            Debug.Assert(b >= BigInteger.Zero && b < n);
            if (a.IsZero || b.IsZero)
                return BigInteger.Zero;
            if (a > b)
                return MulModInternal(a, b, n);
            if (a < b)
                return MulModInternal(b, a, n);
            return SquareMod(a, n);
        }

        public static BigInteger SquareMod(BigInteger a, BigInteger n)
        {
            Debug.Assert(a >= BigInteger.Zero && a < n);
            return MulModInternal(a, a, n);
        }

        private static BigInteger MulModInternal(BigInteger a, BigInteger b, BigInteger n)
        {
            // a * (c + 1) = a * c + a
            // a * 2d = (a * d) + (a * d)
            if (b.IsEven)
            {
                var x = MulModInternal(a, b / Two, n);
                return AddMod(x, x, n);
            }
            if (b.IsOne)
                return a;
            return AddMod(MulModInternal(a, b - BigInteger.One, n), a, n);
        }

        public static BigInteger ModPow(BigInteger b, BigInteger e, BigInteger m)
        {
            return ModPowInternal(b, e, 1, m);
        }

        private static BigInteger ModPowInternal(BigInteger b, BigInteger e, BigInteger p, BigInteger modulus)
        {
            if (e == 0)
                return p;
            if (e % 2 == 0)
                return ModPowInternal(b * b % modulus, e / 2, p, modulus);
            return ModPowInternal(b, e - 1, b * p % modulus, modulus);
        }

        private static ISqrtAlgorithm sqrt = new SqrtNewtonsMethod();

        public static BigInteger Sqrt(BigInteger n)
        {
            return sqrt.Sqrt(n);
        }

        private static IPrimalityAlgorithm millerRabin = new MillerRabin(16);

        public static bool IsPrime(BigInteger n)
        {
            return millerRabin.IsPrime(n);
        }
    }
}
