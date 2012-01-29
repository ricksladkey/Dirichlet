using System;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt128MontgomeryReduction : IReductionAlgorithm<ulong>
    {
        private class Reducer : IReducer<ulong>
        {
            private class Residue : IResidue<ulong>
            {
                private Reducer reducer;
                private ulong r;

                public ulong Rep { get { return r; } }

                public bool IsZero { get { return r == reducer.zeroRep; } }

                public bool IsOne { get { return r == reducer.oneRep; } }

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
                    if (r > reducer.Modulus)
                        r -= reducer.Modulus;
                    return this;
                }

                public IResidue<ulong> Subtract(IResidue<ulong> x)
                {
                    r -= ((Residue)x).r;
                    if (r < reducer.Modulus)
                        r += reducer.Modulus;
                    return this;
                }

                public ulong ToInteger()
                {
                    return reducer.Reduce(r, 1);
                }

                public override string ToString()
                {
                    return ToInteger().ToString();
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

            private ulong n;
            private int rLength;
            private ulong k;
            private uint k0;
            private ulong rSquaredModN;
            private ulong zeroRep;
            private ulong oneRep;

            public ulong Modulus
            {
                get { return n; }
            }

            public Reducer(ulong n)
            {
                this.n = n;
                if ((n & 1) == 0)
                    throw new InvalidOperationException("not relatively prime");
                rLength = (n.GetBitLength() + 31) / 32 * 32;
                var r = (BigInteger)1 << rLength;
                rSquaredModN = (ulong)(r * r % n);
                BigInteger c;
                BigInteger d;
                IntegerMath.ExtendedGreatestCommonDivisor(r, n, out c, out d);
                var rInverse = (ulong)(c < 0 ? c + (long)n : c);
                var k = (ulong)(-d < 0 ? -d + r : -d);
                Debug.Assert(r * rInverse == k * n + 1);
                k0 = (uint)k;
                zeroRep = new Residue(this, 0).Rep;
                oneRep = new Residue(this, 1).Rep;
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
            return new Reducer(n);
        }
    }
}
