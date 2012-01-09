using System;
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

        public bool this[int row, int col]
        {
            get { return this[row][col]; }
            set { this[row][col] = value; }
        }

        public void XorRows(int dst, int src, int col)
        {
            this[dst].Xor(this[src]);
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

        public IEnumerable<int> GetNonZeroIndices(int row)
        {
            for (int j = 0; j < cols; j++)
            {
                if (this[row, j])
                    yield return j;
            }
        }

        public int GetRowWeight(int row)
        {
            int weight = 0;
            var srcRow = this[row];
            for (int col = 0; col < cols; col++)
                weight += srcRow[col] ? 1 : 0;
            return weight;
        }
    }
}
