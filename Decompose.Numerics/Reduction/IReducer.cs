using System.Numerics;

namespace Decompose.Numerics
{
    public interface IReducer<T>
    {
        T Modulus { get; }
        IResidue<T> ToResidue(T x);
    }
}
