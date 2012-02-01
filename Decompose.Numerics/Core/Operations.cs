using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public abstract class Operations<T> : IOperations<T>
    {
        public abstract Integer<T> Wrap(T value);
        public abstract bool IsUnsigned { get; }
        public abstract T Zero { get; }
        public abstract T One { get; }
        public abstract T Two { get; }
        public abstract T Convert(int a);
        public abstract BigInteger ToBigInteger(T a);
        public abstract T Add(T a, T b);
        public abstract T Subtract(T a, T b);
        public abstract T Multiply(T a, T b);
        public abstract T Divide(T a, T b);
        public abstract T Modulus(T a, T b);
        public abstract T Negate(T a);
        public abstract T LeftShift(T a, int n);
        public abstract T RightShift(T a, int n);
        public abstract T And(T a, T b);
        public abstract T Or(T a, T b);
        public abstract T ExclusiveOr(T a, T b);
        public abstract bool IsZero(T a);
        public abstract bool IsOne(T a);
        public abstract bool IsEven(T a);
        public abstract T ModularProduct(T a, T b, T modulus);
        public abstract T ModularPower(T value, T exponent, T modulus);
        public abstract bool Equals(T x, T y);
        public abstract int GetHashCode(T obj);
        public abstract int Compare(T x, T y);
    }
}
