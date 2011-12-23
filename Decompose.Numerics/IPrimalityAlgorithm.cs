using System.Numerics;

namespace Decompose.Numerics
{
    public interface IPrimalityAlgorithm
    {
        bool IsPrime(BigInteger n);
    }
}
