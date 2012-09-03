using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public class Int128
    {
        private UInt128 v;

        private Int128(UInt128 a)
        {
            v = a;
        }

        public bool IsNegative { get { return v.R3 >= int.MaxValue; } }

        public static implicit operator Int128(int a)
        {
            if (a < 0)
                return new Int128(new UInt128((ulong)a, ulong.MaxValue));
            return new Int128((uint)a);
        }

        public static implicit operator Int128(uint a)
        {
            return new Int128(new UInt128(a, 0, 0, 0));
        }

        public static implicit operator Int128(long a)
        {
            if (a < 0)
                return new Int128(new UInt128((ulong)a, ulong.MaxValue));
            return new Int128((uint)a);
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

        public static implicit operator Int128(BigInteger a)
        {
            return new Int128(a < 0 ? ((UInt128)(-a)).TwosComplement : (UInt128)a);
        }

        public static implicit operator BigInteger(Int128 a)
        {
            return a.IsNegative ? -(BigInteger)a.v.TwosComplement : (BigInteger)a.v;
        }

        public static Int128 operator +(Int128 a, Int128 b)
        {
            return new Int128(a.v + b.v);
        }

        public static Int128 operator -(Int128 a, Int128 b)
        {
            return new Int128(a.v + b.v);
        }

        public static Int128 operator -(Int128 a)
        {
            return new Int128(a.v.TwosComplement);
        }
    }
}
