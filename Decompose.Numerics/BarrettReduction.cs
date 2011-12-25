using System;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class BarrettReduction
    {
        public class Residue : IResidue
        {
            private BarrettReduction reduction;
            private Radix32Integer r;

            public bool IsZero { get { return r.IsZero; } }

            public bool IsOne { get { return r.IsOne; } }

            protected Residue(BarrettReduction reduction)
            {
                this.reduction = reduction;
            }

            public Residue(BarrettReduction reduction, BigInteger x)
                : this(reduction)
            {
                r = reduction.Create();
                r.Set(x);
            }

            public IResidue Set(IResidue x)
            {
                r.Set(((Residue)x).r);
                return this;
            }

            public IResidue Copy()
            {
                var residue = new Residue(reduction);
                residue.r = reduction.Create();
                residue.r.Set(r);
                return residue;
            }

            public IResidue Multiply(IResidue x)
            {
#if DEBUG
                var aCopy = r.Copy();
                var bCopy = ((Residue)x).r.Copy();
                var a = ToBigInteger();
                var b = ((Residue)x).r.ToBigInteger();
                var product = a * b;
                var result = product % reduction.p;
#endif
                reduction.reg1.Set(r);
                r.SetProduct(reduction.reg1, x == this ? reduction.reg1 : ((Residue)x).r);
#if DEBUG
                Debug.Assert(r.ToBigInteger() == product);
#endif
                reduction.Reduce(r);
#if DEBUG
                if (r.ToBigInteger() != result)
                {
                    var c = aCopy.Copy().Clear();
                    c.SetProduct(aCopy, bCopy);
                    reduction.Reduce(c);
                    if (c.ToBigInteger() != result)
                        throw new InvalidOperationException();
                }
                Debug.Assert(r.ToBigInteger() == result);
#endif
                return this;
            }

            public IResidue Add(IResidue x)
            {
                r.Add(((Residue)x).r);
                if (r.CompareTo(reduction.pRep) >= 0)
                    r.Subtract(reduction.pRep);
                return this;
            }

            public IResidue Subtract(IResidue x)
            {
                r.Subtract(((Residue)x).r);
                return this;
            }

            public int CompareTo(IResidue other)
            {
                return r.CompareTo(((Residue)other).r);
            }

            public IResidue SetGreatestCommonDivisor(IResidue a, IResidue b)
            {
                r.SetGreatestCommonDivisor(((Residue)a).r, ((Residue)b).r);
                return this;
            }

            public BigInteger ToBigInteger()
            {
                return r.ToBigInteger();
            }

            public override string ToString()
            {
                return ToBigInteger().ToString();
            }
        }

        private BigInteger p;
        private int bLength;
        private BigInteger b;
        private int k;
        private BigInteger mu;

        private int length;
        private int bToTheKMinusOneLength;
        private int bToTheKPlusOneLength;
        private uint[] bits;
        private Radix32Integer muRep;
        private Radix32Integer pRep;
        private Radix32Integer reg2;
        private Radix32Integer reg1;
        private Radix32Integer reg3;

        public BarrettReduction(BigInteger p)
        {
            this.p = p;
            bLength = 32;
            b = BigInteger.One << bLength;
            var pLength = BigIntegerUtils.GetBitLength(p);
            k = (pLength - 1) / bLength + 1;
            mu = BigInteger.Pow(b, 2 * k) / p;

            var muLength = BigIntegerUtils.GetBitLength(mu);
            length = (pLength + 31) / 32 * 2 + (muLength + 31) / 32;
            bits = new uint[4 * length];
            muRep = new Radix32Integer(bits, 0 * length, length);
            muRep.Set(mu);
            pRep = new Radix32Integer(bits, 1 * length, length);
            pRep.Set(p);
            reg1 = new Radix32Integer(bits, 2 * length, length);
            reg2 = new Radix32Integer(bits, 3 * length, length);
            bToTheKMinusOneLength = bLength * (k - 1);
            bToTheKPlusOneLength = bLength * (k + 1);
        }

        public Radix32Integer Create()
        {
            return new Radix32Integer(new uint[length], 0, length);
        }

        public IResidue ToResidue(BigInteger x)
        {
            return new Residue(this, x);
        }

        public void Reduce(Radix32Integer z)
        {
#if DEBUG
            var zOrig = z.ToBigInteger();
#endif
            // var qhat = (z >> (bLength * (k - 1))) * mu >> (bLength * (k + 1));
            reg1.Set(z);
            reg1.RightShift(bToTheKMinusOneLength);
            reg2.SetProduct(reg1, muRep);
            reg2.RightShift(bToTheKPlusOneLength);
#if DEBUG
            var qhat = (zOrig >> (bLength * (k - 1))) * mu >> (bLength * (k + 1));
            Debug.Assert(reg2.ToBigInteger() == qhat);
#endif
            // var r = z % bToTheKPlusOne - qhat * p % bToTheKPlusOne;
            z.Mask(bToTheKPlusOneLength);
#if true
            reg1.SetProductMask(reg2, pRep, bToTheKPlusOneLength);
#else
            reg1.SetProduct(reg2, pRep);
            reg1.Mask(bToTheKPlusOneLength);
#endif
#if DEBUG
            var bToTheKPlusOne = BigInteger.Pow(b, k + 1);
            Debug.Assert(z.ToBigInteger() == zOrig % bToTheKPlusOne);
            Debug.Assert(reg1.ToBigInteger() == qhat * p % bToTheKPlusOne);
#endif
            // if (r.Sign == -1) r += bToTheKPlusOne;
            if (z.CompareTo(reg1) < 0)
                z.AddPowerOfTwo(bToTheKPlusOneLength);
            z.Subtract(reg1);
#if DEBUG
            Debug.Assert(z.ToBigInteger() == zOrig % bToTheKPlusOne - qhat * p % bToTheKPlusOne);
#endif
            // while (r >= p) r -= p;
            while (z.CompareTo(pRep) >= 0)
                z.Subtract(pRep);
        }
    }
}
