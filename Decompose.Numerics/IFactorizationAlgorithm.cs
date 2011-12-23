using System.Collections.Generic;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IFactorizationAlgorithm
    {
        IEnumerable<BigInteger> Factor(BigInteger n);
    }
}
