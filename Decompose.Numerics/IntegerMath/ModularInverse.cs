using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int ModularInverse(int n, int p)
        {
            int x;
            int y;
            ExtendedGreatestCommonDivisor(n, p, out x, out y);
            if (x < 0)
                x += p;
            return x;
        }

        public static uint ModularInverse(uint n, uint p)
        {
            return (uint)ModularInverse((int)n, (int)p);
        }

        public static long ModularInverse(long n, long p)
        {
            long x;
            long y;
            ExtendedGreatestCommonDivisor(n, p, out x, out y);
            if (x < 0)
                x += p;
            return x;
        }

        public static ulong ModularInverse(ulong n, ulong p)
        {
            return (ulong)ModularInverse((long)n, (long)p);
        }

        public static int ModularInverse(BigInteger n, int p)
        {
            if (p == 0)
                return 1;
            int r = (int)(n % p);
            int x;
            int y;
            ExtendedGreatestCommonDivisor(r, p, out x, out y);
            if (x < 0)
                x += p;
            return x;
        }

        public static BigInteger ModularInverse(BigInteger a, BigInteger b)
        {
            var x0 = BigInteger.Zero;
            var x1 = BigInteger.One;
            BigInteger p = b;
            BigInteger q = a < b ? a : a % b;
            ModularInverseCore(ref p, ref q, ref x0, ref x1);
            ModularInverseCore((ulong)p, (ulong)q, ref x0, ref x1);
            if (x0 < 0)
                x0 += b;
            Debug.Assert(a * x0 % b == 1);
            return x0;
        }

        private static void ModularInverseCore(ref BigInteger p, ref BigInteger q, ref BigInteger x0, ref BigInteger x1)
        {
            while (p > ulong.MaxValue)
            {
                Debug.Assert(p >= q);
                var quotient = p / q;
                var tmpp = p;
                p = q;
                q = tmpp - quotient * q;
                var tmpx = x1;
                x1 = x0 - quotient * x1;
                x0 = tmpx;
            }
        }

        private static void ModularInverseCore(ulong p, ulong q, ref BigInteger x0, ref BigInteger x1)
        {
            while (q != 0)
            {
                Debug.Assert(p >= q);
                var quotient = p / q;
                var tmpp = p;
                p = q;
                q = tmpp - quotient * q;
                var tmpx = x1;
                x1 = x0 - quotient * x1;
                x0 = tmpx;
            }
        }
    }
}
