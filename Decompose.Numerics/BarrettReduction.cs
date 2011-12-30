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

                public IResidue Set(BigInteger x)
                {
                    r.Set(x);
                    return this;
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
                    r.Multiply(((Residue)x).r, reducer.reg3);
                    reducer.Reduce(r);
                    return this;
                }

                public IResidue Add(IResidue x)
                {
                    r.AddModulo(((Residue)x).r, reducer.pRep);
                    return this;
                }

                public IResidue Subtract(IResidue x)
                {
                    r.Subtract(((Residue)x).r);
                    return this;
                }

                public bool Equals(IResidue other)
                {
                    return r == ((Residue)other).r;
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
            private Radix32Store store;

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
                store = new Radix32Store(length);
                muRep = store.Create();
                pRep = store.Create();
                reg1 = store.Create();
                reg2 = store.Create();
                reg3 = store.Create();
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
                return new Radix32Integer(length);
            }

            private void Reduce(Radix32Integer z)
            {
                // var qhat = (z >> (bLength * (k - 1))) * mu >> (bLength * (k + 1));
                reg1.Set(z);
                reg1.RightShift(bToTheKMinusOneLength);
#if false
                reg2.SetProductShifted(reg1, muRep, bToTheKPlusOneLength);
#else
                reg2.SetProduct(reg1, muRep);
                reg2.RightShift(bToTheKPlusOneLength);
#endif
                // var r = z % bToTheKPlusOne - qhat * p % bToTheKPlusOne;
                z.Mask(bToTheKPlusOneLength);
#if true
                reg1.SetProductMasked(reg2, pRep, bToTheKPlusOneLength);
#else
                reg1.SetProduct(reg2, pRep);
                reg1.Mask(bToTheKPlusOneLength);
#endif
                // if (r.Sign == -1) r += bToTheKPlusOne;
                if (z < reg1)
                    z.AddPowerOfTwo(bToTheKPlusOneLength);
                z.Subtract(reg1);
                // while (r >= p) r -= p;
                while (z >= pRep)
                    z.Subtract(pRep);
            }
        }

        public IReducer GetReducer(BigInteger n)
        {
            return new Reducer(n);
        }
    }
}
