using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class MobiusRange
    {
        private const int squareSentinel = 255;
        private byte[] primeDivisors;

        public MobiusRange(int n)
        {
            var size = n + 1;
            primeDivisors = new byte[size];
            for (int i = 2; i < size; i++)
            {
                if (primeDivisors[i] == 0)
                {
                    for (int j = 2 * i; j < size; j += i)
                        ++primeDivisors[j];
                }
            }
            for (int i = 2; true; i++)
            {
                if (primeDivisors[i] == 0)
                {
                    var iSquared = i * i;
                    if (iSquared > size)
                        break;
                    for (int j = iSquared; j < size; j += iSquared)
                        primeDivisors[j] = squareSentinel;
                }
            }
#if false
            for (int i = 1; i <= n; i++)
            {
                if (this[i] != IntegerMath.Mobius(i))
                    Debugger.Break();
            }
#endif
        }

        public int this[int index]
        {
            get
            {
                var d = primeDivisors[index];
                if (d == squareSentinel)
                    return 0;
                if (d == 0)
                    return index == 1 ? 1 : -1;
                return ((~d & 1) << 1) - 1;
            }
        }
    }
}
