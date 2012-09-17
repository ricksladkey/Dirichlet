using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class UInt128Operations : IOperations<UInt128>
    {
        public Type Type { get { return typeof(UInt128); } }
        public UInt128 Zero { get { return UInt128.Zero; } }
        public UInt128 One { get { return UInt128.One; } }
        public UInt128 Two { get { return 2; } }
        public bool IsUnsigned { get { return true; } }
        public UInt128 Convert(int a) { return (UInt128)a; }
        public UInt128 Convert(BigInteger a) { return (UInt128)a; }
        public UInt128 Convert(double a) { return (UInt128)a; }
        public int ToInt32(UInt128 a) { return (int)a; }
        public BigInteger ToBigInteger(UInt128 a) { return a; }
        public double ToDouble(UInt128 a) { return (double)a; }
        public UInt128 Add(UInt128 a, UInt128 b) { return a + b; }
        public UInt128 Subtract(UInt128 a, UInt128 b) { return a - b; }
        public UInt128 Multiply(UInt128 a, UInt128 b) { return a * b; }
        public UInt128 Divide(UInt128 a, UInt128 b) { return a / b; }
        public UInt128 Remainder(UInt128 a, UInt128 b) { return a % b; }
        public UInt128 Modulo(UInt128 a, UInt128 b) { return a % b; }
        public UInt128 Negate(UInt128 a) { return 0 - a; }
        public UInt128 LeftShift(UInt128 a, int n) { return n < 64 ? a << n : 0; }
        public UInt128 RightShift(UInt128 a, int n) { return n < 64 ? a >> n : 0; }
        public UInt128 And(UInt128 a, UInt128 b) { return a & b; }
        public UInt128 Or(UInt128 a, UInt128 b) { return a | b; }
        public UInt128 ExclusiveOr(UInt128 a, UInt128 b) { return a ^ b; }
        public UInt128 OnesComplement(UInt128 a) { return ~a; }
        public int Sign(UInt128 a) { return a.Sign; }
        public bool IsZero(UInt128 a) { return a.IsZero; }
        public bool IsOne(UInt128 a) { return a.IsOne; }
        public bool IsEven(UInt128 a) { return a.IsEven; }
        public bool Equals(UInt128 x, UInt128 y) { return x.Equals(y); }
        public int GetHashCode(UInt128 obj) { return obj.GetHashCode(); }
        public int Compare(UInt128 x, UInt128 y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(UInt128 a) { return (uint)a.S0; }

        public UInt128 Power(UInt128 a, UInt128 b) { return IntegerMath.Power(a, b); }
        public UInt128 Root(UInt128 a, UInt128 b) { return IntegerMath.Root(a, b); }
        public UInt128 GreatestCommonDivisor(UInt128 a, UInt128 b) { return (UInt128)IntegerMath.GreatestCommonDivisor(a, b); }
        public UInt128 ModularSum(UInt128 a, UInt128 b, UInt128 modulus) { return (UInt128)IntegerMath.ModularSum(a, b, modulus); }
        public UInt128 ModularDifference(UInt128 a, UInt128 b, UInt128 modulus) { return (UInt128)IntegerMath.ModularDifference(a, b, modulus); }
        public UInt128 ModularProduct(UInt128 a, UInt128 b, UInt128 modulus) { return (UInt128)IntegerMath.ModularProduct(a, b, modulus); }
        public UInt128 ModularQuotient(UInt128 a, UInt128 b, UInt128 modulus) { return (UInt128)IntegerMath.ModularQuotient(a, b, modulus); }
        public UInt128 ModularPower(UInt128 value, UInt128 exponent, UInt128 modulus) { return (UInt128)IntegerMath.ModularPower(value, exponent, modulus); }
        public UInt128 ModularRoot(UInt128 value, UInt128 exponent, UInt128 modulus) { return (UInt128)IntegerMath.ModularRoot(value, exponent, modulus); }
        public UInt128 ModularInverse(UInt128 value, UInt128 modulus) { return (UInt128)IntegerMath.ModularInverse(value, modulus); }

        public UInt128 AbsoluteValue(UInt128 a) { return a; }
        public Complex Log(UInt128 a) { return Math.Log((double)a); }
        public UInt128 Factorial(UInt128 a) { return (UInt128)IntegerMath.Factorial(a); }
    }
}
