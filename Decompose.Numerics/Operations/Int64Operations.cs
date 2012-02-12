using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class Int64Operations : IOperations<long>
    {
        public Type Type { get { return typeof(long); } }
        public long Zero { get { return 0; } }
        public long One { get { return 1; } }
        public long Two { get { return 2; } }
        public bool IsUnsigned { get { return true; } }
        public long Convert(int a) { return (long)a; }
        public long Convert(BigInteger a) { return (long)a; }
        public int ToInt32(long a) { return (int)a; }
        public BigInteger ToBigInteger(long a) { return a; }
        public long Add(long a, long b) { return a + b; }
        public long Subtract(long a, long b) { return a - b; }
        public long Multiply(long a, long b) { return a * b; }
        public long Divide(long a, long b) { return a / b; }
        public long Modulus(long a, long b) { return a % b; }
        public long Negate(long a) { return 0 - a; }
        public long LeftShift(long a, int n) { return n < 64 ? a << n : 0; }
        public long RightShift(long a, int n) { return n < 64 ? a >> n : 0; }
        public long And(long a, long b) { return a & b; }
        public long Or(long a, long b) { return a | b; }
        public long ExclusiveOr(long a, long b) { return a ^ b; }
        public long OnesComplement(long a) { return ~a; }
        public bool IsZero(long a) { return a == 0; }
        public bool IsOne(long a) { return a == 1; }
        public bool IsEven(long a) { return (a & 1) == 0; }
        public bool Equals(long x, long y) { return x.Equals(y); }
        public int GetHashCode(long obj) { return obj.GetHashCode(); }
        public int Compare(long x, long y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(long a) { return (uint)(a & uint.MaxValue); }

        public long Power(long a, long b) { return IntegerMath.Power(a, b); }
        public long SquareRoot(long a) { return IntegerMath.SquareRoot(a); }
        public long GreatestCommonDivisor(long a, long b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public long ModularSum(long a, long b, long modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public long ModularDifference(long a, long b, long modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public long ModularProduct(long a, long b, long modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public long ModularPower(long value, long exponent, long modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public long ModularInverse(long value, long modulus) { return IntegerMath.ModularInverse(value, modulus); }
    }
}
