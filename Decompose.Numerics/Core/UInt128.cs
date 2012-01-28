using System.Numerics;

namespace Decompose.Numerics
{
    public struct UInt128
    {
        private uint r0;
        private uint r1;
        private uint r2;
        private uint r3;
        public static implicit operator UInt128(uint i)
        {
            return new UInt128 { r0 = i };
        }
        public static implicit operator UInt128(ulong i)
        {
            return new UInt128 { r0 = (uint)i, r1 = (uint)(i >> 32) };
        }
        public static explicit operator UInt128(BigInteger i)
        {
            return new UInt128
            {
                r0 = (uint)(i & uint.MaxValue),
                r1 = (uint)(i >> 32 & uint.MaxValue),
                r2 = (uint)(i >> 64 & uint.MaxValue),
                r3 = (uint)(i >> 96),
            };
        }
        public static explicit operator uint(UInt128 r)
        {
            return r.r0;
        }
        public static explicit operator ulong(UInt128 r)
        {
            return (ulong)r.r1 << 32 | r.r0;
        }
        public static implicit operator BigInteger(UInt128 r)
        {
            return ((BigInteger)((ulong)r.r3 << 32 | r.r2) << 64) | ((ulong)r.r1 << 32 | r.r0);
        }
        public static UInt128 operator <<(UInt128 r, int n)
        {
            return LeftShift(ref r, n);
        }
        public static UInt128 operator *(UInt128 u, UInt128 v)
        {
            return Multiply(u.r0, u.r1, v.r0, v.r1);
        }
        public static ulong operator %(UInt128 u, ulong v)
        {
            return Modulus(ref u, v);
        }
        public static ulong ModMul(ulong a, ulong b, ulong modulus)
        {
            var product = Multiply((uint)a, (uint)(a >> 32), (uint)b, (uint)(b >> 32));
            return Modulus(ref product, modulus);
        }
        public static ulong ModPow(ulong value, ulong exponent, ulong modulus)
        {
            return ModPow(value, exponent, 1, modulus);
        }
        private static ulong ModPow(ulong b, ulong e, ulong p, ulong modulus)
        {
            if (e == 0)
                return p;
            if ((e & 1) == 0)
                return ModPow(ModMul(b, b, modulus), e >> 1, p, modulus);
            return ModPow(b, e - 1, ModMul(b, p, modulus), modulus);
        }
        private static UInt128 Multiply(uint u0, uint u1, uint v0, uint v1)
        {
            var carry = (ulong)u0 * v0;
            var w0 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u0 * v1;
            var w1 = (uint)carry;
            carry >>= 32;
            var w2 = (uint)carry;
            carry = w1 + (ulong)u1 * v0;
            w1 = (uint)carry;
            carry >>= 32;
            carry += w2 + (ulong)u1 * v1;
            w2 = (uint)carry;
            carry >>= 32;
            var w3 = (uint)carry;
            return new UInt128 { r0 = w0, r1 = w1, r2 = w2, r3 = w3 };
        }
        private static ulong Modulus(ref UInt128 u, ulong v)
        {
            var v0 = (uint)v;
            if (v == v0)
            {
                if (u.r3 == 0)
                {
                    if (u.r2 == 0)
                        return (ulong)u % v;
                    return Modulus96(ref u, v0);
                }
                return Modulus128(ref u, v0);
            }
            if (u.r3 == 0)
            {
                if (u.r2 == 0)
                    return (ulong)u % v;
                return Modulus96(ref u, v);
            }
            return Modulus128(ref u, v);
        }
        private static ulong Modulus96(ref UInt128 u, uint v)
        {
            var u0 = (ulong)(u.r2 % v);
            var u0u1 = u0 << 32 | u.r1;
            u0 = u0u1 % v;
            u0u1 = u0 << 32 | u.r0;
            return u0u1 % v;
        }
        private static ulong Modulus128(ref UInt128 u, uint v)
        {
            var u0 = (ulong)(u.r3 % v);
            var u0u1 = u0 << 32 | u.r2;
            u0 = u0u1 % v;
            u0u1 = u0 << 32 | u.r1;
            u0 = u0u1 % v;
            u0u1 = u0 << 32 | u.r0;
            return u0u1 % v;
        }
        private static ulong Modulus96(ref UInt128 u, ulong v)
        {
            int d = 64 - v.GetBitLength();
            var vPrime = v << d;
            var v1 = (uint)(vPrime >> 32);
            var v2 = (uint)vPrime;
            UInt128 w = LeftShift(ref u, d);
            ModulusStep(ref w.r3, ref w.r2, ref w.r1, v1, v2);
            ModulusStep(ref w.r2, ref w.r1, ref w.r0, v1, v2);
            return ((ulong)w.r1 << 32 | w.r0) >> d;
        }
        private static ulong Modulus128(ref UInt128 u, ulong v)
        {
            int d = 64 - v.GetBitLength();
            var vPrime = v << d;
            var v1 = (uint)(vPrime >> 32);
            var v2 = (uint)vPrime;
            var r4 = (uint)0;
            UInt128 w = LeftShift(ref u, d);
            ModulusStep(ref r4, ref w.r3, ref w.r2, v1, v2);
            ModulusStep(ref w.r3, ref w.r2, ref w.r1, v1, v2);
            ModulusStep(ref w.r2, ref w.r1, ref w.r0, v1, v2);
            return ((ulong)w.r1 << 32 | w.r0) >> d;
        }
        private static UInt128 LeftShift(ref UInt128 u, int d)
        {
            int dneg = 32 - d;
            return new UInt128
            {
                r0 = u.r0 << d,
                r1 = u.r1 << d | u.r0 >> dneg,
                r2 = u.r2 << d | u.r1 >> dneg,
                r3 = u.r3 << d | u.r2 >> dneg,
            };
        }
        private static void ModulusStep(ref uint u0, ref uint u1, ref uint u2, uint v1, uint v2)
        {
            ulong u0u1 = (ulong)u0 << 32 | u1;
            ulong qhat = u0 == v1 ? (1ul << 32) - 1 : u0u1 / v1;
            ulong r = u0u1 - qhat * v1;
            if (r == (uint)r && v2 * qhat > (r << 32 | u2))
            {
                --qhat;
                r = u0u1 - qhat * v1;
                if (r == (uint)r && v2 * qhat > (r << 32 | u2))
                    --qhat;
            }
            ulong carry = qhat * v2;
            ulong borrow = (ulong)u2 - (uint)carry;
            carry >>= 32;
            u2 = (uint)borrow;
            borrow = (ulong)((long)borrow >> 32);
            carry += qhat * v1;
            borrow += (ulong)u1 - (uint)carry;
            carry >>= 32;
            u1 = (uint)borrow;
            borrow = (ulong)((long)borrow >> 32);
            borrow += u0 - carry;
            u0 = 0;
            borrow = (ulong)((long)borrow >> 32);
            if (borrow != 0)
            {
                --qhat;
                carry = (ulong)u2 + v2;
                u2 = (uint)carry;
                carry >>= 32;
                carry += (ulong)u1 + v1;
                u1 = (uint)carry;
            }
        }
    }
}
