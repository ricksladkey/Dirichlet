using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt64Helper
    {
        public static ulong ModularSum(ulong a, ulong b, ulong modulus)
        {
            var a0 = (uint)a;
            var a1 = (uint)(a >> 32);
            var b0 = (uint)b;
            var b1 = (uint)(b >> 32);
            var n0 = (uint)modulus;
            var n1 = (uint)(modulus >> 32);
            var carry = (ulong)a0 + b0;
            var c0 = (uint)carry;
            carry = (carry >> 32) + a1 + b1;
            var c1 = (uint)carry;
            if (carry > n1 || carry == n1 && c0 >= n0)
            {
                var borrow = (ulong)c0 - n0;
                c0 = (uint)borrow;
                borrow = (ulong)((long)borrow >> 32) + carry - n1;
                c1 = (uint)borrow;
            }
            var c = (ulong)c1 << 32 | c0;
            Debug.Assert(((BigInteger)a + b) % modulus == c);
            return c;
        }

        public static ulong ModularDifference(ulong a, ulong b, ulong modulus)
        {
            var a0 = (uint)a;
            var a1 = (uint)(a >> 32);
            var b0 = (uint)b;
            var b1 = (uint)(b >> 32);
            var n0 = (uint)modulus;
            var n1 = (uint)(modulus >> 32);
            var borrow = (ulong)a0 - b0;
            var c0 = (uint)borrow;
            borrow = (ulong)((long)borrow >> 32) + a1 - b1;
            var c1 = (uint)borrow;
            if (borrow >> 32 != 0)
            {
                var carry = (ulong)c0 + n0;
                c0 = (uint)carry;
                carry = (carry >> 32) + borrow + n1;
                c1 = (uint)carry;
            }
            var c = (ulong)c1 << 32 | c0;
            Debug.Assert(((BigInteger)a - b + modulus) % modulus == c);
            return c;
        }

        public static ulong ModularProduct(ulong a, ulong b, ulong modulus)
        {
            var c = UInt128.Multiply(a, b) % modulus;
            Debug.Assert((BigInteger)a * b % modulus == c);
            return c;
        }

        public static ulong ModularPower(ulong value, ulong exponent, ulong modulus)
        {
            var result = (ulong)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = ModularProduct(result, value, modulus);
                if (exponent != 1)
                    value = ModularProduct(value, value, modulus);
                exponent >>= 1;
            }
            return result;
        }

        public static ulong MultiplyHigh(ulong a, ulong b)
        {
#if false
            UInt128 c;
            Multiply(out c, (uint)a, (uint)(a >> 32), (uint)b, (uint)(b >> 32));
            return (ulong)c.r3 << 32 | c.r2;
#endif
#if true
            var u0 = (uint)a;
            var u1 = (uint)(a >> 32);
            var v0 = (uint)b;
            var v1 = (uint)(b >> 32);
            var carry = (((ulong)u0 * v0) >> 32) + (ulong)u0 * v1;
            return (((uint)carry + (ulong)u1 * v0) >> 32) + (carry >> 32) + (ulong)u1 * v1;
#endif
        }

        public static ulong MultiplyHighApprox(ulong a, ulong b)
        {
            var u0 = (uint)a;
            var u1 = (uint)(a >> 32);
            var v0 = (uint)b;
            var v1 = (uint)(b >> 32);
            var carry = (ulong)u0 * v1;
            return (((uint)carry + (ulong)u1 * v0) >> 32) + (carry >> 32) + (ulong)u1 * v1;
        }
    }
}
