namespace Decompose.Numerics
{
    public interface IMatrix<T>
    {
        int Rows { get; }
        int Cols { get; }
        T this[int i, int j] { get; set; }
    }
}
