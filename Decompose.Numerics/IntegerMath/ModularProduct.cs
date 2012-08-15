using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int ModularProduct(int a, int b, int modulus)
        {
            return (int)((long)a * b % modulus);
        }

        public static uint ModularProduct(uint a, uint b, uint modulus)
        {
            return (uint)((ulong)a * b % modulus);
        }

        public static long ModularProduct(long a, long b, long modulus)
        {
            var result = (long)UInt64Helper.ModularProduct((ulong)Math.Abs(a), (ulong)Math.Abs(b), (ulong)modulus);
            return (a < 0) != (b < 0) ? -result : result;
        }

        public static ulong ModularProduct(ulong a, ulong b, ulong modulus)
        {
            return UInt64Helper.ModularProduct(a, b, modulus);
        }

        public static BigInteger ModularProduct(BigInteger a, BigInteger b, BigInteger modulus)
        {
            return a * b % modulus;
        }
    }
}
