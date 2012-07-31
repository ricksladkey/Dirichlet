using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Decompose.Numerics
{
    public class DivisorRange
    {
        private class Data
        {
            public long[] Products;
            public int[] Divisors;
            public int[] Offsets;
            public long[] OffsetsSquared;
            public Queue<int>[] Queues;

            public Data(int length)
            {
                Products = new long[blockSize];
                Divisors = new int[blockSize];
                Offsets = new int[length];
                OffsetsSquared = new long[length];
                for (var i = 0; i < maxQueues; i++)
                    Queues[i] = new Queue<int>();
            }
        }

        private const int blockSize = 1 << 16;
        private const int maxQueues = 32;

        private long size;
        private int threads;
        private int[] primes;
        private int cycleLimit;
        private int cycleSize;
        private long[] cycleProducts;
        private int[] cycleDivisors;
        private ConcurrentQueue<Data> queue;

        public long Size { get { return size; } }
        public int Threads { get { return threads; } }

        public DivisorRange(long size, int threads)
        {
            this.size = size;
            this.threads = threads;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            primes = new PrimeCollection(limit, 0).Select(p => (int)p).ToArray();
            CreateCycle();
            var arrayLength = Math.Max(1, threads);
            queue = new ConcurrentQueue<Data>();
        }

        public void GetValues(long kmin, long kmax, int[] values)
        {
            GetValues(kmin, kmax, values, kmin, null, 0);
        }

        public void GetValues(long kmin, long kmax, int[] values, long offset)
        {
            GetValues(kmin, kmax, values, offset, null, 0);
        }

        public void GetValues(long kmin, long kmax, int[] values, long offset, long[] sums, long m0)
        {
            // Determine the number of primes appropriate for values up to kmax.
            var plimit = (int)Math.Ceiling(Math.Sqrt(kmax));
            var pmax = primes.Length;
            while (pmax > 0 && primes[pmax - 1] > plimit)
                --pmax;

            if (threads == 0)
            {
                ProcessRange(pmax, kmin, kmax, values, offset, sums, m0);
                if (values != null && kmin <= 1)
                    values[1 - kmin] = 1;
                return;
            }

            // Choose batch size such that: batchSize*threads >= length and batchSize is even.
            // Note that Mertens base value for a batch is unknown because of parallelism.
            var tasks = new Task[threads];
            var length = kmax - kmin;
            var batchSize = ((length + threads - 1) / threads + 1) & ~1;
            for (var thread = 0; thread < threads; thread++)
            {
                var kstart = (long)thread * batchSize + kmin;
                var kend = Math.Min(kstart + batchSize, kmax);
                tasks[thread] = Task.Factory.StartNew(() => ProcessRange(pmax, kstart, kend, values, offset, sums, 0));
            }
            Task.WaitAll(tasks);
            if (values != null && kmin <= 1)
                values[1 - kmin] = 1;

            if (sums == null)
                return;

            // Collect and sum Mertens function totals for each batch.
            var mabs = new long[threads];
            mabs[0] = m0;
            for (var thread = 1; thread < threads; thread++)
            {
                var last = (long)thread * batchSize - 1;
                if (last < sums.Length)
                    mabs[thread] = mabs[thread - 1] + sums[last];
            }

            // Convert relative Mertens function values into absolute Mertens values.
            for (var thread = 0; thread < threads; thread++)
            {
                var index = thread;
                var kstart = (long)thread * batchSize + kmin;
                var kend = Math.Min(kstart + batchSize, kmax);
                tasks[thread] = Task.Factory.StartNew(() =>
                {
                    var abs = mabs[index];
                    var klo = kstart - offset;
                    var khi = kend - offset;
                    for (var k = klo; k < khi; k++)
                        sums[k] += abs;
                });
            }
            Task.WaitAll(tasks);
        }


        private void CreateCycle()
        {
            // Create pre-sieved cycle of the squares of small primes.
            var dmax = 3;
            cycleLimit = Math.Min(primes.Length, dmax);
            cycleSize = 1;
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = (int)primes[i];
                cycleSize *= p;
            }
            cycleProducts = new long[cycleSize];
            for (var i = 0; i < cycleSize; i++)
                cycleProducts[i] = 1;
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = primes[i];
                var pMinus = -p;
                for (var k = 0; k < cycleSize; k += p)
                    cycleProducts[k] *= pMinus;
                var pSquared = (long)p * p;
            }
        }

        private void ProcessRange(int pmax, long kstart, long kend, int[] values, long kmin, long[] sums, long m0)
        {
            // Acquire resources.
            Data data;
            if (!queue.TryDequeue(out data))
                data = new Data(Math.Max(1, primes.Length));
            var products = data.Products;
            var divisors = data.Divisors;
            var offsets = data.Offsets;
            var offsetsSquared = data.OffsetsSquared;

            // Determine the initial cycle offset.
            var cycleOffset = cycleSize - (int)(kstart % cycleSize);
            if (cycleOffset == cycleSize)
                cycleOffset = 0;
            offsets[0] = cycleOffset;

            // Determine the initial offset and offset squared of each prime divisor.
            for (var i = 1; i < pmax; i++)
            {
                var p = primes[i];
                var offset = p - (int)(kstart % p);
                if (offset == p)
                    offset = 0;
                offsets[i] = offset;
                var pSquared = (long)p * p;
                var offsetSquared = pSquared - kstart % pSquared;
                if (offsetSquared == pSquared)
                    offsetSquared = 0;
                offsetsSquared[i] = offsetSquared;
            }

            // Process the whole range in block-sized batches.
            for (var k = kstart; k < kend; k += blockSize)
            {
                var length = (int)Math.Min(blockSize, kend - k);
                SieveBlock(pmax, k, length, products, divisors, offsets, offsetsSquared);
                m0 = AddValues(k, length, products, divisors, values, kmin, sums, m0);
            }

            // Release resources.
            queue.Enqueue(data);
        }

        private void SieveBlock(int pmax, long k0, int length, long[] products, int[] divisors, int[] offsets, long[] offsetsSquared)
        {
            var cycleOffset = offsets[0];
            Array.Copy(cycleProducts, cycleSize - cycleOffset, products, 0, Math.Min(length, cycleOffset));
            while (cycleOffset < length)
            {
                Array.Copy(cycleProducts, 0, products, cycleOffset, Math.Min(cycleSize, length - cycleOffset));
                cycleOffset += cycleSize;
            }
            offsets[0] = cycleOffset - length;

            for (var i = cycleLimit; i < pmax; i++)
            {
                var p = primes[i];
                int k;
                for (k = offsets[i]; k < length; k += p)
                {
                    products[k] *= p;
                    divisors[k] *= 2;
                }
                offsets[i] = k - length;
                var kk = offsetsSquared[i];
                if (kk < length)
                {
                    var power = (long)p * p;
                    do
                    {
                        products[kk] *= p;
                        divisors[kk] = divisors[kk] / 2 * 3;
                        kk += power;
                    }
                    while (kk < length);
                    var a = 2;
                    while (true)
                    {
                        for (var kkk = kk; kkk < length; kkk += power)
                        {
                            products[kkk] *= p;
                            divisors[kkk] = divisors[kkk] / a * (a + 1);
                        }
                        kk *= p;
                        if (kk >= length)
                            break;
                        a++;
                        power *= p;
                    }
                }
                offsetsSquared[i] = kk - length;
            }
        }

        private long AddValues(long k0, int length, long[] products, int[] divisors, int[] values, long kmin, long[] sums, long m0)
        {
            // Each product that is square-free can have at most one more
            // prime factor.  It has that factor if the absolute value of
            // the product is less than the full value.
            var k = k0;
            if (sums == null)
            {
                for (var i = 0; i < length; i++, k++)
                {
                    var value = divisors[i];
                    if (products[i] < k)
                        value *= 2;
                    values[k - kmin] = value;
                }
            }
            else if (values == null)
            {
                for (var i = 0; i < length; i++, k++)
                {
                    var value = divisors[i];
                    if (products[i] < k)
                        value *= 2;
                    m0 += value;
                    sums[k - kmin] = m0;
                }
            }
            else
            {
                for (var i = 0; i < length; i++, k++)
                {
                    var value = divisors[i];
                    if (products[i] < k)
                        value *= 2;
                    values[k - kmin] = (int)value;
                    m0 += value;
                    sums[k - kmin] = m0;
                }
            }
            return m0;
        }
    }
}
