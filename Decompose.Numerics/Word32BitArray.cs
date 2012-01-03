using System.Collections;
using System.Collections.Generic;
using Word = System.Int32;

namespace Decompose.Numerics
{
    public class Word32BitArray : IBitArray
    {
        private const int wordShift = 5;
        private const int wordLength = 1 << wordShift;
        private const int wordMask = wordLength - 1;

        private int length;
        private int words;
        private Word[] bits;

        public int Length
        {
            get { return length; }
            set
            {
                length = value;
                words = (length + wordLength - 1) / wordLength;
                bits = new Word[words];
            }
        }

        public Word32BitArray()
        {
        }

        public Word32BitArray(int length)
        {
            Length = length;
        }

        public bool this[int j]
        {
            get
            {
                return (bits[j >> wordShift] & (Word)1 << (j & wordMask)) != 0;
            }
            set
            {
                if (value)
                    bits[j >> wordShift] |= (Word)1 << (j & wordMask);
                else
                    bits[j >> wordShift] &= ~((Word)1 << (j & wordMask));
            }
        }

        public IEnumerator<bool> GetEnumerator()
        {
            for (int i = 0; i < length; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
