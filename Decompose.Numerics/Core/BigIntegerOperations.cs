using System.Numerics;

namespace Decompose.Numerics
{
    public class BigIntegerOperations : IOperations<BigInteger>
    {
        public bool IsUnsigned { get { return false; } }

        private IRandomNumberGenerator<BigInteger> random;
        public IRandomNumberGenerator<BigInteger> Random
        {
            get
            {
                if (random == null)
                    random = new MersenneTwister(0).CreateInstance<BigInteger>();
                return random;
            }
        }

        public BigInteger Convert(int a)
        {
            return a;
        }

        public BigInteger ToBigInteger(BigInteger a)
        {
            return a;
        }

        public BigInteger Add(BigInteger a, BigInteger b)
        {
            return a + b;
        }

        public BigInteger Subtract(BigInteger a, BigInteger b)
        {
            return a - b;
        }

        public BigInteger Multiply(BigInteger a, BigInteger b)
        {
            return a * b;
        }

        public BigInteger Divide(BigInteger a, BigInteger b)
        {
            return a / b;
        }

        public BigInteger Modulus(BigInteger a, BigInteger b)
        {
            return a % b;
        }

        public BigInteger Negate(BigInteger a)
        {
            return -a;
        }

        public BigInteger LeftShift(BigInteger a, int n)
        {
            return a << n;
        }

        public BigInteger RightShift(BigInteger a, int n)
        {
            return a >> n;
        }

        public BigInteger And(BigInteger a, BigInteger b)
        {
            return a & b;
        }

        public BigInteger Or(BigInteger a, BigInteger b)
        {
            return a | b;
        }

        public BigInteger ExclusiveOr(BigInteger a, BigInteger b)
        {
            return a ^ b;
        }

        public BigInteger ModularProduct(BigInteger a, BigInteger b, BigInteger modulus)
        {
            return a * b % modulus;
        }

        public BigInteger ModularPower(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            return BigInteger.ModPow(value, exponent, modulus);
        }

        public bool IsEven(BigInteger a)
        {
            return a.IsEven;
        }

        public bool Equals(BigInteger x, BigInteger y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(BigInteger obj)
        {
            return obj.GetHashCode();
        }

        public int Compare(BigInteger x, BigInteger y)
        {
            return x.CompareTo(y);
        }
    }
}
