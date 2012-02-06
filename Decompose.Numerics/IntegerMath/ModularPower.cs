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
                return ModularPowerEven(value, exponent, modulus);
            return ModularPowerReduction(1, value, exponent, modulus);
        }

        private static ulong ModularPowerEven(ulong value, ulong exponent, ulong modulus)
        {
            var s = 0;
            var modulusOdd = modulus;
            while ((modulusOdd & 1) == 0)
            {
                modulusOdd >>= 1;
                ++s;
            }
            var result1 = ModularPowerReduction(1, value, exponent, modulusOdd);
            var result2 = ModularPowerTwoToTheN(value, exponent, s);
            var modulusOddInv = ModularInverseTwoToTheN(modulusOdd, s);
            var factor = ((result2 - result1) * modulusOddInv) & (((ulong)1 << s) - 1);
            var result = result1 + modulusOdd * factor;
            return result;
        }

        private static ulong ModularPowerTwoToTheN(ulong value, ulong exponent, int n)
        {
            var mask = ((ulong)1 << n) - 1;
            var result = (ulong)1;
#if false
            exponent &= mask >> 1;
#endif
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = (result * value) & mask;
                if (exponent != 1)
                    value = (value * value) & mask;
                exponent >>= 1;
            }
            return result;
        }

        private static ulong ModularPowerReduction(ulong start, ulong value, ulong exponent, ulong modulus)
        {
            var reducer = reduction.GetReducer(modulus);
            var b = reducer.ToResidue(value);
            var result = reducer.ToResidue(start);
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

        public static ulong ModularPowerOfTwo(ulong exponent, ulong modulus)
        {
            if (exponent < 64)
                return ((ulong)1 << (int)exponent) % modulus;
            var value = ulong.MaxValue % modulus + 1;
            var result = ((ulong)1 << (int)(exponent & 63));
            exponent >>= 6;
            if (modulus <= uint.MaxValue)
                return ModularProduct(result, ModularPower(value, exponent, (uint)modulus), modulus);
            if ((modulus & 1) == 0)
                return ModularProduct(result, ModularPowerEven(value, exponent, modulus), modulus);
            return ModularPowerReduction(result, value, exponent, modulus);
        }
    }
}
