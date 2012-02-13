using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int Power(int value, int exponent)
        {
            var result = (int)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = result * value;
                if (exponent != 1)
                    value = value * value;
                exponent >>= 1;
            }
            return result;
        }

        public static uint Power(uint value, uint exponent)
        {
            var result = (uint)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = result * value;
                if (exponent != 1)
                    value = value * value;
                exponent >>= 1;
            }
            return result;
        }

        public static long Power(long value, long exponent)
        {
            var result = (long)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = result * value;
                if (exponent != 1)
                    value = value * value;
                exponent >>= 1;
            }
            return result;
        }

        public static ulong Power(ulong value, ulong exponent)
        {
            var result = (ulong)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = result * value;
                if (exponent != 1)
                    value = value * value;
                exponent >>= 1;
            }
            return result;
        }

        public static BigInteger Power(BigInteger value, BigInteger exponent)
        {
            if (exponent < int.MaxValue)
                return BigInteger.Pow(value, (int)exponent);
            var result = (BigInteger)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = result * value;
                if (exponent != 1)
                    value = value * value;
                exponent >>= 1;
            }
            return result;
        }

        public static Rational Power(Rational value, Rational exponent)
        {
            if (value.IsInteger && exponent.IsInteger)
                return Power((BigInteger)value, (BigInteger)exponent);
            if (value.IsInteger && exponent.Numerator == 1)
                return Root((BigInteger)value, (BigInteger)exponent.Denominator);
            throw new NotImplementedException("rational power not implemented");
        }
    }
}
