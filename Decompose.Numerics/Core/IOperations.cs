using System.Collections.Generic;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IOperations<T> : IEqualityComparer<T>, IComparer<T>
    {
        bool IsUnsigned { get; }
        T Zero { get; }
        T One { get; }
        T Two { get; }
        T Convert(int a);
        Integer<T> Wrap(T value);
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
        bool IsZero(T a);
        bool IsOne(T a);
        bool IsEven(T a);
        T ModularProduct(T a, T b, T modulus);
        T ModularPower(T value, T exponent, T modulus);
    }
}
