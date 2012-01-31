using System.Collections.Generic;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IOperations<T> : IEqualityComparer<T>, IComparer<T>
    {
        bool IsUnsigned { get; }
        IRandomNumberGenerator<T> Random { get; }
        T Convert(int a);
        BigInteger ToBigInteger(T a);
        T Add(T a, T b);
        T Subtract(T a, T b);
        T Multiply(T a, T b);
        T Divide(T a, T b);
        T Modulus(T a, T b);
        T Negate(T a);
        T LeftShift(T a, int n);
        T RightShift(T a, int n);
        T And(T a, T b);
        T Or(T a, T b);
        T ExclusiveOr(T a, T b);
        T ModularProduct(T a, T b, T modulus);
        T ModularPower(T value, T exponent, T modulus);
        bool IsEven(T a);
    }
}
