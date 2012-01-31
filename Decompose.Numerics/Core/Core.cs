using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class Core
    {
        public static void ExtendedGreatestCommonDivisor<T>(IOperations<T> ops, T a, T b, out T c, out T d)
        {
            var x = ops.Zero;
            var lastx = ops.One;
            var y = ops.One;
            var lasty = ops.Zero;

            if (ops.Compare(a, b) < 0)
            {
                var tmpa = a;
                a = b;
                b = tmpa;
                x = ops.One;
                lastx = ops.Zero;
                y = ops.Zero;
                lasty = ops.One;
            }
            while (!ops.IsZero(b))
            {
                Debug.Assert(ops.Compare(a, b) >= 0);
                var b2 = ops.LeftShift(b, 1);
                if (ops.Compare(a, b2) < 0)
                {
                    Debug.Assert(ops.IsOne(ops.Divide(a, b)));
                    var tmpa = a;
                    a = b;
                    b = ops.Subtract(tmpa, b);
                    var tmpx = x;
                    x = ops.Subtract(lastx, x);
                    lastx = tmpx;
                    var tmpy = y;
                    y = ops.Subtract(lasty, y);
                    lasty = tmpy;
                }
                else if (ops.Compare(a, ops.Add(b2, b)) < 0)
                {
                    Debug.Assert(ops.Divide(a, b).Equals(ops.Convert(2)));
                    var tmpa = a;
                    a = b;
                    b = ops.Subtract(tmpa, ops.LeftShift(b, 1));
                    var tmpx = x;
                    x = ops.Subtract(lastx, ops.LeftShift(x, 1));
                    lastx = tmpx;
                    var tmpy = y;
                    y = ops.Subtract(lasty, ops.LeftShift(y, 1));
                    lasty = tmpy;
                }
                else
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
            }
            c = lastx;
            d = lasty;
        }
    }
}
