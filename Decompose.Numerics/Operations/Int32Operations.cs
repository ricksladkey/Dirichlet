using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class Int32Operations : IOperations<int>
    {
        public Type Type { get { return typeof(int); } }
        public int Zero { get { return 0; } }
        public int One { get { return 1; } }
        public int Two { get { return 2; } }
        public bool IsUnsigned { get { return false; } }
        public int Convert(int a) { return (int)a; }
        public int Convert(BigInteger a) { return (int)a; }
        public int ToInt32(int a) { return a; }
        public BigInteger ToBigInteger(int a) { return a; }
        public double ToDouble(int a) { return (double)a; }
        public int Add(int a, int b) { return a + b; }
        public int Subtract(int a, int b) { return a - b; }
        public int Multiply(int a, int b) { return a * b; }
        public int Divide(int a, int b) { return a / b; }
        public int Modulus(int a, int b) { return a % b; }
        public int Negate(int a) { return 0 - a; }
        public int LeftShift(int a, int n) { return n < 32 ? a << n : 0; }
        public int RightShift(int a, int n) { return n < 32 ? a >> n : 0; }
        public int And(int a, int b) { return a & b; }
        public int Or(int a, int b) { return a | b; }
        public int ExclusiveOr(int a, int b) { return a ^ b; }
        public int OnesComplement(int a) { return ~a; }
        public bool IsZero(int a) { return a == 0; }
        public bool IsOne(int a) { return a == 1; }
        public bool IsEven(int a) { return (a & 1) == 0; }
        public bool Equals(int x, int y) { return x.Equals(y); }
        public int GetHashCode(int obj) { return obj.GetHashCode(); }
        public int Compare(int x, int y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(int a) { return (uint)(a & uint.MaxValue); }

        public int Power(int a, int b) { return IntegerMath.Power(a, b); }
        public int Root(int a, int b) { return IntegerMath.Root(a, b); }
        public int GreatestCommonDivisor(int a, int b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public int ModularSum(int a, int b, int modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public int ModularDifference(int a, int b, int modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public int ModularProduct(int a, int b, int modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public int ModularPower(int value, int exponent, int modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public int ModularRoot(int value, int exponent, int modulus) { return IntegerMath.ModularRoot(value, exponent, modulus); }
        public int ModularInverse(int value, int modulus) { return IntegerMath.ModularInverse(value, modulus); }
    }
}
