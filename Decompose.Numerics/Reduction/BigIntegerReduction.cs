using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class BigIntegerReduction : BigIntegerOperations, IReductionAlgorithm<BigInteger>
    {
        private class Reducer : IReducer<BigInteger>
        {
            private class Residue : IResidue<BigInteger>
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

                public IResidue<BigInteger> Set(BigInteger x)
                {
                    r = x;
                    return this;
                }

                public IResidue<BigInteger> Set(IResidue<BigInteger> x)
                {
                    r = ((Residue)x).r;
                    return this;
                }

                public IResidue<BigInteger> Copy()
                {
                    var residue = new Residue(reducer);
                    residue.r = r;
                    return residue;
                }

                public IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
                    r = r * ((Residue)x).r % reducer.Modulus;
                    return this;
                }

                public IResidue<BigInteger> Add(IResidue<BigInteger> x)
                {
                    r += ((Residue)x).r;
                    if (r >= reducer.Modulus)
                        r -= reducer.Modulus;
                    return this;
                }

                public IResidue<BigInteger> Subtract(IResidue<BigInteger> x)
                {
                    r -= ((Residue)x).r;
                    if (r < 0)
                        r += reducer.Modulus;
                    return this;
                }

                public BigInteger Value()
                {
                    return r;
                }

                public override string ToString()
                {
                    return Value().ToString();
                }

                public bool Equals(IResidue<BigInteger> other)
                {
                    return r == ((Residue)other).r;
                }

                public int CompareTo(IResidue<BigInteger> other)
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

            public IResidue<BigInteger> ToResidue(BigInteger x)
            {
                return new Residue(this, x);
            }
        }

        public IReducer<BigInteger> GetReducer(BigInteger n)
        {
            return new Reducer(n);
        }
    }
}
