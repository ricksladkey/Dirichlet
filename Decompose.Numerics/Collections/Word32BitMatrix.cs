using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Word = System.Int32;

namespace Decompose.Numerics
{
    public class Word32BitMatrix : IBitMatrix
    {
        private const int wordShift = 5;
        private const int wordLength = 1 << wordShift;
        private const int wordMask = wordLength - 1;

        private int rows;
        private int cols;
        private int words;
        private Word[] bits;

        public int WordLength
        {
            get { return wordLength; }
        }

        public int Rows
        {
            get { return rows; }
        }

        public int Cols
        {
            get { return cols; }
        }

        public Word32BitMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            words = (cols + wordLength - 1) / wordLength;
            bits = new Word[rows * words];
        }

        public bool this[int row, int col]
        {
            get
            {
                return (bits[row * words + (col >> wordShift)] & (Word)1 << (col & wordMask)) != 0;
            }
            set
            {
                if (value)
                    bits[row * words + (col >> wordShift)] |= (Word)1 << (col & wordMask);
                else
                    bits[row * words + (col >> wordShift)] &= ~((Word)1 << (col & wordMask));
            }
        }

        public void XorRows(int dst, int src, int col)
        {
            int dstRow = dst * words;
            int srcRow = src * words;
            for (int word = col / wordLength; word < words; word++)
                bits[dstRow + word] ^= bits[srcRow + word];
        }

        public void Clear()
        {
            int size = rows * words;
            for (int i = 0; i < size; i++)
                bits[i] = 0;
        }

        public void CopySubMatrix(IBitMatrix other, int row, int col)
        {
            if (other is Word32BitMatrix)
            {
                CopySubMatrix((Word32BitMatrix)other, row, col);
                return;
            }
            for (int i = 0; i < other.Rows; i++)
            {
                for (int j = 0; j < other.Cols; j++)
                    this[row + i, col + j] = other[i, j];
            }
        }

        public void CopySubMatrix(Word32BitMatrix other, int row, int col)
        {
            int dstOffset = row * words + (col >> wordShift);
            for (int i = 0; i < other.rows; i++)
            {
                int dstRow = i * words + dstOffset;
                int srcRow = i * other.words;
                for (int j = 0; j < other.words; j++)
                    bits[dstRow + j] = other.bits[srcRow + j];
            }
        }

        public IEnumerable<bool> GetRow(int row)
        {
            for (int j = 0; j < cols; j++)
                yield return this[row, j];
        }

        public IEnumerable<int> GetNonZeroIndices(int row)
        {
            int srcRow = row * words;
            for (int word = 0; word < words; word++)
            {
                var value = bits[srcRow + word];
                if (value != 0)
                {
                    int col = word * wordLength;
                    for (int j = 0; j < wordLength; j++)
                    {
                        if ((value & (Word)1 << j) != 0)
                            yield return col + j;
                    }
                }
            }
        }

        public int GetRowWeight(int row)
        {
            int weight = 0;
            int srcRow = row * words;
            for (int word = 0; word < words; word++)
            {
                if (bits[srcRow + word] != 0)
                    weight += bits[srcRow + word].GetBitCount();
            }
            return weight;
        }

        public override string ToString()
        {
            return string.Format("Rows = {0}, Cols = {1}", Rows, Cols);
        }
    }
}
