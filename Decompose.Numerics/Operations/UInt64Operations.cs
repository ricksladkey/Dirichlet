using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class UInt64Operations : IOperations<ulong>
    {
        public Type Type { get { return typeof(ulong); } }
        public ulong Zero { get { return 0; } }
        public ulong One { get { return 1; } }
        public ulong Two { get { return 2; } }
        public bool IsUnsigned { get { return true; } }
        public ulong Convert(int a) { return (ulong)a; }
        public ulong Convert(BigInteger a) { return (ulong)a; }
        public int ToInt32(ulong a) { return (int)a; }
        public BigInteger ToBigInteger(ulong a) { return a; }
        public ulong Add(ulong a, ulong b) { return a + b; }
        public ulong Subtract(ulong a, ulong b) { return a - b; }
        public ulong Multiply(ulong a, ulong b) { return a * b; }
        public ulong Divide(ulong a, ulong b) { return a / b; }
        public ulong Modulus(ulong a, ulong b) { return a % b; }
        public ulong Negate(ulong a) { return 0 - a; }
        public ulong LeftShift(ulong a, int n) { return n < 64 ? a << n : 0; }
        public ulong RightShift(ulong a, int n) { return n < 64 ? a >> n : 0; }
        public ulong And(ulong a, ulong b) { return a & b; }
        public ulong Or(ulong a, ulong b) { return a | b; }
        public ulong ExclusiveOr(ulong a, ulong b) { return a ^ b; }
        public ulong Not(ulong a) { return ~a; }
        public bool IsZero(ulong a) { return a == 0; }
        public bool IsOne(ulong a) { return a == 1; }
        public bool IsEven(ulong a) { return (a & 1) == 0; }
        public bool Equals(ulong x, ulong y) { return x.Equals(y); }
        public int GetHashCode(ulong obj) { return obj.GetHashCode(); }
        public int Compare(ulong x, ulong y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(ulong a) { return (uint)(a & uint.MaxValue); }

        public ulong Power(ulong a, ulong b) { return IntegerMath.Power(a, b); }
        public ulong SquareRoot(ulong a) { return IntegerMath.SquareRoot(a); }
        public ulong GreatestCommonDivisor(ulong a, ulong b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public ulong ModularSum(ulong a, ulong b, ulong modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public ulong ModularDifference(ulong a, ulong b, ulong modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public ulong ModularProduct(ulong a, ulong b, ulong modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public ulong ModularPower(ulong value, ulong exponent, ulong modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public ulong ModularInverse(ulong value, ulong modulus) { return IntegerMath.ModularInverse(value, modulus); }
    }
}
