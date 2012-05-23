using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class DivisionFreeDivisorSummatoryFunction
    {
        private struct WorkItem
        {
            public long Min;
            public long Max;
        }

        private const ulong smax = (ulong)1 << 62;
        private const long maximumBatchSize = (long)1 << 28;

        private int threads;
        private BigInteger n;
        private long root6;
        private UInt128 sum;

        public DivisionFreeDivisorSummatoryFunction(int threads)
        {
            this.threads = threads;
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
            return sum;
        }

        private void Evaluate(long x1, long x2)
        {
            var x = x2;
            x = S1(x1, x);
            //x = S2(x1, x);
            x = S3(x1, x);
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
                tasks[consumer] = Task.Factory.StartNew(() => ConsumeItems(thread, queue, n));
            }

            // Produce work items.
            ProduceItems(queue, 1, xmax);

            // Wait for completion.
            queue.CompleteAdding();
            Task.WaitAll(tasks);
        }

        private void ProduceItems(BlockingCollection<WorkItem> queue, long imin, long imax)
        {
            var batchSize = Math.Min(maximumBatchSize, (imax - imin + threads - 1) / threads);
            for (var i = imin; i < imax; i += batchSize)
                queue.Add(new WorkItem { Min = i, Max = Math.Min(i + batchSize - 1, imax) });
        }

        private void ConsumeItems(int thread, BlockingCollection<WorkItem> queue, BigInteger n)
        {
            var item = default(WorkItem);
            while (queue.TryTake(out item, Timeout.Infinite))
                Evaluate(item.Min, item.Max);
        }

        private long S1(long x1, long x2)
        {
            var s = (UInt128)0;
            var s2 = (ulong)0;
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
                gamma += 2 * delta;
                beta += (ulong)delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 1));
                Debug.Assert(gamma == beta - (BigInteger)(x - 1) * delta);

                s2 += beta;
                if (s2 > smax)
                {
                    s += s2;
                    s2 = 0;
                }
                --x;
            }
            s += s2;
            AddToSum(ref s);
            return x;
        }

        private long S2(long x1, long x2)
        {
            var s = (UInt128)0;
            var s2 = (ulong)0;
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

                s2 += beta;
                if (s2 > smax)
                {
                    s += s2;
                    s2 = 0;
                }

                if (beta > smax)
                {
                    s += count * alpha;
                    alpha += beta;
                    beta = 0;
                    count = 0;
                }

                --x;
            }
            s += s2 + count * alpha;
            AddToSum(ref s);
            return x;
        }

        private long S3(long x1, long x2)
        {
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
