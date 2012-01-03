using System.Collections;
using System.Collections.Generic;

namespace Decompose.Numerics
{
    public class Word32BitMatrix
    {
        private const int wordLength = 32;

        private int rows;
        private int cols;
        private int words;
        private int[][] bits;

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
            bits = new int[rows][];
            for (int i = 0; i < rows; i++)
                bits[i] = new int[words];
        }

        public bool this[int i, int j]
        {
            get
            {
                return (bits[i][j / wordLength] & 1 << j % wordLength) != 0;
            }
            set
            {
                if (value)
                    bits[i][j / wordLength] |= 1 << j % wordLength;
                else
                    bits[i][j / wordLength] &= ~(1 << j % wordLength);
            }
        }

        public void XorRows(int dst, int src)
        {
            var dstRow = bits[dst];
            var srcRow = bits[src];
            for (int j = 0; j < words; j++)
                dstRow[j] ^= srcRow[j];
        }

        public bool IsRowEmpty(int i)
        {
            var row = bits[i];
            for (int j = 0; j < words; j++)
            {
                if (row[j] != 0)
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
