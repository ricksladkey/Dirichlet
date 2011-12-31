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

        private static BigInteger limit = (BigInteger)uint.MaxValue;
        private static BigInteger four = (BigInteger)4;
        private static BigInteger eight = (BigInteger)8;

        public static int JacobiSymbol(BigInteger m, BigInteger n)
        {
            int result = 1;
            while (true)
            {
                m = m % n;
                if (n <= limit)
                    return result * JacobiSymbol((uint)m, (uint)n);
                if (m.IsZero)
                    return 0;
                if (m.IsEven)
                {
                    uint k = (uint)(n % eight);
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
                    if ((uint)(m % four) == 3 && (uint)(n % four) == 3)
                        result *= -1;
                    var tmp = m;
                    m = n;
                    n = tmp;
                }
            }
        }

        public static int JacobiSymbol(uint m, uint n)
        {
            int result = 1;
            while (true)
            {
                m = m % n;
                if (m == 0)
                    return 0;
                if ((m & 1) == 0)
                {
                    uint k = n & 7;
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

        public static bool IsQuadraticResidue(BigInteger n, BigInteger p)
        {
            return BigInteger.ModPow(n, (p - 1) / 2, p).IsOne;
        }

        public static BigInteger ModularSquareRoot(BigInteger n, BigInteger p)
        {
            var r = ModularSquareRootCore(n, p);
            if (r > p / BigIntegerUtils.Two)
                return p - r;
            return r;
        }

        private static BigInteger ModularSquareRootCore(BigInteger n, BigInteger p)
        {
            if (p == 2)
                return BigInteger.One;
            var q = p - 1;
            var s = 0;
            while (q.IsEven)
            {
                q >>= 1;
                ++s;
            }
            if (s == 1)
                return BigInteger.ModPow(n, (p + 1) / 4, p);
            var z = BigIntegerUtils.Two;
            while (JacobiSymbol(z, p) != -1)
                ++z;
            var c = BigInteger.ModPow(z, q, p);
            var r = BigInteger.ModPow(n, (q + 1) / 2, p);
            var t = BigInteger.ModPow(n, q, p);
            var m = s;
            while (!t.IsOne)
            {
                int i = 0;
                var k = t;
                while (!k.IsOne)
                {
                    k = k * k % p;
                    ++i;
                }
                var b = BigInteger.ModPow(c, BigInteger.Pow(BigIntegerUtils.Two, m - i - 1), p);
                r = r * b % p;
                var b2 = b * b % p;
                t = t * b2 % p;
                c = b2;
                m = i;
            }
            return r;
        }
    }
}
