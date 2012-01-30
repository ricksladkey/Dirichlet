using System.Numerics;

namespace Decompose.Numerics
{
    public interface IReductionAlgorithm<T> : IOperations<T>
    {
        IReducer<T> GetReducer(T n);
    }
}
