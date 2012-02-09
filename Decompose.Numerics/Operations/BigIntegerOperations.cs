using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public class BigIntegerOperations : Operations<BigInteger>
    {
        public override Type Type { get { return typeof(BigInteger); } }
        public override Integer<BigInteger> Wrap(BigInteger value) { return new Integer<BigInteger>(value, this); }
        public override BigInteger Zero { get { return BigInteger.Zero; } }
        public override BigInteger One { get { return BigInteger.One; } }
        public override BigInteger Two { get { return 2; } }
        public override bool IsUnsigned { get { return false; } }
        public override BigInteger Convert(int a) { return a; }
        public override BigInteger ToBigInteger(BigInteger a) { return a; }
        public override BigInteger Add(BigInteger a, BigInteger b) { return a + b; }
        public override BigInteger Subtract(BigInteger a, BigInteger b) { return a - b; }
        public override BigInteger Multiply(BigInteger a, BigInteger b) { return a * b; }
        public override BigInteger Divide(BigInteger a, BigInteger b) { return a / b; }
        public override BigInteger Modulus(BigInteger a, BigInteger b) { return a % b; }
        public override BigInteger Negate(BigInteger a) { return -a; }
        public override BigInteger LeftShift(BigInteger a, int n) { return a << n; }
        public override BigInteger RightShift(BigInteger a, int n) { return a >> n; }
        public override BigInteger And(BigInteger a, BigInteger b) { return a & b; }
        public override BigInteger Or(BigInteger a, BigInteger b) { return a | b; }
        public override BigInteger ExclusiveOr(BigInteger a, BigInteger b) { return a ^ b; }
        public override bool IsZero(BigInteger a) { return a.IsZero; }
        public override bool IsOne(BigInteger a) { return a.IsOne; }
        public override bool IsEven(BigInteger a) { return a.IsEven; }
        public override bool Equals(BigInteger x, BigInteger y) { return x.Equals(y); }
        public override int GetHashCode(BigInteger obj) { return obj.GetHashCode(); }
        public override int Compare(BigInteger x, BigInteger y) { return x.CompareTo(y); }
        public override uint LeastSignificantWord(BigInteger a) { return (uint)(a & uint.MaxValue); }

        public override BigInteger SquareRoot(BigInteger a) { return IntegerMath.SquareRoot(a); }
        public override BigInteger GreatestCommonDivisor(BigInteger a, BigInteger b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public override BigInteger ModularSum(BigInteger a, BigInteger b, BigInteger modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public override BigInteger ModularDifference(BigInteger a, BigInteger b, BigInteger modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public override BigInteger ModularProduct(BigInteger a, BigInteger b, BigInteger modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public override BigInteger ModularPower(BigInteger value, BigInteger exponent, BigInteger modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public override BigInteger ModularInverse(BigInteger value, BigInteger modulus) { return IntegerMath.ModularInverse(value, modulus); }
    }
}
