using System;
using System.Diagnostics;
using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt128MontgomeryReduction : UInt64Operations, IReductionAlgorithm<ulong>
    {
        private class Reducer : IReducer<ulong>
        {
            private class Residue : IResidue<ulong>
            {
                private Reducer reducer;
                private ulong r;

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
            private uint k0;
            private ulong rSquaredModN;
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
                var rLength = n == (uint)n ? 32 : 64;
                var rMinusOne = rLength == 32 ? uint.MaxValue : ulong.MaxValue;
                var rDivN = rMinusOne / n;
                var rModN = rMinusOne - rDivN * n + 1;
                rSquaredModN = IntegerMath.ModularProduct(rModN, rModN, n);
#if true
                long c;
                long d;
                IntegerMath.ExtendedGreatestCommonDivisor((long)rModN, (long)n, out c, out d);
                d = d - (long)rDivN * c;
                var rInverse = (ulong)(c < 0 ? c + (long)n : c);
                var k = (-d < 0 ? rMinusOne - (ulong)d + 1 : (ulong)-d);
                Debug.Assert(((BigInteger)rMinusOne + 1) * rInverse == (BigInteger)k * n + 1);
                k0 = (uint)k;
#else
                k0 = (uint)(((UInt128)IntegerMath.ModularInverse(rModN, n) << rLength) / n);
#endif
#if false
                var r = BigInteger.One << rLength;
                BigInteger cPrime;
                BigInteger dPrime;
                IntegerMath.ExtendedGreatestCommonDivisor(r, n, out cPrime, out dPrime);
                var rInversePrime = cPrime < 0 ? cPrime + (long)n : cPrime;
                var kPrime = -dPrime < 0 ? -dPrime + r : -dPrime;
                Debug.Assert(k == kPrime);
                Debug.Assert(rInversePrime == rInverse);
#endif
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
