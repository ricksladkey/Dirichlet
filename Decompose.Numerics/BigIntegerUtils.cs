using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public static class BigIntegerUtils
    {
        public static BigInteger Two = (BigInteger)2;

        public static void ExtendedGreatestCommonDivisor(BigInteger a, BigInteger b, out BigInteger c, out BigInteger d)
        {
            var x = BigInteger.Zero;
            var lastx = BigInteger.One;
            var y = BigInteger.One;
            var lasty = BigInteger.Zero;

            while (!b.IsZero)
            {
                var quotient = a / b;
                var tmpa = a;
                a = b;
                b = tmpa % b;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        private static ISqrtAlgorithm<BigInteger> sqrt = new SqrtNewtonsMethod();

        public static BigInteger Sqrt(BigInteger n)
        {
            return sqrt.Sqrt(n);
        }

        private static IPrimalityAlgorithm<BigInteger> millerRabin = new MillerRabin(16);

        public static bool IsPrime(BigInteger n)
        {
            return millerRabin.IsPrime(n);
        }
    }
}
