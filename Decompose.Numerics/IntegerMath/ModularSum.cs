using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int ModularSum(int a, int b, int modulus)
        {
            Debug.Assert(modulus > 0 && a >= 0 && a < modulus && b >= 0 && b < modulus);
            var sum = (uint)a + (uint)b;
            if (sum >= (uint)modulus)
                sum -= (uint)modulus;
            return (int)sum;
        }

        public static uint ModularSum(uint a, uint b, uint modulus)
        {
            Debug.Assert(modulus > 0 && a >= 0 && a < modulus && b >= 0 && b < modulus);
            var sum = (ulong)a + (ulong)b;
            if (sum >= (ulong)modulus)
                sum -= (ulong)modulus;
            return (uint)sum;
        }

        public static long ModularSum(long a, long b, long modulus)
        {
            Debug.Assert(modulus > 0 && a >= 0 && a < modulus && b >= 0 && b < modulus);
            var sum = (ulong)a + (ulong)b;
            if (sum >= (ulong)modulus)
                sum -= (ulong)modulus;
            return (long)sum;
        }

        public static ulong ModularSum(ulong a, ulong b, ulong modulus)
        {
            Debug.Assert(modulus > 0 && a >= 0 && a < modulus && b >= 0 && b < modulus);
            return UInt64Helper.ModularSum(a, b, modulus);
        }

        public static BigInteger ModularSum(BigInteger a, BigInteger b, BigInteger modulus)
        {
            Debug.Assert(modulus > 0 && a >= 0 && a < modulus && b >= 0 && b < modulus);
            var sum = a + b;
            return sum >= modulus ? sum - modulus : sum;
        }
    }
}
