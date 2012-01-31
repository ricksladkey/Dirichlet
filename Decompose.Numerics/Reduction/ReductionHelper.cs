using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public static class ReductionHelper
    {
        public static void ModularPower<T>(IResidue<T> value, T exponent)
        {
            var reducer = value.Reducer;
            var ops = reducer.Reduction;
            var result = reducer.ToResidue(ops.One);
            while (!ops.IsZero(exponent))
            {
                if (!ops.IsEven(exponent))
                    result.Multiply(value);
                value.Multiply(value);
                exponent = ops.RightShift(exponent, 1);
            }
            value.Set(result);
        }
    }
}
