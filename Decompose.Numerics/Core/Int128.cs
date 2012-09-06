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

        private static readonly Int128 minValue = (Int128)((UInt128)1 << 127);
        private static readonly Int128 maxValue = (Int128)((UInt128)1 << 127 - 1);
        private static readonly Int128 zero = (Int128)0;
        private static readonly Int128 one = (Int128)1;

        public static Int128 MinValue { get { return minValue; } }
        public static Int128 MaxValue { get { return maxValue; } }
        public static Int128 Zero { get { return zero; } }
        public static Int128 One { get { return one; } }

        public static Int128 Parse(string value)
        {
            Int128 c;
            var a = BigInteger.Parse(value);
            if (a < 0)
            {
                UInt128 cneg;
                UInt128.Create(out cneg, ref a);
                UInt128.Negate(out c.v, ref cneg);
            }
            else
                UInt128.Create(out c.v, ref a);
            return c;
        }

        public uint R0 { get { return v.R0; } }
        public uint R1 { get { return v.R1; } }
        public uint R2 { get { return v.R2; } }
        public uint R3 { get { return v.R3; } }

        public ulong S0 { get { return v.S0; } }
        public ulong S1 { get { return v.S1; } }

        public bool IsZero { get { return v.IsZero; } }
        public bool IsOne { get { return v.IsOne; } }
        public bool IsPowerOfTwo { get { return v.IsPowerOfTwo; } }
        public bool IsEven { get { return v.IsEven; } }
        public uint LeastSignificantWord { get { return v.LeastSignificantWord; } }
        public bool IsNegative { get { return v.R3 > int.MaxValue; } }

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
            Int128 c;
            if (a < 0)
                UInt128.Create(out c.v, (ulong)(long)a, ulong.MaxValue);
            else
                UInt128.Create(out c.v, (ulong)a, 0);
            return c;
        }

        public static implicit operator Int128(uint a)
        {
            Int128 c;
            UInt128.Create(out c.v, (ulong)a, 0);
            return c;
        }

        public static implicit operator Int128(long a)
        {
            Int128 c;
            if (a < 0)
                UInt128.Create(out c.v, (ulong)a, ulong.MaxValue);
            else
                UInt128.Create(out c.v, (ulong)a, 0);
            return c;
        }

        public static implicit operator Int128(ulong a)
        {
            Int128 c;
            UInt128.Create(out c.v, a, 0);
            return c;
        }

        public static explicit operator Int128(UInt128 a)
        {
            Int128 c;
            c.v = a;
            return c;
        }

        public static explicit operator UInt128(Int128 a)
        {
            return a.v;
        }

        public static explicit operator Int128(BigInteger a)
        {
            Int128 c;
            if (a < 0)
            {
                UInt128 b;
                var aneg = -a;
                UInt128.Create(out b, ref aneg);
                UInt128.Negate(out c.v, ref b);
            }
            else
                UInt128.Create(out c.v, ref a);
            return c;
        }

        public static implicit operator BigInteger(Int128 a)
        {
            if (a.IsNegative)
            {
                UInt128 c;
                UInt128.Negate(out c, ref a.v);
                return -(BigInteger)c;
            }
            return (BigInteger)a.v;
        }

        public static explicit operator double(Int128 a)
        {
            if (a.IsNegative)
            {
                UInt128 c;
                UInt128.Negate(out c, ref a.v);
                return -(double)c;
            }
            return (double)a.v;
        }

        public static Int128 operator <<(Int128 a, int b)
        {
            Int128 c;
            UInt128.LeftShift(out c.v, ref a.v, b);
            return c;
        }

        public static Int128 operator >>(Int128 a, int b)
        {
            if (a.IsNegative)
                throw new NotImplementedException();
            Int128 c;
            UInt128.RightShift(out c.v, ref a.v, b);
            return c;
        }

        public static Int128 operator &(Int128 a, Int128 b)
        {
            Int128 c;
            UInt128.And(out c.v, ref a.v, ref b.v);
            return c;
        }

        public static int operator &(Int128 a, int b)
        {
            if (b < 0)
                throw new NotImplementedException();
            return (int)(a.v & (uint)b);
        }

        public static int operator &(int a, Int128 b)
        {
            if (a < 0)
                throw new NotImplementedException();
            return (int)(b.v & (uint)a);
        }

        public static long operator &(Int128 a, long b)
        {
            if (b < 0)
                throw new NotImplementedException();
            return (long)(a.v & (ulong)b);
        }

        public static long operator &(long a, Int128 b)
        {
            if (a < 0)
                throw new NotImplementedException();
            return (long)(b.v & (ulong)a);
        }

        public static Int128 operator |(Int128 a, Int128 b)
        {
            Int128 c;
            UInt128.Or(out c.v, ref a.v, ref b.v);
            return c;
        }

        public static Int128 operator ^(Int128 a, Int128 b)
        {
            Int128 c;
            UInt128.ExclusiveOr(out c.v, ref a.v, ref b.v);
            return c;
        }

        public static Int128 operator ~(Int128 a)
        {
            Int128 c;
            UInt128.Not(out c.v, ref a.v);
            return c;
        }

        public static Int128 operator +(Int128 a, long b)
        {
            Int128 c;
            if (b < 0)
                UInt128.Subtract(out c.v, ref a.v, (ulong)(-b));
            else
                UInt128.Add(out c.v, ref a.v, (ulong)b);
            return c;
        }

        public static Int128 operator +(long a, Int128 b)
        {
            Int128 c;
            if (b < 0)
                UInt128.Subtract(out c.v, ref b.v, (ulong)(-a));
            else
                UInt128.Add(out c.v, ref b.v, (ulong)a);
            return c;
        }

        public static Int128 operator +(Int128 a, Int128 b)
        {
            Int128 c;
            UInt128.Add(out c.v, ref a.v, ref b.v);
            return c;
        }

        public static Int128 operator ++(Int128 a)
        {
            Int128 c;
            UInt128.Add(out c.v, ref a.v, 1);
            return c;
        }

        public static Int128 operator -(Int128 a, long b)
        {
            Int128 c;
            if (b < 0)
                UInt128.Add(out c.v, ref a.v, (ulong)(-b));
            else
                UInt128.Subtract(out c.v, ref a.v, (ulong)b);
            return c;
        }

        public static Int128 operator -(Int128 a, Int128 b)
        {
            Int128 c;
            UInt128.Subtract(out c.v, ref a.v, ref b.v);
            return c;
        }

        public static Int128 operator -(Int128 a)
        {
            Int128 c;
            UInt128.Negate(out c.v, ref a.v);
            return c;
        }

        public static Int128 operator --(Int128 a)
        {
            Int128 c;
            UInt128.Subtract(out c.v, ref a.v, 1);
            return c;
        }

        public static Int128 operator *(Int128 a, int b)
        {
            Int128 c;
            if (a.IsNegative)
            {
                UInt128 aneg;
                UInt128.Negate(out aneg, ref a.v);
                if (b < 0)
                    UInt128.Multiply(out c.v, ref aneg, (uint)(-b));
                else
                {
                    UInt128 cneg;
                    UInt128.Multiply(out cneg, ref aneg, (uint)b);
                    UInt128.Negate(out c.v, ref cneg);
                }
            }
            else
            {
                if (b < 0)
                {
                    UInt128 cneg;
                    UInt128.Multiply(out cneg, ref a.v, (uint)(-b));
                    UInt128.Negate(out c.v, ref cneg);
                }
                else
                    UInt128.Multiply(out c.v, ref a.v, (uint)b);
            }
            Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
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
                UInt128 aneg;
                UInt128.Negate(out aneg, ref a.v);
                if (b < 0)
                    UInt128.Multiply(out c.v, ref aneg, (ulong)(-b));
                else
                {
                    UInt128 cneg;
                    UInt128.Multiply(out cneg, ref aneg, (ulong)b);
                    UInt128.Negate(out c.v, ref cneg);
                }
            }
            else
            {
                if (b < 0)
                {
                    UInt128 cneg;
                    UInt128.Multiply(out cneg, ref a.v, (ulong)(-b));
                    UInt128.Negate(out c.v, ref cneg);
                }
                else
                    UInt128.Multiply(out c.v, ref a.v, (ulong)b);
            }
            Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
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
                UInt128 aneg;
                UInt128.Negate(out aneg, ref a.v);
                if (b.IsNegative)
                {
                    UInt128 bneg;
                    UInt128.Negate(out bneg, ref b.v);
                    UInt128.Multiply(out c.v, ref aneg, ref bneg);
                }
                else
                {
                    UInt128 cneg;
                    UInt128.Multiply(out cneg, ref aneg, ref b.v);
                    UInt128.Negate(out c.v, ref cneg);
                }
            }
            else
            {
                if (b.IsNegative)
                {
                    UInt128 bneg;
                    UInt128 cneg;
                    UInt128.Negate(out bneg, ref b.v);
                    UInt128.Multiply(out cneg, ref a.v, ref bneg);
                    UInt128.Negate(out c.v, ref cneg);
                }
                else
                    UInt128.Multiply(out c.v, ref a.v, ref b.v);
            }
            Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
            return c;
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
            return v.GetHashCode();
        }

        public static Int128 AddProduct(Int128 a, UInt128 b, int c)
        {
            UInt128 d;
            Int128 e;
            if (c < 0)
            {
                UInt128.Multiply(out d, ref b, (uint)(-c));
                UInt128.Subtract(out e.v, ref a.v, ref d);
            }
            else
            {
                UInt128.Multiply(out d, ref b, (uint)c);
                UInt128.Add(out e.v, ref a.v, ref d);
            }
            return e;
        }

        public static Int128 AddProduct(Int128 a, UInt128 b, long c)
        {
            UInt128 d;
            Int128 e;
            if (c < 0)
            {
                UInt128.Multiply(out d, ref b, (ulong)(-c));
                UInt128.Subtract(out e.v, ref a.v, ref d);
            }
            else
            {
                UInt128.Multiply(out d, ref b, (ulong)c);
                UInt128.Add(out e.v, ref a.v, ref d);
            }
            return e;
        }

        public static Int128 SubtractProduct(Int128 a, UInt128 b, int c)
        {
            UInt128 d;
            Int128 e;
            if (c < 0)
            {
                UInt128.Multiply(out d, ref b, (uint)(-c));
                UInt128.Add(out e.v, ref a.v, ref d);
            }
            else
            {
                UInt128.Multiply(out d, ref b, (uint)c);
                UInt128.Subtract(out e.v, ref a.v, ref d);
            }
            return e;
        }

        public static Int128 SubtractProduct(Int128 a, UInt128 b, long c)
        {
            UInt128 d;
            Int128 e;
            if (c < 0)
            {
                UInt128.Multiply(out d, ref b, (ulong)(-c));
                UInt128.Add(out e.v, ref a.v, ref d);
            }
            else
            {
                UInt128.Multiply(out d, ref b, (ulong)c);
                UInt128.Subtract(out e.v, ref a.v, ref d);
            }
            return e;
        }
    }
}
