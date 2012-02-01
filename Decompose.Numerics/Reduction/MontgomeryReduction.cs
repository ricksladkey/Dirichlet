using System;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class MontgomeryReduction : BigIntegerOperations, IReductionAlgorithm<BigInteger>
    {
        private class Reducer : IReducer<BigInteger>
        {
            private class Residue : IResidue<BigInteger>
            {
                private Reducer reducer;
                private Word32Integer r;

                public IReducer<BigInteger> Reducer { get { return reducer; } }
                public Word32Integer Rep { get { return r; } }
                public bool IsZero { get { return r == reducer.zeroRep; } }
                public bool IsOne { get { return r == reducer.oneRep; } }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, BigInteger x)
                    : this(reducer)
                {
                    r = reducer.CreateRep();
                    r.Set(x).Multiply(reducer.rSquaredModNRep, reducer.reg3);
                    reducer.Reduce(r);
                }

                public IResidue<BigInteger> Set(BigInteger x)
                {
                    r.Set(x).Multiply(reducer.rSquaredModNRep, reducer.reg3);
                    reducer.Reduce(r);
                    return this;
                }

                public IResidue<BigInteger> Set(IResidue<BigInteger> x)
                {
                    r.Set(((Residue)x).r);
                    return this;
                }

                public IResidue<BigInteger> Copy()
                {
                    var residue = new Residue(reducer);
                    residue.r = reducer.CreateRep();
                    residue.r.Set(r);
                    return residue;
                }

                public IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
#if false
                    // Use SOS for everything.
                    r.Multiply(((Residue)x).r, reducer.reg3);
                    reducer.Reduce(r);
                    return this;
#endif
#if false
                    // Use SOS for squaring and CIOS otherwise.
                    if (this == x)
                    {
                        r.Multiply(r, reducer.reg3);
                        reducer.Reduce(r);
                        return this;
                    }
                    else
                    {
                        reducer.reg3.Set(r);
                        reducer.Reduce(r, reducer.reg3, ((Residue)x).r);
                        return this;
                    }
#endif
#if true
                    // Use CIOS for everything.
                    reducer.reg3.Set(r);
                    if (this == x)
                        reducer.Reduce(r, reducer.reg3, reducer.reg3);
                    else
                        reducer.Reduce(r, reducer.reg3, ((Residue)x).r);
                    return this;
#endif
                }

                public IResidue<BigInteger> Add(IResidue<BigInteger> x)
                {
                    r.AddModulo(((Residue)x).r, reducer.nRep);
                    return this;
                }

                public IResidue<BigInteger> Subtract(IResidue<BigInteger> x)
                {
                    r.Subtract(((Residue)x).r);
                    return this;
                }

                public IResidue<BigInteger> Power(BigInteger x)
                {
                    ReductionHelper.Power(this, x);
                    return this;
                }

                public bool Equals(IResidue<BigInteger> other)
                {
                    return r == ((Residue)other).r;
                }

                public int CompareTo(IResidue<BigInteger> other)
                {
                    return r.CompareTo(((Residue)other).r);
                }

                public BigInteger Value()
                {
                    reducer.reg3.Set(r);
                    reducer.Reduce(reducer.reg3);
                    return reducer.reg3.ToBigInteger();
                }

                public override string ToString()
                {
                    return Value().ToString();
                }
            }

            private IReductionAlgorithm<BigInteger> reduction;
            private BigInteger n;
            private int rLength;
            private int length;
            private BigInteger k;
            private uint k0;
            private Word32IntegerStore store;

            private Word32Integer nRep;
            private Word32Integer rSquaredModNRep;
            private Word32Integer kRep;
            private Word32Integer reg1;
            private Word32Integer reg2;
            private Word32Integer reg3;
            private Word32Integer zeroRep;
            private Word32Integer oneRep;

            public IReductionAlgorithm<BigInteger> Reduction { get { return reduction; } }
            public BigInteger Modulus { get { return n; } }

            public Reducer(IReductionAlgorithm<BigInteger> reduction, BigInteger n)
            {
                this.reduction = reduction;
                this.n = n;
                if (n.IsEven)
                    throw new InvalidOperationException("not relatively prime");
                rLength = (n.GetBitLength() + 31) / 32 * 32;
                length = 2 * rLength / 32 + 1;
                var r = BigInteger.One << rLength;
                var rSquaredModN = r * r % n;
                k = r - IntegerMath.ModularInverse(n, r);
                k0 = (uint)(k & uint.MaxValue);

                store = new Word32IntegerStore(length);
                nRep = store.Create();
                rSquaredModNRep = store.Create();
                kRep = store.Create();
                reg1 = store.Create();
                reg2 = store.Create();
                reg3 = store.Create();
                zeroRep = store.Create();
                oneRep = store.Create();

                nRep.Set(n);
                rSquaredModNRep.Set(rSquaredModN);
                kRep.Set(k);
                zeroRep.Set(new Residue(this, BigInteger.Zero).Rep);
                oneRep.Set(new Residue(this, BigInteger.One).Rep);
            }

            public IResidue<BigInteger> ToResidue(BigInteger x)
            {
                return new Residue(this, x);
            }

            private Word32Integer CreateRep()
            {
                return new Word32Integer(length);
            }

            private void Reduce(Word32Integer t, Word32Integer u, Word32Integer v)
            {
                t.MontgomeryCIOS(u, v, nRep, k0);
                if (t >= nRep)
                    t.Subtract(nRep);
                Debug.Assert(t < nRep);
            }

            private void Reduce(Word32Integer t)
            {
                t.MontgomerySOS(nRep, k0);
                if (t >= nRep)
                    t.Subtract(nRep);
                Debug.Assert(t < nRep);
            }

            private void BasicReduce(Word32Integer t)
            {
                reg1.SetMasked(t, rLength);
                reg2.SetProductMasked(reg1, kRep, rLength);
                reg1.SetProduct(reg2, nRep);
                t.Add(reg1);
                t.RightShift(rLength);
                if (t >= nRep)
                    t.Subtract(nRep);
                Debug.Assert(t < nRep);
            }
        }

        public IReducer<BigInteger> GetReducer(BigInteger n)
        {
            return new Reducer(this, n);
        }
    }
}
