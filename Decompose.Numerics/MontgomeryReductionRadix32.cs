using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
#if false
    public class MontgomeryReductionRadix32 : IReduction
    {
        public class Residue : IResidue
        {
            private MontgomeryReductionRadix32 reduction;
            private uint[] r;

            protected Residue(MontgomeryReductionRadix32 reduction)
            {
                this.reduction = reduction;
            }

            public Residue(MontgomeryReductionRadix32 reduction, BigInteger x)
                : this(reduction)
            {
                r = new uint[reduction.length];
                Radix32.Assign(r, 0, x, reduction.length);
                this.r = reduction.REDC(x % reduction.n * reduction.rSquaredModN);
            }

            public IResidue Copy()
            {
                var residue = new Residue(reduction);
                residue.r = (uint[])r.Clone();
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

            public BigInteger ToBigInteger()
            {
                return reduction.REDC(r);
            }
        }

        private BigInteger n;
        private int rLength;
        private int length;
        private BigInteger r;
        private BigInteger rMinusOne;
        private BigInteger rSquaredModN;
        private BigInteger rInverse;
        private BigInteger k;
        private uint[] stack;
        private int stackPointer;
        private int ni;
        private int ki;
        private int mi;
        private int tmpi;

        public MontgomeryReductionRadix32(BigInteger n)
        {
            this.n = n;
            rLength = BigIntegerUtils.GetBitLength(n);
            length = (rLength * 2 + 31) / 32;
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
            stack = new uint[4 * length];
            stackPointer = 0;
            ni = StackAllocate();
            ki = StackAllocate();
            mi = StackAllocate();
            tmpi = StackAllocate();
            Radix32.Assign(stack, ni, n, length);
            Radix32.Assign(stack, ki, k, length);
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

        private int StackAllocate()
        {
            var sp = stackPointer;
            stackPointer += length;
            return sp;
        }

        private BigInteger REDC(uint[] t, int ti)
        {
            Radix32.Copy(stack, tmpi, t, ti, length);
            Radix32.Mask(stack, tmpi, rLength - 1, length);
            Radix32.Multiply(stack, mi, stack, tmpi, stack, ki, length);
            Radix32.Mask(stack, mi, rLength - 1, length);
            var s = (t + m * n) >> rLength;
            return s >= n ? s - n : s;
        }
    }
#endif
}
