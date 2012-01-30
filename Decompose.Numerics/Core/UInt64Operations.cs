using System.Numerics;
namespace Decompose.Numerics
{
    public class UInt64Operations : IOperations<ulong>
    {
        private IRandomNumberAlgorithm<ulong> random;
        public IRandomNumberAlgorithm<ulong> Random
        {
            get
            {
                if (random == null)
                    random = new MersenneTwister64(0);
                return random;
            }
        }

        public ulong Convert(int a)
        {
            return (ulong)a;
        }

        public BigInteger ToBigInteger(ulong a)
        {
            return a;
        }

        public ulong Add(ulong a, ulong b)
        {
            return a + b;
        }

        public ulong Subtract(ulong a, ulong b)
        {
            return a - b;
        }

        public ulong Multiply(ulong a, ulong b)
        {
            return a * b;
        }

        public ulong Divide(ulong a, ulong b)
        {
            return a / b;
        }

        public ulong LeftShift(ulong a, int n)
        {
            return a << n;
        }

        public ulong RightShift(ulong a, int n)
        {
            return a >> n;
        }

        public ulong ModularProduct(ulong a, ulong b, ulong modulus)
        {
            return a * b % modulus;
        }

        public ulong ModularPower(ulong value, ulong exponent, ulong modulus)
        {
            return IntegerMath.ModularPower(value, exponent, modulus);
        }

        public bool IsEven(ulong a)
        {
            return (a & 1) == 0;
        }

        public bool Equals(ulong x, ulong y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(ulong obj)
        {
            return obj.GetHashCode();
        }

        public int Compare(ulong x, ulong y)
        {
            return x.CompareTo(y);
        }
    }
}
