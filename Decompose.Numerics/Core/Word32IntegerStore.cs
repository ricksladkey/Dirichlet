using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class Word32IntegerStore
    {
        private uint[] bits;
        private int length;
        private int size;
        private int allocated;

        public Word32IntegerStore(int length)
        {
            this.length = length;
            size = (length + 3) / 4 * 4;
            bits = new uint[1024];
            allocated = 0;
        }

        public Word32Integer Create()
        {
            var result = new Word32Integer(bits, allocated, length);
            allocated += size;
            return result;
        }
    }
}
