using System.Collections.Generic;
namespace Decompose.Numerics
{
    public interface IBitMatrix : IMatrix<bool>
    {
        int WordLength { get; }
        void XorRows(int dst, int src, int col);
        void Clear();
        void CopySubMatrix(IBitMatrix other, int row, int col);
        IEnumerable<bool> GetRow(int row);
        int GetRowWeight(int row);
    }
}
