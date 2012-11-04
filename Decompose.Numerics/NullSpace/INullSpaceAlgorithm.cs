using System.Collections.Generic;

namespace Decompose.Numerics
{
    public interface INullSpaceAlgorithm<TArray, TMatrix>
    {
        IEnumerable<TArray> Solve(TMatrix matrix);
    }
}
