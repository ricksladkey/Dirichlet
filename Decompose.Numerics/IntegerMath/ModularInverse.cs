using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int ModularInverse(int a, int b)
        {
            Debug.Assert(GreatestCommonDivisor(a, b) == 1);
            int x;
            int y;
            ExtendedGreatestCommonDivisor(a, b, out x, out y);
            if (x < 0)
                x += b;
            Debug.Assert((BigInteger)a * x % b == 1);
            return x;
        }

        public static uint ModularInverse(uint a, uint b)
        {
            return (uint)ModularInverse((int)a, (int)b);
        }

        public static long ModularInverse(long a, long b)
        {
            Debug.Assert(GreatestCommonDivisor(a, b) == 1);
            long x;
            long y;
            ExtendedGreatestCommonDivisor(a, b, out x, out y);
            if (x < 0)
                x += b;
            Debug.Assert((BigInteger)a * x % b == 1);
            return x;
        }

        public static ulong ModularInverse(ulong a, ulong b)
        {
            Debug.Assert(GreatestCommonDivisor(a, b) == 1);
            var x0 = (Int65)0;
            var x1 = (Int65)1;
            var p = b;
            var q = a < b ? a : a % b;
            var tmpx = (Int65)0;
            while (q != 0)
            {
                Debug.Assert(p >= q);
                var quotient = p / q;
                var tmpp = p;
                p = q;
                q = tmpp - quotient * q;
                tmpx = x1;
                x1.Multiply(quotient);
                x1.SetDifference(ref x0, ref x1);
                x0 = tmpx;
            }
            if (x0.Sign != 1)
                x0 += b;
            var result = (ulong)x0;
            Debug.Assert((BigInteger)a * result % b == 1);
            return result;
        }

        public static int ModularInverse(BigInteger a, int b)
        {
            Debug.Assert(GreatestCommonDivisor(a, b) == 1);
            if (b == 0)
                return 1;
            int r = (int)(a % b);
            int x;
            int y;
            ExtendedGreatestCommonDivisor(r, b, out x, out y);
            if (x < 0)
                x += b;
            Debug.Assert((BigInteger)a * x % b == 1);
            return x;
        }

        public static BigInteger ModularInverse(BigInteger a, BigInteger b)
        {
            Debug.Assert(GreatestCommonDivisor(a, b) == 1);
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

        public static int ModularInverseTwoToTheN(int d, int n)
        {
            return (int)ModularInverseTwoToTheN((uint)d, n);
        }

        public static uint ModularInverseTwoToTheN(uint d, int n)
        {
            var dInv = d;
            for (int m = 3; m < n; m *= 2)
                dInv = dInv * (2 - dInv * d);
            if (n < 32)
                dInv &= ((uint)1 << n) - 1;
            return dInv;
        }

        public static long ModularInverseTwoToTheN(long d, int n)
        {
            return (long)ModularInverseTwoToTheN((ulong)d, n);
        }

        public static ulong ModularInverseTwoToTheN(ulong d, int n)
        {
            var dInv = d;
            for (int m = 3; m < n; m *= 2)
                dInv = dInv * (2 - dInv * d);
            if (n < 64)
                dInv &= ((ulong)1 << n) - 1;
            return dInv;
        }

        public static BigInteger ModularInverseTwoToTheN(BigInteger d, int n)
        {
            var dInv = d;
            var mask = ((BigInteger)1 << n) - 1;
            for (int m = 3; m < n; m *= 2)
                dInv = (dInv * (2 - dInv * d)) & mask;
            return dInv;
        }
    }
}
