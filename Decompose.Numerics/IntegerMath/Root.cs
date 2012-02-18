using System;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static T FloorRoot<T>(T a, T b)
        {
            var aAbs = Number<T>.Abs(a);
            var degree = (Number<T>)b;
            var degreeInt = (int)degree;
            if ((degreeInt & 1) == 0 && a != aAbs)
                throw new InvalidOperationException("negative radicand");
            var cPrev = Number<T>.Zero;
            var c0 = (Number<T>)Math.Round(Math.Exp(Number<T>.Log(aAbs).Real / degreeInt));
            var degreeMinusOne = (T)(Number<T>)(degreeInt - 1);
            while (true)
            {
                var c1 = (aAbs / Number<T>.Power(c0, degreeMinusOne) + degreeMinusOne * c0) / b;
                if (c1 == cPrev)
                {
                    c0 = Number<T>.Min(c0, c1);
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

        public static T PerfectPower<T>(T a)
        {
            var bits = (Number<T>)Math.Floor(Number<T>.Log(a, 2).Real);
            for (var b = bits; b > 1; b--)
            {
                var c = FloorRoot<T>(a, b);
                if (Number<T>.Power(c, b) == a)
                    return b;
            }
            return Number<T>.One;
        }
    }
}
