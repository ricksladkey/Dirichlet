using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class BigIntegerReduction : IReductionAlgorithm
    {
        private class Reducer : IReducer
        {
            private class Residue : IResidue
            {
                private Reducer reducer;
                private BigInteger r;

                public bool IsZero { get { return r.IsZero; } }

                public bool IsOne { get { return r.IsOne; } }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, BigInteger x)
                    : this(reducer)
                {
                    this.r = x % reducer.Modulus;
                }

                public IResidue Set(IResidue x)
                {
                    r = ((Residue)x).r;
                    return this;
                }

                public IResidue Copy()
                {
                    var residue = new Residue(reducer);
                    residue.r = r;
                    return residue;
                }

                public IResidue Multiply(IResidue x)
                {
                    r = r * ((Residue)x).r % reducer.Modulus;
                    return this;
                }

                public IResidue Add(IResidue x)
                {
                    r += ((Residue)x).r;
                    if (r > reducer.Modulus)
                        r -= reducer.Modulus;
                    return this;
                }

                public IResidue Subtract(IResidue x)
                {
                    r -= ((Residue)x).r;
                    if (r < reducer.Modulus)
                        r += reducer.Modulus;
                    return this;
                }

                public BigInteger ToBigInteger()
                {
                    return r;
                }

                public override string ToString()
                {
                    return ToBigInteger().ToString();
                }

                public bool Equals(IResidue other)
                {
                    return r == ((Residue)other).r;
                }

                public int CompareTo(IResidue other)
                {
                    return r.CompareTo(((Residue)other).r);
                }
            }

            private BigInteger n;

            public BigInteger Modulus
            {
                get { return n; }
            }

            public Reducer(BigInteger n)
            {
                this.n = n;
            }

            public IResidue ToResidue(BigInteger x)
            {
                return new Residue(this, x);
            }
        }

        public IReducer GetReducer(BigInteger n)
        {
            return new Reducer(n);
        }
    }
}
