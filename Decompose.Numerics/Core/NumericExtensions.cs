using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
    public static class NumericExtensions
    {
        public static int ModularSum(this IEnumerable<int> source, int n)
        {
            return source.Aggregate(0, (sofar, current) => sofar + current) % n;
        }

        public static int Product(this IEnumerable<int> source)
        {
            return source.Aggregate(1, (sofar, current) => sofar * current);
        }

        public static int ModularProduct(this IEnumerable<int> source, int n)
        {
            return (int)source.Aggregate((long)1, (sofar, current) => sofar * current % n);
        }

        public static BigInteger Sum(this IEnumerable<BigInteger> source)
        {
            return source.Aggregate(BigInteger.Zero, (sofar, current) => sofar + current);
        }

        public static BigInteger ModularSum(this IEnumerable<BigInteger> source, BigInteger n)
        {
            return source.Aggregate(BigInteger.Zero, (sofar, current) => sofar + current) % n;
        }

        public static BigInteger Product(this IEnumerable<BigInteger> source)
        {
            return source.Aggregate(BigInteger.One, (sofar, current) => sofar * current);
        }

        public static BigInteger ModularProduct(this IEnumerable<BigInteger> source, BigInteger n)
        {
            var product = BigInteger.One;
            foreach (var factor in source)
            {
                var f = factor;
                if (f >= n)
                    f %= n;
                else if (f.Sign == -1)
                    f = f % n + n;
                product *= f;
                if (product >= n)
                    product %= n;
                else if (product.Sign == -1)
                    product = product % n + n;
            }
            return product;
        }

        public static int GetBitLength(this long x)
        {
            return GetBitLength((ulong)x);
        }

        public static int GetBitLength(this int x)
        {
            return GetBitLength((uint)x);
        }

        public static int GetBitLength(this ulong x)
        {
            int i = 0;
            if ((x & 0xffffffff00000000) != 0)
            {
                i += 32;
                x >>= 32;
            }
            return i + GetBitLength((uint)x);
        }

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
            return 0;
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

        public static int GetBitCount(this long word)
        {
            return GetBitCount((ulong)word);
        }

        public static int GetBitCount(this int word)
        {
            return GetBitCount((uint)word);
        }

        public static int GetBitCount(this short word)
        {
            return GetBitCount((ushort)word);
        }

        public static int GetBitCount(this ulong word)
        {
            return GetBitCount((uint)word) + GetBitCount((uint)(word >> 32));
        }

        public static int GetBitCount(this uint word)
        {
            return GetBitCount((ushort)word) + GetBitCount((ushort)(word >> 16));
        }

        public static int GetBitCount(this ushort word)
        {
            return GetBitCount((byte)word) + GetBitCount((byte)(word >> 8));
        }

        public static int GetBitCount(this byte word)
        {
            return bitCounts[word & ((1 << 4) - 1)] + bitCounts[word >> 4];
        }

        private static int[] bitCounts = new[]
        {
            0, 1, 1, 2, 1, 2, 2, 3,
            1, 2, 2, 3, 2, 3, 3, 4,
        };
    }
}
