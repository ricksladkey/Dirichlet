using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
#if false
        public static BigInteger AddMod(BigInteger a, BigInteger b, BigInteger n)
        {
            Debug.Assert(a >= BigInteger.Zero && a < n);
            Debug.Assert(b >= BigInteger.Zero && b < n);
            var sum = a + b;
            if (sum >= n)
                sum -= n;
            return sum;
        }

        public static BigInteger SubAbsMod(BigInteger a, BigInteger b, BigInteger n)
        {
            Debug.Assert(a >= BigInteger.Zero && a < n);
            Debug.Assert(b >= BigInteger.Zero && b < n);
            if (a > b)
                return a - b;
            return b - a;
        }

        public static BigInteger MulMod(BigInteger a, BigInteger b, BigInteger n)
        {
            Debug.Assert(a >= BigInteger.Zero && a < n);
            Debug.Assert(b >= BigInteger.Zero && b < n);
            if (a.IsZero || b.IsZero)
                return BigInteger.Zero;
            int compare = a.CompareTo(b);
            if (compare < 0)
                return MulModInternal(b, a, n);
            if (compare > 0)
                return MulModInternal(a, b, n);
            return SquareMod(a, n);
        }

        private static BigInteger MulModInternal(BigInteger a, BigInteger b, BigInteger n)
        {
            // a * (c + 1) = a * c + a
            // a * 2d = (a * d) + (a * d)
            if (b.IsEven)
            {
                var x = MulModInternal(a, b >> 1, n);
                return AddMod(x, x, n);
            }
            if (b.IsOne)
                return a;
            return AddMod(MulModInternal(a, b - BigInteger.One, n), a, n);
        }

        public static BigInteger SquareMod(BigInteger a, BigInteger n)
        {
            Debug.Assert(a >= BigInteger.Zero && a < n);
            return MulModInternal(a, a, n);
        }

        public static BigInteger ModPow(BigInteger b, BigInteger e, BigInteger m)
        {
            return ModPowInternal(b, e, 1, m);
        }

        private static BigInteger ModPowInternal(BigInteger b, BigInteger e, BigInteger p, BigInteger modulus)
        {
            if (e == 0)
                return p;
            if (e % 2 == 0)
                return ModPowInternal(b * b % modulus, e / 2, p, modulus);
            return ModPowInternal(b, e - 1, b * p % modulus, modulus);
        }
#endif
}
