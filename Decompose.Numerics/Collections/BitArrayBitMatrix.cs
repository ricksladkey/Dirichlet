using System.Collections;
using System.Collections.Generic;

namespace Decompose.Numerics
{
    public class BitArrayBitMatrix : List<BitArray>, IBitMatrix
    {
        private int rows;
        private int cols;

        public int WordLength
        {
            get { return 32; }
        }

        public int Rows
        {
            get { return rows; }
        }

        public int Cols
        {
            get { return cols; }
        }

        public BitArrayBitMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            for (int i = 0; i < rows; i++)
                this.Add(new BitArray(cols));
        }

        public bool this[int i, int j]
        {
            get { return this[i][j]; }
            set { this[i][j] = value; }
        }

        public void XorRows(int dst, int src)
        {
            this[dst].Xor(this[src]);
        }

        public bool IsRowEmpty(int i)
        {
            var row = this[i];
            for (int j = 0; j < cols; j++)
            {
                if (row[j])
                    return false;
            }
            return true;
        }

        public new void Clear()
        {
            for (int i = 0; i < rows; i++)
                this[i].SetAll(false);
        }

        public void CopySubMatrix(IBitMatrix other, int row, int col)
        {
            for (int i = 0; i < other.Rows; i++)
            {
                for (int j = 0; j < other.Cols; j++)
                    this[row + i, col + j] = other[i, j];
            }
        }

        public IEnumerable<bool> GetRow(int row)
        {
            for (int j = 0; j < cols; j++)
                yield return this[row, j];
        }
    }
}
