using System.Numerics;
using System.Diagnostics;

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

        public static ulong ModularPower(ulong value, ulong exponent, ulong modulus)
        {
            if (modulus <= uint.MaxValue)
                return ModularPower((uint)(value % modulus), exponent, (uint)modulus);
            return ModularPowerReduction(1, value, exponent, modulus);
        }

        public static BigInteger ModularPower(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            return BigInteger.ModPow(value, exponent, modulus);
        }

        public static ulong ModularPowerOfTwo(ulong exponent, ulong modulus)
        {
            if (exponent < 64)
                return ((ulong)1 << (int)exponent) % modulus;

            // Handle the first six bits, which we know won't overflow.
            var value = ulong.MaxValue % modulus + 1;
            var result = ((ulong)1 << (int)(exponent & 63));
            exponent >>= 6;

#if false
            if (modulus <= uint.MaxValue)
                return ModularProduct((uint)(result % modulus), ModularPower((uint)value, exponent, (uint)modulus), (uint)modulus);
#else
            if (modulus <= uint.MaxValue)
            {
                if (exponent <= uint.MaxValue)
                    return ModularPowerReduction((uint)(result % modulus), (uint)value, (uint)exponent, (uint)modulus);
                return ModularProduct((uint)(result % modulus), ModularPower((uint)value, exponent, (uint)modulus), (uint)modulus);
            }
#endif
            return ModularPowerReduction(result, value, exponent, modulus);
        }

        public static uint ModularPowerPowerOfTwoModulus(uint value, ulong exponent, int n)
        {
            Debug.Assert(value > 0 && n > 0 && n <= 32);
            return (uint)ModularPowerPowerOfTwoModulus((ulong)value, exponent, n);
        }

        public static ulong ModularPowerPowerOfTwoModulus(ulong value, ulong exponent, int n)
        {
            Debug.Assert(value > 0 && n > 0 && n <= 64);
            var mask = n == 64 ? ulong.MaxValue : ((ulong)1 << n) - 1;

            // Remove factors of two from value.
            var s = 0;
            while ((value & 1) == 0)
            {
                value >>= 1;
                ++s;
            }

            // Reduce exponent since value and 2^n are relatively prime
            // and phi(2^n) = 2^(n-1).
            exponent &= mask >> 1;

            // Compute value ^ exponent % 2^n, ignoring overflow.
            var result = (ulong)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result *= value;
                if (exponent != 1)
                    value *= value;
                exponent >>= 1;
            }

            // Result is result ^ 2^s % 2^n.
            return (result << s) & mask;
        }

        private static uint ModularPowerReduction(uint start, uint value, uint exponent, uint modulus)
        {
            if ((modulus & 1) != 0)
                return ModularPowerReductionOdd(start, value, exponent, modulus);

            // See: http://cs.ucsb.edu/~koc/docs/j34.pdf
            Debug.Assert(modulus > 0);
            var s = 0;
            var modulusOdd = modulus;
            while ((modulusOdd & 1) == 0)
            {
                modulusOdd >>= 1;
                ++s;
            }
            var result1 = ModularPowerReductionOdd(start, value, exponent, modulusOdd);
            var result2 = ModularPowerPowerOfTwoModulus(value, exponent, s);
            var modulusOddInv = ModularInversePowerOfTwoModulus(modulusOdd, s);
            var factor = ((result2 - result1) * modulusOddInv) & (((uint)1 << s) - 1);
            var result = result1 + modulusOdd * factor;
            Debug.Assert(result < modulus);
            return result;
        }

        private static IReductionAlgorithm<uint> reductionUInt32 = new UInt32MontgomeryReduction();

        private static uint ModularPowerReductionOdd(uint start, uint value, uint exponent, uint modulus)
        {
            var reducer = reductionUInt32.GetReducer(modulus);
            return ModularProduct(start, reducer.ToResidue(value).Power(exponent).Value, modulus);
        }

        private static ulong ModularPowerReduction(ulong start, ulong value, ulong exponent, ulong modulus)
        {
            if ((modulus & 1) != 0)
                return ModularPowerReductionOdd(start, value, exponent, modulus);

            // See: http://cs.ucsb.edu/~koc/docs/j34.pdf
            Debug.Assert(modulus > 0);
            var s = 0;
            var modulusOdd = modulus;
            while ((modulusOdd & 1) == 0)
            {
                modulusOdd >>= 1;
                ++s;
            }
            var result1 = ModularPowerReductionOdd(start, value, exponent, modulusOdd);
            var result2 = ModularPowerPowerOfTwoModulus(value, exponent, s);
            var modulusOddInv = ModularInversePowerOfTwoModulus(modulusOdd, s);
            var factor = ((result2 - result1) * modulusOddInv) & (((ulong)1 << s) - 1);
            var result = result1 + modulusOdd * factor;
            Debug.Assert(result < modulus);
            return result;
        }

        private static IReductionAlgorithm<ulong> reductionUInt64 = new UInt64MontgomeryReduction();

        private static ulong ModularPowerReductionOdd(ulong start, ulong value, ulong exponent, ulong modulus)
        {
            var reducer = reductionUInt64.GetReducer(modulus);
            return reducer.ToResidue(value).Power(exponent).Multiply(reducer.ToResidue(start)).Value;
        }
    }
}
