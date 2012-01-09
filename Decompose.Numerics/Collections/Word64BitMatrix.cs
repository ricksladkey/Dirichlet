using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Word = System.Int64;
using System.Diagnostics;

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

        public Word64BitMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            words = (cols + wordLength - 1) / wordLength;
            bits = new Word[rows * words];
        }

        public Word64BitMatrix(IBitMatrix matrix)
            : this(matrix.Rows, matrix.Cols)
        {
            for (int i = 0; i < rows; i++)
            {
                foreach (var j in matrix.GetNonZeroIndices(i))
                    this[i, j] = true;
            }
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

        public void XorRows(int dst, int src, int col)
        {
            var dstRow = dst * words;
            var srcRow = src * words;
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
            if (other is Word64BitMatrix)
            {
                CopySubMatrix((Word64BitMatrix)other, row, col);
                return;
            }
            for (int i = 0; i < other.Rows; i++)
            {
                for (int j = 0; j < other.Cols; j++)
                    this[row + i, col + j] = other[i, j];
            }
        }

        public void CopySubMatrix(Word64BitMatrix other, int row, int col)
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
                var value = (ulong)bits[srcRow + word];
                int w = 0;
                while (value != 0)
                {
                    if ((value & 1) != 0)
                        ++w;
                    value >>= 1;
                }
                weight += w;
                //Debug.Assert(w == bits[srcRow + word].GetBitCount());
            }
            Debug.Assert(weight == GetRow(row).Select(bit => bit ? 1 : 0).Sum());
            return weight;
        }

        public override string ToString()
        {
            return string.Format("Rows = {0}, Cols = {1}", Rows, Cols);
        }
    }
}
