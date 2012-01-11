using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace Decompose.Numerics
{
    public static class IntegerMath
    {
        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static BigInteger Min(BigInteger a, BigInteger b)
        {
            return a < b ? a : b;
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static BigInteger Max(BigInteger a, BigInteger b)
        {
            return a > b ? a : b;
        }

        public static int QuotientFloor(int a, int b)
        {
            return a / b;
        }

        public static BigInteger QuotientFloor(BigInteger a, BigInteger b)
        {
            return a / b;
        }

        public static int QuotientCeiling(int a, int b)
        {
            return (a + b - 1) / b;
        }

        public static BigInteger QuotientCeiling(BigInteger a, BigInteger b)
        {
            return (a + b - 1) / b;
        }

        public static int MultipleOfFloor(int a, int b)
        {
            return a / b * b;
        }

        public static BigInteger MultipleOfFloor(BigInteger a, BigInteger b)
        {
            return a / b * b;
        }

        public static int MultipleOfCeiling(int a, int b)
        {
            return (a + b - 1) / b * b;
        }

        public static BigInteger MultipleOfCeiling(BigInteger a, BigInteger b)
        {
            return (a + b - 1) / b * b;
        }

        public static int Modulus(int n, int p)
        {
            return (n % p + p) % p;
        }

        public static BigInteger Modulus(BigInteger n, BigInteger p)
        {
            return (n % p + p) % p;
        }

        public static int ModularInverse(int n, int p)
        {
            int x;
            int y;
            ExtendedGreatestCommonDivisor(n, p, out x, out y);
            if (x < 0)
                x += p;
            return x;
        }

        public static BigInteger ModularInverse(BigInteger n, BigInteger p)
        {
            BigInteger x;
            BigInteger y;
            ExtendedGreatestCommonDivisor(n, p, out x, out y);
            if (x < 0)
                x += p;
            return x;
        }

        public static bool IsSquareFree(IEnumerable<int> factors)
        {
            return factors
                .OrderBy(factor => factor)
                .GroupBy(factor => factor)
                .All(grouping => grouping.Count() < 2);
        }

        public static bool IsSquareFree(IEnumerable<BigInteger> factors)
        {
            return factors
                .OrderBy(factor => factor)
                .GroupBy(factor => factor)
                .All(grouping => grouping.Count() < 2);
        }

        public static void ExtendedGreatestCommonDivisor(int a, int b, out int c, out int d)
        {
            var x = 0;
            var lastx = 1;
            var y = 1;
            var lasty = 0;

            while (b != 0)
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

        public static int Sqrt(int n)
        {
            return (int)Math.Floor(Math.Sqrt(n));
        }

        private static ISqrtAlgorithm<BigInteger> sqrt = new SqrtNewtonsMethod();

        public static BigInteger Sqrt(BigInteger n)
        {
            return sqrt.Sqrt(n);
        }

        private static IPrimalityAlgorithm<int> trialDivision = new TrialDivisionPrimality();

        public static bool IsPrime(int n)
        {
            return trialDivision.IsPrime(n);
        }

        private static IPrimalityAlgorithm<BigInteger> millerRabin = new MillerRabin(16);

        public static bool IsPrime(BigInteger n)
        {
            return millerRabin.IsPrime(n);
        }

        public static BigInteger NextPrime(BigInteger n)
        {
            while (!IsPrime(n))
                ++n;
            return n;
        }

        public static int GetDigitLength(BigInteger n, int b)
        {
            return (int)Math.Ceiling(BigInteger.Log(n, b));
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
                    int k = (int)(n % eight);
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

        public static bool IsQuadraticResidue(BigInteger n, BigInteger p)
        {
            return BigInteger.ModPow(n, (p - 1) / 2, p).IsOne;
        }

        public static int ModularSquareRoot(BigInteger n, int p)
        {
            return (int)ModularSquareRoot(n, (BigInteger)p);
        }

        public static BigInteger ModularSquareRoot(BigInteger n, BigInteger p)
        {
            var r = ModularSquareRootCore(n, p);
            if (r > p / BigIntegers.Two)
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
            var z = BigIntegers.Two;
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
                var b = BigInteger.ModPow(c, BigInteger.Pow(BigIntegers.Two, m - i - 1), p);
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
