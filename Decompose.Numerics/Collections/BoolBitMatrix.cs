using System;
using System.Collections.Generic;
namespace Decompose.Numerics
{
    public class BoolBitMatrix : List<bool[]>, IBitMatrix
    {
        private int rows;
        private int cols;

        public int WordLength
        {
            get { return 1; }
        }

        public int Rows
        {
            get { return rows; }
        }

        public int Cols
        {
            get { return cols; }
        }

        public bool this[int row, int col]
        {
            get { return this[row][col]; }
            set { this[row][col] = value; }
        }

        public BoolBitMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            for (int i = 0; i < rows; i++)
                this.Add(new bool[cols]);
        }

        public BoolBitMatrix(IBitMatrix other)
            : this(other.Rows, other.Cols)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    this[i, j] = other[i, j];
            }
        }

        public void XorRows(int dst, int src, int col)
        {
            var dstRow = this[dst];
            var srcRow = this[src];
            for (int j = col; j < cols; j++)
                dstRow[j] ^= srcRow[j];
        }

        public new void Clear()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    this[i][j] = false;
            }
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
            for (int col = 0; col < cols; col++)
                weight += this[row][col] ? 1 : 0;
            return weight;
        }
    }
}
