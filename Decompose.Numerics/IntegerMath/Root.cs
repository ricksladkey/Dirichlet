using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int Root(int a, int b)
        {
            if ((b & 1) == 0 && a < 0)
                throw new ArgumentException("negative radicand");
            var result = (int)Math.Round(Math.Exp(Math.Log(Math.Abs(a)) / b));
            if (a < 0)
                result = -result;
            if (Power(result, b) != a)
                throw new InvalidOperationException("not a even power");
            return result;
        }

        public static uint Root(uint a, uint b)
        {
            if ((b & 1) == 0 && a < 0)
                throw new ArgumentException("negative radicand");
            var result = (uint)Math.Round(Math.Exp(Math.Log(a) / b));
            if (Power(result, b) != a)
                throw new InvalidOperationException("not a even power");
            return result;
        }

        public static long Root(long a, long b)
        {
            if ((b & 1) == 0 && a < 0)
                throw new ArgumentException("negative radicand");
            var result = (int)Math.Round(Math.Exp(Math.Log(Math.Abs(a)) / b));
            if (a < 0)
                result = -result;
            if (Power(result, b) != a)
                throw new InvalidOperationException("not a even power");
            return result;
        }

        public static ulong Root(ulong a, ulong b)
        {
            if ((b & 1) == 0 && a < 0)
                throw new ArgumentException("negative radicand");
            var result = (ulong)Math.Round(Math.Exp(Math.Log(a) / b));
            if (Power(result, b) != a)
                throw new InvalidOperationException("not a even power");
            return result;
        }

        public static BigInteger Root(BigInteger a, BigInteger b)
        {
            if (b.IsEven && a < 0)
                throw new ArgumentException("negative radicand");
            var result = (BigInteger)Math.Round(Math.Exp(BigInteger.Log(BigInteger.Abs(a)) / (double)b));
            if (a < 0)
                result = -result;
            if (Power(result, b) != a)
                throw new InvalidOperationException("not a even power");
            return result;
        }
    }
}
