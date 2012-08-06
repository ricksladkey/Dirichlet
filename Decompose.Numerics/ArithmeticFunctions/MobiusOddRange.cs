using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class MobiusOddRange
    {
        private class Data
        {
            public long[] Products;
            public int[] Offsets;
            public long[] OffsetsSquared;

            public Data(int length)
            {
                Products = new long[blockSize];
                Offsets = new int[length];
                OffsetsSquared = new long[length];
            }
        }

        private const int blockSize = 1 << 17;

        private long size;
        private int threads;
        private int[] primes;
        private int cycleLimit;
        private int cycleSize;
        private long[] cycle;
        private ConcurrentQueue<Data> queue;

        public long Size { get { return size; } }
        public int Threads { get { return threads; } }

        public MobiusOddRange(long size, int threads)
        {
            this.size = size;
            this.threads = threads;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            primes = new PrimeCollection(limit, 0).Select(p => (int)p).ToArray();
            CreateCycle();
            var arrayLength = Math.Max(1, threads);
            queue = new ConcurrentQueue<Data>();
        }

        public void GetValues(long kmin, long kmax, sbyte[] values)
        {
            GetValues(kmin, kmax, values, kmin, null, 0);
        }

        public void GetValues(long kmin, long kmax, sbyte[] values, long offset)
        {
            GetValues(kmin, kmax, values, offset, null, 0);
        }

        public void GetValues(long kmin, long kmax, sbyte[] values, long offset, long[] sums, long m0)
        {
            // validate range.
            Debug.Assert(kmin % 2 == 1 && kmax % 2 == 1);

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
            var tasks = new Task[threads];
            var length = kmax - kmin;
            var batchSize = ((length + threads - 1) / threads + 1) & ~1;
            for (var thread = 0; thread < threads; thread++)
            {
                var kstart = (long)thread * batchSize + kmin;
                var kend = Math.Min(kstart + batchSize, kmax);
                tasks[thread] = Task.Factory.StartNew(() => ProcessRange(pmax, kstart, kend, values, offset, sums, m0));
            }
            Task.WaitAll(tasks);
            if (values != null && kmin <= 1)
                values[1 - kmin] = 1;

            if (sums == null)
                return;

            // Collect and sum Mertens function totals for each batch.
            var mabs = new long[threads];
            mabs[0] = 0;
            for (var thread = 1; thread < threads; thread++)
            {
                var kstart = (long)thread * batchSize + kmin;
                if (kstart < kmax)
                    mabs[thread] = mabs[thread - 1] + sums[((kstart - kmin) >> 1) - 1] - m0;
            }

            // Convert relative Mertens function values into absolute Mertens values.
            for (var thread = 1; thread < threads; thread++)
            {
                var index = thread;
                var kstart = (long)thread * batchSize + kmin;
                var kend = Math.Min(kstart + batchSize, kmax);
                tasks[thread] = Task.Factory.StartNew(() => BumpRange(mabs[index], kstart, kend, offset, sums));
            }
            Task.WaitAll(tasks);
        }

        private void BumpRange(long abs, long kstart, long kend, long offset, long[] sums)
        {
            var klo = (int)(kstart - offset) >> 1;
            var khi = (int)(kend - offset) >> 1;
            for (var k = klo; k < khi; k++)
                sums[k] += abs;
        }

        private void CreateCycle()
        {
            // Create pre-sieved cycle of the squares of small primes.
            var dmax = 3;
            cycleLimit = Math.Min(primes.Length, dmax);
            cycleSize = 1;
            for (var i = 1; i < cycleLimit; i++)
            {
                var p = (int)primes[i];
                cycleSize *= p * p;
            }
            cycle = new long[cycleSize];
            for (var i = 0; i < cycleSize; i++)
                cycle[i] = 1;
            for (var i = 1; i < cycleLimit; i++)
            {
                var p = primes[i];
                var pMinus = -p;
                for (var k = p / 2; k < cycleSize; k += p)
                    cycle[k] *= pMinus;
                var pSquared = (long)p * p;
                for (var k = pSquared / 2; k < cycleSize; k += pSquared)
                    cycle[k] = 0;
            }
        }

        private void ProcessRange(int pmax, long kstart, long kend, sbyte[] values, long kmin, long[] sums, long m0)
        {
            // Acquire resources.
            Data data;
            if (!queue.TryDequeue(out data))
                data = new Data(Math.Max(1, primes.Length));
            var products = data.Products;
            var offsets = data.Offsets;
            var offsetsSquared = data.OffsetsSquared;

            // Determine the initial cycle offset.
            var cycleOffset = cycleSize - (int)((kstart >> 1) % cycleSize);
            if (cycleOffset == cycleSize)
                cycleOffset = 0;
            offsets[0] = cycleOffset;

            // Determine the initial offset and offset squared of each prime divisor.
            for (var i = cycleLimit; i < pmax; i++)
            {
                var p = primes[i];
                var offset = p - (int)(((kstart + p) >> 1) % p);
                if (offset == p)
                    offset = 0;
                Debug.Assert((kstart + 2 * offset) % p == 0);
                offsets[i] = offset;
                var pSquared = (long)p * p;
                var offsetSquared = pSquared - ((kstart + pSquared) >> 1) % pSquared;
                if (offsetSquared == pSquared)
                    offsetSquared = 0;
                offsetsSquared[i] = offsetSquared;
            }

            // Process the whole range in block-sized batches.
            for (var k = kstart; k < kend; k += blockSize)
            {
                var length = (int)Math.Min(blockSize, kend - k) >> 1;
                SieveBlock(pmax, k, length, products, offsets, offsetsSquared);
                m0 = AddValues(k, length, products, values, kmin, sums, m0);
            }

            // Release resources.
            queue.Enqueue(data);
        }

        private void SieveBlock(int pmax, long k0, int length, long[] products, int[] offsets, long[] offsetsSquared)
        {
            var cycleOffset = offsets[0];
            Debug.Assert(primes.Where((i, p) => i > 0 && i < cycleLimit).All(p => (k0 + 2 * cycleOffset) % p == 1));
            Array.Copy(cycle, cycleSize - cycleOffset, products, 0, Math.Min(length, cycleOffset));
            while (cycleOffset < length)
            {
                Array.Copy(cycle, 0, products, cycleOffset, Math.Min(cycleSize, length - cycleOffset));
                cycleOffset += cycleSize;
            }
            offsets[0] = cycleOffset - length;

            for (var i = cycleLimit; i < pmax; i++)
            {
                var p = primes[i];
                var pMinus = -p;
                int k;
                for (k = offsets[i]; k < length; k += p)
                {
                    Debug.Assert((k0 + 2 * k) % p == 0);
                    products[k] *= pMinus;
                }
                offsets[i] = k - length;
                long kk = offsetsSquared[i];
                if (kk < length)
                {
                    var pSquared = (long)p * p;
                    do
                    {
                        Debug.Assert((k0 + 2 * kk) % pSquared == 0);
                        products[kk] = 0;
                        kk += pSquared;
                    }
                    while (kk < length);
                }
                offsetsSquared[i] = kk - length;
            }
        }

        private long AddValues(long k0, int length, long[] products, sbyte[] values, long kmin, long[] sums, long m0)
        {
            // Each product that is square-free can have at most one more
            // prime factor.  It has that factor if the absolute value of
            // the product is less than the full value.
            var k = k0;
            if (sums == null)
            {
                for (var i = 0; i < length; i++, k += 2)
                {
                    // Look ma, no branching.
                    var p = products[i];
                    var pos = -p >> 63; // pos = -1 if p > 0, zero otherwise
                    var neg = p >> 63; // neg = -1 if p is < 0, zero otherwise
                    var abs = (p + neg) ^ neg; // abs = |p|
                    var flip = (abs - k) >> 63; // flip = -1 if abs < k, zero otherwise
                    values[(k - kmin) >> 1] = (sbyte)(((neg - pos) ^ flip) - flip); // value = pos - neg if flip = -1, neg - pos otherwise
                    Debug.Assert(k == 0 || values[(k - kmin) >> 1] == Math.Sign(p) * (Math.Abs(p) != k ? -1 : 1));
                }
            }
            else if (values == null)
            {
                for (var i = 0; i < length; i++, k += 2)
                {
                    // Look ma, no branching.
                    var p = products[i];
                    var pos = -p >> 63; // pos = -1 if p > 0, zero otherwise
                    var neg = p >> 63; // neg = -1 if p is < 0, zero otherwise
                    var abs = (p + neg) ^ neg; // abs = |p|
                    var flip = (abs - k) >> 63; // flip = -1 if abs < k, zero otherwise
                    var value = ((neg - pos) ^ flip) - flip; // value = pos - neg if flip = -1, neg - pos otherwise
                    m0 += value;
                    sums[(k - kmin) >> 1] = m0;
                    Debug.Assert(k == 0 || value == Math.Sign(p) * (Math.Abs(p) != k ? -1 : 1));
                }
            }
            else
            {
                for (var i = 0; i < length; i++, k += 2)
                {
                    // Look ma, no branching.
                    var p = products[i];
                    var pos = -p >> 63; // pos = -1 if p > 0, zero otherwise
                    var neg = p >> 63; // neg = -1 if p is < 0, zero otherwise
                    var abs = (p + neg) ^ neg; // abs = |p|
                    var flip = (abs - k) >> 63; // flip = -1 if abs < k, zero otherwise
                    var value = ((neg - pos) ^ flip) - flip; // value = pos - neg if flip = -1, neg - pos otherwise
                    values[(k - kmin) >> 1] = (sbyte)value;
                    m0 += value;
                    sums[(k - kmin) >> 1] = m0;
                    Debug.Assert(k == 0 || value == Math.Sign(p) * (Math.Abs(p) != k ? -1 : 1));
                }
            }
            return m0;
        }
    }
}
