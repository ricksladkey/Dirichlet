using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Word = System.Int64;

namespace Decompose.Numerics
{
    public class Word64BitMatrix : IBitMatrix
    {
        private const int wordShift = 6;
        private const int wordLength = 1 << wordShift;
        private const int wordMask = wordLength - 1;

        private int rows;
        private int cols;
        private int words;
        private Word[] bits;

        public int Rows
        {
            get { return rows; }
        }

        public int Cols
        {
            get { return cols; }
        }

        public Word64BitMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            words = (cols + wordLength - 1) / wordLength;
            bits = new Word[rows * words];
        }

        public bool this[int i, int j]
        {
            get
            {
                return (bits[i * words + (j >> wordShift)] & (Word)1 << (j & wordMask)) != 0;
            }
            set
            {
                if (value)
                    bits[i * words + (j >> wordShift)] |= (Word)1 << (j & wordMask);
                else
                    bits[i * words + (j >> wordShift)] &= ~((Word)1 << (j & wordMask));
            }
        }

        public void XorRows(int dst, int src)
        {
            var dstRow = dst * words;
            var srcRow = src * words;
            for (int j = 0; j < words; j++)
                bits[dstRow + j] ^= bits[srcRow + j];
        }

        public bool IsRowEmpty(int i)
        {
            var row = i * words;
            for (int j = 0; j < words; j++)
            {
                if (bits[row] != 0)
                    return false;
            }
            return true;
        }

        public void RemoveRow(int i)
        {
            for (int row = i + 1; row < rows; row++)
                bits[row - 1] = bits[row];
            --rows;
        }
    }
}
