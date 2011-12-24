using System.Numerics;

namespace Decompose.Numerics
{
    public interface IReduction
    {
        IResidue ToResidue(BigInteger x);
        IResidue Multiply(IResidue x, IResidue y);
    }
}
