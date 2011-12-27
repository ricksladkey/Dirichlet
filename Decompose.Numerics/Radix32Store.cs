using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class Radix32Store
    {
        private uint[] bits;
        private int length;
        private int size;
        private int allocated;

        public Radix32Store(int length)
        {
            this.length = length;
            size = (length + 3) / 4 * 4;
            bits = new uint[1024];
            allocated = 0;
        }

        public Radix32Integer Create()
        {
            var result = new Radix32Integer(bits, allocated, length);
            allocated += size;
            return result;
        }
    }
}
