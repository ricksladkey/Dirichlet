using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class Word32IntegerReduction : BigIntegerOperations, IReductionAlgorithm<BigInteger>
    {
        private class Reducer : IReducer<BigInteger>
        {
            private class Residue : IResidue<BigInteger>
            {
                private Reducer reducer;
                private Word32Integer r;

                public IReducer<BigInteger> Reducer { get { return reducer; } }
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

                public IResidue<BigInteger> Set(BigInteger x)
                {
                    r.Set(x);
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

                public IResidue<BigInteger> Multiply(IResidue<BigInteger> x)
                {
                    reducer.reg3.Set(r);
                    if (this == x)
                        r.SetSquare(reducer.reg3);
                    else
                        r.SetProduct(reducer.reg3, ((Residue)x).r);
                    reducer.Reduce(r);
                    return this;
                }

                public IResidue<BigInteger> Add(IResidue<BigInteger> x)
                {
                    r.AddModulo(((Residue)x).r, reducer.nRep);
                    return this;
                }

                public IResidue<BigInteger> Subtract(IResidue<BigInteger> x)
                {
                    r.SubtractModulo(((Residue)x).r, reducer.nRep);
                    return this;
                }

                public bool Equals(IResidue<BigInteger> other)
                {
                    return r.CompareTo(((Residue)other).r) == 0;
                }

                public int CompareTo(IResidue<BigInteger> other)
                {
                    return r.CompareTo(((Residue)other).r);
                }

                public BigInteger Value()
                {
                    return r.ToBigInteger();
                }

                public override string ToString()
                {
                    return Value().ToString();
                }
            }

            private IReductionAlgorithm<BigInteger> reduction;
            private BigInteger n;
            private int length;
            private Word32IntegerStore store;

            private Word32Integer nRep;
            private Word32Integer reg1;
            private Word32Integer reg2;
            private Word32Integer reg3;

            public IReductionAlgorithm<BigInteger> Reduction { get { return reduction; } }
            public BigInteger Modulus { get { return n; } }

            public Reducer(IReductionAlgorithm<BigInteger> reduction, BigInteger n)
            {
                this.reduction = reduction;
                this.n = n;
                length = (n.GetBitLength() + 31) / 32 * 2 + 1;
                store = new Word32IntegerStore(length);
                nRep = store.Create();
                reg1 = store.Create();
                reg2 = store.Create();
                reg3 = store.Create();
                nRep.Set(n);
            }

            public IResidue<BigInteger> ToResidue(BigInteger x)
            {
                return new Residue(this, x);
            }

            private Word32Integer CreateRep()
            {
                return store.Create();
            }

            private void Reduce(Word32Integer r)
            {
                r.Modulo(nRep);
            }
        }

        public IReducer<BigInteger> GetReducer(BigInteger n)
        {
            return new Reducer(this, n);
        }
    }
}
