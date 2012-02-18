using System;
using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static T Root<T>(T a, T b)
        {
            var result = FloorRoot(a, b);
            if (Integer<T>.Power(result, b) != a)
                throw new InvalidOperationException("not a perfect power");
            return result;
        }

        public static T FloorRoot<T>(T a, T b)
        {
            var aAbs = Integer<T>.Abs(a);
            var bInt = (int)(Integer<T>)b;
            if ((bInt & 1) == 0 && a != aAbs)
                throw new InvalidOperationException("negative radicand");
            var cPrev = Integer<T>.Zero;
            var c0 = (Integer<T>)Math.Round(Math.Exp(Integer<T>.Log(aAbs).Real / bInt));
            var bMinusOne = (Integer<T>)(bInt - 1);
            while (true)
            {
                var c1 = (aAbs / Integer<T>.Power(c0, bMinusOne) + bMinusOne * c0) / b;
                if (c1 == cPrev)
                {
                    c0 = Integer<T>.Min(c0, c1);
                    break;
                }
                cPrev = c0;
                c0 = c1;
            }
            Debug.Assert(Integer<T>.Power(c0, b) <= aAbs && Integer<T>.Power(c0 + 1, b) > aAbs);
            return a == aAbs ? c0 : -c0;
        }
    }
}
