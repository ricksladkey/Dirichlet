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
            BitMatrixHelper.CopySubMatrix(this, other, 0, 0);
        }

        public void XorRows(int dst, int src, int col)
        {
            BitMatrixHelper.XorRows(this, dst, src, col);
        }

        public new void Clear()
        {
            BitMatrixHelper.Clear(this);
        }

        public void CopySubMatrix(IBitMatrix other, int row, int col)
        {
            BitMatrixHelper.CopySubMatrix(this, other, row, col);
        }

        public IEnumerable<bool> GetRow(int row)
        {
            return BitMatrixHelper.GetRow(this, row);
        }

        public IEnumerable<int> GetNonZeroCols(int row)
        {
            return BitMatrixHelper.GetNonZeroCols(this, row);
        }

        public IEnumerable<bool> GetCol(int col)
        {
            return BitMatrixHelper.GetCol(this, col);
        }

        public IEnumerable<int> GetNonZeroRows(int col)
        {
            return BitMatrixHelper.GetNonZeroRows(this, col);
        }

        public int GetRowWeight(int row)
        {
            return BitMatrixHelper.GetRowWeight(this, row);
        }

        public int GetColWeight(int col)
        {
            return BitMatrixHelper.GetColWeight(this, col);
        }

        public IEnumerable<int> GetRowWeights()
        {
            return BitMatrixHelper.GetRowWeights(this);
        }

        public IEnumerable<int> GetColWeights()
        {
            return BitMatrixHelper.GetColWeights(this);
        }
    }
}
