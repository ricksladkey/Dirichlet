using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int ModularSum(int a, int b, int modulus)
        {
            var sum = (uint)a + (uint)b;
            if (sum >= (uint)modulus)
                sum -= (uint)modulus;
            return (int)sum;
        }

        public static uint ModularSum(uint a, uint b, uint modulus)
        {
            var sum = (ulong)a + (ulong)b;
            if (sum >= (ulong)modulus)
                sum -= (ulong)modulus;
            return (uint)sum;
        }

        public static long ModularSum(long a, long b, long modulus)
        {
            var sum = (ulong)a + (ulong)b;
            if (sum >= (ulong)modulus)
                sum -= (ulong)modulus;
            return (long)sum;
        }

        public static ulong ModularSum(ulong a, ulong b, ulong modulus)
        {
            return UInt128.ModularSum(a, b, modulus);
        }

        public static BigInteger ModularSum(BigInteger a, BigInteger b, BigInteger modulus)
        {
            var sum = a + b;
            return sum >= modulus ? sum - modulus : sum;
        }
    }
}
