using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Decompose.Numerics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct UInt128 : IComparable<UInt128>, IEquatable<UInt128>
    {
        [FieldOffset(0)]
        private uint r0;
        [FieldOffset(4)]
        private uint r1;
        [FieldOffset(8)]
        private uint r2;
        [FieldOffset(12)]
        private uint r3;

        [FieldOffset(0)]
        private ulong s0;
        [FieldOffset(8)]
        private ulong s1;

        private static UInt128 maxValue = ~(UInt128)0;
        private static UInt128 zero = (UInt128)0;
        private static UInt128 one = (UInt128)1;

        public static UInt128 MaxValue
        {
            get { return maxValue; }
        }

        public static UInt128 Zero
        {
            get { return zero; }
        }

        public static UInt128 One
        {
            get { return one; }
        }

        public static UInt128 Parse(string value)
        {
            return (UInt128)BigInteger.Parse(value);
        }

        public UInt128(uint r0, uint r1, uint r2, uint r3)
        {
            this.s0 = this.s1 = 0;
            this.r0 = r0;
            this.r1 = r1;
            this.r2 = r2;
            this.r3 = r3;
        }

        public uint LeastSignificantWord
        {
            get { return r0; }
        }

        public uint R0 { get { return r0; } }
        public uint R1 { get { return r1; } }
        public uint R2 { get { return r2; } }
        public uint R3 { get { return r3; } }

        public ulong S0 { get { return s0; } }
        public ulong S1 { get { return s1; } }

        public bool IsZero { get { return (s0 | s1) == 0; } }
        public bool IsOne { get { return (s1 ^ s0) == 1; } }
        public bool IsPowerOfTwo { get { return (this & (this - 1)).IsZero; } }
        public bool IsEven { get { return (r0 & 1) == 0; } }

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
            return s0.GetBitCount() + s1.GetBitCount();
        }

        public static explicit operator UInt128(double a)
        {
            if (a < 0)
                throw new InvalidCastException();
            return (ulong)a;
        }

        public static explicit operator UInt128(int a)
        {
            if (a < 0)
                throw new InvalidCastException();
            var c = default(UInt128);
            c.r0 = (uint)a;
            return c;
        }

        public static implicit operator UInt128(uint a)
        {
            var c = default(UInt128);
            c.r0 = a;
            return c;
        }

        public static explicit operator UInt128(long a)
        {
            if (a < 0)
                throw new InvalidCastException();
            var c = default(UInt128);
            c.s0 = (ulong)a;
            return c;
        }

        public static implicit operator UInt128(ulong a)
        {
            var c = default(UInt128);
            c.s0 = a;
            return c;
        }

        public static explicit operator UInt128(BigInteger a)
        {
            var s0 = (ulong)(a & ulong.MaxValue);
            var s1 = (ulong)(a >> 64);
            var c = default(UInt128);
            c.s0 = s0;
            c.s1 = s1;
            return c;
        }

        public static explicit operator double(UInt128 a)
        {
            if (a.s1 == 0)
                return (ulong)a;
            var shift = a.GetBitLength() - 64;
            return (double)(ulong)(a >> shift) * ((long)1 << shift);
        }

        public static explicit operator int(UInt128 a)
        {
            return (int)a.r0;
        }

        public static explicit operator uint(UInt128 a)
        {
            return a.r0;
        }

        public static explicit operator long(UInt128 a)
        {
            return (long)a.s0;
        }

        public static explicit operator ulong(UInt128 a)
        {
            return a.s0;
        }

        public static implicit operator BigInteger(UInt128 a)
        {
            return (BigInteger)a.s1 << 64 | a.s0;
        }

        public static UInt128 operator <<(UInt128 a, int b)
        {
            UInt128 c;
            LeftShift(out c, ref a, b);
            return c;
        }

        public static UInt128 operator >>(UInt128 a, int b)
        {
            UInt128 c;
            RightShift(out c, ref a, b);
            return c;
        }

        public static UInt128 operator &(UInt128 a, UInt128 b)
        {
            UInt128 c;
            And(out c, ref a, ref b);
            return c;
        }

        public static UInt128 operator |(UInt128 a, UInt128 b)
        {
            UInt128 c;
            Or(out c, ref a, ref b);
            return c;
        }

        public static UInt128 operator ^(UInt128 a, UInt128 b)
        {
            UInt128 c;
            ExclusiveOr(out c, ref a, ref b);
            return c;
        }

        public static UInt128 operator ~(UInt128 a)
        {
            UInt128 c;
            Not(out c, ref a);
            return c;
        }

        public static UInt128 operator +(UInt128 a, UInt128 b)
        {
            UInt128 c;
            c.r0 = c.r1 = c.r2 = c.r3 = 0;
            c.s0 = a.s0 + b.s0;
            c.s1 = a.s1 + b.s1;
            if (c.s0 < a.s0 && c.s0 < b.s0)
                ++c.s1;
            return c;
        }

        public static UInt128 operator ++(UInt128 a)
        {
            UInt128 c;
            c.r0 = c.r1 = c.r2 = c.r3 = 0;
            c.s0 = a.s0 + 1;
            c.s1 = a.s1;
            if (a.s0 == uint.MaxValue)
                ++c.s1;
            return c;
        }

        public static UInt128 operator -(UInt128 a, UInt128 b)
        {
            UInt128 c;
            c.r0 = c.r1 = c.r2 = c.r3 = 0;
            c.s0 = a.s0 - b.s0;
            c.s1 = a.s1 - b.s1;
            if (a.s0 < b.s0)
                --c.s1;
            return c;
        }

        public static UInt128 operator --(UInt128 a)
        {
            UInt128 c;
            c.r0 = c.r1 = c.r2 = c.r3 = 0;
            c.s0 = a.s0 - 1;
            c.s1 = a.s1;
            if (a.s0 == 0)
                --c.s1;
            return c;
        }

        public static UInt128 operator *(ulong a, UInt128 b)
        {
            UInt128 c;
            if ((b.r3 | b.r2) != 0)
            {
                UInt128 aa = a;
                Multiply(out c, ref aa, ref b);
            }
            else
                Multiply(out c, (uint)a, (uint)(a >> 32), b.r0, b.r1);
            Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
            return c;
        }

        public static UInt128 operator *(UInt128 a, ulong b)
        {
            return b * a;
        }

        public static UInt128 operator *(UInt128 a, UInt128 b)
        {
            UInt128 c;
            if ((a.r2 | a.r3 | b.r2 | b.r3) != 0)
            {
                Multiply(out c, ref a, ref b);
                Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
            }
            else
                Multiply(out c, a.r0, a.r1, b.r0, b.r1);
            Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
            return c;
        }

        public static UInt128 operator /(UInt128 a, ulong b)
        {
            UInt128 c;
            Divide(out c, ref a, b);
            return c;
        }

        public static UInt128 operator /(UInt128 a, UInt128 b)
        {
            UInt128 c;
            Divide(out c, ref a, (ulong)b);
            return c;
        }

        public static ulong operator %(UInt128 a, ulong b)
        {
            return Modulus(ref a, b);
        }

        public static UInt128 operator %(UInt128 a, UInt128 b)
        {
            return Modulus(ref a, b.s0);
        }

        public static bool operator <(UInt128 a, UInt128 b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(UInt128 a, int b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(int a, UInt128 b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <(UInt128 a, uint b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(uint a, UInt128 b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <(UInt128 a, long b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(long a, UInt128 b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <(UInt128 a, ulong b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(ulong a, UInt128 b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <=(UInt128 a, UInt128 b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(UInt128 a, int b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(int a, UInt128 b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator <=(UInt128 a, uint b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(uint a, UInt128 b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator <=(UInt128 a, long b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(long a, UInt128 b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator <=(UInt128 a, ulong b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(ulong a, UInt128 b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator >(UInt128 a, UInt128 b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(UInt128 a, int b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(int a, UInt128 b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >(UInt128 a, uint b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(uint a, UInt128 b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >(UInt128 a, long b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(long a, UInt128 b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >(UInt128 a, ulong b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(ulong a, UInt128 b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >=(UInt128 a, UInt128 b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(UInt128 a, int b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(int a, UInt128 b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator >=(UInt128 a, uint b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(uint a, UInt128 b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator >=(UInt128 a, long b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(long a, UInt128 b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator >=(UInt128 a, ulong b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(ulong a, UInt128 b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator ==(UInt128 a, UInt128 b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(UInt128 a, ulong b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(ulong a, UInt128 b)
        {
            return b.Equals(a);
        }

        public static bool operator !=(UInt128 a, UInt128 b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(UInt128 a, ulong b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(ulong a, UInt128 b)
        {
            return !b.Equals(a);
        }

        public int CompareTo(UInt128 other)
        {
            if (s1 != other.s1)
                return s1.CompareTo(other.s1);
            return s0.CompareTo(other.s0);
        }

        public int CompareTo(int other)
        {
            if (s1 != 0 || other < 0)
                return 1;
            return ((uint)this).CompareTo((uint)other);
        }

        public int CompareTo(uint other)
        {
            if (s1 != 0)
                return 1;
            return ((uint)this).CompareTo(other);
        }

        public int CompareTo(long other)
        {
            if (s1 != 0 || other < 0)
                return 1;
            return ((ulong)this).CompareTo((ulong)other);
        }

        public int CompareTo(ulong other)
        {
            if (s1 != 0)
                return 1;
            return ((ulong)this).CompareTo(other);
        }

        public bool Equals(UInt128 other)
        {
            return s0 == other.s0 && s1 == other.s1;
        }

        public bool Equals(ulong other)
        {
            return s0 == other && s1 == 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UInt128))
                return false;
            return Equals((UInt128)obj);
        }

        public override int GetHashCode()
        {
            return s0.GetHashCode() ^ s1.GetHashCode();
        }

        public static UInt128 Multiply(ulong a, ulong b)
        {
            UInt128 c;
            Multiply(out c, (uint)a, (uint)(a >> 32), (uint)b, (uint)(b >> 32));
            return c;
        }

        public static UInt128 Double(UInt128 a)
        {
            UInt128 c = default(UInt128);
            c.s1 = a.s1 << 1 | a.s0 >> 63;
            c.s0 = a.r0 << 1;
            return c;
        }

        public static UInt128 Square(ulong a)
        {
            UInt128 c;
            Square(out c, (uint)a, (uint)(a >> 32));
            return c;
        }

        public static UInt128 Square(UInt128 a)
        {
            UInt128 c;
            Square(out c, a.r0, a.r1);
            return c;
        }

        private static void Add(out UInt128 w, ref UInt128 u, ref UInt128 v)
        {
            w.s0 = w.s1 = 0;
            var carry = (ulong)u.r0 + v.r0;
            w.r0 = (uint)carry;
            carry = (carry >> 32) + u.r1 + v.r1;
            w.r1 = (uint)carry;
            carry = (carry >> 32) + u.r2 + v.r2;
            w.r2 = (uint)carry;
            carry = (carry >> 32) + u.r3 + v.r3;
            w.r3 = (uint)carry;
        }

        private static void Subtract(out UInt128 w, ref UInt128 u, ref UInt128 v)
        {
            w.s0 = w.s1 = 0;
            var borrow = (ulong)u.r0 - v.r0;
            w.r0 = (uint)borrow;
            borrow = (ulong)((long)borrow >> 32) + u.r1 - v.r1;
            w.r1 = (uint)borrow;
            borrow = (ulong)((long)borrow >> 32) + u.r2 - v.r2;
            w.r2 = (uint)borrow;
            borrow = (ulong)((long)borrow >> 32) + u.r3 - v.r3;
            w.r3 = (uint)borrow;
        }

        private static void Square(out UInt128 w, uint u0, uint u1)
        {
            w.s0 = w.s1 = 0;
            var carry = (ulong)u0 * u0;
            w.r0 = (uint)carry;
            var u0u1 = (ulong)u0 * u1;
            carry = (carry >> 32) + u0u1;
            w.r1 = (uint)carry;
            w.r2 = (uint)(carry >> 32);
            carry = w.r1 + u0u1;
            w.r1 = (uint)carry;
            carry = (carry >> 32) + w.r2 + (ulong)u1 * u1;
            w.r2 = (uint)carry;
            w.r3 = (uint)(carry >> 32);
        }

        private static void Multiply(out UInt128 w, uint u0, uint u1, uint v0, uint v1)
        {
            w.s0 = w.s1 = 0;
            var carry = (ulong)u0 * v0;
            w.r0 = (uint)carry;
            carry = (carry >> 32) + (ulong)u0 * v1;
            w.r1 = (uint)carry;
            w.r2 = (uint)(carry >> 32);
            carry = w.r1 + (ulong)u1 * v0;
            w.r1 = (uint)carry;
            carry = (carry >> 32) + w.r2 + (ulong)u1 * v1;
            w.r2 = (uint)carry;
            w.r3 = (uint)(carry >> 32);
        }

        private static void Multiply(out UInt128 w, ref UInt128 u, ref UInt128 v)
        {
            w.s0 = w.s1 = 0;
            var u0 = u.r0;
            var u1 = u.r1;
            var u2 = u.r2;
            var v0 = v.r0;
            var v1 = v.r1;
            var v2 = v.r2;
            var carry = (ulong)u0 * v0;
            w.r0 = (uint)carry;
            carry = (carry >> 32) + (ulong)u0 * v1;
            w.r1 = (uint)carry;
            w.r2 = (uint)(carry >> 32);
            carry = w.r1 + (ulong)u1 * v0;
            w.r1 = (uint)carry;
            carry = (carry >> 32) + w.r2 + (ulong)u1 * v1 +
                (ulong)u2 * v0 + (ulong)u0 * v2 +
                (((ulong)u1 * v2 + (ulong)u2 * v1) << 32);
            w.r2 = (uint)carry;
            w.r3 = (uint)(carry >> 32);
        }

        private static void Divide(out UInt128 w, ref UInt128 u, ulong v)
        {
            var v0 = (uint)v;
            if (v == v0)
            {
                if (u.r3 == 0)
                {
                    if (u.r2 == 0)
                        Division64(out w, ref u, v);
                    else
                        Division96(out w, ref u, v0);
                }
                else
                    Division128(out w, ref u, v0);
            }
            else if (u.r3 == 0)
            {
                if (u.r2 == 0)
                    Division64(out w, ref u, v);
                else
                    Division96(out w, ref u, v);
            }
            else
                Division128(out w, ref u, v);
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

        private static void Division64(out UInt128 w, ref UInt128 u, ulong v)
        {
            w.s0 = w.s1 = 0;
            var q = (ulong)u / v;
            w.r3 = 0;
            w.r2 = 0;
            w.r1 = (uint)(q >> 32);
            w.r0 = (uint)q;
        }

        private static void Division96(out UInt128 w, ref UInt128 u, uint v)
        {
            w.s0 = w.s1 = 0;
            w.r3 = 0;
            w.r2 = u.r2 / v;
            var u0 = (ulong)(u.r2 - w.r2 * v);
            var u0u1 = u0 << 32 | u.r1;
            w.r1 = (uint)(u0u1 / v);
            u0 = u0u1 - w.r1 * v;
            u0u1 = u0 << 32 | u.r0;
            w.r0 = (uint)(u0u1 / v);
        }

        private static void Division128(out UInt128 w, ref UInt128 u, uint v)
        {
            w.s0 = w.s1 = 0;
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

        private static void Division96(out UInt128 w, ref UInt128 u, ulong v)
        {
            w.s0 = w.s1 = 0;
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
            w.r3 = 0;
            w.r2 = 0;
            w.r1 = ModDiv(r3, ref r2, ref r1, v1, v2);
            w.r0 = ModDiv(r2, ref r1, ref r0, v1, v2);
        }

        private static void Division128(out UInt128 w, ref UInt128 u, ulong v)
        {
            w.s0 = w.s1 = 0;
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
            w.r3 = 0;
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
            w.r0 = w.r1 = w.r2 = w.r3 = 0;
            var dneg = 64 - d;
            if (d < 64)
            {
                if (d == 0)
                {
                    w = u;
                    return;
                }
                w.s0 = u.s0 << d;
                w.s1 = u.s1 << d | u.s0 >> dneg;
                return;
            }
            if (d == 64)
            {
                w.s0 = 0;
                w.s1 = u.s0;
                return;
            }
            w.s0 = 0;
            w.s1 = u.s0 << (d - 64);
        }

        private static void RightShift(out UInt128 w, ref UInt128 u, int d)
        {
            w.r0 = w.r1 = w.r2 = w.r3 = 0;
            var dneg = 64 - d;
            if (d < 64)
            {
                if (d == 0)
                {
                    w = u;
                    return;
                }
                w.s0 = u.s0 >> d | u.s1 << dneg;
                w.s1 = u.s1 >> d;
                return;
            }
            if (d == 64)
            {
                w.s0 = u.s1;
                w.s1 = 0;
                return;
            }
            w.s0 = u.s1 >> (d - 64);
            w.s1 = 0;
        }

        private static void And(out UInt128 w, ref UInt128 u, ref UInt128 v)
        {
            w.r0 = w.r1 = w.r2 = w.r3 = 0;
            w.s0 = u.s0 & v.s0;
            w.s1 = u.s1 & v.s1;
        }

        private static void Or(out UInt128 w, ref UInt128 u, ref UInt128 v)
        {
            w.r0 = w.r1 = w.r2 = w.r3 = 0;
            w.s0 = u.s0 | v.s0;
            w.s1 = u.s1 | v.s1;
        }

        private static void ExclusiveOr(out UInt128 w, ref UInt128 u, ref UInt128 v)
        {
            w.r0 = w.r1 = w.r2 = w.r3 = 0;
            w.s0 = u.s0 ^ v.s0;
            w.s1 = u.s1 ^ v.s1;
        }

        private static void Not(out UInt128 w, ref UInt128 u)
        {
            w.r0 = w.r1 = w.r2 = w.r3 = 0;
            w.s0 = ~u.s0;
            w.s1 = ~u.s1;
        }
    }
}
