using System.Collections.Generic;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IOperations<T> : IEqualityComparer<T>, IComparer<T>
    {
        IRandomNumberAlgorithm<T> Random { get; }
        T Convert(int a);
        BigInteger ToBigInteger(T a);
        T Add(T a, T b);
        T Subtract(T a, T b);
        T Multiply(T a, T b);
        T Divide(T a, T b);
        T LeftShift(T a, int n);
        T RightShift(T a, int n);
        T ModularProduct(T a, T b, T modulus);
        T ModularPower(T value, T exponent, T modulus);
        bool IsEven(T a);
    }
}
