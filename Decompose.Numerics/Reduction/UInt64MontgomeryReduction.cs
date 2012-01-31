using System;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt64MontgomeryReduction : UInt64Operations, IReductionAlgorithm<ulong>
    {
        private class Reducer : IReducer<ulong>
        {
            private class Residue : IResidue<ulong>
            {
                private Reducer reducer;
                private ulong r;

                public IReducer<ulong> Reducer { get { return reducer; } }
                public ulong Rep { get { return r; } }
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

                public Residue(Reducer reducer, ulong x)
                    : this(reducer)
                {
                    r = reducer.Reduce(x % reducer.n, reducer.rSquaredModN);
                }

                public IResidue<ulong> Set(ulong x)
                {
                    r = reducer.Reduce(x % reducer.n, reducer.rSquaredModN);
                    return this;
                }

                public IResidue<ulong> Set(IResidue<ulong> x)
                {
                    r = ((Residue)x).r;
                    return this;
                }

                public IResidue<ulong> Copy()
                {
                    var residue = new Residue(reducer);
                    residue.r = r;
                    return residue;
                }

                public IResidue<ulong> Multiply(IResidue<ulong> x)
                {
                    r = reducer.Reduce(r, ((Residue)x).r);
                    return this;
                }

                public IResidue<ulong> Add(IResidue<ulong> x)
                {
                    r += ((Residue)x).r;
                    if (r >= reducer.Modulus)
                        r -= reducer.Modulus;
                    return this;
                }

                public IResidue<ulong> Subtract(IResidue<ulong> x)
                {
                    var xr = ((Residue)x).r;
                    if (r < xr)
                        r += reducer.Modulus - xr;
                    else
                        r -= xr;
                    return this;
                }

                public IResidue<ulong> Power(ulong x)
                {
                    ResidueHelper.ModularPower(this, x);
                    return this;
                }

                public ulong Value()
                {
                    return reducer.Reduce(r, 1);
                }

                public override string ToString()
                {
                    return Value().ToString();
                }

                public bool Equals(IResidue<ulong> other)
                {
                    return r == ((Residue)other).r;
                }

                public int CompareTo(IResidue<ulong> other)
                {
                    return r.CompareTo(((Residue)other).r);
                }
            }

            private IReductionAlgorithm<ulong> reduction;
            private ulong n;
            private uint k0;
            private ulong rSquaredModN;
            private ulong oneRep;

            public IReductionAlgorithm<ulong> Reduction { get { return reduction; } }
            public ulong Modulus { get { return n; } }

            public Reducer(IReductionAlgorithm<ulong> reduction, ulong n)
            {
                this.reduction = reduction;
                this.n = n;
                if ((n & 1) == 0)
                    throw new InvalidOperationException("not relatively prime");
                if (n == (uint)n)
                    throw new NotSupportedException("single word modulus");
                var rMinusOne = ulong.MaxValue;
                var rDivN = rMinusOne / n;
                var rModN = rMinusOne - rDivN * n + 1;
                rSquaredModN = IntegerMath.ModularProduct(rModN, rModN, n);

                //k0 = (uint)(((UInt128)IntegerMath.ModularInverse(rModN, n) << 64) / n);
                long c;
                long d;
                IntegerMath.ExtendedGreatestCommonDivisor((long)rModN, (long)n, out c, out d);
                d -= (long)rDivN * c;
                var k = (-d < 0 ? rMinusOne - (ulong)d + 1 : (ulong)-d);
                k0 = (uint)k;
            }

            public IResidue<ulong> ToResidue(ulong x)
            {
                return new Residue(this, x);
            }

            private ulong Reduce(ulong u, ulong v)
            {
                return UInt128.Montgomery(u, v, n, k0);
            }
        }

        public IReducer<ulong> GetReducer(ulong n)
        {
            return new Reducer(this, n);
        }
    }
}
