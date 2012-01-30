using System.Numerics;

namespace Decompose.Numerics
{
    public class UInt32Operations : IOperations<uint>
    {
        private IRandomNumberAlgorithm<uint> random;
        public IRandomNumberAlgorithm<uint> Random
        {
            get
            {
                if (random == null)
                    random = new MersenneTwister32(0);
                return random;
            }
        }

        public uint Convert(int a)
        {
            return (uint)a;
        }

        public BigInteger ToBigInteger(uint a)
        {
            return a;
        }

        public uint Add(uint a, uint b)
        {
            return a + b;
        }

        public uint Subtract(uint a, uint b)
        {
            return a - b;
        }

        public uint Multiply(uint a, uint b)
        {
            return a * b;
        }

        public uint Divide(uint a, uint b)
        {
            return a / b;
        }

        public uint LeftShift(uint a, int n)
        {
            return a << n;
        }

        public uint RightShift(uint a, int n)
        {
            return a >> n;
        }

        public uint ModularProduct(uint a, uint b, uint modulus)
        {
            return a * b % modulus;
        }

        public uint ModularPower(uint value, uint exponent, uint modulus)
        {
            return IntegerMath.ModularPower(value, exponent, modulus);
        }

        public bool IsEven(uint a)
        {
            return (a & 1) == 0;
        }

        public bool Equals(uint x, uint y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(uint obj)
        {
            return obj.GetHashCode();
        }

        public int Compare(uint x, uint y)
        {
            return x.CompareTo(y);
        }
    }
}
