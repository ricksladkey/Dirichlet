using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int ModularInverse(int a, int b)
        {
            int x;
            int y;
            ExtendedGreatestCommonDivisor(a, b, out x, out y);
            if (x < 0)
                x += b;
            return x;
        }

        public static uint ModularInverse(uint a, uint b)
        {
            return (uint)ModularInverse((int)a, (int)b);
        }

        public static long ModularInverse(long a, long b)
        {
            long x;
            long y;
            ExtendedGreatestCommonDivisor(a, b, out x, out y);
            if (x < 0)
                x += b;
            return x;
        }

        public static ulong ModularInverse(ulong a, ulong b)
        {
            var x0 = (ulong)0;
            var x1 = (ulong)1;
            var p = b;
            var q = a < b ? a : a % b;
            while (q != 0)
            {
                Debug.Assert(p >= q);
                var quotient = p / q;
                var tmpp = p;
                p = q;
                q = tmpp - quotient * q;
                var tmpx = x1;
                var qx = ModularProduct(quotient, x1, b);
                if (x0 < qx)
                    x1 = x0 + (b - qx);
                else
                    x1 = x0 - qx;
                x0 = tmpx;
            }
            Debug.Assert((BigInteger)a * x0 % b == 1);
            return x0;
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
            var p = b;
            var q = a < b ? a : a % b;
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
