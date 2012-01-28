using System.Numerics;

namespace Decompose.Numerics
{
    public struct UInt128
    {
        private uint r0;
        private uint r1;
        private uint r2;
        private uint r3;
        public static UInt128 Parse(string value)
        {
            return (UInt128)BigInteger.Parse(value);
        }
        public override string ToString()
        {
            return ((BigInteger)this).ToString();
        }
        public int GetBitLength()
        {
            if (r3 != 0)
                return r3.GetBitLength() + 96;
            if (r2 != 0)
                return r2.GetBitLength() + 64;
            if (r1 != 0)
                return r1.GetBitLength() + 32;
            return r0.GetBitLength();
        }
        public int GetBitCount()
        {
            return r0.GetBitCount() + r1.GetBitCount() + r2.GetBitCount() + r3.GetBitCount();
        }
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
            return (BigInteger)((ulong)r.r3 << 32 | r.r2) << 64 | (ulong)r.r1 << 32 | r.r0;
        }
        public static UInt128 operator <<(UInt128 r, int n)
        {
            UInt128 w;
            LeftShift(out w, ref r, n);
            return w;
        }
        public static UInt128 operator *(UInt128 u, UInt128 v)
        {
            return Multiply(u.r0, u.r1, v.r0, v.r1);
        }
        public static ulong operator %(UInt128 u, ulong v)
        {
            return Modulus(ref u, v);
        }
        public static ulong ModularProduct(ulong a, ulong b, ulong modulus)
        {
            var product = Multiply((uint)a, (uint)(a >> 32), (uint)b, (uint)(b >> 32));
            return Modulus(ref product, modulus);
        }
        public static ulong ModularPower(ulong value, ulong exponent, ulong modulus)
        {
            return ModularPower(value, exponent, 1, modulus);
        }
        private static ulong ModularPower(ulong b, ulong e, ulong p, ulong modulus)
        {
            if (e == 0)
                return p;
            if ((e & 1) == 0)
                return ModularPower(ModularProduct(b, b, modulus), e >> 1, p, modulus);
            return ModularPower(b, e - 1, ModularProduct(b, p, modulus), modulus);
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
            var dneg = ((uint)(v >> 32)).GetBitLength();
            var d = 32 - dneg;
            var vPrime = v << d;
            var v1 = (uint)(vPrime >> 32);
            var v2 = (uint)vPrime;
            var r0 = u.r0 << d;
            var r1 = u.r1 << d | u.r0 >> dneg;
            var r2 = u.r2 << d | u.r1 >> dneg;
            var r3 = u.r2 >> dneg;
            ModulusStep(ref r3, ref r2, ref r1, v1, v2);
            ModulusStep(ref r2, ref r1, ref r0, v1, v2);
            return ((ulong)r1 << 32 | r0) >> d;
        }
        private static ulong Modulus128(ref UInt128 u, ulong v)
        {
            var dneg = ((uint)(v >> 32)).GetBitLength();
            var d = 32 - dneg;
            var vPrime = v << d;
            var v1 = (uint)(vPrime >> 32);
            var v2 = (uint)vPrime;
            var r0 = u.r0 << d;
            var r1 = u.r1 << d | u.r0 >> dneg;
            var r2 = u.r2 << d | u.r1 >> dneg;
            var r3 = u.r3 << d | u.r2 >> dneg;
            var r4 = u.r3 >> dneg;
            ModulusStep(ref r4, ref r3, ref r2, v1, v2);
            ModulusStep(ref r3, ref r2, ref r1, v1, v2);
            ModulusStep(ref r2, ref r1, ref r0, v1, v2);
            return ((ulong)r1 << 32 | r0) >> d;
        }
        private static uint LeftShift(out UInt128 w, ref UInt128 u, int d)
        {
            var dneg = 32 - d;
            w.r0 = u.r0 << d;
            w.r1 = u.r1 << d | u.r0 >> dneg;
            w.r2 = u.r2 << d | u.r1 >> dneg;
            w.r3 = u.r3 << d | u.r2 >> dneg;
            return u.r3 >> dneg;
        }
        private static void ModulusStep(ref uint u0, ref uint u1, ref uint u2, uint v1, uint v2)
        {
            var u0u1 = (ulong)u0 << 32 | u1;
            var qhat = u0 == v1 ? uint.MaxValue : u0u1 / v1;
            var r = u0u1 - qhat * v1;
            if (r == (uint)r && v2 * qhat > (r << 32 | u2))
            {
                --qhat;
                r = u0u1 - qhat * v1;
                if (r == (uint)r && v2 * qhat > (r << 32 | u2))
                    --qhat;
            }
            var carry = qhat * v2;
            var borrow = (long)u2 - (uint)carry;
            carry >>= 32;
            u2 = (uint)borrow;
            borrow >>= 32;
            carry += qhat * v1;
            borrow += (long)u1 - (uint)carry;
            carry >>= 32;
            u1 = (uint)borrow;
            borrow >>= 32;
            borrow += (long)u0 - (uint)carry;
            u0 = 0;
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
