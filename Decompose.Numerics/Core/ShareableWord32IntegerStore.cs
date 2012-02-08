using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class ShareableWord32IntegerStore : IStore<Word32Integer>
    {
        public Word32Integer Allocate()
        {
            return new Word32Integer(4);
        }

        public void Release(Word32Integer item)
        {
        }
    }
}
