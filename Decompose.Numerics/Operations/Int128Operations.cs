using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class Int128Operations : IOperations<Int128>
    {
        public Type Type { get { return typeof(Int128); } }
        public Int128 Zero { get { return 0; } }
        public Int128 One { get { return 1; } }
        public Int128 Two { get { return 2; } }
        public bool IsUnsigned { get { return false; } }
        public Int128 Convert(int a) { return (Int128)a; }
        public Int128 Convert(BigInteger a) { return (Int128)a; }
        public Int128 Convert(double a) { return (Int128)a; }
        public int ToInt32(Int128 a) { return (int)a; }
        public BigInteger ToBigInteger(Int128 a) { return a; }
        public double ToDouble(Int128 a) { return (double)a; }
        public Int128 Add(Int128 a, Int128 b) { return a + b; }
        public Int128 Subtract(Int128 a, Int128 b) { return a - b; }
        public Int128 Multiply(Int128 a, Int128 b) { return a * b; }
        public Int128 Divide(Int128 a, Int128 b) { return a / b; }
        public Int128 Remainder(Int128 a, Int128 b) { return a % b; }
        public Int128 Modulo(Int128 a, Int128 b) { return a % b; }
        public Int128 Negate(Int128 a) { return 0 - a; }
        public Int128 LeftShift(Int128 a, int n) { return n < 64 ? a << n : 0; }
        public Int128 RightShift(Int128 a, int n) { return n < 64 ? a >> n : 0; }
        public Int128 And(Int128 a, Int128 b) { return a & b; }
        public Int128 Or(Int128 a, Int128 b) { return a | b; }
        public Int128 ExclusiveOr(Int128 a, Int128 b) { return a ^ b; }
        public Int128 OnesComplement(Int128 a) { return ~a; }
        public int Sign(Int128 a) { return a != 0 ? 1 : 0; }
        public bool IsZero(Int128 a) { return a == 0; }
        public bool IsOne(Int128 a) { return a == 1; }
        public bool IsEven(Int128 a) { return (a & 1) == 0; }
        public bool Equals(Int128 x, Int128 y) { return x.Equals(y); }
        public int GetHashCode(Int128 obj) { return obj.GetHashCode(); }
        public int Compare(Int128 x, Int128 y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(Int128 a) { return (uint)(a & uint.MaxValue); }

        public Int128 Power(Int128 a, Int128 b) { return IntegerMath.Power(a, b); }
        public Int128 Root(Int128 a, Int128 b) { return IntegerMath.Root(a, b); }
        public Int128 GreatestCommonDivisor(Int128 a, Int128 b) { return (Int128)IntegerMath.GreatestCommonDivisor(a, b); }
        public Int128 ModularSum(Int128 a, Int128 b, Int128 modulus) { return (Int128)IntegerMath.ModularSum(a, b, modulus); }
        public Int128 ModularDifference(Int128 a, Int128 b, Int128 modulus) { return (Int128)IntegerMath.ModularDifference(a, b, modulus); }
        public Int128 ModularProduct(Int128 a, Int128 b, Int128 modulus) { return (Int128)IntegerMath.ModularProduct(a, b, modulus); }
        public Int128 ModularQuotient(Int128 a, Int128 b, Int128 modulus) { return (Int128)IntegerMath.ModularQuotient(a, b, modulus); }
        public Int128 ModularPower(Int128 value, Int128 exponent, Int128 modulus) { return (Int128)IntegerMath.ModularPower(value, exponent, modulus); }
        public Int128 ModularRoot(Int128 value, Int128 exponent, Int128 modulus) { return (Int128)IntegerMath.ModularRoot(value, exponent, modulus); }
        public Int128 ModularInverse(Int128 value, Int128 modulus) { return (Int128)IntegerMath.ModularInverse(value, modulus); }

        public Int128 AbsoluteValue(Int128 a) { return a; }
        public Complex Log(Int128 a) { return Math.Log((double)a); }
        public Int128 Factorial(Int128 a) { return (Int128)IntegerMath.Factorial(a); }
    }
}
