using System;

namespace Decompose.Numerics
{
    public class UInt64MontgomeryReduction : UInt64Operations, IReductionAlgorithm<ulong>
    {
        private class Reducer : Reducer<UInt64MontgomeryReduction, ulong>
        {
            private class Residue : Residue<Reducer, ulong, ulong>
            {
                public override bool IsZero { get { return r == 0; } }

                public override bool IsOne
                {
                    get
                    {
                        if (reducer.oneRep == 0)
                            reducer.oneRep = reducer.Reduce(1, reducer.rSquaredModN);
                        return r == reducer.oneRep;
                    }
                }

                public Residue(Reducer reducer, ulong x)
                    : base(reducer)
                {
                    Set(x);
                }

                public override IResidue<ulong> Set(ulong x)
                {
                    r = reducer.Reduce(x % reducer.modulus, reducer.rSquaredModN);
                    return this;
                }

                public override IResidue<ulong> Set(IResidue<ulong> x)
                {
                    r = GetRep(x);
                    return this;
                }

                public override IResidue<ulong> Copy()
                {
                    return new Residue(reducer, r);
                }

                public override IResidue<ulong> Multiply(IResidue<ulong> x)
                {
                    r = reducer.Reduce(r, GetRep(x));
                    return this;
                }

                public override IResidue<ulong> Add(IResidue<ulong> x)
                {
                    r = UInt128.ModularSum(r, GetRep(x), reducer.modulus);
                    return this;
                }

                public override IResidue<ulong> Subtract(IResidue<ulong> x)
                {
                    r = UInt128.ModularDifference(r, GetRep(x), reducer.modulus);
                    return this;
                }

                public override ulong Value()
                {
                    return reducer.Reduce(r, 1);
                }
            }

            private uint k0;
            private ulong rSquaredModN;
            private ulong oneRep;

            public Reducer(UInt64MontgomeryReduction reduction, ulong modulus)
                : base(reduction, modulus)
            {
                if ((modulus & 1) == 0)
                    throw new InvalidOperationException("not relatively prime");
                int rLength = modulus == (uint)modulus ? 32 : 64;
                var rMinusOne = rLength == 32 ? uint.MaxValue : ulong.MaxValue;
                var rDivN = rMinusOne / modulus;
                var rModN = rMinusOne - rDivN * modulus + 1;
                rSquaredModN = IntegerMath.ModularProduct(rModN, rModN, modulus);

                if (modulus <= long.MaxValue)
                {
                    long c;
                    long d;
                    IntegerMath.ExtendedGreatestCommonDivisor((long)rModN, (long)modulus, out c, out d);
                    d = -(d - (long)rDivN * c);
                    var k = (d < 0 ? rMinusOne - (ulong)-d + 1 : (ulong)d);
                    k0 = (uint)k;
                }
                else
                {
#if false
                    var r = (BigInteger)1 << rLength;
                    var k = r - IntegerMath.ModularInverse(modulus, r);
                    k0 = (uint)(k & uint.MaxValue);
#endif
#if false
                    var store = new Word32IntegerStore(4);
                    var nRep = store.Allocate().Set(modulus);
                    var r = store.Allocate().Set(1).LeftShift(rLength);
                    var inv = store.Allocate().SetModularInverse(nRep, r, store);
                    var k = store.Allocate().Set(r).Subtract(inv);
                    k0 = k.LeastSignificantWord;
#endif
#if true
                    k0 = (uint)(((UInt128)IntegerMath.ModularInverse(rModN, modulus) << rLength) / modulus);
#endif
                }
            }

            public override IResidue<ulong> ToResidue(ulong x)
            {
                return new Residue(this, x);
            }

            private ulong Reduce(ulong u, ulong v)
            {
                return UInt128.Montgomery(u, v, modulus, k0);
            }
        }

        public IReducer<ulong> GetReducer(ulong modulus)
        {
            return new Reducer(this, modulus);
        }
    }
}
