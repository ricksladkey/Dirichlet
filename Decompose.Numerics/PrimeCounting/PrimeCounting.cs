using System;
using System.Diagnostics;
using System.Linq;

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
            Debug.Assert(parity == PiWithPowers(x) % 2);
            for (int j = 2; true; j++)
            {
                var root = IntegerMath.FloorRoot(x, j);
                if (root == 1)
                    break;
                parity ^= ParityOfPi(root);
            }
            Debug.Assert(parity == Pi(x) % 2);
            return parity;
        }

        private int SumTwoToTheOmega(int x)
        {
            int limit = (int)Math.Floor(Math.Sqrt(x));
            int sum = 0;
            for (int d = 1; d <= limit; d++)
            {
                var mu = Mu(d);
                var tau = TauSum(x / (d * d));
                sum += mu * tau;
            }
            return sum;
        }

        private int Mu(int y)
        {
            var factors = new TrialDivisionFactorization().Factor(y).ToArray();
            if (!IntegerMath.IsSquareFree(factors))
                return 0;
            return factors.Length % 2 == 0 ? 1 : -1;
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
    }
}
