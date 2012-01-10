using System.Numerics;

namespace Decompose.Numerics
{
    public interface IReductionAlgorithm
    {
        IReducer GetReducer(BigInteger n);
    }
}
