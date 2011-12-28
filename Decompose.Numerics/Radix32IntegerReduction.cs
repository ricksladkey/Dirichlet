using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class Radix32IntegerReduction : IReductionAlgorithm
    {
        private class Reducer : IReducer
        {
            private class Residue : IResidue
            {
                private Reducer reducer;
                private Radix32Integer r;

                public bool IsZero { get { return r.IsZero; } }

                public bool IsOne { get { return r.IsOne; } }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, BigInteger x)
                    : this(reducer)
                {
                    r = reducer.CreateRep();
                    r.Set(x);
                    reducer.Reduce(r);
                }

                public IResidue Set(IResidue x)
                {
                    r.Set(((Residue)x).r);
                    return this;
                }

                public IResidue Copy()
                {
                    var residue = new Residue(reducer);
                    residue.r = reducer.CreateRep();
                    residue.r.Set(r);
                    return residue;
                }

                public IResidue Multiply(IResidue x)
                {
                    reducer.reg3.Set(r);
                    if (this == x)
                        r.SetSquare(reducer.reg3);
                    else
                        r.SetProduct(reducer.reg3, ((Residue)x).r);
                    reducer.Reduce(r);
                    return this;
                }

                public IResidue Add(IResidue x)
                {
                    r.Add(((Residue)x).r);
                    if (r.CompareTo(reducer.nRep) >= 0)
                        r.Subtract(reducer.nRep);
                    return this;
                }

                public IResidue Subtract(IResidue x)
                {
                    r.Subtract(((Residue)x).r);
                    return this;
                }

                public bool Equals(IResidue other)
                {
                    return r.CompareTo(((Residue)other).r) == 0;
                }

                public int CompareTo(IResidue other)
                {
                    return r.CompareTo(((Residue)other).r);
                }

                public BigInteger ToBigInteger()
                {
                    return r.ToBigInteger();
                }

                public override string ToString()
                {
                    return ToBigInteger().ToString();
                }
            }

            private BigInteger n;
            private int length;
            private Radix32Store store;

            private Radix32Integer nRep;
            private Radix32Integer reg1;
            private Radix32Integer reg2;
            private Radix32Integer reg3;

            public BigInteger Modulus
            {
                get { return n; }
            }

            public Reducer(BigInteger n)
            {
                this.n = n;
                length = (n.GetBitLength() + 31) / 32 * 2 + 1;
                store = new Radix32Store(length);
                nRep = CreateRep();
                nRep.Set(n);
                reg1 = CreateRep();
                reg2 = CreateRep();
                reg3 = CreateRep();
            }

            public IResidue ToResidue(BigInteger x)
            {
                return new Residue(this, x);
            }

            private Radix32Integer CreateRep()
            {
                return new Radix32Integer(length);
            }

            private void Reduce(Radix32Integer r)
            {
                r.Modulo(nRep, reg1, reg2);
            }
        }

        public IReducer GetReducer(BigInteger n)
        {
            return new Reducer(n);
        }
    }
}
