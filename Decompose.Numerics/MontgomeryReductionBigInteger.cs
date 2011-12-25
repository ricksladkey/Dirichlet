using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class MontgomeryReductionBigInteger : IReduction
    {
        public class Residue : IResidue
        {
            private MontgomeryReductionBigInteger reduction;
            private BigInteger r;

            public bool IsZero { get { return r.IsZero; } }

            public bool IsOne { get { return r.IsOne; } }

            protected Residue(MontgomeryReductionBigInteger reduction)
            {
                this.reduction = reduction;
            }

            public Residue(MontgomeryReductionBigInteger reduction, BigInteger x)
                : this(reduction)
            {
                this.r = reduction.REDC(x % reduction.n * reduction.rSquaredModN);
            }

            public IResidue Set(IResidue x)
            {
                r = ((Residue)x).r;
                return this;
            }

            public IResidue Copy()
            {
                var residue = new Residue(reduction);
                residue.r = r;
                return residue;
            }

            public IResidue Multiply(IResidue x)
            {
                r = reduction.REDC(r * ((Residue)x).r);
                return this;
            }

            public IResidue Add(IResidue x)
            {
                r = BigIntegerUtils.AddMod(r, ((Residue)x).r, reduction.n);
                return this;
            }

            public IResidue Subtract(IResidue x)
            {
                r -= ((Residue)x).r;
                return this;
            }

            public IResidue SetGreatestCommonDivisor(IResidue a, IResidue b)
            {
                r = BigInteger.GreatestCommonDivisor(((Residue)a).r, ((Residue)b).r);
                return this;
            }

            public BigInteger ToBigInteger()
            {
                return reduction.REDC(r);
            }

            public override string ToString()
            {
                return ToBigInteger().ToString();
            }

            public int CompareTo(IResidue other)
            {
                return r.CompareTo(((Residue)other).r);
            }
        }

        private BigInteger n;
        private int rLength;
        private BigInteger r;
        private BigInteger rMinusOne;
        private BigInteger rSquaredModN;
        private BigInteger rInverse;
        private BigInteger k;

        public MontgomeryReductionBigInteger(BigInteger n)
        {
            this.n = n;
            rLength = BigIntegerUtils.GetBitLength(n);
            r = BigInteger.One << rLength;
            rMinusOne = r - BigInteger.One;
            rSquaredModN = r * r % n;
            var results = BigIntegerUtils.ExtendedGreatestCommonDivisor(r, n);
            rInverse = results[0];
            k = -results[1];
            if (rInverse.Sign == -1)
                rInverse += n;
            if (k.Sign == -1)
                k += r;
            Debug.Assert(r * rInverse == k * n + 1);
        }

        public IResidue ToResidue(BigInteger x)
        {
            return new Residue(this, x);
        }

        public IResidue Multiply(IResidue x, IResidue y)
        {
            var residue = x.Copy();
            residue.Multiply(y);
            return residue;
        }

        private BigInteger REDC(BigInteger t)
        {
            var m = ((t & rMinusOne) * k) & rMinusOne;
            var s = (t + m * n) >> rLength;
            return s >= n ? s - n : s;
        }
    }
}
