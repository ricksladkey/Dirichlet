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
                public bool IsZero { get { return r == 0; } }
                public bool IsOne { get { return r == reducer.oneRep; } }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, BigInteger x)
                    : this(reducer)
                {
                    r = reducer.CreateRep();
                    r.Set(x).Multiply(reducer.rSquaredModNRep, reducer.store);
                    reducer.Reduce(r);
                }

                public IResidue<BigInteger> Set(BigInteger x)
                {
                    r.Set(x).Multiply(reducer.rSquaredModNRep, reducer.store);
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

#if false
                public IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
                    // Use SOS for everything.
                    r.Multiply(((Residue)x).r, reducer.store);
                    reducer.Reduce(r);
                    return this;
                }
#endif

#if false
                public IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
                    // Use SOS for squaring and CIOS otherwise.
                    if (this == x)
                    {
                        r.Multiply(r, reducer.store);
                        reducer.Reduce(r);
                    }
                    else
                    {
                        var reg1 = reducer.store.Allocate().Set(r);
                        reg1.Set(r);
                        reducer.Reduce(r, reg1, ((Residue)x).r);
                    }
                    return this;
                }
#endif

#if true
                public IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
                    // Use CIOS for everything.
                    var reg1 = reducer.store.Allocate().Set(r);
                    if (this == x)
                        reducer.Reduce(r, reg1, reg1);
                    else
                        reducer.Reduce(r, reg1, ((Residue)x).r);
                    return this;
                }
#endif

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
                    var reg1 = reducer.store.Allocate().Set(r);
                    reg1.Set(r);
                    reducer.Reduce(reg1);
                    var result = (BigInteger)reg1;
                    reducer.store.Release(reg1);
                    return result;
                }

                public override string ToString()
                {
                    return Value().ToString();
                }
            }

            private IReductionAlgorithm<BigInteger> reduction;
            private BigInteger n;
            private int length;
            private uint k0;
            private Word32IntegerStore store;

            private Word32Integer nRep;
            private Word32Integer rSquaredModNRep;
            private Word32Integer oneRep;

            public IReductionAlgorithm<BigInteger> Reduction { get { return reduction; } }
            public BigInteger Modulus { get { return n; } }

            public Reducer(IReductionAlgorithm<BigInteger> reduction, BigInteger n)
            {
                this.reduction = reduction;
                this.n = n;
                if (n.IsEven)
                    throw new InvalidOperationException("not relatively prime");
                var rLength = (n.GetBitLength() + 31) / 32 * 32;
                length = 2 * rLength / 32 + 1;
                var r = BigInteger.One << rLength;
                var rSquaredModN = r * r % n;
#if false
                var k = r - IntegerMath.ModularInverse(n, r);
                k0 = (uint)(k & uint.MaxValue);
#endif

                store = new Word32IntegerStore(length);
                nRep = store.Allocate().Set(n);
                rSquaredModNRep = store.Allocate().Set(rSquaredModN);
                oneRep = store.Allocate().Set(new Residue(this, BigInteger.One).Rep);

                nRep.Set(n);
                rSquaredModNRep.Set(rSquaredModN);
#if true
                var rRep = store.Allocate().Set(r);
                var nInv = store.Allocate().SetModularInverse(nRep, rRep, store);
                var kRep = store.Allocate().Set(r).Subtract(nInv);
                k0 = kRep.LeastSignificantWord;
                store.Release(nInv);
                store.Release(kRep);
                store.Release(rRep);
#endif
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
        }

        public IReducer<BigInteger> GetReducer(BigInteger n)
        {
            return new Reducer(this, n);
        }
    }
}
