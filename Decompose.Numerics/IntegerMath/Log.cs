using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static T FloorLog<T>(T a, int b)
        {
            var result = (Number<T>)Math.Floor(Number<T>.Log(a, b).Real);
            if (Number<T>.Power(2, result + 1) <= a)
                ++result;
            Debug.Assert(Number<T>.Power(2, result) <= a && Number<T>.Power(2, result + 1) > a);
            return result;
        }

        public static T CeilingLog<T>(T a, int b)
        {
            var result = (Number<T>)Math.Ceiling(Number<T>.Log(a, b).Real);
            if (Number<T>.Power(2, result - 1) >= a)
                --result;
            Debug.Assert(Number<T>.Power(2, result - 1) < a && Number<T>.Power(2, result) >= a);
            return result;
        }

        public static T FloorLog<T>(T a, double b)
        {
            return (Number<T>)Math.Floor(Number<T>.Log(a, b).Real);
        }

        public static T CeilingLog<T>(T a, double b)
        {
            return (Number<T>)Math.Ceiling(Number<T>.Log(a, b).Real);
        }
    }
}
