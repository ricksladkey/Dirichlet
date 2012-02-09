using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public class Int64Operations : Operations<long>
    {
        public override Type Type { get { return typeof(long); } }
        public override Integer<long> Wrap(long value) { return new Integer<long>(value, this); }
        public override long Zero { get { return 0; } }
        public override long One { get { return 1; } }
        public override long Two { get { return 2; } }
        public override bool IsUnsigned { get { return true; } }
        public override long Convert(int a) { return (long)a; }
        public override BigInteger ToBigInteger(long a) { return a; }
        public override long Add(long a, long b) { return a + b; }
        public override long Subtract(long a, long b) { return a - b; }
        public override long Multiply(long a, long b) { return a * b; }
        public override long Divide(long a, long b) { return a / b; }
        public override long Modulus(long a, long b) { return a % b; }
        public override long Negate(long a) { return 0 - a; }
        public override long LeftShift(long a, int n) { return n < 64 ? a << n : 0; }
        public override long RightShift(long a, int n) { return n < 64 ? a >> n : 0; }
        public override long And(long a, long b) { return a & b; }
        public override long Or(long a, long b) { return a | b; }
        public override long ExclusiveOr(long a, long b) { return a ^ b; }
        public override bool IsZero(long a) { return a == 0; }
        public override bool IsOne(long a) { return a == 1; }
        public override bool IsEven(long a) { return (a & 1) == 0; }
        public override bool Equals(long x, long y) { return x.Equals(y); }
        public override int GetHashCode(long obj) { return obj.GetHashCode(); }
        public override int Compare(long x, long y) { return x.CompareTo(y); }
        public override uint LeastSignificantWord(long a) { return (uint)(a & uint.MaxValue); }

        public override long SquareRoot(long a) { return IntegerMath.SquareRoot(a); }
        public override long GreatestCommonDivisor(long a, long b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public override long ModularSum(long a, long b, long modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public override long ModularDifference(long a, long b, long modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public override long ModularProduct(long a, long b, long modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public override long ModularPower(long value, long exponent, long modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public override long ModularInverse(long value, long modulus) { return IntegerMath.ModularInverse(value, modulus); }
    }
}
