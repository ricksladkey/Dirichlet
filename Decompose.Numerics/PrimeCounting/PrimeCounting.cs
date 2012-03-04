using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class PrimeCounting
    {
        private class DivisorsRange
        {
            private byte[] divisors;

            public DivisorsRange(int n)
            {
                int size = n + 1;
                divisors = new byte[size];
                for (int i = 1; i < size; i++)
                {
                    for (int j = i; j < size; j += i)
                        ++divisors[j];
                }
#if false
                for (int i = 1; i <= n; i++)
                {
                    if (divisors[i] != IntegerMath.NumberOfDivisors(i))
                        Debugger.Break();
                }
#endif
            }

            public int this[int index]
            {
                get { return divisors[index]; }
            }
        }

        private class MobiusRange
        {
            private const int squareSentinel = 255;
            private byte[] primeDivisors;

            public MobiusRange(int n)
            {
                int size = n + 1;
                primeDivisors = new byte[size];
                for (int i = 2; i < size; i++)
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

        private const int sizeSmall = 1024;
        private int[] piSmall;
        private int[] tauSumSmall;

        public PrimeCounting()
        {
            var n = sizeSmall;
            var i = 0;
            var count = 0;
            piSmall = new int[n];
            foreach (var p in new SieveOfErostothones())
            {
                while (i < p && i < n)
                    piSmall[i++] = count;
                if (i < n)
                    piSmall[i++] = ++count;
                if (i == n)
                    break;
            }
            var divisors = new DivisorsRange(n);
            tauSumSmall = new int[n];
            tauSumSmall[0] = divisors[0];
            for (var j = 1; j < n; j++)
                tauSumSmall[j] = (tauSumSmall[j - 1] + divisors[j]) & 3;
#if false
            for (int j = 0; j < n; j++)
            {
                if (tauSumSmall[j] != TauSumSmall(j))
                    Debugger.Break();
            }
#endif
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
            if (y < tauSumSmall.Length)
                return tauSumSmall[y];

            //Console.WriteLine("TauSum({0})", y);
#if false
            var sum = 0;
            if (y < 1024)
                return TauSumSmall(y);
            var limit = (int)IntegerMath.FloorSquareRoot(y);
            var threads = 8;
            var incr = (limit + threads - 1) / threads;
            Parallel.For(0, 8,
                () => 0,
                (thread, loop, subtotal) =>
                {
                    var min = incr * thread;
                    var max = Math.Min(min + incr, limit);
                    for (var n = min + 1; n <= max; n++)
                        subtotal ^= (int)((y / n) & 1);
                    return subtotal;
                },
                subtotal => Interlocked.Add(ref sum, subtotal));
            sum = 2 * sum - limit * limit;
#endif
#if false
            var sum = 0;
            var limit = (int)IntegerMath.FloorSquareRoot(y);
            for (var n = 1; n <= limit; n++)
                sum ^= (int)((y / n) & 1);
            sum = 2 * sum - limit * limit;
#endif
#if false
            var sum = 0;
            var n = (long)1;
            var squared = y - 1;
            while (true)
            {
                sum ^= (int)((y / n) & 1);
                squared -= 2 * n + 1;
                if (squared < 0)
                    break;
                ++n;
            }
            sum = 2 * sum - (int)((n * n) & 3);
#endif
#if true
            var sqrt = 0;
            var sum = TauSumInner(y, out sqrt);
            sum = 2 * sum - (int)((sqrt * sqrt) & 3);
#endif
            return sum & 3;
        }

        public int TauSumInner(long y, out int sqrt)
        {
            if (y <= int.MaxValue)
                return TauSumInnerSmall((int)y, out sqrt);
            return TauSumInnerLarge(y, out sqrt);
        }

        public int TauSumInnerSmall(int y, out int sqrt)
        {
            if (y == 0)
            {
                sqrt = 0;
                return 0;
            }
            var sum = 0;
            var n = 1;
            var squared = y - 1;
            while (true)
            {
                sum ^= (int)(y / n);
                squared -= 2 * n + 1;
                if (squared < 0)
                    break;
                ++n;
            }
            sqrt = (int)n;
            return sum & 1;
        }

        public int TauSumInnerLarge(long y, out int sqrt)
        {
            var limit = (int)Math.Floor(Math.Sqrt(y));
            var sum1 = 0;
            var current = limit - 1;
            var delta = 1;
            var i = limit;
            while (i > 0)
            {
                var product = (long)(current + delta) * i;
                if (product > y)
                    --delta;
                else if (product + i <= y)
                {
                    ++delta;
                    if (product + 2 * i <= y)
                        break;
                }
                current += delta;
                sum1 ^= current;
                --i;
            }
            sum1 &= 1;
            var sum2 = 0;
            var count2 = 0;
            while (i > 0)
            {
                sum2 ^= (int)(y % (i << 1)) - i;
                --i;
                ++count2;
            }
            sum2 = (sum2 >> 31) & 1;
            if ((count2 & 1) != 0)
                sum2 ^= 1;
            sqrt = limit;
            return sum1 ^ sum2;
        }

        private int TauSumSmall(long y)
        {
            if (y == 0)
                return 0;
            var sum = 0;
            var n = (long)1;
            var squared = y - 1;
            while (true)
            {
                sum ^= (int)((y / n) & 1);
                squared -= 2 * n + 1;
                if (squared < 0)
                    break;
                ++n;
            }
            sum = 2 * sum - (int)((n * n) & 3);
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
            var nLast = (BigInteger)0;
            var tauLast = 0;
            for (var d = (BigInteger)1; d <= limit; d++)
            {
                var mu = IntegerMath.Mobius(d);
                if (mu != 0)
                {
                    var n = x / (d * d);
                    var tau = n == nLast ? tauLast : TauSum(n);
                    if (mu == 1)
                        sum += tau;
                    else
                        sum += 4 - tau;
                    tauLast = tau;
                    nLast = n;
                }
            }
            return sum;
        }

        private int SumTwoToTheOmega(BigInteger x, int limit)
        {
            if (x < UInt128.MaxValue)
                return SumTwoToTheOmega((UInt128)x, limit);
            var sum = 0;
            var mobius = new MobiusRange(limit + 1);
#if false
            var nLast = (BigInteger)0;
            var tauLast = 0;
            for (var d = 1; d <= limit; d++)
            {
                var mu = mobius[d];
                if (mu != 0)
                {
                    var n = x / ((long)d * d);
                    var tau = n == nLast ? tauLast : TauSum(n);
                    if (mu == 1)
                        sum += tau;
                    else
                        sum += 4 - tau;
                    tauLast = tau;
                    nLast = n;
                }
            }
#endif
#if false
            Parallel.For(1, limit + 1,
                () => 0,
                (d, loop, subtotal) =>
                {
                    var mu = mobius[d];
                    if (mu != 0)
                    {
                        var n = x / ((long)d * d);
                        var tau = TauSum(n);
                        if (mu == 1)
                            subtotal += tau;
                        else
                            subtotal += 4 - tau;
                    }
                    return subtotal;
                },
                subtotal => Interlocked.Add(ref sum, subtotal));
#endif
#if true
            var chunkSize = 32;
            var chunks = (limit + chunkSize - 1) / chunkSize;
            Parallel.For(0, chunks,
                () => 0,
                (chunk, loop, subtotal) =>
                {
                    var min = chunk * chunkSize;
                    var max = Math.Min(min + chunkSize, limit);
                    for (var d = min + 1; d <= max; d++)
                    {
                        var mu = mobius[d];
                        if (mu != 0)
                        {
                            var n = x / ((long)d * d);
                            var tau = TauSum(n);
                            if (mu == 1)
                                subtotal += tau;
                            else
                                subtotal += 4 - tau;
                        }
                    }
                    return subtotal;
                },
                subtotal => Interlocked.Add(ref sum, subtotal));
#endif
            return sum;
        }

        private int SumTwoToTheOmega(UInt128 x, int limit)
        {
            var sum = 0;
            var mobius = new MobiusRange(limit + 1);
            var chunkSize = 10;
            var chunks = (limit + chunkSize - 1) / chunkSize;
            Parallel.For(0, chunks,
                () => 0,
                (chunk, loop, subtotal) =>
                {
                    var min = chunk * chunkSize;
                    var max = Math.Min(min + chunkSize, limit);
                    for (var d = min + 1; d <= max; d++)
                    {
                        var mu = mobius[d];
                        if (mu != 0)
                        {
                            var n = x / ((ulong)d * (ulong)d);
                            var tau = TauSum(n);
                            if (mu == 1)
                                subtotal += tau;
                            else
                                subtotal += 4 - tau;
                        }
                    }
                    return subtotal;
                },
                subtotal => Interlocked.Add(ref sum, subtotal));
            return sum;
        }

        public int TauSum(BigInteger y)
        {
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

#if false
        private Dictionary<BigInteger, int> tauSumMap = new Dictionary<BigInteger, int>();
        public Dictionary<BigInteger, int> TauSumMap { get { return tauSumMap; } }
#endif

        private int TauSum(UInt128 y)
        {
#if false
            lock (tauSumMap)
            {
                var yy = (BigInteger)y;
                if (!tauSumMap.ContainsKey(yy))
                    tauSumMap[yy] = 0;
                ++tauSumMap[yy];
            }
#endif
            if (y <= long.MaxValue)
                return TauSum((long)y);
            var sum = 0;
            var n = (UInt128)1;
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
