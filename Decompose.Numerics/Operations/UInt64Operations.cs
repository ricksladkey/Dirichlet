using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt64Operations : Operations<ulong>
    {
        public override Integer<ulong> Wrap(ulong value) { return new Integer<ulong>(value, this); }
        public override ulong Zero { get { return 0; } }
        public override ulong One { get { return 1; } }
        public override ulong Two { get { return 2; } }
        public override bool IsUnsigned { get { return true; } }
        public override ulong Convert(int a) { return (ulong)a; }
        public override BigInteger ToBigInteger(ulong a) { return a; }
        public override ulong Add(ulong a, ulong b) { return a + b; }
        public override ulong Subtract(ulong a, ulong b) { return a - b; }
        public override ulong Multiply(ulong a, ulong b) { return a * b; }
        public override ulong Divide(ulong a, ulong b) { return a / b; }
        public override ulong Modulus(ulong a, ulong b) { return a % b; }
        public override ulong Negate(ulong a) { return 0 - a; }
        public override ulong LeftShift(ulong a, int n) { return n < 64 ? a << n : 0; }
        public override ulong RightShift(ulong a, int n) { return n < 64 ? a >> n : 0; }
        public override ulong And(ulong a, ulong b) { return a & b; }
        public override ulong Or(ulong a, ulong b) { return a | b; }
        public override ulong ExclusiveOr(ulong a, ulong b) { return a ^ b; }
        public override bool IsZero(ulong a) { return a == 0; }
        public override bool IsOne(ulong a) { return a == 1; }
        public override bool IsEven(ulong a) { return (a & 1) == 0; }
        public override bool Equals(ulong x, ulong y) { return x.Equals(y); }
        public override int GetHashCode(ulong obj) { return obj.GetHashCode(); }
        public override int Compare(ulong x, ulong y) { return x.CompareTo(y); }
        public override uint LeastSignificantWord(ulong a) { return (uint)(a & uint.MaxValue); }

        public override ulong SquareRoot(ulong a) { return IntegerMath.SquareRoot(a); }
        public override ulong GreatestCommonDivisor(ulong a, ulong b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public override ulong ModularSum(ulong a, ulong b, ulong modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public override ulong ModularDifference(ulong a, ulong b, ulong modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public override ulong ModularProduct(ulong a, ulong b, ulong modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public override ulong ModularPower(ulong value, ulong exponent, ulong modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public override ulong ModularInverse(ulong value, ulong modulus) { return IntegerMath.ModularInverse(value, modulus); }
    }
}
