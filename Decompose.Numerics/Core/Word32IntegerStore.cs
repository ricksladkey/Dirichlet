using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class Word32IntegerStore
    {
        private int length;
        private Stack<Word32Integer> freeList;

        public Word32IntegerStore(int length)
        {
            this.length = length;
            this.freeList = new Stack<Word32Integer>();
        }

        public Word32Integer Allocate()
        {
            if (freeList.Count != 0)
                return freeList.Pop();
            return new Word32Integer(length);
        }

        public void Release(Word32Integer a)
        {
            freeList.Push(a);
        }
    }
}
