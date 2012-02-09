using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class Core
    {
        public static void ExtendedGreatestCommonDivisor<T>(T a, T b, out T c, out T d)
        {
            var ops = Operations.Get<T>();
            var x = ops.Zero;
            var lastx = ops.One;
            var y = ops.One;
            var lasty = ops.Zero;

            while (!ops.IsZero(b))
            {
                var quotient = ops.Divide(a, b);
                var tmpa = a;
                a = b;
                b = ops.Subtract(tmpa, ops.Multiply(quotient, b));
                var tmpx = x;
                x = ops.Subtract(lastx, ops.Multiply(quotient, x));
                lastx = tmpx;
                var tmpy = y;
                y = ops.Subtract(lasty, ops.Multiply(quotient, y));
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }
    }
}
