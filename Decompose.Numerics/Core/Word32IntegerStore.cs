using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class Word32IntegerStore
    {
        private int length;

        public Word32IntegerStore(int length)
        {
            this.length = length;
        }

        public Word32Integer Create()
        {
            return new Word32Integer(length);
        }
    }
}
