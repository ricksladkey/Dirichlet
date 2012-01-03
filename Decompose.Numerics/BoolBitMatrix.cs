using System.Collections.Generic;
namespace Decompose.Numerics
{
    public class BoolBitMatrix : List<bool[]>, IBitMatrix
    {
        private int rows;
        private int cols;

        public int Rows
        {
            get { return rows; }
        }

        public int Cols
        {
            get { return cols; }
        }

        public BoolBitMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            for (int i = 0; i < rows; i++)
                this.Add(new bool[cols]);
        }

        public bool this[int i, int j]
        {
            get { return this[i][j]; }
            set { this[i][j] = value; }
        }

        public void XorRows(int dst, int src)
        {
            var dstRow = this[dst];
            var srcRow = this[src];
            for (int j = 0; j < cols; j++)
                dstRow[j] ^= srcRow[j];
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
    }
}
