using System.Collections.Generic;

namespace Decompose.Numerics
{
    public interface IBitArray : IArray<bool>
    {
        IEnumerable<int> GetNonZeroIndices();
    }
}
