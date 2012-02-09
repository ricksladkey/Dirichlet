using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public static class Operations
    {
        private static Dictionary<Type, IOperations> operations = new Dictionary<Type, IOperations>
        {
            { typeof(int), new Int32Operations() },
            { typeof(uint), new UInt32Operations() },
            { typeof(long), new Int64Operations() },
            { typeof(ulong), new UInt64Operations() },
            { typeof(BigInteger), new BigIntegerOperations() },
        };

        public static IOperations<T> Get<T>()
        {
            var type = typeof(T);
            IOperations ops;
            if (!operations.TryGetValue(typeof(T), out ops))
                throw new NotImplementedException("type not supported");
            return (IOperations<T>)ops;
        }
    }

    public abstract class Operations<T> : IOperations<T>
    {
        public abstract Type Type { get; }
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
        public abstract uint LeastSignificantWord(T a);
        public abstract bool Equals(T x, T y);
        public abstract int GetHashCode(T obj);
        public abstract int Compare(T x, T y);

        public abstract T SquareRoot(T a);
        public abstract T GreatestCommonDivisor(T a, T b);
        public abstract T ModularSum(T a, T b, T modulus);
        public abstract T ModularDifference(T a, T b, T modulus);
        public abstract T ModularProduct(T a, T b, T modulus);
        public abstract T ModularPower(T value, T exponent, T modulus);
        public abstract T ModularInverse(T value, T modulus);
    }
}
