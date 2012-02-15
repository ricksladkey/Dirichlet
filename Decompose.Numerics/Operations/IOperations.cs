using System;
using System.Collections.Generic;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IOperations
    {
        Type Type { get; }
    }

    public interface IOperations<T> : IOperations, IEqualityComparer<T>, IComparer<T>
    {
        bool IsUnsigned { get; }
        T Zero { get; }
        T One { get; }
        T Two { get; }
        T Convert(int a);
        T Convert(BigInteger a);
        int ToInt32(T a);
        BigInteger ToBigInteger(T a);
        double ToDouble(T a);
        T Add(T a, T b);
        T Subtract(T a, T b);
        T Multiply(T a, T b);
        T Divide(T a, T b);
        T Modulo(T a, T b);
        T Remainder(T a, T b);
        T Power(T a, T b);
        T Root(T a, T b);
        T Negate(T a);
        T LeftShift(T a, int n);
        T RightShift(T a, int n);
        T And(T a, T b);
        T Or(T a, T b);
        T ExclusiveOr(T a, T b);
        T OnesComplement(T a);
        bool IsZero(T a);
        bool IsOne(T a);
        bool IsEven(T a);
        uint LeastSignificantWord(T a);

        T GreatestCommonDivisor(T a, T b);
        T ModularSum(T a, T b, T modulus);
        T ModularDifference(T a, T b, T modulus);
        T ModularProduct(T a, T b, T modulus);
        T ModularQuotient(T a, T b, T modulus);
        T ModularPower(T value, T exponent, T modulus);
        T ModularRoot(T value, T exponent, T modulus);
        T ModularInverse(T value, T modulus);
    }
}
