using System;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt32MontgomeryReduction : UInt32Operations, IReductionAlgorithm<uint>
    {
        private class Reducer : IReducer<uint>
        {
            private class Residue : IResidue<uint>
            {
                private Reducer reducer;
                private uint r;

                public IReducer<uint> Reducer { get { return reducer; } }
                public uint Rep { get { return r; } }
                public bool IsZero { get { return r == 0; } }

                public bool IsOne
                {
                    get
                    {
                        if (reducer.oneRep == 0)
                            reducer.oneRep = reducer.Reduce(1, reducer.rSquaredModN);
                        return r == reducer.oneRep;
                    }
                }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, uint x)
                    : this(reducer)
                {
                    r = reducer.Reduce(x % reducer.n, reducer.rSquaredModN);
                }

                public IResidue<uint> Set(uint x)
                {
                    r = reducer.Reduce(x % reducer.n, reducer.rSquaredModN);
                    return this;
                }

                public IResidue<uint> Set(IResidue<uint> x)
                {
                    r = ((Residue)x).r;
                    return this;
                }

                public IResidue<uint> Copy()
                {
                    var residue = new Residue(reducer);
                    residue.r = r;
                    return residue;
                }

                public IResidue<uint> Multiply(IResidue<uint> x)
                {
                    r = reducer.Reduce(r, ((Residue)x).r);
                    return this;
                }

                public IResidue<uint> Add(IResidue<uint> x)
                {
                    r += ((Residue)x).r;
                    if (r >= reducer.Modulus)
                        r -= reducer.Modulus;
                    return this;
                }

                public IResidue<uint> Subtract(IResidue<uint> x)
                {
                    var xr = ((Residue)x).r;
                    if (r < xr)
                        r += reducer.Modulus - xr;
                    else
                        r -= xr;
                    return this;
                }

                public IResidue<uint> Power(uint x)
                {
                    ResidueHelper.ModularPower(this, x);
                    return this;
                }

                public uint Value()
                {
                    return reducer.Reduce(r, 1);
                }

                public override string ToString()
                {
                    return Value().ToString();
                }

                public bool Equals(IResidue<uint> other)
                {
                    return r == ((Residue)other).r;
                }

                public int CompareTo(IResidue<uint> other)
                {
                    return r.CompareTo(((Residue)other).r);
                }
            }

            private IReductionAlgorithm<uint> reduction;
            private uint n;
            private uint k0;
            private uint rSquaredModN;
            private uint oneRep;

            public IReductionAlgorithm<uint> Reduction { get { return reduction; } }
            public uint Modulus { get { return n; } }

            public Reducer(IReductionAlgorithm<uint> reduction, uint n)
            {
                this.reduction = reduction;
                this.n = n;
                if ((n & 1) == 0)
                    throw new InvalidOperationException("not relatively prime");
                var rMinusOne = uint.MaxValue;
                var rDivN = rMinusOne / n;
                var rModN = rMinusOne - rDivN * n + 1;
                rSquaredModN = IntegerMath.ModularProduct(rModN, rModN, n);

                //k0 = (uint)(((UInt128)IntegerMath.ModularInverse(rModN, n) << 32) / n);
                long c;
                long d;
                IntegerMath.ExtendedGreatestCommonDivisor((long)rModN, (long)n, out c, out d);
                d -= (long)rDivN * c;
                var k = (-d < 0 ? rMinusOne - (uint)d + 1 : (uint)-d);
                k0 = (uint)k;
            }

            public IResidue<uint> ToResidue(uint x)
            {
                return new Residue(this, x);
            }

            private uint Reduce(uint u, uint v)
            {
                return UInt128.Montgomery(u, v, n, k0);
            }
        }

        public IReducer<uint> GetReducer(uint n)
        {
            return new Reducer(this, n);
        }
    }
}
