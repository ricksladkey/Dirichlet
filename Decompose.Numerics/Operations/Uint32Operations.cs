using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt32Operations : Operations<uint>
    {
        public override Type Type { get { return typeof(uint); } }
        public override uint Zero { get { return 0; } }
        public override uint One { get { return 1; } }
        public override uint Two { get { return 2; } }
        public override bool IsUnsigned { get { return false; } }
        public override uint Convert(int a) { return (uint)a; }
        public override BigInteger ToBigInteger(uint a) { return a; }
        public override uint Add(uint a, uint b) { return a + b; }
        public override uint Subtract(uint a, uint b) { return a - b; }
        public override uint Multiply(uint a, uint b) { return a * b; }
        public override uint Divide(uint a, uint b) { return a / b; }
        public override uint Modulus(uint a, uint b) { return a % b; }
        public override uint Negate(uint a) { return 0 - a; }
        public override uint LeftShift(uint a, int n) { return n < 32 ? a << n : 0; }
        public override uint RightShift(uint a, int n) { return n < 32 ? a >> n : 0; }
        public override uint And(uint a, uint b) { return a & b; }
        public override uint Or(uint a, uint b) { return a | b; }
        public override uint ExclusiveOr(uint a, uint b) { return a ^ b; }
        public override uint Not(uint a) { return ~a; }
        public override bool IsZero(uint a) { return a == 0; }
        public override bool IsOne(uint a) { return a == 1; }
        public override bool IsEven(uint a) { return (a & 1) == 0; }
        public override bool Equals(uint x, uint y) { return x.Equals(y); }
        public override int GetHashCode(uint obj) { return obj.GetHashCode(); }
        public override int Compare(uint x, uint y) { return x.CompareTo(y); }
        public override uint LeastSignificantWord(uint a) { return (uint)(a & uint.MaxValue); }

        public override uint SquareRoot(uint a) { return IntegerMath.SquareRoot(a); }
        public override uint GreatestCommonDivisor(uint a, uint b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public override uint ModularSum(uint a, uint b, uint modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public override uint ModularDifference(uint a, uint b, uint modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public override uint ModularProduct(uint a, uint b, uint modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public override uint ModularPower(uint value, uint exponent, uint modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public override uint ModularInverse(uint value, uint modulus) { return IntegerMath.ModularInverse(value, modulus); }
    }
}
