using System.Numerics;

namespace Decompose.Numerics
{
    public class Int32Operations : IOperations<int>
    {
        public bool IsUnsigned { get { return false; } }

        private IRandomNumberGenerator<int> random;
        public IRandomNumberGenerator<int> Random
        {
            get
            {
                if (random == null)
                    random = new MersenneTwister(0).CreateInstance<int>();
                return random;
            }
        }

        public int Convert(int a)
        {
            return (int)a;
        }

        public BigInteger ToBigInteger(int a)
        {
            return a;
        }

        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }

        public int Divide(int a, int b)
        {
            return a / b;
        }

        public int Modulus(int a, int b)
        {
            return a % b;
        }

        public int Negate(int a)
        {
            return 0 - a;
        }

        public int LeftShift(int a, int n)
        {
            return a << n;
        }

        public int RightShift(int a, int n)
        {
            return a >> n;
        }

        public int And(int a, int b)
        {
            return a & b;
        }

        public int Or(int a, int b)
        {
            return a | b;
        }

        public int ExclusiveOr(int a, int b)
        {
            return a ^ b;
        }

        public int ModularProduct(int a, int b, int modulus)
        {
            return a * b % modulus;
        }

        public int ModularPower(int value, int exponent, int modulus)
        {
            return IntegerMath.ModularPower(value, exponent, modulus);
        }

        public bool IsEven(int a)
        {
            return (a & 1) == 0;
        }

        public bool Equals(int x, int y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }

        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }
}
