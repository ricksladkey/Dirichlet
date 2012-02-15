using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class UInt32Operations : IOperations<uint>
    {
        public Type Type { get { return typeof(uint); } }
        public uint Zero { get { return 0; } }
        public uint One { get { return 1; } }
        public uint Two { get { return 2; } }
        public bool IsUnsigned { get { return false; } }
        public uint Convert(int a) { return (uint)a; }
        public uint Convert(BigInteger a) { return (uint)a; }
        public uint Convert(double a) { return (uint)a; }
        public int ToInt32(uint a) { return (int)a; }
        public BigInteger ToBigInteger(uint a) { return a; }
        public double ToDouble(uint a) { return (double)a; }
        public uint Add(uint a, uint b) { return a + b; }
        public uint Subtract(uint a, uint b) { return a - b; }
        public uint Multiply(uint a, uint b) { return a * b; }
        public uint Divide(uint a, uint b) { return a / b; }
        public uint Remainder(uint a, uint b) { return a % b; }
        public uint Modulo(uint a, uint b) { return a % b; }
        public uint Negate(uint a) { return 0 - a; }
        public uint LeftShift(uint a, int n) { return n < 32 ? a << n : 0; }
        public uint RightShift(uint a, int n) { return n < 32 ? a >> n : 0; }
        public uint And(uint a, uint b) { return a & b; }
        public uint Or(uint a, uint b) { return a | b; }
        public uint ExclusiveOr(uint a, uint b) { return a ^ b; }
        public uint OnesComplement(uint a) { return ~a; }
        public bool IsZero(uint a) { return a == 0; }
        public bool IsOne(uint a) { return a == 1; }
        public bool IsEven(uint a) { return (a & 1) == 0; }
        public bool Equals(uint x, uint y) { return x.Equals(y); }
        public int GetHashCode(uint obj) { return obj.GetHashCode(); }
        public int Compare(uint x, uint y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(uint a) { return (uint)(a & uint.MaxValue); }

        public uint Power(uint a, uint b) { return IntegerMath.Power(a, b); }
        public uint Root(uint a, uint b) { return IntegerMath.Root(a, b); }
        public uint GreatestCommonDivisor(uint a, uint b) { return IntegerMath.GreatestCommonDivisor(a, b); }
        public uint ModularSum(uint a, uint b, uint modulus) { return IntegerMath.ModularSum(a, b, modulus); }
        public uint ModularDifference(uint a, uint b, uint modulus) { return IntegerMath.ModularDifference(a, b, modulus); }
        public uint ModularProduct(uint a, uint b, uint modulus) { return IntegerMath.ModularProduct(a, b, modulus); }
        public uint ModularQuotient(uint a, uint b, uint modulus) { return IntegerMath.ModularQuotient(a, b, modulus); }
        public uint ModularPower(uint value, uint exponent, uint modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
        public uint ModularRoot(uint value, uint exponent, uint modulus) { return IntegerMath.ModularRoot(value, exponent, modulus); }
        public uint ModularInverse(uint value, uint modulus) { return IntegerMath.ModularInverse(value, modulus); }
    }
}
