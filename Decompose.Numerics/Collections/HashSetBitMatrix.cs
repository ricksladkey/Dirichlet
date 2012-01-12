using System;
using System.Collections;
using System.Collections.Generic;

namespace Decompose.Numerics
{
    public class HashSetBitMatrix : IBitMatrix
    {
        private int rows;
        private int cols;

        private HashSet<int>[] rowSets;

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
            rowSets = new HashSet<int>[rows];
            for (int i = 0; i < rows; i++)
                rowSets[i] = new HashSet<int>();
        }

        public bool this[int row, int col]
        {
            get { return rowSets[row].Contains(col); }
            set
            {
                if (value)
                    rowSets[row].Add(col);
                else
                    rowSets[row].Remove(col);
            }
        }

        public void XorRows(int dst, int src, int col)
        {
            rowSets[dst].SymmetricExceptWith(rowSets[src]);
        }

        public void Clear()
        {
            for (int i = 0; i < rows; i++)
                rowSets[i].Clear();
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
            return rowSets[row];
        }

        public int GetRowWeight(int row)
        {
            return rowSets[row].Count;
        }

        public int GetColWeight(int col)
        {
            int weight = 0;
            for (int row = 0; row < rows; row++)
                weight += this[row, col] ? 1 : 0;
            return weight;
        }

        public IEnumerable<int> GetRowWeights()
        {
            for (int row = 0; row < rows; row++)
                yield return GetRowWeight(row);
        }

        public IEnumerable<int> GetColWeights()
        {
            var weights = new int[cols];
            for (int row = 0; row < rows; row++)
            {
                foreach (var col in rowSets[row])
                    ++weights[col];
            }
            return weights;
        }
    }
}
