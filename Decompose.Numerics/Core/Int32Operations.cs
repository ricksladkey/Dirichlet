using System.Numerics;

namespace Decompose.Numerics
{
    public class Int32Operations : IOperations<int>
    {
        public int Zero { get { return 0; } }
        public int One { get { return 1; } }
        public int Two { get { return 2; } }
        public bool IsUnsigned { get { return false; } }
        public int Convert(int a) { return (int)a; }
        public BigInteger ToBigInteger(int a) { return a; }
        public int Add(int a, int b) { return a + b; }
        public int Subtract(int a, int b) { return a - b; }
        public int Multiply(int a, int b) { return a * b; }
        public int Divide(int a, int b) { return a / b; }
        public int Modulus(int a, int b) { return a % b; }
        public int Negate(int a) { return 0 - a; }
        public int LeftShift(int a, int n) { return a << n; }
        public int RightShift(int a, int n) { return a >> n; }
        public int And(int a, int b) { return a & b; }
        public int Or(int a, int b) { return a | b; }
        public int ExclusiveOr(int a, int b) { return a ^ b; }
        public bool IsZero(int a) { return a == 0; }
        public bool IsOne(int a) { return a == 1; }
        public bool IsEven(int a) { return (a & 1) == 0; }
        public bool Equals(int x, int y) { return x.Equals(y); }
        public int GetHashCode(int obj) { return obj.GetHashCode(); }
        public int Compare(int x, int y) { return x.CompareTo(y); }
        public int ModularProduct(int a, int b, int modulus) { return a * b % modulus; }
        public int ModularPower(int value, int exponent, int modulus) { return IntegerMath.ModularPower(value, exponent, modulus); }
    }
}
