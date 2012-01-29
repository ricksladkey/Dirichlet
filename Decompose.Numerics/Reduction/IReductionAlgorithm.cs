using System.Numerics;

namespace Decompose.Numerics
{
    public interface IReductionAlgorithm<TInteger>
    {
        IReducer<TInteger> GetReducer(TInteger n);
    }
}
