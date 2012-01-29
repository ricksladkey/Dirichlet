using System.Numerics;

namespace Decompose.Numerics
{
    public interface IReducer<TInteger>
    {
        TInteger Modulus { get; }
        IResidue<TInteger> ToResidue(TInteger x);
    }
}
