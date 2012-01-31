using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt64Operations : IOperations<ulong>
    {
        public ulong Zero { get { return 0; } }
        public ulong One { get { return 1; } }
        public ulong Two { get { return 2; } }
        public bool IsUnsigned { get { return true; } }
        public ulong Convert(int a) { return (ulong)a; }
        public BigInteger ToBigInteger(ulong a) { return a; }
        public ulong Add(ulong a, ulong b) { return a + b; }
        public ulong Subtract(ulong a, ulong b) { return a - b; }
        public ulong Multiply(ulong a, ulong b) { return a * b; }
        public ulong Divide(ulong a, ulong b) { return a / b; }
        public ulong Modulus(ulong a, ulong b) { return a % b; }
        public ulong Negate(ulong a) { return 0 - a; }
        public ulong LeftShift(ulong a, int n) { return a << n; }
        public ulong RightShift(ulong a, int n) { return a >> n; }
        public ulong And(ulong a, ulong b) { return a & b; }
        public ulong Or(ulong a, ulong b) { return a | b; }
        public ulong ExclusiveOr(ulong a, ulong b) { return a ^ b; }
        public bool IsZero(ulong a) { return a == 0; }
        public bool IsOne(ulong a) { return a == 1; }
        public bool IsEven(ulong a) { return (a & 1) == 0; }
        public bool Equals(ulong x, ulong y) { return x.Equals(y); }
        public int GetHashCode(ulong obj) { return obj.GetHashCode(); }
        public int Compare(ulong x, ulong y) { return x.CompareTo(y); }
        public ulong ModularProduct(ulong a, ulong b, ulong modulus) { return a * b % modulus; }
        public ulong ModularPower(ulong value, ulong exponent, ulong modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
    }
}
