using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class MontgomeryReduction
    {
        private BigInteger n;
        private int rLength;
        private BigInteger r;
        private BigInteger rMinusOne;
        private BigInteger rSquaredModN;
        private BigInteger rInverse;
        private BigInteger k;

        public MontgomeryReduction(BigInteger n)
        {
            this.n = n;
            rLength = BigIntegerUtils.GetBitLength(n);
            r = BigInteger.One << rLength;
            rMinusOne = r - BigInteger.One;
            rSquaredModN = r * r % n;
            BigInteger c;
            BigInteger d;
            BigIntegerUtils.ExtendedGreatestCommonDivisor(r, n, out c, out d);
            rInverse = c;
            k = -d;
            if (rInverse.Sign == -1)
                rInverse += n;
            if (k.Sign == -1)
                k += r;
            Debug.Assert(r * rInverse == k * n + 1);
        }

        public BigInteger ToResidue(BigInteger x)
        {
            return REDC(x % n * rSquaredModN);
        }

        public BigInteger FromResidue(BigInteger x)
        {
            return REDC(x);
        }

        public BigInteger Multiply(BigInteger x, BigInteger y)
        {
            return REDC(x * y);
        }

        private BigInteger REDC(BigInteger t)
        {
            var m = ((t & rMinusOne) * k) & rMinusOne;
            var s = (t + m * n) >> rLength;
            return s >= n ? s - n : s;
        }
    }
}
