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
        private static readonly Int128 maxValue = (Int128)(((UInt128)1 << 127) - 1);
        private static readonly Int128 zero = (Int128)0;
        private static readonly Int128 one = (Int128)1;
        private static readonly Int128 minusOne = (Int128)(-1);

        public static Int128 MinValue { get { return minValue; } }
        public static Int128 MaxValue { get { return maxValue; } }
        public static Int128 Zero { get { return zero; } }
        public static Int128 One { get { return one; } }
        public static Int128 MinusOne { get { return minusOne; } }

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
        public int Sign { get { return IsNegative ? -1 : v.Sign; } }

        public override string ToString()
        {
            return ((BigInteger)this).ToString();
        }

        public int GetBitLength()
        {
            return v.GetBitLength();
        }

        public int GetBitCount()
        {
            return v.GetBitCount();
        }

        public static explicit operator Int128(double a)
        {
            Int128 c;
            if (a < 0)
            {
                UInt128 cneg;
                UInt128.Create(out cneg, -a);
                UInt128.Negate(out c.v, ref cneg);
            }
            else
                UInt128.Create(out c.v, a);
            return c;
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

        public static explicit operator int(Int128 a)
        {
            return (int)a.v.R0;
        }

        public static explicit operator uint(Int128 a)
        {
            return a.v.R0;
        }

        public static explicit operator long(Int128 a)
        {
            return (long)a.v.S0;
        }

        public static explicit operator ulong(Int128 a)
        {
            return a.v.S0;
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
                return -UInt128.ConvertToDouble(ref c);
            }
            return UInt128.ConvertToDouble(ref a.v);
        }

        public static Int128 operator <<(Int128 a, int b)
        {
            Int128 c;
            UInt128.LeftShift(out c.v, ref a.v, b);
            return c;
        }

        public static Int128 operator >>(Int128 a, int b)
        {
            Int128 c;
            UInt128.ArithmeticRightShift(out c.v, ref a.v, b);
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
            return (int)(b.v & (uint)a);
        }

        public static long operator &(Int128 a, long b)
        {
            return (long)(a.v & (ulong)b);
        }

        public static long operator &(long a, Int128 b)
        {
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
            if (a < 0)
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

        public static Int128 operator +(Int128 a)
        {
            return a;
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

        public static Int128 operator /(Int128 a, int b)
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
            Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
            return c;
        }

        public static Int128 operator /(Int128 a, long b)
        {
            Int128 c;
            if (a.IsNegative)
            {
                UInt128 aneg;
                UInt128.Negate(out aneg, ref a.v);
                if (b < 0)
                    UInt128.Divide(out c.v, ref aneg, (ulong)(-b));
                else
                {
                    UInt128 cneg;
                    UInt128.Divide(out cneg, ref aneg, (ulong)b);
                    UInt128.Negate(out c.v, ref cneg);
                }
            }
            else
            {
                if (b < 0)
                {
                    UInt128 cneg;
                    UInt128.Divide(out cneg, ref a.v, (ulong)(-b));
                    UInt128.Negate(out c.v, ref cneg);
                }
                else
                    UInt128.Divide(out c.v, ref a.v, (ulong)b);
            }
            Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
            return c;
        }

        public static Int128 operator /(Int128 a, Int128 b)
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
                    UInt128.Divide(out c.v, ref aneg, ref bneg);
                }
                else
                {
                    UInt128 cneg;
                    UInt128.Divide(out cneg, ref aneg, ref b.v);
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
                    UInt128.Divide(out cneg, ref a.v, ref bneg);
                    UInt128.Negate(out c.v, ref cneg);
                }
                else
                    UInt128.Divide(out c.v, ref a.v, ref b.v);
            }
            Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
            return c;
        }

        public static int operator %(Int128 a, int b)
        {
            if (a.IsNegative)
            {
                UInt128 aneg;
                UInt128.Negate(out aneg, ref a.v);
                if (b < 0)
                    return (int)UInt128.Modulo(ref aneg, (uint)(-b));
                else
                    return -(int)UInt128.Modulo(ref aneg, (uint)b);
            }
            else
            {
                if (b < 0)
                    return -(int)UInt128.Modulo(ref a.v, (uint)(-b));
                else
                    return (int)UInt128.Modulo(ref a.v, (uint)b);
            }
        }

        public static long operator %(Int128 a, long b)
        {
            if (a.IsNegative)
            {
                UInt128 aneg;
                UInt128.Negate(out aneg, ref a.v);
                if (b < 0)
                    return (long)UInt128.Modulo(ref aneg, (ulong)(-b));
                else
                    return -(long)UInt128.Modulo(ref aneg, (ulong)b);
            }
            else
            {
                if (b < 0)
                    return -(long)UInt128.Modulo(ref a.v, (ulong)(-b));
                else
                    return (long)UInt128.Modulo(ref a.v, (ulong)b);
            }
        }

        public static Int128 operator %(Int128 a, Int128 b)
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
                    UInt128.Modulo(out c.v, ref aneg, ref bneg);
                }
                else
                {
                    UInt128 cneg;
                    UInt128.Modulo(out cneg, ref aneg, ref b.v);
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
                    UInt128.Modulo(out cneg, ref a.v, ref bneg);
                    UInt128.Negate(out c.v, ref cneg);
                }
                else
                    UInt128.Modulo(out c.v, ref a.v, ref b.v);
            }
            Debug.Assert((BigInteger)c == (BigInteger)a % (BigInteger)b);
            return c;
        }

        public static bool operator <(Int128 a, UInt128 b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(UInt128 a, Int128 b)
        {
            return b.CompareTo(a) > 0;
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

        public static bool operator <=(Int128 a, UInt128 b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(UInt128 a, Int128 b)
        {
            return b.CompareTo(a) >= 0;
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

        public static bool operator >(Int128 a, UInt128 b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(UInt128 a, Int128 b)
        {
            return b.CompareTo(a) < 0;
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

        public static bool operator >=(Int128 a, UInt128 b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(UInt128 a, Int128 b)
        {
            return b.CompareTo(a) <= 0;
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

        public static bool operator ==(UInt128 a, Int128 b)
        {
            return b.Equals(a);
        }

        public static bool operator ==(Int128 a, UInt128 b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(Int128 a, Int128 b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(Int128 a, int b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(int a, Int128 b)
        {
            return b.Equals(a);
        }

        public static bool operator ==(Int128 a, uint b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(uint a, Int128 b)
        {
            return b.Equals(a);
        }

        public static bool operator ==(Int128 a, long b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(long a, Int128 b)
        {
            return b.Equals(a);
        }

        public static bool operator ==(Int128 a, ulong b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(ulong a, Int128 b)
        {
            return b.Equals(a);
        }

        public static bool operator !=(UInt128 a, Int128 b)
        {
            return !b.Equals(a);
        }

        public static bool operator !=(Int128 a, UInt128 b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(Int128 a, Int128 b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(Int128 a, int b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(int a, Int128 b)
        {
            return !b.Equals(a);
        }

        public static bool operator !=(Int128 a, uint b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(uint a, Int128 b)
        {
            return !b.Equals(a);
        }

        public static bool operator !=(Int128 a, long b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(long a, Int128 b)
        {
            return !b.Equals(a);
        }

        public static bool operator !=(Int128 a, ulong b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(ulong a, Int128 b)
        {
            return !b.Equals(a);
        }

        public int CompareTo(UInt128 other)
        {
            if (IsNegative)
                return -1;
            return v.CompareTo(other);
        }

        public int CompareTo(Int128 other)
        {
            return SignedCompare(ref v, other.S0, other.S1);
        }

        public int CompareTo(int other)
        {
            if (other < 0)
                return SignedCompare(ref v, (ulong)(long)other, ulong.MaxValue);
            return SignedCompare(ref v, (ulong)other, 0);
        }

        public int CompareTo(uint other)
        {
            return SignedCompare(ref v, (ulong)other, 0);
        }

        public int CompareTo(long other)
        {
            if (other < 0)
                return SignedCompare(ref v, (ulong)other, ulong.MaxValue);
            return SignedCompare(ref v, (ulong)other, 0);
        }

        public int CompareTo(ulong other)
        {
            return SignedCompare(ref v, other, 0);
        }

        private static int SignedCompare(ref UInt128 a, ulong s0, ulong s1)
        {
            if (a.S1 != s1)
                return (a.S1 ^ ((ulong)1 << 63)).CompareTo(s1 ^ ((ulong)1 << 63));
            return a.S0.CompareTo(s0);
        }

        public bool Equals(UInt128 other)
        {
            return !IsNegative && v.Equals(other);
        }

        public bool Equals(Int128 other)
        {
            return v.Equals(other.v);
        }

        public bool Equals(int other)
        {
            if (other < 0)
                return v.S1 == ulong.MaxValue && v.S0 == (uint)other;
            return v.S1 == 0 && v.S0 == (uint)other;
        }

        public bool Equals(uint other)
        {
            return v.S1 == 0 && v.S0 == other;
        }

        public bool Equals(long other)
        {
            if (other < 0)
                return v.S1 == ulong.MaxValue && v.S0 == (ulong)other;
            return v.S1 == 0 && v.S0 == (ulong)other;
        }

        public bool Equals(ulong other)
        {
            return v.S1 == 0 && v.S0 == other;
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

        public static Int128 Abs(Int128 a)
        {
            if (!a.IsNegative)
                return a;
            Int128 c;
            UInt128.Negate(out c.v, ref a.v);
            return c;
        }

        public static Int128 Double(Int128 a)
        {
            Int128 c;
            UInt128.Double(out c.v, ref a.v);
            return c;
        }

        public static Int128 Square(long a)
        {
            if (a < 0)
                a = -a;
            Int128 c;
            UInt128.Square(out c.v, (ulong)a);
            return c;
        }

        public static Int128 Square(Int128 a)
        {
            Int128 c;
            if (a.IsNegative)
            {
                UInt128 aneg;
                UInt128.Negate(out aneg, ref a.v);
                UInt128.Square(out c.v, ref aneg);
            }
            else
                UInt128.Square(out c.v, ref a.v);
            return c;
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

        public static void Pow(out Int128 result, ref Int128 value, int exponent)
        {
            if (exponent < 0)
                throw new InvalidOperationException();
            if (value.IsNegative)
            {
                UInt128 valueneg;
                UInt128.Negate(out valueneg, ref value.v);
                if ((exponent & 1) == 0)
                    UInt128.Pow(out result.v, ref valueneg, (uint)exponent);
                else
                {
                    UInt128 resultneg;
                    UInt128.Pow(out resultneg, ref valueneg, (uint)exponent);
                    UInt128.Negate(out result.v, ref resultneg);
                }
            }
            else
                UInt128.Pow(out result.v, ref value.v, (uint)exponent);
        }

        public static Int128 Pow(Int128 value, int exponent)
        {
            Int128 result;
            Pow(out result, ref value, exponent);
            return result;
        }

        public static ulong FloorSqrt(Int128 a)
        {
            if (a.IsNegative)
                throw new InvalidOperationException();
            return UInt128.FloorSqrt(a.v);
        }

        public static ulong CeilingSqrt(Int128 a)
        {
            if (a.IsNegative)
                throw new InvalidOperationException();
            return UInt128.CeilingSqrt(a.v);
        }

    }
}
