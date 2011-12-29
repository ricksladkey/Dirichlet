using System.Numerics;

namespace Decompose.Numerics
{
    public static class NumericExtensions
    {
        public static int GetBitLength(this uint x)
        {
            int i = 0;
            if ((x & 0xffff0000) != 0)
            {
                i += 16;
                x >>= 16;
            }
            if ((x & 0xff00) != 0)
            {
                i += 8;
                x >>= 8;
            }
            return i + GetBitLength((byte)x);
        }

        public static int GetBitLength(this byte x)
        {
            int i = 0;
            if ((x & 0xf0) != 0)
            {
                i += 4;
                x >>= 4;
            }
            if ((x & 0xc) != 0)
            {
                i += 2;
                x >>= 2;
            }
            if ((x & 0x2) != 0)
                return i + 2;
            if ((x & 0x1) != 0)
                return i + 1;
            return i;
        }

        public static int GetBitLength(this BigInteger n)
        {
            var bytes = n.ToByteArray();
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                var b = bytes[i];
                if (b == 0)
                    continue;
                return 8 * i + b.GetBitLength();
            }
            return 0;
        }
    }
}
