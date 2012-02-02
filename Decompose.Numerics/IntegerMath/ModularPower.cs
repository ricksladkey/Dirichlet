using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int ModularPower(int value, int exponent, int modulus)
        {
            return (int)ModularPower((uint)value, (uint)exponent, (uint)modulus);
        }

        public static uint ModularPower(uint value, uint exponent, uint modulus)
        {
            var result = (uint)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = (uint)((ulong)result * value % modulus);
                if (exponent != 1)
                    value = (uint)((ulong)value * value % modulus);
                exponent >>= 1;
            }
            return result;
        }

        public static uint ModularPower(uint value, ulong exponent, uint modulus)
        {
            var result = (uint)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = (uint)((ulong)result * value % modulus);
                if (exponent != 1)
                    value = (uint)((ulong)value * value % modulus);
                exponent >>= 1;
            }
            return result;
        }

        public static long ModularPower(long value, long exponent, long modulus)
        {
            return (long)ModularPower((ulong)value, (ulong)exponent, (ulong)modulus);
        }

        private static IReductionAlgorithm<ulong> reduction = new UInt64MontgomeryReduction();

        public static ulong ModularPower(ulong value, ulong exponent, ulong modulus)
        {
            if (modulus <= uint.MaxValue)
                return ModularPower((uint)(value % modulus), exponent, (uint)modulus);
            if ((modulus & 1) == 0)
                return UInt128.ModularPower(value, exponent, modulus);
            var reducer = reduction.GetReducer(modulus);
            var b = reducer.ToResidue(value);
            var result = reducer.ToResidue(1);
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result.Multiply(b);
                if (exponent != 1)
                    b.Multiply(b);
                exponent >>= 1;
            }
            return result.Value();
        }

        public static BigInteger ModularPower(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            return BigInteger.ModPow(value, exponent, modulus);
        }
    }
}
