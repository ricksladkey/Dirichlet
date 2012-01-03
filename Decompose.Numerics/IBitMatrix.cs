namespace Decompose.Numerics
{
    public interface IBitMatrix : IMatrix<bool>
    {
        void XorRows(int dst, int src);
        bool IsRowEmpty(int i);
    }
}
