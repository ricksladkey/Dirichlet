namespace Decompose.Numerics
{
    public interface IDivisionAlgorithm<T>
    {
        T Divide(T k);
        T Modulus(T k);
        bool IsDivisible(T k);
    }
}
