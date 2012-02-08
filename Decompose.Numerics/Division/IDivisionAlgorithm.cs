namespace Decompose.Numerics
{
    public interface IDivisionAlgorithm<TDividend, TDivisor>
    {
        TDividend Divide(TDividend k);
        TDivisor Modulus(TDividend k);
        bool IsDivisible(TDividend k);
    }
}
