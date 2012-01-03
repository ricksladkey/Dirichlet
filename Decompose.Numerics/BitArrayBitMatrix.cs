using System.Collections;
using System.Collections.Generic;

namespace Decompose.Numerics
{
    public class BitArrayBitMatrix : List<BitArray>
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
    }
}
