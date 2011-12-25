using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class MontgomeryReduction : IReductionAlgorithm
    {
        private class Reducer : IReducer
        {
            private class Residue : IResidue
            {
                private Reducer reducer;
                private Radix32Integer r;

                public Radix32Integer Rep { get { return r; } }

                public bool IsZero { get { return r.CompareTo(reducer.zeroRep) == 0; } }

                public bool IsOne { get { return r.CompareTo(reducer.oneRep) == 0; } }

                protected Residue(Reducer reducer)
                {
                    this.reducer = reducer;
                }

                public Residue(Reducer reducer, BigInteger x)
                    : this(reducer)
                {
                    reducer.reg3.Set(x % reducer.n);
                    r = reducer.CreateRep();
                    r.SetProduct(reducer.reg3, reducer.rSquaredModNRep);
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
                    reducer.reg1.Set(r);
                    r.SetProduct(reducer.reg1, x == this ? reducer.reg1 : ((Residue)x).r);
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
                    reducer.reg3.Set(r);
                    reducer.Reduce(reducer.reg3);
                    return reducer.reg3.ToBigInteger();
                }

                public override string ToString()
                {
                    return ToBigInteger().ToString() + " (" + r.ToBigInteger() + ")";
                }
            }

            private BigInteger n;
            private int rLength;
            private int length;
            private BigInteger r;
            private BigInteger rMinusOne;
            private BigInteger rSquaredModN;
            private BigInteger rInverse;
            private BigInteger k;
            private uint[] bits;

            private Radix32Integer nRep;
            private Radix32Integer rSquaredModNRep;
            private Radix32Integer kRep;
            private Radix32Integer reg1;
            private Radix32Integer reg2;
            private Radix32Integer reg3;
            private Radix32Integer zeroRep;
            private Radix32Integer oneRep;

            public BigInteger Modulus
            {
                get { return n; }
            }

            public Reducer(BigInteger n)
            {
                this.n = n;
                rLength = (BigIntegerUtils.GetBitLength(n) + 31) / 32 * 32;
                length = (rLength * 2 + 31) / 32;
                r = BigInteger.One << rLength;
                rMinusOne = r - BigInteger.One;
                rSquaredModN = r * r % n;
                var results = BigIntegerUtils.ExtendedGreatestCommonDivisor(r, n);
                rInverse = results[0];
                k = -results[1];
                if (rInverse.Sign == -1)
                    rInverse += n;
                if (k.Sign == -1)
                    k += r;
                Debug.Assert(r * rInverse == k * n + 1);

                bits = new uint[8 * length];
                nRep = new Radix32Integer(bits, 0 * length, length);
                rSquaredModNRep = new Radix32Integer(bits, 1 * length, length);
                kRep = new Radix32Integer(bits, 2 * length, length);
                reg1 = new Radix32Integer(bits, 3 * length, length);
                reg2 = new Radix32Integer(bits, 4 * length, length);
                reg3 = new Radix32Integer(bits, 5 * length, length);
                zeroRep = new Radix32Integer(bits, 6 * length, length);
                oneRep = new Radix32Integer(bits, 7 * length, length);

                nRep.Set(n);
                rSquaredModNRep.Set(rSquaredModN);
                kRep.Set(k);
                zeroRep.Set(new Residue(this, BigInteger.Zero).Rep);
                oneRep.Set(new Residue(this, BigInteger.One).Rep);
            }

            public IResidue ToResidue(BigInteger x)
            {
                return new Residue(this, x);
            }

            private Radix32Integer CreateRep()
            {
                return new Radix32Integer(new uint[length], 0, length);
            }

            private void Reduce(Radix32Integer t)
            {
                reg1.Set(t);
                reg1.Mask(rLength);
                reg2.SetProduct(reg1, kRep);
                reg2.Mask(rLength);
                reg1.SetProduct(reg2, nRep);
                t.Add(reg1);
                t.RightShift(rLength);
                if (t.CompareTo(nRep) >= 0)
                    t.Subtract(nRep);
            }
        }

        public IReducer GetReducer(BigInteger n)
        {
            return new Reducer(n);
        }
    }
}
