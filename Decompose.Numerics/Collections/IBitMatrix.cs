using System.Collections.Generic;
namespace Decompose.Numerics
{
    public interface IBitMatrix : IMatrix<bool>
    {
        int WordLength { get; }
        void XorRows(int dst, int src);
        bool IsRowEmpty(int i);
        void Clear();
        void CopySubMatrix(IBitMatrix other, int row, int col);
        IEnumerable<bool> GetRow(int row);
    }
}
