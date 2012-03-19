using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class PrimeCounting
    {
        private const int sizeSmall = 1024;
        private const int chunkSize = 32;
        private int threads;
        private int[] piSmall;
        private int[] tauSumSmall;
        private int[] mobiusSmall;

        public PrimeCounting(int threads)
        {
            this.threads = threads;
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
            var divisors = new DivisorsCollection(n);
            tauSumSmall = new int[n];
            tauSumSmall[0] = divisors[0];
            for (var j = 1; j < n; j++)
                tauSumSmall[j] = (tauSumSmall[j - 1] + divisors[j]) & 3;
            mobiusSmall = new MobiusCollection(sizeSmall, 0).ToArray();
        }

        public int Pi(int x)
        {
            if (x < piSmall.Length)
                return piSmall[x];
            return new PrimeCollection(x + 1, threads).Count;
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

        public int ParityOfPi(BigInteger x)
        {
            // pi(x) mod 2 = SumTwoToTheOmega(x)/2 mod 2- sum(pi(floor(x^(1/j)) mod 2)
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
            // sum(2^w(d), d=[1,x]) mod 4 = sum(mu(d)TauSum(x/d^2), d=[1,floor(sqrt(x))]) mod 4
            var limit = IntegerMath.FloorSquareRoot(x);
            if (limit <= ulong.MaxValue)
                return SumTwoToTheOmega((UInt128)x, (ulong)limit);
            throw new NotImplementedException();
        }

        private int SumTwoToTheOmegaSimple(long x, int limit)
        {
            var mobius = new MobiusCollection(limit + 1, 2 * threads);
            var sum = 0;
            var nLast = (long)0;
            var tauLast = 0;
            for (var d = 1; d <= limit; d++)
            {
                var mu = mobius[d];
                if (mu != 0)
                {
                    var n = x / ((long)d * d);
                    var tau = n == nLast ? tauLast : TauSum((ulong)n);
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

        private uint blockSize = 1 << 24;
        private uint singleLimit1 = 10;
        private uint singleLimit2 = 100;

        private struct WorkItem
        {
            public ulong Min;
            public ulong Max;
        }

        private int SumTwoToTheOmega(UInt128 x, ulong limit)
        {
            var sum = 0;

            ulong d;
            for (d = 1; d < singleLimit1; d++)
            {
                var mu = mobiusSmall[d];
                if (mu != 0)
                {
                    var tau = TauSumParallel(x / (d * d));
                    if (mu == 1)
                        sum += tau;
                    else
                        sum += 4 - tau;
                }
            }

            var mobius = new MobiusRange((long)limit + 1, 0);
            var queue = new BlockingCollection<WorkItem>();
            var units = limit < (1 << 16) ? 1 : 100;
            var consumers = Math.Max(1, threads);
            var tasks = new Task[consumers];
            for (var consumer = 0; consumer < consumers; consumer++)
            {
                var thread = consumer;
                tasks[consumer] = Task.Factory.StartNew(() => ConsumeItems(thread, queue, mobius, x, ref sum));
            }
            for (d = singleLimit1; d < singleLimit2; d++)
            {
                if (mobiusSmall[d] != 0)
                    queue.Add(new WorkItem { Min = d, Max = d + 1 });
            }
            for (var unit = 0; unit < units; unit++)
            {
                var dmin = d;
                var dmax = unit == units - 1 ? limit + 1 : (ulong)Math.Exp((unit + 1) * Math.Log(limit + 1) / units);
                if (dmin >= dmax)
                    continue;
                if (dmax - dmin > blockSize)
                    break;
                queue.Add(new WorkItem { Min = dmin, Max = dmax });
                d = dmax;
            }
            while (d < limit + 1)
            {
                var dmin = d;
                var dmax = Math.Min(dmin + blockSize, limit + 1);
                queue.Add(new WorkItem { Min = dmin, Max = dmax });
                d = dmax;
            }
            queue.CompleteAdding();
            Task.WaitAll(tasks);
            return sum & 3;
        }

        private void ConsumeItems(int thread, BlockingCollection<WorkItem> queue, MobiusRange mobius, UInt128 x, ref int sum)
        {
            var values = new sbyte[blockSize];
            var item = default(WorkItem);
            while (queue.TryTake(out item, Timeout.Infinite))
            {
                if (item.Max == item.Min + 1 && item.Min < (ulong)mobiusSmall.Length)
                    values[0] = (sbyte)mobiusSmall[item.Min];
                else
                    mobius.GetValues((long)item.Min, (long)item.Max, values);
                Interlocked.Add(ref sum, SumTwoToTheOmega(values, x, item.Min, item.Max));
            }
        }

        private int SumTwoToTheOmega(sbyte[] mobius, UInt128 x, ulong dmin, ulong dmax)
        {
            if (x < ulong.MaxValue)
                return SumTwoToTheOmegaMedium(mobius, (ulong)x, (uint)dmin, (uint)dmax);
            return SumTwoToTheOmegaLarge(mobius, x, dmin, dmax);
        }

        private int SumTwoToTheOmegaMedium(sbyte[] mobius, ulong x, uint dmin, uint dmax)
        {
            var sum = 0;
            var last = (ulong)0;
            var current = x / ((ulong)dmax * dmax);
            var delta = dmax == 1 ? (long)0 : x / ((ulong)(dmax - 1) * (dmax - 1)) - current;
            var d = dmax - 1;
            var count = 0;
            while (d >= dmin)
            {
                var mu = mobius[d - dmin];
                if (mu != 0)
                {
                    var dSquared = (ulong)d * d;
                    var product = (current + delta) * dSquared;
                    if (product > x)
                    {
                        do
                        {
                            --delta;
                            product -= dSquared;
                        }
                        while (product > x);
                    }
                    else if (product + dSquared <= x)
                    {
                        ++delta;
                        product += dSquared;
                        if (product + dSquared <= x)
                            break;
                    }
                    current += delta;
                    Debug.Assert(x / dSquared == current);
                    if (current != last)
                    {
                        if ((count & 3) != 0)
                        {
                            var tau = TauSum(last);
                            if (count > 0)
                                sum += count * tau;
                            else
                                sum -= count * (4 - tau);
                        }
                        count = 0;
                        last = current;
                    }
                    count += mu;
                }
                --d;
            }
            while (d >= dmin)
            {
                var mu = mobius[d - dmin];
                if (mu != 0)
                {
                    current = x / ((ulong)d * d);
                    if (current != last)
                    {
                        var tau = TauSum(last);
                        if (count > 0)
                            sum += count * tau;
                        else
                            sum -= count * (4 - tau);
                        count = 0;
                        last = current;
                    }
                    count += mu;
                }
                --d;
            }
            {
                var tau = TauSum(last);
                if (count > 0)
                    sum += count * tau;
                else
                    sum -= count * (4 - tau);
            }
            return sum;
        }

        private int SumTwoToTheOmegaLarge(sbyte[] mobius, UInt128 x, ulong dmin, ulong dmax)
        {
            var sum = 0;
            var last = (UInt128)0;
            var current = x / ((UInt128)dmax * dmax);
            var delta = dmax == 1 ? (long)0 : x / ((UInt128)(dmax - 1) * (dmax - 1)) - current;
            var d = dmax - 1;
            var count = 0;
            while (d >= dmin)
            {
                var mu = mobius[d - dmin];
                if (mu != 0)
                {
                    var dSquared = (UInt128)d * d;
                    var product = (current + delta) * dSquared;
                    if (product > x)
                    {
                        do
                        {
                            --delta;
                            product -= dSquared;
                        }
                        while (product > x);
                    }
                    else if (product + dSquared <= x)
                    {
                        ++delta;
                        product += dSquared;
                        if (product + dSquared <= x)
                            break;
                    }
                    current += delta;
                    Debug.Assert(x / dSquared == current);
                    if (current != last)
                    {
                        if ((count & 3) != 0)
                        {
                            var tau = TauSum(last);
                            if (count > 0)
                                sum += count * tau;
                            else
                                sum -= count * (4 - tau);
                        }
                        count = 0;
                        last = current;
                    }
                    count += mu;
                }
                --d;
            }
            while (d >= dmin)
            {
                var mu = mobius[d - dmin];
                if (mu != 0)
                {
                    current = x / ((UInt128)d * d);
                    if (current != last)
                    {
                        var tau = TauSum(last);
                        if (count > 0)
                            sum += count * tau;
                        else
                            sum -= count * (4 - tau);
                        count = 0;
                        last = current;
                    }
                    count += mu;
                }
                --d;
            }
            {
                var tau = TauSum(last);
                if (count > 0)
                    sum += count * tau;
                else
                    sum -= count * (4 - tau);
            }
            return sum;
        }

        public int TauSumSimple(long y)
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

        public int TauSumParallel(UInt128 y)
        {
            if (y < (ulong)tauSumSmall.Length)
                return tauSumSmall[(int)y];
            var sqrt = (ulong)0;
            var sum = TauSumInnerParallel(y, out sqrt);
            sum = 2 * sum - (int)((sqrt * sqrt) & 3);
            return sum & 3;
        }

        private int TauSum(ulong y)
        {
            // sum(tau(d), d=[1,y]) = 2 sum(y/d, d=[1,floor(sqrt(y))]) - floor(sqrt(y))^2
            if (y < (ulong)tauSumSmall.Length)
                return tauSumSmall[y];
            var sqrt = (uint)0;
            var sum = TauSumInner(y, out sqrt);
            sum = 2 * sum - (int)((sqrt * sqrt) & 3);
            return sum & 3;
        }

        private int TauSum(UInt128 y)
        {
            // sum(tau(d), d=[1,y]) = 2 sum(y/d, d=[1,floor(sqrt(y))]) - floor(sqrt(y))^2
            if (y < (ulong)tauSumSmall.Length)
                return tauSumSmall[(int)y];
            var sqrt = (ulong)0;
            var sum = TauSumInner(y, out sqrt);
            sum = 2 * sum - (int)((sqrt * sqrt) & 3);
            return sum & 3;
        }

        public int TauSumInnerSimple(long y, out int sqrt)
        {
            // Computes sum(floor(y/d), d=[1,floor(sqrt(y))]) mod 2.
            if (y == 0)
            {
                sqrt = 0;
                return 0;
            }
            var sum = 0;
            var n = (long)1;
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

        public int TauSumInner(ulong y, out uint sqrt)
        {
            // Computes sum(floor(y/d), d=[1,floor(sqrt(y))]) mod 2.
            // To avoid division, we start at the
            // end and proceed backwards using multiplication
            // with estimates.  We keep track of the
            // difference between steps and let
            // it increase by at most one each iteration.
            // As soon as it starts changing too quickly
            // we resort to a different method where
            // the quantity floor(y/d) is odd iff y mod 2d >= d.
            if (y <= uint.MaxValue)
                return TauSumInnerSmall((uint)y, out sqrt);
            return TauSumInnerMedium(y, out sqrt);
        }

        public int TauSumInner(UInt128 y, out ulong sqrt)
        {
            if (y <= uint.MaxValue)
            {
                var isqrt = (uint)0;
                var result = TauSumInnerSmall((uint)y, out isqrt);
                sqrt = (ulong)isqrt;
                return result;
            }
            if (y <= ulong.MaxValue)
            {
                var isqrt = (uint)0;
                var result = TauSumInnerMedium((ulong)y, out isqrt);
                sqrt = (ulong)isqrt;
                return result;
            }
            return TauSumInnerLarge(y, out sqrt);
        }

        public int TauSumInnerParallel(UInt128 y, out ulong sqrt)
        {
#if false
            Console.WriteLine("TauSumInnerParallel: y = {0}", y);
            var timer = new Stopwatch();
            timer.Restart();
#endif
            var limit = (ulong)IntegerMath.FloorSquareRoot((BigInteger)y);
            var sum = 0;
            if (threads == 0)
                sum += TauSumInnerWorker(y, 1, limit + 1);
            else
            {
                var tasks = new Task[threads];
                var batchSize = (limit + (ulong)threads - 1) / (ulong)threads;
                for (var thread = (uint)0; thread < threads; thread++)
                {
                    var imin = 1 + thread * batchSize;
                    var imax = Math.Min(imin + batchSize, limit + 1);
#if false
                    Console.WriteLine("imin = {0}, imax = {1}", imin, imax);
#endif
                    tasks[thread] = Task.Factory.StartNew(() =>
                        Interlocked.Add(ref sum, TauSumInnerWorker(y, imin, imax)));
                }
                Task.WaitAll(tasks);
            }
#if false
            Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
            sqrt = limit;
            return sum & 1;
        }

        public int TauSumInnerSmall(uint y, out uint sqrt)
        {
            var limit = (uint)Math.Floor(Math.Sqrt(y));
            var sum = (uint)0;
            var current = limit - 1;
            var delta = (uint)1;
            var i = limit;
            while (i > 0)
            {
                var product = (current + delta) * i;
                if (product > y)
                    --delta;
                else if (product + i <= y)
                {
                    ++delta;
                    if (product + 2 * i <= y)
                        break;
                }
                current += delta;
                Debug.Assert(y / i == current);
                sum ^= current;
                --i;
            }
            while (i >= 1)
            {
                sum ^= (uint)(y / i);
                --i;
            }
            sqrt = limit;
            return (int)(sum & 1);
        }

        public int TauSumInnerMedium(ulong y, out uint sqrt)
        {
            sqrt = (uint)Math.Floor(Math.Sqrt(y));
            return TauSumInnerWorkerMedium(y, 1, sqrt + 1);
        }

        public int TauSumInnerLarge(UInt128 y, out ulong sqrt)
        {
            sqrt = (ulong)IntegerMath.FloorSquareRoot((BigInteger)y);
            return TauSumInnerWorkerLarge(y, 1, sqrt + 1);
        }

        private int TauSumInnerWorker(UInt128 y, ulong imin, ulong imax)
        {
#if true
            if (y < ulong.MaxValue)
                return TauSumInnerWorkerMedium((ulong)y, (uint)imin, (uint)imax);
#endif
            return TauSumInnerWorkerLarge(y, imin, imax);
        }

        private int TauSumInnerWorkerMedium(ulong y, uint imin, uint imax)
        {
            var sum = (uint)0;
            var current = y / imax;
            var delta = y / (imax - 1) - current;
            var i = imax - 1;
            while (i >= imin)
            {
                var product = (current + delta) * i;
                if (product > y)
                    --delta;
                else if (product + i <= y)
                {
                    ++delta;
                    product += i;
                    if (product + i <= y)
                        break;
                }
                current += delta;
                Debug.Assert(y / i == current);
                sum ^= (uint)current;
                --i;
            }
            while (i >= imin)
            {
                sum ^= (uint)(y / i);
                --i;
            }
            return (int)(sum & 1);
        }

        private int TauSumInnerWorkerLarge(UInt128 y, ulong imin, ulong imax)
        {
            var sum = (uint)0;
            for (var i = imin; i < imax; i++)
                sum ^= (uint)(y / i);
            return (int)(sum & 1);
        }
    }
}
