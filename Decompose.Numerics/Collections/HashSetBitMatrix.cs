using System;
using System.Collections;
using System.Collections.Generic;

namespace Decompose.Numerics
{
    public class HashSetBitMatrix : List<HashSet<int>>, IBitMatrix
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

        public HashSetBitMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            for (int i = 0; i < rows; i++)
                this.Add(new HashSet<int>());
        }

        public bool this[int row, int col]
        {
            get { return this[row].Contains(col); }
            set
            {
                if (value)
                    this[row].Add(col);
                else
                    this[row].Remove(col);
            }
        }

        public void XorRows(int dst, int src, int col)
        {
            this[dst].SymmetricExceptWith(this[src]);
        }

        public new void Clear()
        {
            for (int i = 0; i < rows; i++)
                this[i].Clear();
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

        public IEnumerable<int> GetNonZeroCols(int row)
        {
            return this[row];
        }

        public int GetRowWeight(int row)
        {
            return this[row].Count;
        }
    }
}
