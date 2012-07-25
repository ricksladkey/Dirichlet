using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        private const int maxShift = 64;
        private static double log2 = Math.Log(2);

        private const int maxRepShift = 53;
        private static readonly long maxRep = (long)1 << maxRepShift;
        private static readonly BigInteger maxRepSquared = (BigInteger)maxRep * maxRep;

        public static int FloorSquareRoot(int n)
        {
            return (int)Math.Floor(Math.Sqrt(n));
        }

        public static uint FloorSquareRoot(uint n)
        {
            return (uint)Math.Floor(Math.Sqrt(n));
        }

        public static long FloorSquareRoot(long a)
        {
            if (a <= maxRep)
                return (long)Math.Floor(Math.Sqrt((double)a));
            var s = (long)Math.Floor(Math.Sqrt((double)a));
            var r = a - s * s;
            if (r < 0)
                --s;
            else if (r > (s << 1)) // r >= 2 * s + 1
                ++s;
            Debug.Assert(FloorSquareRoot<BigInteger>(a) == s);
            return s;
        }

        public static long CeilingSquareRoot(long a)
        {
            if (a <= maxRep)
                return (long)Math.Ceiling(Math.Sqrt((double)a));
            var s = (long)Math.Ceiling(Math.Sqrt((double)a));
            var r = s * s - a;
            if (r < 0)
                ++s;
            else if (r > (s << 1)) // r >= 2 * s + 1
                --s;
            Debug.Assert(CeilingSquareRoot<BigInteger>(a) == s);
            return s;
        }

        public static ulong FloorSquareRoot(ulong a)
        {
            return (ulong)FloorSquareRoot((long)a);
        }

        public static ulong CeilingSquareRoot(ulong a)
        {
            return (ulong)CeilingSquareRoot((long)a);
        }

        public static BigInteger FloorSquareRoot(BigInteger a)
        {
            if (a <= maxRep)
                return (BigInteger)Math.Floor(Math.Sqrt((double)a));
            else if (a <= maxRepSquared)
            {
                var s = (BigInteger)Math.Floor(Math.Sqrt((double)a));
                var r = a - s * s;
                if (r.Sign == -1)
                    --s;
                else if (r > (s << 1)) // r >= 2 * s + 1
                    ++s;
                Debug.Assert(FloorSquareRoot<BigInteger>(a) == s);
                return s;
            }
            return FloorSquareRoot<BigInteger>(a);
        }

        public static BigInteger CeilingSquareRoot(BigInteger a)
        {
            if (a <= maxRep)
                return (BigInteger)Math.Ceiling(Math.Sqrt((double)a));
            else if (a <= maxRepSquared)
            {
                var s = (BigInteger)Math.Ceiling(Math.Sqrt((double)a));
                var r = s * s - a;
                if (r.Sign == -1)
                    ++s;
                else if (r > (s << 1)) // r >= 2 * s + 1
                    --s;
                Debug.Assert(CeilingSquareRoot<BigInteger>(a) == s);
                return s;
            }
            return CeilingSquareRoot<BigInteger>(a);
        }

        public static T FloorSquareRoot<T>(T a)
        {
            if (((Number<T>)a).Sign < 0)
                throw new InvalidOperationException("negative radicand");
            Number<T> power;
            return FloorSquareRootCore(a, out power);
        }

        public static T CeilingSquareRoot<T>(T a)
        {
            if (((Number<T>)a).Sign < 0)
                throw new InvalidOperationException("negative radicand");
            Number<T> power;
            var c = FloorSquareRootCore(a, out power);
            return power == a ? c : c + 1;
        }

        public static T FloorRoot<T>(T a, T b)
        {
            var degree = (Number<T>)b;
            var absA = Number<T>.Abs(a);
            if (degree.IsEven && a != absA)
                throw new InvalidOperationException("negative radicand");
            Number<T> power;
            var c = FloorRootCore(absA, degree, out power);
            return a == absA ? c : -c;
        }

        public static T CeilingRoot<T>(T a, T b)
        {
            var c = (Number<T>)FloorRoot(a, b);
            if (Number<T>.Power(c, b) != a)
                ++c;
            return c;
        }

        public static Rational FloorRoot(Rational a, Rational b)
        {
            return FloorRoot(Rational.Floor(a), (BigInteger)b);
        }

        public static Rational CeilingRoot(Rational a, Rational b)
        {
            var c = FloorRoot(a, b);
            if (IntegerMath.Power(a, c) != c)
                ++c;
            return c;
        }

        public static T Root<T>(T a, T b)
        {
            var degree = (Number<T>)b;
            var absA = Number<T>.Abs(a);
            if (degree.IsEven && a != absA)
                throw new InvalidOperationException("negative radicand");
            Number<T> power;
            var c = FloorRootCore(absA, degree, out power);
            if (power != absA)
                throw new InvalidOperationException("not a perfect power");
            return a == absA ? c : -c;
        }

        public static T PerfectPower<T>(T a)
        {
            // See: Sieve Algorithms for Perfect Power Testing,
            // E. Bach and J. Sorenson, Algorithmica 9 (1993) 313-328.
            // Algorithm B (modified).
            var absA = Number<T>.Abs(a);
            var bits = IntegerMath.FloorLog((T)absA, 2);
            var logA = Number<T>.Log(absA).Real;
            foreach (var p in primes)
            {
                if (absA != a && p == 2)
                    continue;
                var b = (Number<T>)p;
                if (b > bits)
                    break;
                if (!IsPossiblePerfectPower(a, p))
                    continue;
                Number<T> power;
                var c = FloorRootCore<T>(absA, logA, b, out power);
                if (power == absA)
                    return b * PerfectPower<T>(absA == a ? c : -c);
            }
            return Number<T>.One;
        }

        private static Dictionary<int, int[]> moduliMap;

        private static void CreateModuliMap()
        {
            moduliMap = new Dictionary<int, int[]>();
            foreach (var p in primes.Take(1000))
            {
                var n = 6;
                var moduli = primes.Where(q => q % p == 1).Take(n);
                moduliMap[p] = moduli.ToArray();
            }
        }

        private static bool IsPossiblePerfectPower<T>(T a, int p)
        {
            if (!moduliMap.ContainsKey(p))
                return true;
            var moduli = moduliMap[p];
            for (int i = 0; i < moduli.Length; i++)
            {
                var modulus = moduli[i];
                var exponent = (modulus - 1) / p;
                var value = (int)((Number<T>)a % modulus);
                if (value != 0 && IntegerMath.ModularPower(value, exponent, modulus) != 1)
                    return false;
            }
            return true;
        }

        private static Number<T> FloorSquareRootCore<T>(Number<T> a, out Number<T> power)
        {
            return FloorSquareRootCore(a, Number<T>.Log(a).Real, out power);
        }

        private static Number<T> FloorSquareRootCore<T>(Number<T> a, double logA, out Number<T> power)
        {
            if (a.IsZero)
            {
                power = Number<T>.Zero;
                return a;
            }
            var log = logA / 2;
            var shift = Math.Max((int)Math.Floor(log / log2) - maxShift, 0);
            log -= shift * log2;
            var c = (Number<T>)Math.Floor(Math.Exp(log)) << shift;
            if (shift == 0)
            {
                power = c * c;
                if (power <= a && power + (c << 1) + 1 > a)
                    return c;
            }
            var cPrev = Number<T>.Zero;
            while (true)
            {
                var cNext = (a / c + c) >> 1;
                if (cNext == cPrev)
                {
                    if (cNext < c)
                        c = cNext;
                    break;
                }
                cPrev = c;
                c = cNext;
            }
            power = c * c;
            Debug.Assert(power <= a && Power((BigInteger)c + 1, 2) > a);
            return c;
        }

        private static Number<T> FloorRootCore<T>(Number<T> a, Number<T> degree, out Number<T> power)
        {
            return FloorRootCore(a, Number<T>.Log(a).Real, degree, out power);
        }

        private static Number<T> FloorRootCore<T>(Number<T> a, double logA, Number<T> degree, out Number<T> power)
        {
            if (a.IsZero)
            {
                power = Number<T>.Zero;
                return a;
            }
            var log = logA / (double)degree;
            var shift = Math.Max((int)Math.Floor(log / log2) - maxShift, 0);
            log -= shift * log2;
            var c = (Number<T>)Math.Floor(Math.Exp(log)) << shift;
            if (shift == 0)
            {
                power = Number<T>.Power(c, degree);
                if (power <= a && Number<T>.Power(c + 1, degree) > a)
                    return c;
            }
            var cPrev = Number<T>.Zero;
            var degreeMinusOne = degree - 1;
            while (true)
            {
                var cNext = (a / Number<T>.Power(c, degreeMinusOne) + degreeMinusOne * c) / degree;
                if (cNext == cPrev)
                {
                    if (cNext < c)
                        c = cNext;
                    break;
                }
                cPrev = c;
                c = cNext;
            }
            power = Number<T>.Power(c, degree);
            Debug.Assert(power <= a && Power((BigInteger)c + 1, degree) > a);
            return c;
        }
    }
}
