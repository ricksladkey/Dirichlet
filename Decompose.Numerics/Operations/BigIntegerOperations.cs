using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class BigIntegerOperations : IOperations<BigInteger>
    {
        public Type Type { get { return typeof(BigInteger); } }
        public BigInteger MinValue { get { return BigInteger.Zero; } }
        public BigInteger MaxValue { get { return BigInteger.Zero; } }
        public BigInteger Zero { get { return BigInteger.Zero; } }
        public BigInteger One { get { return BigInteger.One; } }
        public bool IsUnsigned { get { return false; } }
        public BigInteger Convert(int a) { return a; }
        public BigInteger Convert(BigInteger a) { return a; }
        public BigInteger Convert(double a) { return (BigInteger)a; }
        public int ToInt32(BigInteger a) { return (int)a; }
        public BigInteger ToBigInteger(BigInteger a) { return a; }
        public double ToDouble(BigInteger a) { return (double)a; }
        public BigInteger Add(BigInteger a, BigInteger b) { return a + b; }
        public BigInteger Subtract(BigInteger a, BigInteger b) { return a - b; }
        public BigInteger Multiply(BigInteger a, BigInteger b) { return a * b; }
        public BigInteger Divide(BigInteger a, BigInteger b) { return a / b; }
        public BigInteger Modulo(BigInteger a, BigInteger b) { var result = a % b; if (result.Sign == -1) result += b; return result; }
        public BigInteger Remainder(BigInteger a, BigInteger b) { return a % b; }
        public BigInteger Negate(BigInteger a) { return -a; }
        public BigInteger LeftShift(BigInteger a, int n) { return a << n; }
        public BigInteger RightShift(BigInteger a, int n) { return a >> n; }
        public BigInteger And(BigInteger a, BigInteger b) { return a & b; }
        public BigInteger Or(BigInteger a, BigInteger b) { return a | b; }
        public BigInteger ExclusiveOr(BigInteger a, BigInteger b) { return a ^ b; }
        public BigInteger OnesComplement(BigInteger a) { return ~a; }
        public int Sign(BigInteger a) { return a.Sign; }
        public bool IsZero(BigInteger a) { return a.IsZero; }
        public bool IsOne(BigInteger a) { return a.IsOne; }
        public bool IsEven(BigInteger a) { return a.IsEven; }
        public bool Equals(BigInteger x, BigInteger y) { return x.Equals(y); }
        public int GetHashCode(BigInteger obj) { return obj.GetHashCode(); }
        public int Compare(BigInteger x, BigInteger y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(BigInteger a) { return (uint)(a & uint.MaxValue); }

        public BigInteger Power(BigInteger a, BigInteger b) { return IntegerMath.Power(a, b); }
        public BigInteger Root(BigInteger a, BigInteger b) { return IntegerMath.Root(a, b); }
        public BigInteger GreatestCommonDivisor(BigInteger a, BigInteger b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public BigInteger ModularSum(BigInteger a, BigInteger b, BigInteger modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public BigInteger ModularDifference(BigInteger a, BigInteger b, BigInteger modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public BigInteger ModularProduct(BigInteger a, BigInteger b, BigInteger modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public BigInteger ModularQuotient(BigInteger a, BigInteger b, BigInteger modulus) { return IntegerMath.ModularQuotient(a, b, modulus); }
        public BigInteger ModularPower(BigInteger value, BigInteger exponent, BigInteger modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public BigInteger ModularRoot(BigInteger value, BigInteger exponent, BigInteger modulus) { return IntegerMath.ModularRoot(value, exponent, modulus); }
        public BigInteger ModularInverse(BigInteger value, BigInteger modulus) { return IntegerMath.ModularInverse(value, modulus); }

        public BigInteger AbsoluteValue(BigInteger a) { return BigInteger.Abs(a); }
        public Complex Log(BigInteger a) { return BigInteger.Log(a); }
        public BigInteger Factorial(BigInteger a) { return IntegerMath.Factorial(a); }
    }
}
