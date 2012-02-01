using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class UInt64Reduction : UInt64Operations, IReductionAlgorithm<ulong>
    {
        private class Reducer : IReducer<ulong>
        {
            private class Residue : IResidue<ulong>
            {
                private Reducer reducer;
                private ulong r;

                public IReducer<ulong> Reducer { get { return reducer; } }
                public bool IsZero { get { return r == 0; } }
                public bool IsOne { get { return r == 1; } }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, ulong x)
                    : this(reducer)
                {
                    this.r = x % reducer.Modulus;
                }

                public IResidue<ulong> Set(ulong x)
                {
                    r = x;
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
                    r = UInt128.ModularProduct(r, ((Residue)x).r, reducer.Modulus);
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
                    r = IntegerMath.ModularPower(r, x, reducer.Modulus);
                    return this;
                }

                public ulong Value()
                {
                    return r;
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

            public IReductionAlgorithm<ulong> Reduction { get { return reduction; } }
            public ulong Modulus { get { return n; } }

            public Reducer(IReductionAlgorithm<ulong> reduction, ulong n)
            {
                this.reduction = reduction;
                this.n = n;
            }

            public IResidue<ulong> ToResidue(ulong x)
            {
                return new Residue(this, x);
            }
        }

        public IReducer<ulong> GetReducer(ulong n)
        {
            return new Reducer(this, n);
        }
    }
}
