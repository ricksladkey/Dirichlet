using System.Numerics;

namespace Decompose.Numerics
{
    public interface IReducer
    {
        BigInteger Modulus { get; }
        IResidue ToResidue(BigInteger x);
    }
}
