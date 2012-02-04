using System;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class MontgomeryReduction : BigIntegerOperations, IReductionAlgorithm<BigInteger>
    {
        private class Reducer : Reducer<MontgomeryReduction, BigInteger>
        {
            private class Residue : Residue<Reducer, BigInteger, Word32Integer>
            {
                public override bool IsZero { get { return r == 0; } }
                public override bool IsOne { get { return r == reducer.oneRep; } }

                public Residue(Reducer reducer, BigInteger x)
                    : base(reducer, reducer.CreateRep())
                {
                    Set(x);
                }

                public override IResidue<BigInteger> Set(BigInteger x)
                {
                    r.Set(x).Multiply(reducer.rSquaredModNRep, reducer.store);
                    reducer.Reduce(r);
                    return this;
                }

                public override IResidue<BigInteger> Set(IResidue<BigInteger> x)
                {
                    r.Set(GetRep(x));
                    return this;
                }

                public override IResidue<BigInteger> Copy()
                {
                    var residue = new Residue(reducer, reducer.CreateRep());
                    residue.r.Set(r);
                    return residue;
                }

#if false
                public override IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
                    // Use SOS for everything.
                    r.Multiply(GetRep(x), reducer.store);
                    reducer.Reduce(r);
                    return this;
                }
#endif

#if false
                public override IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
                    // Use SOS for squaring and CIOS otherwise.
                    if (x == this)
                    {
                        r.Multiply(r, reducer.store);
                        reducer.Reduce(r);
                    }
                    else
                    {
                        var reg1 = reducer.store.Allocate().Set(r);
                        reg1.Set(r);
                        reducer.Reduce(r, reg1, GetRep(x));
                    }
                    return this;
                }
#endif

#if true
                public override IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
                    // Use CIOS for everything.
                    var reg1 = reducer.store.Allocate().Set(r);
                    if (x == this)
                        reducer.Reduce(r, reg1, reg1);
                    else
                        reducer.Reduce(r, reg1, GetRep(x));
                    return this;
                }
#endif

                public override IResidue<BigInteger> Add(IResidue<BigInteger> x)
                {
                    r.AddModulo(GetRep(x), reducer.nRep);
                    return this;
                }

                public override IResidue<BigInteger> Subtract(IResidue<BigInteger> x)
                {
                    r.Subtract(GetRep(x));
                    return this;
                }

                public override BigInteger Value()
                {
                    var reg1 = reducer.store.Allocate().Set(r);
                    reg1.Set(r);
                    reducer.Reduce(reg1);
                    var result = (BigInteger)reg1;
                    reducer.store.Release(reg1);
                    return result;
                }
            }

            private int length;
            private uint k0;
            private Word32IntegerStore store;

            private Word32Integer nRep;
            private Word32Integer rSquaredModNRep;
            private Word32Integer oneRep;

            public Reducer(MontgomeryReduction reduction, BigInteger modulus)
                : base(reduction, modulus)
            {
                if (modulus.IsEven)
                    throw new InvalidOperationException("not relatively prime");
                var rLength = (modulus.GetBitLength() + 31) / 32 * 32;
                length = 2 * rLength / 32 + 1;
                var r = BigInteger.One << rLength;
                var rSquaredModN = r * r % modulus;

                store = new Word32IntegerStore(length);
                nRep = store.Allocate().Set(modulus);
                rSquaredModNRep = store.Allocate().Set(rSquaredModN);

                nRep.Set(modulus);
                rSquaredModNRep.Set(rSquaredModN);
#if false
                var k = r - IntegerMath.ModularInverse(modulus, r);
                k0 = (uint)(k & uint.MaxValue);
#endif
#if true
                var rRep = store.Allocate().Set(r);
                var nInv = store.Allocate().SetModularInverse(nRep, rRep, store);
                var kRep = store.Allocate().Set(rRep).Subtract(nInv);
                k0 = kRep.LeastSignificantWord;
                store.Release(rRep);
                store.Release(nInv);
                store.Release(kRep);
#endif
                oneRep = store.Allocate().Set(1).Multiply(rSquaredModNRep, store);
                Reduce(oneRep);
            }

            public override IResidue<BigInteger> ToResidue(BigInteger x)
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
