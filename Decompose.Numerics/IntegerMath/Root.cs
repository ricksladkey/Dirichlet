using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static T FloorRoot<T>(T a, T b)
        {
            var aAbs = Number<T>.Abs(a);
            var degree = (Number<T>)b;
            if (degree.IsEven && a != aAbs)
                throw new InvalidOperationException("negative radicand");
            var log = Number<T>.Log(aAbs).Real / (double)degree;
            var log2 = Math.Log(2);
            var shift = (int)Math.Floor(log / log2);
            log -= shift * log2;
            var shift1 = Math.Min(shift, 64);
            var shift2 = shift - shift1;
            var c0 = (Number<T>)Math.Floor(Math.Exp(log + shift1 * log2)) << shift2;
            if (Number<T>.Power(c0, b) <= aAbs && Number<T>.Power(c0 + 1, b) > aAbs)
                return a == aAbs ? c0 : -c0;

            var cPrev = Number<T>.Zero;
            var degreeMinusOne = degree - 1;
            while (true)
            {
                var c1 = (aAbs / Number<T>.Power(c0, degreeMinusOne) + degreeMinusOne * c0) / degree;
                if (c1 == cPrev)
                {
                    if (c1 < c0)
                        c0 = c1;
                    break;
                }
                cPrev = c0;
                c0 = c1;
            }
            Debug.Assert(Number<T>.Power(c0, b) <= aAbs && Number<T>.Power(c0 + 1, b) > aAbs);
            return a == aAbs ? c0 : -c0;
        }

        public static T Root<T>(T a, T b)
        {
            var result = FloorRoot(a, b);
            if (Number<T>.Power(result, b) != a)
                throw new InvalidOperationException("not a perfect power");
            return result;
        }

        private static IEnumerable<int> primes = new SieveOfErostothones();

        public static T PerfectPower<T>(T a)
        {
            var bits = (Number<T>)Math.Floor(Number<T>.Log(a, 2).Real);
            foreach (var p in primes)
            {
                var b = (Number<T>)p;
                if (b > bits)
                    break;
                var c = FloorRoot<T>(a, b);
                if (Number<T>.Power(c, b) == a)
                    return b * PerfectPower(c);
            }
            return Number<T>.One;
        }
    }
}
