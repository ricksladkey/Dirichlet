using System.Numerics;

namespace Decompose.Numerics
{
    public static class NumericExtensions
    {
        public static int GetBitLength(this uint x)
        {
            int i = 0;
            while (x > 0)
            {
                x >>= 1;
                i++;
            }
            return i;
        }

        public static int GetBitLength(this BigInteger n)
        {
            var bytes = n.ToByteArray();
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                var b = bytes[i];
                for (int j = 8 - 1; j >= 0; j--)
                {
                    if ((b & (1 << j)) != 0)
                        return i * 8 + j + 1;
                }
            }
            return 0;
        }
    }
}
