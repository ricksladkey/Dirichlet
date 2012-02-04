using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int ModularDifference(int a, int b, int modulus)
        {
            return a < b ? a + modulus - b : a - b;
        }

        public static uint ModularDifference(uint a, uint b, uint modulus)
        {
            return a < b ? (uint)((ulong)a + modulus - b) : a - b;
        }

        public static long ModularDifference(long a, long b, long modulus)
        {
            return a < b ? (long)((ulong)a + (ulong)modulus - (ulong)b) : a - b;
        }

        public static ulong ModularDifference(ulong a, ulong b, ulong modulus)
        {
            return UInt128.ModularDifference(a, b, modulus);
        }

        public static BigInteger ModularDifference(BigInteger a, BigInteger b, BigInteger modulus)
        {
            return a < b ? a + modulus - b : a - b;
        }
    }
}
