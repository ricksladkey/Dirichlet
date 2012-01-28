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
        public static UInt128 operator *(UInt128 u, UInt128 v)
        {
            return Multiply(u.r0, u.r1, v.r0, v.r1);
        }
        public static ulong operator %(UInt128 u, ulong v)
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
            int d = 32 - u.r3.GetBitLength();
            //UInt128 uhat = LeftShfit(u, d);
            return 0;
        }
        private static ulong Modulus128(ref UInt128 u, ulong v)
        {
            return 0;
        }
    }
}
