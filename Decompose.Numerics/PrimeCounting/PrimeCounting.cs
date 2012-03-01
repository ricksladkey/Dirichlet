using System;
using System.Linq;
using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class PrimeCounting
    {
        private class MobiusRange
        {
            private const int squareSentinel = 255;
            private byte[] primeDivisors;

            public MobiusRange(int n)
            {
                int size = n + 1;
                primeDivisors = new byte[size];
                for (int i = 2; i < n; i++)
                {
                    if (primeDivisors[i] == 0)
                    {
                        for (int j = i; j < size; j += i)
                            ++primeDivisors[j];
                    }
                }
                for (int i = 2; true; i++)
                {
                    if (primeDivisors[i] == 1)
                    {
                        var iSquared = i * i;
                        if (iSquared > size)
                            break;
                        for (int j = iSquared; j < size; j += iSquared)
                            primeDivisors[j] = squareSentinel;
                    }
                }
#if false
                for (int i = 1; i <= n; i++)
                {
                    if (this[i] != IntegerMath.Mobius(i))
                        Debugger.Break();
                }
#endif
            }

            public int this[int index]
            {
                get
                {
                    var d = primeDivisors[index];
                    if (d == squareSentinel)
                        return 0;
                    return d % 2 == 0 ? 1 : -1;
                }
            }
        }

        private int[] piSmall;

        public PrimeCounting()
        {
            var i = 0;
            var count = 0;
            piSmall = new int[1024];
            foreach (var p in new SieveOfErostothones())
            {
                while (i < p && i < piSmall.Length)
                    piSmall[i++] = count;
                if (i < piSmall.Length)
                    piSmall[i++] = ++count;
                if (i == piSmall.Length)
                    break;
            }
        }

        public int Pi(int x)
        {
            if (x < piSmall.Length)
                return piSmall[x];
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
            if (x < piSmall.Length)
                return piSmall[x] % 2;
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
            var limit = (int)IntegerMath.FloorSquareRoot(x);
            var sum = 0;
            var mobius = new MobiusRange(limit + 1);
            for (var d = 1; d <= limit; d++)
            {
                var mu = IntegerMath.Mobius(d);
                if (mu == 1)
                    sum += TauSum(x / ((long)d * d));
                else if (mu == -1)
                    sum += 4 - TauSum(x / ((long)d * d));
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
                sum ^= (int)(term & 1);
                ++n;
            }
            sum = 2 * sum + n - 1;
            return sum & 3;
        }

        public int ParityOfPi(BigInteger x)
        {
            if (x < piSmall.Length)
                return piSmall[(int)x] % 2;
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

        private int SumTwoToTheOmega(BigInteger x)
        {
            //Console.WriteLine("SumTwoToTheOmega({0})", x);
            var limit = IntegerMath.FloorSquareRoot(x);
            if (limit <= int.MaxValue)
                return SumTwoToTheOmega(x, (int)limit);
            var sum = 0;
            for (var d = (BigInteger)1; d <= limit; d++)
            {
                var mu = IntegerMath.Mobius(d);
                if (mu == 1)
                    sum += TauSum(x / (d * d));
                else if (mu == -1)
                    sum += 4 - TauSum(x / (d * d));
            }
            return sum;
        }

        private int SumTwoToTheOmega(BigInteger x, int limit)
        {
            var sum = 0;
            var mobius = new MobiusRange(limit + 1);
            for (var d = 1; d <= limit; d++)
            {
                var mu = mobius[d];
                if (mu == 1)
                    sum += TauSum(x / ((long)d * d));
                else if (mu == -1)
                    sum += 4 - TauSum(x / ((long)d * d));
            }
            return sum;
        }

        private int TauSum(BigInteger y)
        {
            //Console.WriteLine("TauSum({0})", y);
            if (y <= long.MaxValue)
                return TauSum((long)y);
            var sum = 0;
            var  n = (BigInteger)1;
            while (true)
            {
                var term = y / n - n;
                if (term < 0)
                    break;
                sum ^= (int)(term & 1);
                ++n;
            }
            sum = 2 * sum + (int)((n - 1) & 3);
            return (int)(sum & 3);
        }
    }
}
