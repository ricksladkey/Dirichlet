using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt32Operations : IOperations<uint>
    {
        public uint Zero { get { return 0; } }
        public uint One { get { return 1; } }
        public uint Two { get { return 2; } }
        public bool IsUnsigned { get { return false; } }
        public uint Convert(int a) { return (uint)a; }
        public BigInteger ToBigInteger(uint a) { return a; }
        public uint Add(uint a, uint b) { return a + b; }
        public uint Subtract(uint a, uint b) { return a - b; }
        public uint Multiply(uint a, uint b) { return a * b; }
        public uint Divide(uint a, uint b) { return a / b; }
        public uint Modulus(uint a, uint b) { return a % b; }
        public uint Negate(uint a) { return 0 - a; }
        public uint LeftShift(uint a, int n) { return a << n; }
        public uint RightShift(uint a, int n) { return a >> n; }
        public uint And(uint a, uint b) { return a & b; }
        public uint Or(uint a, uint b) { return a | b; }
        public uint ExclusiveOr(uint a, uint b) { return a ^ b; }
        public bool IsZero(uint a) { return a == 0; }
        public bool IsOne(uint a) { return a == 1; }
        public bool IsEven(uint a) { return (a & 1) == 0; }
        public bool Equals(uint x, uint y) { return x.Equals(y); }
        public int GetHashCode(uint obj) { return obj.GetHashCode(); }
        public int Compare(uint x, uint y) { return x.CompareTo(y); }
        public uint ModularProduct(uint a, uint b, uint modulus) { return a * b % modulus; }
        public uint ModularPower(uint value, uint exponent, uint modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
    }
}
