using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int Root(int a, int b)
        {
            if (b == 2)
                return SquareRoot(a);
            throw new NotImplementedException();
        }

        public static uint Root(uint a, uint b)
        {
            if (b == 2)
                return SquareRoot(a);
            throw new NotImplementedException();
        }

        public static long Root(long a, long b)
        {
            if (b == 2)
                return SquareRoot(a);
            throw new NotImplementedException();
        }

        public static ulong Root(ulong a, ulong b)
        {
            if (b == 2)
                return SquareRoot(a);
            throw new NotImplementedException();
        }

        public static BigInteger Root(BigInteger a, BigInteger b)
        {
            if (b == 2)
                return SquareRoot(a);
            throw new NotImplementedException();
        }
    }
}
