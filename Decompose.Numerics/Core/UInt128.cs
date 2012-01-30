using System;
using System.Diagnostics;
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
        public static UInt128 operator *(ulong u, UInt128 v)
        {
            UInt128 result = default(UInt128);
            Multiply(ref result, (uint)u, (uint)(u >> 32), v.r0, v.r1);
            return result;
        }
        public static UInt128 operator *(UInt128 u, ulong v)
        {
            UInt128 result = default(UInt128);
            Multiply(ref result, u.r0, u.r1, (uint)v, (uint)(v >> 32));
            return result;
        }
        public static UInt128 operator *(UInt128 u, UInt128 v)
        {
            UInt128 result = default(UInt128);
            Multiply(ref result, u.r0, u.r1, v.r0, v.r1);
            return result;
        }
        public static UInt128 operator /(UInt128 u, ulong v)
        {
            UInt128 w = default(UInt128);
            Divide(ref w, ref u, v);
            return w;
        }
        public static UInt128 operator /(UInt128 u, UInt128 v)
        {
            UInt128 w = default(UInt128);
            Divide(ref w, ref u, (ulong)v);
            return w;
        }
        public static ulong operator %(UInt128 u, ulong v)
        {
            return Modulus(ref u, v);
        }
        public static UInt128 operator %(UInt128 u, UInt128 v)
        {
            return Modulus(ref u, (ulong)v.r1 << 32 | v.r0);
        }
        public static ulong ModularProduct(ulong a, ulong b, ulong modulus)
        {
            UInt128 result = default(UInt128);
            Multiply(ref result, (uint)a, (uint)(a >> 32), (uint)b, (uint)(b >> 32));
            return Modulus(ref result, modulus);
        }
        public static ulong ModularPower(ulong value, ulong exponent, ulong modulus)
        {
            var result = (ulong)1;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                    result = ModularProduct(result, value, modulus);
                value = ModularProduct(value, value, modulus);
                exponent >>= 1;
            }
            return result;
        }
        public static ulong Montgomery(ulong u, ulong v, ulong n, uint k0)
        {
            var u0 = (uint)u;
            var u1 = (uint)(u >> 32);
            var v0 = (uint)v;
            var v1 = (uint)(v >> 32);
            var n0 = (uint)n;
            var n1 = (uint)(n >> 32);

            var carry = (ulong)u0 * v0;
            var t0 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u1 * v0;
            var t1 = (uint)carry;
            carry >>= 32;
            var t2 = (uint)carry;

            var m = (ulong)(t0 * k0);
            carry = t0 + m * n0;
            carry >>= 32;
            carry += t1 + m * n1;
            t0 = (uint)carry;
            carry >>= 32;
            carry += t2;
            t1 = (uint)carry;
            carry >>= 32;
            t2 = (uint)carry;

            carry = t0 + (ulong)u0 * v1;
            t0 = (uint)carry;
            carry >>= 32;
            carry += t1 + (ulong)u1 * v1;
            t1 = (uint)carry;
            carry >>= 32;
            carry += t2;
            t2 = (uint)carry;
            carry >>= 32;
            var t3 = (uint)carry;

            m = (ulong)(t0 * k0);
            carry = t0 + m * n0;
            carry >>= 32;
            carry += t1 + m * n1;
            t0 = (uint)carry;
            carry >>= 32;
            carry += t2;
            t1 = (uint)carry;
            carry >>= 32;
            t2 = t3 + (uint)carry;

            if (t2 != 0)
            {
                var borrow = (ulong)t0 - n0;
                t0 = (uint)borrow;
                borrow = (ulong)((long)borrow >> 32);
                borrow += (ulong)t1 - n1;
                t1 = (uint)borrow;
                borrow = (ulong)((long)borrow >> 32);
            }
            var result = (ulong)t1 << 32 | t0;
            return result >= n ? result - n : result;
        }
        public static uint Montgomery(uint u0, uint v0, uint n0, uint k0)
        {
            var carry = (ulong)u0 * v0;
            var t0 = (uint)carry;
            carry >>= 32;
            var t1 = (uint)carry;

            var m = (ulong)(t0 * k0);
            carry = t0 + m * n0;
            carry >>= 32;
            carry += t1;
            t0 = (uint)carry;
            carry >>= 32;
            t1 = (uint)carry;

            var result = (ulong)t1 << 32 | t0;
            return (uint)(result >= n0 ? result - n0 : result);
        }
        private static void Multiply(ref UInt128 w, uint u0, uint u1, uint v0, uint v1)
        {
            var carry = (ulong)u0 * v0;
            w.r0 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u0 * v1;
            w.r1 = (uint)carry;
            carry >>= 32;
            w.r2 = (uint)carry;
            carry = w.r1 + (ulong)u1 * v0;
            w.r1 = (uint)carry;
            carry >>= 32;
            carry += w.r2 + (ulong)u1 * v1;
            w.r2 = (uint)carry;
            carry >>= 32;
            w.r3 = (uint)carry;
        }
        private static void Divide(ref UInt128 w, ref UInt128 u, ulong v)
        {
            var v0 = (uint)v;
            if (v == v0)
            {
                if (u.r3 == 0)
                {
                    if (u.r2 == 0)
                        w.Set((ulong)u / v);
                    else
                        Division96(ref w, ref u, v0);
                }
                else
                    Division128(ref w, ref u, v0);
            }
            else if (u.r3 == 0)
            {
                if (u.r2 == 0)
                    w.Set((ulong)u / v);
                else
                    Division96(ref w, ref u, v);
            }
            else
                Division128(ref w, ref u, v);
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
        private static void Division96(ref UInt128 w, ref UInt128 u, uint v)
        {
            w.r2 = u.r2 / v;
            var u0 = (ulong)(u.r2 - w.r2 * v);
            var u0u1 = u0 << 32 | u.r1;
            w.r1 = (uint)(u0u1 / v);
            u0 = u0u1 - w.r1 * v;
            u0u1 = u0 << 32 | u.r0;
            w.r0 = (uint)(u0u1 / v);
        }
        private static void Division128(ref UInt128 w, ref UInt128 u, uint v)
        {
            w.r3 = u.r3 / v;
            var u0 = (ulong)(u.r3 - w.r3 * v);
            var u0u1 = u0 << 32 | u.r2;
            w.r2 = (uint)(u0u1 / v);
            u0 = u0u1 - w.r2 * v;
            u0u1 = u0 << 32 | u.r1;
            w.r1 = (uint)(u0u1 / v);
            u0 = u0u1 - w.r1 * v;
            u0u1 = u0 << 32 | u.r0;
            w.r0 = (uint)(u0u1 / v);
        }
        private static void Division96(ref UInt128 w, ref UInt128 u, ulong v)
        {
            var dneg = ((uint)(v >> 32)).GetBitLength();
            var d = 32 - dneg;
            var vPrime = v << d;
            var v1 = (uint)(vPrime >> 32);
            var v2 = (uint)vPrime;
            var r0 = u.r0;
            var r1 = u.r1;
            var r2 = u.r2;
            var r3 = (uint)0;
            if (d != 0)
            {
                r3 = r2 >> dneg;
                r2 = r2 << d | r1 >> dneg;
                r1 = r1 << d | r0 >> dneg;
                r0 = r0 << d;
            }
            w.r1 = ModDiv(r3, ref r2, ref r1, v1, v2);
            w.r0 = ModDiv(r2, ref r1, ref r0, v1, v2);
        }
        private static void Division128(ref UInt128 w, ref UInt128 u, ulong v)
        {
            var dneg = ((uint)(v >> 32)).GetBitLength();
            var d = 32 - dneg;
            var vPrime = v << d;
            var v1 = (uint)(vPrime >> 32);
            var v2 = (uint)vPrime;
            var r0 = u.r0;
            var r1 = u.r1;
            var r2 = u.r2;
            var r3 = u.r3;
            var r4 = (uint)0;
            if (d != 0)
            {
                r4 = r3 >> dneg;
                r3 = r3 << d | r2 >> dneg;
                r2 = r2 << d | r1 >> dneg;
                r1 = r1 << d | r0 >> dneg;
                r0 = r0 << d;
            }
            w.r2 = ModDiv(r4, ref r3, ref r2, v1, v2);
            w.r1 = ModDiv(r3, ref r2, ref r1, v1, v2);
            w.r0 = ModDiv(r2, ref r1, ref r0, v1, v2);
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
            var r0 = u.r0;
            var r1 = u.r1;
            var r2 = u.r2;
            var r3 = (uint)0;
            if (d != 0)
            {
                r3 = r2 >> dneg;
                r2 = r2 << d | r1 >> dneg;
                r1 = r1 << d | r0 >> dneg;
                r0 = r0 << d;
            }
            ModDiv(r3, ref r2, ref r1, v1, v2);
            ModDiv(r2, ref r1, ref r0, v1, v2);
            return ((ulong)r1 << 32 | r0) >> d;
        }
        private static ulong Modulus128(ref UInt128 u, ulong v)
        {
            var dneg = ((uint)(v >> 32)).GetBitLength();
            var d = 32 - dneg;
            var vPrime = v << d;
            var v1 = (uint)(vPrime >> 32);
            var v2 = (uint)vPrime;
            var r0 = u.r0;
            var r1 = u.r1;
            var r2 = u.r2;
            var r3 = u.r3;
            var r4 = (uint)0;
            if (d != 0)
            {
                r4 = r3 >> dneg;
                r3 = r3 << d | r2 >> dneg;
                r2 = r2 << d | r1 >> dneg;
                r1 = r1 << d | r0 >> dneg;
                r0 = r0 << d;
            }
            ModDiv(r4, ref r3, ref r2, v1, v2);
            ModDiv(r3, ref r2, ref r1, v1, v2);
            ModDiv(r2, ref r1, ref r0, v1, v2);
            return ((ulong)r1 << 32 | r0) >> d;
        }
        private static uint ModDiv(uint u0, ref uint u1, ref uint u2, uint v1, uint v2)
        {
            var u0u1 = (ulong)u0 << 32 | u1;
            var qhat = u0 == v1 ? uint.MaxValue : u0u1 / v1;
            var r = u0u1 - qhat * v1;
            if (r == (uint)r && v2 * qhat > (r << 32 | u2))
            {
                --qhat;
                r += v1;
                if (r == (uint)r && v2 * qhat > (r << 32 | u2))
                {
                    --qhat;
                    r += v1;
                }
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
            if (borrow != 0)
            {
                --qhat;
                carry = (ulong)u2 + v2;
                u2 = (uint)carry;
                carry >>= 32;
                carry += (ulong)u1 + v1;
                u1 = (uint)carry;
            }
            return (uint)qhat;
        }
        private static void LeftShift(out UInt128 w, ref UInt128 u, int d)
        {
            if (d == 64)
            {
                w.r0 = 0;
                w.r1 = 0;
                w.r2 = u.r0;
                w.r3 = u.r1;
                return;
            }
            if (d == 32)
            {
                w.r0 = 0;
                w.r1 = u.r0;
                w.r2 = u.r1;
                w.r3 = u.r2;
                return;
            }
            if (d < 32)
            {
                var dneg = 32 - d;
                w.r0 = u.r0 << d;
                w.r1 = u.r1 << d | u.r0 >> dneg;
                w.r2 = u.r2 << d | u.r1 >> dneg;
                w.r3 = u.r3 << d | u.r2 >> dneg;
                return;
            }
            throw new NotImplementedException();
        }
        private void Set(ulong value)
        {
            r0 = (uint)value;
            r1 = (uint)(value >> 32);
        }
    }
}
