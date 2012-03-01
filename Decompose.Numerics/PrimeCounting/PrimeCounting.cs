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

        public int ParityOfPi(long x)
        {
            if (x < 2)
                return 0;
            var parity = SumTwoToTheOmega(x) / 2 % 2;
            for (var j = 2; true; j++)
            {
                var root = IntegerMath.FloorRoot(x, j);
                if (root == 1)
                    break;
                parity ^= ParityOfPi(root);
            }
            return parity;
        }

        private int SumTwoToTheOmega(long x)
        {
            var limit = IntegerMath.FloorSquareRoot(x);
            var sum = 0;
            for (var d = (long)1; d <= limit; d++)
            {
                var mu = IntegerMath.Mobius(d);
                if (mu == 1)
                    sum += TauSum(x / (d * d));
                else if (mu == -1)
                    sum += 4 - TauSum(x / (d * d));
            }
            return sum;
        }

        private int TauSum(long y)
        {
            var sum = 0;
            var n = 1;
            while (true)
            {
                var term = y / n - n;
                if (term < 0)
                    break;
                sum += (int)term;
                ++n;
            }
            sum = 2 * sum + n - 1;
            return sum & 3;
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
            if (y <= long.MaxValue)
                return TauSum((long)y);
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
