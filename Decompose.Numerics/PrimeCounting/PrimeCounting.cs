using System;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
    public class PrimeCounting
    {
        public int Pi(int x)
        {
            return new SieveOfErostothones().TakeWhile(p => p <= x).Count();
        }

        public int PiWithPowers(int x)
        {
            var sum = Pi(x);
            for (int j = 2; true; j++)
            {
                var root = IntegerMath.FloorRoot(x, j);
                if (root == 1)
                    break;
                sum += Pi(root);
            }
            return sum;
        }

        public int ParityOfPi(int x)
        {
            if (x < 2)
                return 0;
            var parity = SumTwoToTheOmega(x) / 2 % 2;
            for (int j = 2; true; j++)
            {
                var root = IntegerMath.FloorRoot(x, j);
                if (root == 1)
                    break;
                parity ^= ParityOfPi(root);
            }
            return parity;
        }

        private int SumTwoToTheOmega(int x)
        {
            var limit = IntegerMath.FloorSquareRoot(x);
            var sum = 0;
            for (int d = 1; d <= limit; d++)
            {
                var mu = IntegerMath.Mobius(d);
                var tau = TauSum(x / (d * d));
                sum += mu * tau;
            }
            return sum;
        }

        private int TauSum(int y)
        {
            var sum = 0;
            int n = 1;
            while (true)
            {
                var term = y / n - n;
                if (term < 0)
                    break;
                sum += term;
                ++n;
            }
            sum = 2 * sum + n - 1;
            return sum;
        }

        public BigInteger ParityOfPi(BigInteger x)
        {
            if (x < 2)
                return 0;
            var parity = SumTwoToTheOmega(x) / 2 % 2;
            for (int j = 2; true; j++)
            {
                var root = IntegerMath.FloorRoot(x, j);
                if (root == 1)
                    break;
                parity ^= ParityOfPi(root);
            }
            return parity;
        }

        private BigInteger SumTwoToTheOmega(BigInteger x)
        {
            var limit = IntegerMath.FloorSquareRoot(x);
            var sum = (BigInteger)0;
            for (BigInteger d = 1; d <= limit; d++)
            {
                var mu = IntegerMath.Mobius(d);
                var tau = TauSum(x / (d * d));
                sum += mu * tau;
            }
            return sum;
        }

        private BigInteger TauSum(BigInteger y)
        {
            var sum = (BigInteger)0;
            var  n = (BigInteger)1;
            while (true)
            {
                var term = y / n - n;
                if (term < 0)
                    break;
                sum += term;
                ++n;
            }
            sum = 2 * sum + n - 1;
            return sum;
        }
    }
}
