using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class UInt32Reduction : UInt32Operations, IReductionAlgorithm<uint>
    {
        private class Reducer : IReducer<uint>
        {
            private class Residue : IResidue<uint>
            {
                private Reducer reducer;
                private uint r;

                public IReducer<uint> Reducer { get { return reducer; } }
                public bool IsZero { get { return r == 0; } }
                public bool IsOne { get { return r == 1; } }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, uint x)
                    : this(reducer)
                {
                    this.r = x % reducer.Modulus;
                }

                public IResidue<uint> Set(uint x)
                {
                    r = x;
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
                    r = IntegerMath.ModularProduct(r, ((Residue)x).r, reducer.Modulus);
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
                    r = IntegerMath.ModularPower(r, x, reducer.Modulus);
                    return this;
                }

                public uint Value()
                {
                    return r;
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

            public IReductionAlgorithm<uint> Reduction { get { return reduction; } }
            public uint Modulus { get { return n; } }

            public Reducer(IReductionAlgorithm<uint> reduction, uint n)
            {
                this.reduction = reduction;
                this.n = n;
            }

            public IResidue<uint> ToResidue(uint x)
            {
                return new Residue(this, x);
            }
        }

        public IReducer<uint> GetReducer(uint n)
        {
            return new Reducer(this, n);
        }
    }
}
