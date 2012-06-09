using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static T FloorLog<T>(T a, int b)
        {
            return (Number<T>)Math.Floor(Number<T>.Log(a, b).Real);
        }

        public static T CeilingLog<T>(T a, int b)
        {
            return (Number<T>)Math.Ceiling(Number<T>.Log(a, b).Real);
        }
    }
}
