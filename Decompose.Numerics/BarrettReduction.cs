using System;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class BarrettReduction : IReductionAlgorithm
    {
        private class Reducer : IReducer
        {
            private class Residue : IResidue
            {
                private Reducer reducer;
                private Radix32Integer r;

                public bool IsZero { get { return r.IsZero; } }

                public bool IsOne { get { return r.IsOne; } }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, BigInteger x)
                    : this(reducer)
                {
                    r = reducer.CreateRep();
                    r.Set(x);
                    reducer.Reduce(r);
                }

                public IResidue Set(IResidue x)
                {
                    r.Set(((Residue)x).r);
                    return this;
                }

                public IResidue Copy()
                {
                    var residue = new Residue(reducer);
                    residue.r = reducer.CreateRep();
                    residue.r.Set(r);
                    return residue;
                }

                public IResidue Multiply(IResidue x)
                {
                    reducer.reg3.Set(r);
                    if (this == x)
                        r.SetSquare(reducer.reg3);
                    else
                        r.SetProduct(reducer.reg3, ((Residue)x).r);
                    reducer.Reduce(r);
                    return this;
                }

                public IResidue Add(IResidue x)
                {
                    r.Add(((Residue)x).r);
                    if (r.CompareTo(reducer.pRep) >= 0)
                        r.Subtract(reducer.pRep);
                    return this;
                }

                public IResidue Subtract(IResidue x)
                {
                    r.Subtract(((Residue)x).r);
                    return this;
                }

                public bool Equals(IResidue other)
                {
                    return r.CompareTo(((Residue)other).r) == 0;
                }

                public int CompareTo(IResidue other)
                {
                    return r.CompareTo(((Residue)other).r);
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

            public BigInteger Modulus
            {
                get { return p; }
            }

            public Reducer(BigInteger p)
            {
                this.p = p;
                bLength = 32;
                b = BigInteger.One << bLength;
                var pLength = p.GetBitLength();
                k = (pLength - 1) / bLength + 1;
                mu = BigInteger.Pow(b, 2 * k) / p;

                var muLength = mu.GetBitLength();
                length = (pLength + 31) / 32 * 2 + (muLength + 31) / 32;
                bits = new uint[5 * length];
                muRep = new Radix32Integer(bits, 0 * length, length);
                pRep = new Radix32Integer(bits, 1 * length, length);
                reg1 = new Radix32Integer(bits, 2 * length, length);
                reg2 = new Radix32Integer(bits, 3 * length, length);
                reg3 = new Radix32Integer(bits, 4 * length, length);
                muRep.Set(mu);
                pRep.Set(p);
                bToTheKMinusOneLength = bLength * (k - 1);
                bToTheKPlusOneLength = bLength * (k + 1);
            }

            public IResidue ToResidue(BigInteger x)
            {
                return new Residue(this, x);
            }

            private Radix32Integer CreateRep()
            {
                return new Radix32Integer(new uint[length], 0, length);
            }

            private void Reduce(Radix32Integer z)
            {
                // var qhat = (z >> (bLength * (k - 1))) * mu >> (bLength * (k + 1));
                reg1.Set(z);
                reg1.RightShift(bToTheKMinusOneLength);
                reg2.SetProductShifted(reg1, muRep, bToTheKPlusOneLength);
                // var r = z % bToTheKPlusOne - qhat * p % bToTheKPlusOne;
                z.Mask(bToTheKPlusOneLength);
                reg1.SetProductMasked(reg2, pRep, bToTheKPlusOneLength);
                // if (r.Sign == -1) r += bToTheKPlusOne;
                if (z.CompareTo(reg1) < 0)
                    z.AddPowerOfTwo(bToTheKPlusOneLength);
                z.Subtract(reg1);
                // while (r >= p) r -= p;
                while (z.CompareTo(pRep) >= 0)
                    z.Subtract(pRep);
            }
        }

        public IReducer GetReducer(BigInteger n)
        {
            return new Reducer(n);
        }
    }
}
