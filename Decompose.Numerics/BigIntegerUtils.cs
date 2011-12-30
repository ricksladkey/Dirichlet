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

        private static BigInteger limit = (BigInteger)int.MaxValue;
        private static BigInteger four = (BigInteger)4;
        private static BigInteger eight = (BigInteger)8;

        public static int JacobiSymbol(BigInteger m, BigInteger n)
        {
            int result = 1;
            while (true)
            {
                m = m % n;
                if (n <= limit)
                    return result * JacobiSymbol((int)m, (int)n);
                if (m.IsZero)
                    return 0;
                if (m.IsEven)
                {
                    var k = (int)(n % eight);
                    var toggle = k == 1 || k == 7 ? 1 : -1;
                    do
                    {
                        m >>= 1;
                        result *= toggle;
                    } while (m.IsEven);
                }
                if (m.IsOne)
                    return result;
                if (!n.IsEven)
                {
                    if ((int)(m % four) == 3 && (int)(n % four) == 3)
                        result *= -1;
                    var tmp = m;
                    m = n;
                    n = tmp;
                }
            }
        }

        public static int JacobiSymbol(int m, int n)
        {
            int result = 1;
            while (true)
            {
                m = m % n;
                if (m == 0)
                    return 0;
                if ((m & 1) == 0)
                {
                    int k = n & 7;
                    int toggle = k == 1 || k == 7 ? 1 : -1;
                    do
                    {
                        m >>= 1;
                        result *= toggle;
                    } while ((m & 1) == 0);
                }
                if (m == 1)
                    return result;
                if ((n & 1) != 0)
                {
                    if ((m & 3) == 3 && (n & 3) == 3)
                        result *= -1;
                    var tmp = m;
                    m = n;
                    n = tmp;
                }
            }
        }
    }
}
