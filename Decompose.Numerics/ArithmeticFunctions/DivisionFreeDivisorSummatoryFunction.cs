using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class DivisionFreeDivisorSummatoryFunction : IDivisorSummatoryFunction<BigInteger>
    {
        private struct WorkItem
        {
            public long Min;
            public long Max;
        }

        private const ulong tmax = (ulong)1 << 62;
        private const long maximumBatchSize = (long)1 << 28;
        private const long nmax = (long)1 << 58;
        private const long nmaxOdd = (long)1 << 60;

        private int threads;
        private bool simple;
        private bool odd;
        private BigInteger n;
        private long root6;
        private UInt128 sum;

        public DivisionFreeDivisorSummatoryFunction(int threads, bool simple, bool odd)
        {
            this.threads = threads;
            this.simple = simple;
            this.odd = odd;
        }

        public BigInteger Evaluate(BigInteger n)
        {
            this.n = n;
            sum = 0;
            var xmax = (long)IntegerMath.FloorSquareRoot(n);
            root6 = (long)IntegerMath.CeilingRoot(n, 6);
            if (threads <= 1)
                Evaluate(1, xmax);
            else
                EvaluateParallel(1, xmax);
            if (odd)
            {
                var xmax2 = (xmax + 1) / 2;
                return 2 * (BigInteger)sum - (BigInteger)xmax2 * xmax2;
            }
            return 2 * (BigInteger)sum - (BigInteger)xmax * xmax;
        }

        public BigInteger Evaluate(BigInteger n, BigInteger x1, BigInteger x2)
        {
            this.n = n;
            sum = 0;
            if (threads <= 1 || x2 - x1 < ((long)1 << 10))
                Evaluate((long)x1, (long)x2);
            else
                EvaluateParallel((long)x1, (long)x2);
            return odd ? sum / 2 : sum;
        }

        private void Evaluate(long x1, long x2)
        {
            var x = x2;
            if (!simple)
            {
                x = odd ? S1Odd(x1, x) : S1(x1, x);
                //x = S2(x1, x);
            }
            x = odd ? S3Odd(x1, x) : S3(x1, x);
        }

        private void EvaluateParallel(long xmin, long xmax)
        {
            // Create consumers.
            var queue = new BlockingCollection<WorkItem>();
            var consumers = threads;
            var tasks = new Task[consumers];
            for (var consumer = 0; consumer < consumers; consumer++)
            {
                var thread = consumer;
                tasks[consumer] = Task.Factory.StartNew(() => ConsumeItems(thread, queue));
            }

            // Produce work items.
            ProduceItems(queue, xmin, xmax);

            // Wait for completion.
            queue.CompleteAdding();
            Task.WaitAll(tasks);
        }

        private void ProduceItems(BlockingCollection<WorkItem> queue, long imin, long imax)
        {
            var batchSize = Math.Min(maximumBatchSize, (imax - imin + 1 + threads - 1) / threads);
            for (var i = imin; i <= imax; i += batchSize)
                queue.Add(new WorkItem { Min = i, Max = Math.Min(i + batchSize - 1, imax) });
        }

        private void ConsumeItems(int thread, BlockingCollection<WorkItem> queue)
        {
            var item = default(WorkItem);
            while (queue.TryTake(out item, Timeout.Infinite))
                Evaluate(item.Min, item.Max);
        }

        private long S1(long x1, long x2)
        {
            if (n < nmax)
                return S1Small(x1, x2);
            var s = (UInt128)0;
            var t = (ulong)0;
            var x = x2;
            var beta = (ulong)(n / (x + 1));
            var eps = (long)(n % (x + 1));
            var delta = (long)(n / x - beta);
            var gamma = (long)beta - x * delta;
            while (x >= x1)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += delta + delta;
                beta += (ulong)delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 1));
                Debug.Assert(gamma == beta - (BigInteger)(x - 1) * delta);

                t += beta;
                if (t > tmax)
                {
                    s += t;
                    t = 0;
                }
                --x;
            }
            s += t;
            AddToSum(ref s);
            return x;
        }

        private long S1Small(long x1, long x2)
        {
            var t = (ulong)0;
            var x = x2;
            var beta = (ulong)(n / (x + 1));
            var eps = (long)(n % (x + 1));
            var delta = (long)(n / x - beta);
            var gamma = (long)beta - x * delta;
            while (x >= x1)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += delta + delta;
                beta += (ulong)delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 1));
                Debug.Assert(gamma == beta - (BigInteger)(x - 1) * delta);

                t += beta;
                --x;
            }
            var s = (UInt128)t;
            AddToSum(ref s);
            return x;
        }

        private long S2(long x1, long x2)
        {
            var s = (UInt128)0;
            var t = (ulong)0;
            var x = x2;
            var beta = (ulong)(n / (x + 1));
            var eps = (long)(n % (x + 1));
            var delta = (long)(n / x - beta);
            var gamma = (long)beta - x * delta;
            var alpha = (UInt128)beta;
            beta = 0;
            var xmin = IntegerMath.Max(x1, root6);
            var count = (ulong)0;
            while (x >= xmin)
            {
                eps += gamma;
                var delta2 = eps >= 0 ? eps / x : (eps - x + 1) / x;
                delta += delta2;
                var a = x * delta2;
                eps -= a;
                gamma += 2 * delta - a;
                beta += (ulong)delta;
                ++count;

                Debug.Assert(eps == n % x);
                Debug.Assert((alpha + beta) == n / x);
                Debug.Assert(delta == (alpha + beta) - n / (x + 1));
                Debug.Assert(gamma == (BigInteger)(alpha + beta) - ((x - 1) * delta));

                t += beta;
                if (t > tmax)
                {
                    s += t;
                    t = 0;
                }

                if (beta > tmax)
                {
                    s += count * alpha;
                    alpha += beta;
                    beta = 0;
                    count = 0;
                }

                --x;
            }
            s += t + count * alpha;
            AddToSum(ref s);
            return x;
        }

        private long S3(long x1, long x2)
        {
            if (n < ulong.MaxValue)
                return S3UInt64(x1, x2);

            var s = (UInt128)0;
            var nRep = (UInt128)n;
            var x = x2;
            while (x >= x1)
            {
                s += nRep / (ulong)x;
                --x;
            }
            AddToSum(ref s);
            return x;
        }

        private long S3UInt64(long x1, long x2)
        {
            var s = (UInt128)0;
            var t = (ulong)0;
            var nRep = (ulong)n;
            var x = x2;
            while (x >= x1)
            {
                t += nRep / (ulong)x;
                if (t > tmax)
                {
                    s += t;
                    t = 0;
                }
                --x;
            }
            s += t;
            AddToSum(ref s);
            return x;
        }

        private long S1Odd(long x1, long x2)
        {
            if (n < nmaxOdd)
                return S1OddSmall(x1, x2);
            var s = (UInt128)0;
            var t = (ulong)0;
            var x = (x2 & 1) == 0 ? x2 - 1 : x2;
            var beta = (ulong)(n / (x + 2));
            var eps = (long)(n % (x + 2));
            var delta = (long)(n / x - beta);
            var gamma = 2 * (long)beta - x * delta;
            while (x >= x1)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += 4 * delta;
                beta += (ulong)delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 2));
                Debug.Assert(gamma == 2 * beta - (BigInteger)(x - 2) * delta);

                t += beta + (beta & 1);
                if (t > tmax)
                {
                    s += t;
                    t = 0;
                }
                x -= 2;
            }
            s += t;
            AddToSum(ref s);
            return x;
        }

        private long S1OddSmall(long x1, long x2)
        {
            var t = (ulong)0;
            var x = (x2 & 1) == 0 ? x2 - 1 : x2;
            var beta = (ulong)(n / (x + 2));
            var eps = (long)(n % (x + 2));
            var delta = (long)(n / x - beta);
            var gamma = 2 * (long)beta - x * delta;
            while (x >= x1)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += 4 * delta;
                beta += (ulong)delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 2));
                Debug.Assert(gamma == 2 * beta - (BigInteger)(x - 2) * delta);

                t += beta + (beta & 1);
                x -= 2;
            }
            var s = (UInt128)t;
            AddToSum(ref s);
            return x;
        }

        private long S3Odd(long x1, long x2)
        {
            if (n < ulong.MaxValue)
                return S3OddUInt64(x1, x2);

            var s = (UInt128)0;
            var tOdd = (ulong)0;
            var nRep = (UInt128)n;
            var x = (x2 & 1) == 0 ? x2 - 1 : x2;
            while (x >= x1)
            {
                var beta = nRep / (ulong)x;
                s += beta;
                if (!beta.IsEven)
                    ++tOdd;
                x -= 2;
            }
            s += tOdd;
            AddToSum(ref s);
            return x;
        }

        private long S3OddUInt64(long x1, long x2)
        {
            var s = (UInt128)0;
            var t = (ulong)0;
            var nRep = (ulong)n;
            var x = (x2 & 1) == 0 ? x2 - 1 : x2;
            while (x >= x1)
            {
                var beta = nRep / (ulong)x;
                t += beta + (beta & 1);
                if (t > tmax)
                {
                    s += t;
                    t = 0;
                }
                x -= 2;
            }
            s += t;
            AddToSum(ref s);
            return x;
        }

        private void AddToSum(ref UInt128 s)
        {
            if (!s.IsZero)
            {
                lock (this)
                    sum += s;
            }
        }
    }
}
