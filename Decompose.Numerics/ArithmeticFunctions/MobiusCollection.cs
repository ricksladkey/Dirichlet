using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
#if false
    public class MobiusCollection
    {
        private const int squareSentinel = 128;
        private int size;
        private byte[] primeDivisors;

        public int Size { get { return size; } }

        public MobiusCollection(int size)
        {
            this.size = size;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            primeDivisors = new byte[size];
            for (int i = 2; i < limit; i++)
            {
                if (primeDivisors[i] == 0)
                {
                    for (int j = i; j < size; j += i)
                        ++primeDivisors[j];
                    var iSquared = i * i;
                    for (int j = iSquared; j < size; j += iSquared)
                        primeDivisors[j] = squareSentinel;
                }
            }
            for (int i = limit; i < size; i++)
            {
                if (primeDivisors[i] == 0)
                {
                    for (int j = i; j < size; j += i)
                        ++primeDivisors[j];
                }
            }
        }

        public int this[int index]
        {
            get
            {
                var d = primeDivisors[index];
                if (d >= squareSentinel)
                    return 0;
                return ((~d & 1) << 1) - 1;
            }
        }
    }
#else
    public class MobiusCollection
    {
        private const int squareSentinel = 128;
        private int size;
        private int[] products;

        public int Size { get { return size; } }

        public MobiusCollection(int size)
        {
            this.size = size;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            products = new int[size];
            products[1] = 1;
            for (var i = 2; i < size; i++)
                products[i] = -1;
            for (var i = 2; i < limit; i++)
            {
                if (products[i] == -1)
                {
                    var factor = -i;
                    for (var j = i << 1; j < size; j += i)
                        products[j] *= factor;
                    var iSquared = i * i;
                    for (var j = iSquared; j < size; j += iSquared)
                        products[j] = 0;
                }
            }
            for (var i = 2; i < size; i++)
            {
                if (products[i] == i || products[i] == -i)
                    products[i] = -products[i];
            }
        }

        public int this[int index]
        {
            get
            {
                var p = products[index];
                return p > 0 ? 1 : p < 0 ? -1 : 0;
            }
        }
    }
#endif
}
