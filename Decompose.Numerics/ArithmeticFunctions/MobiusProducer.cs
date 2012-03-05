using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class MobiusProducer : BlockingCollection<MobiusProducer.MobiusItem>
    {
        public struct MobiusItem
        {
            public int Value { get; set; }
            public int Mobius { get; set; }
        }

        private const int squareSentinel = 128;

        private int size;
        private byte[] primeDivisors;

        public MobiusProducer(int n)
        {
            size = n + 1;
        }

        public void ProduceValues()
        {
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            primeDivisors = new byte[size];
            Add(new MobiusItem { Value = 1, Mobius = 1 });
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
                var d = primeDivisors[i];
                Add(new MobiusItem { Value = i, Mobius = d >= squareSentinel ? 0 : ((d & 1) << 1) - 1 });
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
    }
}
