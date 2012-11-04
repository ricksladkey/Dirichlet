using System.Numerics;

namespace Decompose.Numerics
{
    public interface IReductionAlgorithm<T>
    {
        IReducer<T> GetReducer(T n);
    }
}
