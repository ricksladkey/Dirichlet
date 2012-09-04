using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public struct Int128 : IComparable<Int128>, IEquatable<Int128>
    {
        private UInt128 v;

        private Int128(UInt128 a)
        {
            v = a;
        }

        public uint R0 { get { return v.R0; } }
        public uint R1 { get { return v.R1; } }
        public uint R2 { get { return v.R2; } }
        public uint R3 { get { return v.R3; } }

        public ulong S0 { get { return v.S0; } }
        public ulong S1 { get { return v.S1; } }

        public bool IsZero { get { return (v.S0 | v.S1) == 0; } }
        public bool IsOne { get { return (v.S1 ^ v.S0) == 1; } }
        public bool IsPowerOfTwo { get { return (v & (v - 1)).IsZero; } }
        public bool IsEven { get { return (v.R0 & 1) == 0; } }
        public uint LeastSignificantWord { get { return v.R0; } }
        public bool IsNegative { get { return v.R3 >= int.MaxValue; } }
        public Int128 TwosComplement { get { return new Int128(v.TwosComplement); } }

        public int GetBitLength()
        {
            return v.GetBitLength();
        }

        public int GetBitCount()
        {
            return v.GetBitCount();
        }

        public override string ToString()
        {
            return ((BigInteger)this).ToString();
        }

        public static implicit operator Int128(int a)
        {
            if (a < 0)
                return new Int128(new UInt128((ulong)(long)a, ulong.MaxValue));
            return new Int128(new UInt128((ulong)a, 0));
        }

        public static implicit operator Int128(uint a)
        {
            return new Int128(new UInt128(a, 0, 0, 0));
        }

        public static implicit operator Int128(long a)
        {
            if (a < 0)
                return new Int128(new UInt128((ulong)a, ulong.MaxValue));
            return new Int128(new UInt128((ulong)a, 0));
        }

        public static implicit operator Int128(ulong a)
        {
            return new Int128(new UInt128(a, 0));
        }

        public static explicit operator Int128(UInt128 a)
        {
            return new Int128(a);
        }

        public static explicit operator UInt128(Int128 a)
        {
            return a.v;
        }

        public static explicit operator Int128(BigInteger a)
        {
            return new Int128(a < 0 ? ((UInt128)(-a)).TwosComplement : (UInt128)a);
        }

        public static implicit operator BigInteger(Int128 a)
        {
            return a.IsNegative ? -(BigInteger)a.v.TwosComplement : (BigInteger)a.v;
        }

        public static explicit operator double(Int128 a)
        {
            return a.IsNegative ? -(double)a.v.TwosComplement : (double)a.v;
        }

        public static Int128 operator <<(Int128 a, int b)
        {
            return new Int128(a.v << b);
        }

        public static Int128 operator >>(Int128 a, int b)
        {
            if (a.IsNegative)
                throw new NotImplementedException();
            return new Int128(a.v >> b);
        }

        public static Int128 operator &(Int128 a, Int128 b)
        {
            return new Int128(a.v & b.v);
        }

        public static int operator &(Int128 a, int b)
        {
            if (b < 0)
                throw new NotImplementedException();
            return (int)(a.v & (uint)b);
        }

        public static int operator &(int a, Int128 b)
        {
            return b & a;
        }

        public static long operator &(Int128 a, long b)
        {
            if (b < 0)
                throw new NotImplementedException();
            return (long)(a.v & (ulong)b);
        }

        public static long operator &(long a, Int128 b)
        {
            return b & a;
        }

        public static Int128 operator |(Int128 a, Int128 b)
        {
            return new Int128(a.v | b.v);
        }

        public static Int128 operator ^(Int128 a, Int128 b)
        {
            return new Int128(a.v ^ b.v);
        }

        public static Int128 operator ~(Int128 a)
        {
            return new Int128(~a.v);
        }

        public static Int128 operator +(Int128 a, int b)
        {
            if (b < 0)
                return new Int128(a.v - (uint)(-b));
            return new Int128(a.v + (uint)b);
        }

        public static Int128 operator +(int a, Int128 b)
        {
            if (a < 0)
                return new Int128(b.v - (uint)(-a));
            return new Int128(b.v + (uint)a);
        }

        public static Int128 operator +(Int128 a, long b)
        {
            if (b < 0)
                return new Int128(a.v - (ulong)(-b));
            return new Int128(a.v + (ulong)b);
        }

        public static Int128 operator +(long a, Int128 b)
        {
            if (a < 0)
                return new Int128(b.v - (ulong)(-a));
            return new Int128(b.v + (ulong)a);
        }

        public static Int128 operator +(Int128 a, Int128 b)
        {
            return new Int128(a.v + b.v);
        }

        public static Int128 operator ++(Int128 a)
        {
            return new Int128(++a.v);
        }

        public static Int128 operator -(Int128 a, int b)
        {
            if (b < 0)
                return new Int128(a.v + (uint)(-b));
            return new Int128(a.v - (uint)b);
        }

        public static Int128 operator -(Int128 a, long b)
        {
            if (b < 0)
                return new Int128(a.v + (ulong)(-b));
            return new Int128(a.v - (ulong)b);
        }

        public static Int128 operator -(Int128 a, Int128 b)
        {
            return new Int128(a.v - b.v);
        }

        public static Int128 operator --(Int128 a)
        {
            return new Int128(--a.v);
        }

        public static Int128 operator *(Int128 a, int b)
        {
            Int128 c;
            if (a.IsNegative)
            {
                if (b < 0)
                    c.v = a.v.TwosComplement * (uint)(-b);
                else
                    c.v = (a.v.TwosComplement * (uint)b).TwosComplement;
            }
            else
            {
                if (b < 0)
                    c.v = (a.v * (uint)(-b)).TwosComplement;
                else
                    c.v = a.v * (uint)b;
            }
            Debug.Assert((BigInteger)a * (BigInteger)b == (BigInteger)c);
            return c;
        }

        public static Int128 operator *(int a, Int128 b)
        {
            return b * a;
        }

        public static Int128 operator *(Int128 a, long b)
        {
            Int128 c;
            if (a.IsNegative)
            {
                if (b < 0)
                    c.v = a.v.TwosComplement * (ulong)(-b);
                else
                    c.v = (a.v.TwosComplement * (ulong)b).TwosComplement;
            }
            else
            {
                if (b < 0)
                    c.v = (a.v * (ulong)(-b)).TwosComplement;
                else
                    c.v = a.v * (ulong)b;
            }
            Debug.Assert((BigInteger)a * (BigInteger)b == (BigInteger)c);
            return c;
        }

        public static Int128 operator *(long a, Int128 b)
        {
            return b * a;
        }

        public static Int128 operator *(Int128 a, Int128 b)
        {
            Int128 c;
            if (a.IsNegative)
            {
                if (b.IsNegative)
                    c.v = a.v.TwosComplement * b.v.TwosComplement;
                else
                    c.v = (a.v.TwosComplement * b.v).TwosComplement;
            }
            else
            {
                if (b.IsNegative)
                    c.v = (a.v * b.v.TwosComplement).TwosComplement;
                else
                    c.v = a.v * b.v;
            }
            Debug.Assert((BigInteger)a * (BigInteger)b == (BigInteger)c);
            return c;
        }

        public static Int128 operator -(Int128 a)
        {
            return new Int128(a.v.TwosComplement);
        }

        public static bool operator <(Int128 a, Int128 b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(Int128 a, int b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(int a, Int128 b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <(Int128 a, uint b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(uint a, Int128 b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <(Int128 a, long b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(long a, Int128 b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <(Int128 a, ulong b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(ulong a, Int128 b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <=(Int128 a, Int128 b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(Int128 a, int b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(int a, Int128 b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator <=(Int128 a, uint b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(uint a, Int128 b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator <=(Int128 a, long b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(long a, Int128 b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator <=(Int128 a, ulong b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(ulong a, Int128 b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator >(Int128 a, Int128 b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(Int128 a, int b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(int a, Int128 b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >(Int128 a, uint b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(uint a, Int128 b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >(Int128 a, long b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(long a, Int128 b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >(Int128 a, ulong b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(ulong a, Int128 b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >=(Int128 a, Int128 b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(Int128 a, int b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(int a, Int128 b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator >=(Int128 a, uint b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(uint a, Int128 b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator >=(Int128 a, long b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(long a, Int128 b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator >=(Int128 a, ulong b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(ulong a, Int128 b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator ==(Int128 a, Int128 b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(Int128 a, ulong b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(ulong a, Int128 b)
        {
            return b.Equals(a);
        }

        public static bool operator !=(Int128 a, Int128 b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(Int128 a, ulong b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(ulong a, Int128 b)
        {
            return !b.Equals(a);
        }

        public int CompareTo(Int128 other)
        {
            if (IsNegative)
            {
                if (other.IsNegative)
                    return other.v.CompareTo(v);
                return -1;
            }
            if (other.IsNegative)
                return 1;
            return v.CompareTo(other.v);
        }

        public int CompareTo(int other)
        {
            return CompareTo((Int128)other);
        }

        public int CompareTo(uint other)
        {
            if (IsNegative)
                return -1;
            return v.CompareTo(other);
        }

        public int CompareTo(long other)
        {
            return CompareTo((Int128)other);
        }

        public int CompareTo(ulong other)
        {
            if (IsNegative)
                return -1;
            return v.CompareTo(other);
        }

        public bool Equals(Int128 other)
        {
            return v == other.v;
        }

        public bool Equals(ulong other)
        {
            return v == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Int128))
                return false;
            return Equals((Int128)obj);
        }

        public override int GetHashCode()
        {
            return v.S0.GetHashCode() ^ v.S1.GetHashCode();
        }
    }
}
