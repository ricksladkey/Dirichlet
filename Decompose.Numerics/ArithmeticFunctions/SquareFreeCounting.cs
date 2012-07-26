using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Decompose.Numerics
{
    public class SquareFreeCounting
    {
        private struct WorkItem
        {
            public long Min;
            public long Max;
        }

        private const long maximumBatchSize = (long)1 << 24;
        private const long tmax = (long)1 << 62;
        private const long tmin = -tmax;
        private const long C1 = 2;

        private int threads;
        private bool simple;
        private BigInteger n;
        private long imax;
        private long xmax;
        private BigInteger sum;
        private MobiusRange mobius;
        private MertensRange mertens;
        private long[] xi;
        private long[] mx;

        public SquareFreeCounting(int threads, bool simple)
        {
            this.threads = threads;
            this.simple = simple;
        }

        public BigInteger Evaluate(BigInteger n)
        {
            this.n = n;
            if (n == 1)
                return 1;
            sum = 0;
            imax = (long)IntegerMath.FloorRoot(n, 5) / C1;
            xmax = imax != 0 ? Xi(imax) : (long)IntegerMath.FloorPower(n, 1, 2);
            mobius = new MobiusRange(xmax + 1, 0);
            mertens = new MertensRange(mobius);
            xi = new long[imax + 1];
            mx = new long[imax + 1];

            // Initialize xi and mx.
            for (var i = 1; i <= imax; i++)
                xi[i] = Xi(i);
            for (var i = 1; i <= imax; i++)
                mx[i] = 1;

            if (threads <= 1)
            {
                var values = new sbyte[xmax + 1];
                var m = new long[xmax + 1];
                Evaluate(1, xmax, values, m);
            }
            else
                EvaluateParallel(1, xmax);
            EvaluateTail();
            return sum;
        }

        private void EvaluateTail()
        {
            // Finialize mx.
            ComputeMx();

            // Compute tail.
            var s = (BigInteger)0;
            for (var i = 1; i < imax; i++)
                s += mx[i];
            s -= (imax - 1) * mx[imax];

            AddToSum(s);
        }

        private long Xi(long i)
        {
            return (long)IntegerMath.FloorSquareRoot(n / i);
        }

        private void UpdateMx(long[] m, long x1, long x2)
        {
            // Add the contributions to each mx from all the small m values.
            for (var i = 1; i <= imax; i++)
            {
                var x = xi[i];
                var sqrt = IntegerMath.FloorSquareRoot(x);
                var jmin = Math.Max(2, FirstDivisorNotAbove(x, x2));
                var jmax = Math.Min(sqrt, LastDivisorNotBelow(x, x1));
                var kmin = Math.Max(1, x1);
                var kmax = Math.Min(x2, x / sqrt - 1);
                var s = (long)0;
                for (var j = jmin; j <= jmax; j++)
                    s += m[x / j - x1];
                var current = x / kmin;
                for (var k = kmin; k <= kmax; k++)
                {
                    var next = x / (k + 1);
                    s += (current - next) * m[k - x1];
                    current = next;
                }
                Interlocked.Add(ref mx[i], -s);
            }
        }

        private void ComputeMx()
        {
            // Add the remaining contributions to each mx from other mx values.
            for (var i = imax; i >= 1; i--)
            {
                var xi = Xi(i);
                var jmax = IntegerMath.FloorSquareRoot(xi);
                var jmin = Math.Max(2, FirstDivisorNotAbove(xi, xmax));
                var s = (long)0;
                for (var j = (long)2; j < jmin; j++)
                    s += mx[j * j * i];
                mx[i] -= s;
            }
        }

        private long FirstDivisorNotAbove(long xi, long x)
        {
            // Return the largest value of j such that xi / j <= x.
            if (x <= 1)
                return xi;
            var j = (xi + x - 1) / x;
            if (j > 1 && xi / (j - 1) <= x)
                --j;
            Debug.Assert(j == 1 || xi / (j - 1) > x);
            Debug.Assert(xi / j <= x);
            return j;
        }

        private long LastDivisorNotBelow(long xi, long x)
        {
            // Return the smallest value of j such that xi / j >= x.
            var j = xi / x;
            if (j > 1 && xi / (j + 1) >= x)
                ++j;
            Debug.Assert(j == 0 || xi / j >= x);
            Debug.Assert(xi / (j + 1) < x);
            return j;
        }

        private void Evaluate(long x1, long x2, sbyte[] values, long[] m)
        {
            mobius.GetValues(x1, x2 + 1, values);
            var x = x2;
            if (!simple)
                x = S1(x1, x, values);
            x = S3(x1, x, values);

            var s = mertens.Evaluate(x1 - 1);
            var kmax = x2 - x1;
            for (var k = 0; k <= kmax; k++)
            {
                s += values[k];
                m[k] = s;
            }
            UpdateMx(m, x1, x2);
        }

        private void EvaluateParallel(long x1, long x2)
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
            ProduceItems(queue, x1, x2);

            // Wait for completion.
            queue.CompleteAdding();
            Task.WaitAll(tasks);
        }

        private void ProduceItems(BlockingCollection<WorkItem> queue, long imin, long imax)
        {
#if false
            var split = (long)IntegerMath.FloorPower(n, 4, 15);
            BatchRange(queue, imin, split, maximumBatchSize / 10);
            BatchRange(queue, split + 1, imax, maximumBatchSize);
#else
            BatchRange(queue, imin, imax, maximumBatchSize);
#endif
        }

        private void BatchRange(BlockingCollection<WorkItem> queue, long imin, long imax, long batchSizeLimit)
        {
            var batchSize = Math.Min(batchSizeLimit, (imax - imin + 1 + threads - 1) / threads);
            for (var i = imin; i <= imax; i += batchSize)
                queue.Add(new WorkItem { Min = i, Max = Math.Min(i + batchSize - 1, imax) });
        }

        private void ConsumeItems(int thread, BlockingCollection<WorkItem> queue)
        {
            var values = new sbyte[maximumBatchSize];
            var m = new long[maximumBatchSize];
            var item = default(WorkItem);
            try
            {
                while (queue.TryTake(out item, Timeout.Infinite))
                    Evaluate(item.Min, item.Max, values, m);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private long S1(long x1, long x2, sbyte[] values)
        {
            var s = (UInt128)n;
            var t = (long)0;
            var x = x2;
            var beta = (long)(n / (x + 1));
            var eps = (long)(n % (x + 1));
            var delta = (long)(n / x - beta);
            var gamma = (long)beta - x * delta;
            var alpha = beta / (x + 1);
            var alphax = (alpha + 1) * (x + 1);
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
                beta += delta;
                alphax -= alpha + 1;
                if (alphax <= beta)
                {
                    ++alpha;
                    alphax += x;
                    if (alphax <= beta)
                    {
                        ++alpha;
                        alphax += x;
                        if (alphax <= beta)
                            break;
                    }
                }

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 1));
                Debug.Assert(gamma == beta - (BigInteger)(x - 1) * delta);
                Debug.Assert(alpha == n / ((BigInteger)x * x));

                var mu = values[x - x1];
                if (mu == -1)
                    t -= alpha;
                else if (mu == 1)
                    t += alpha;
                if (t > tmax)
                {
                    s += (ulong)t;
                    t = 0;
                }
                else if (t < tmin)
                {
                    s -= (ulong)-t;
                    t = 0;
                }
                --x;
            }
            if (t > 0)
                s += (ulong)t;
            else if (t < 0)
                s -= (ulong)-t;
            AddToSum(s - n);
            return x;
        }

        private long S3(long x1, long x2, sbyte[] values)
        {
            var nRep = (UInt128)n;
            var s = nRep;
            var xx = (ulong)x1 * (ulong)x1;
            var dx = 2 * (ulong)x1 + 1;
            for (var x = x1; x <= x2; x++)
            {
                var mu = values[x - x1];
                if (mu == 1)
                    s += nRep / xx;
                else if (mu == -1)
                    s -= nRep / xx;
                xx += dx;
                dx += 2;
            }
            AddToSum(s - n);
            return x1 - 1;
        }

        private void AddToSum(BigInteger s)
        {
            if (!s.IsZero)
            {
                lock (this)
                    sum += s;
            }
        }

        private static BigInteger[] data10 = new BigInteger[]
        {
            BigInteger.Parse("1"),
            BigInteger.Parse("7"),
            BigInteger.Parse("61"),
            BigInteger.Parse("608"),
            BigInteger.Parse("6083"),
            BigInteger.Parse("60794"),
            BigInteger.Parse("607926"),
            BigInteger.Parse("6079291"),
            BigInteger.Parse("60792694"),
            BigInteger.Parse("607927124"),
            BigInteger.Parse("6079270942"),
            BigInteger.Parse("60792710280"),
            BigInteger.Parse("607927102274"),
            BigInteger.Parse("6079271018294"),
            BigInteger.Parse("60792710185947"),
            BigInteger.Parse("607927101854103"),
            BigInteger.Parse("6079271018540405"),
            BigInteger.Parse("60792710185403794"),
            BigInteger.Parse("607927101854022750"),
            BigInteger.Parse("6079271018540280875"),
            BigInteger.Parse("60792710185402613302"),
            BigInteger.Parse("607927101854026645617"),
            BigInteger.Parse("6079271018540266153468"),
            BigInteger.Parse("60792710185402662868753"),
            BigInteger.Parse("607927101854026628773299"),
            BigInteger.Parse("6079271018540266286424910"),
            BigInteger.Parse("60792710185402662866945299"),
            BigInteger.Parse("607927101854026628664226541"),
            BigInteger.Parse("6079271018540266286631251028"),
            BigInteger.Parse("60792710185402662866327383816"),
            BigInteger.Parse("607927101854026628663278087296"),
            BigInteger.Parse("6079271018540266286632795633943"),
            BigInteger.Parse("60792710185402662866327694188957"),
            BigInteger.Parse("607927101854026628663276901540346"),
            BigInteger.Parse("6079271018540266286632767883637220"),
            BigInteger.Parse("60792710185402662866327677953999263"),
            BigInteger.Parse("607927101854026628663276779463775476"),
        };

        public static BigInteger PowerOfTen(int n)
        {
            return data10[n];
        }
    }
}
