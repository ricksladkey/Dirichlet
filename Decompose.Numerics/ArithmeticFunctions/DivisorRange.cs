using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class DivisorRange
    {
        private class Data
        {
            public long[] Products;
            public int[] Values;
            public int[] Offsets;
            public long[] OffsetsSquared;

            public Data(int length)
            {
                Products = new long[blockSize];
                Values = new int[blockSize];
                Offsets = new int[length];
                OffsetsSquared = new long[length];
            }
        }

        private const int blockSize = 1 << 14;

        private long size;
        private int threads;
        private int[] primes;
        private int cycleLimit;
        private int cycleSize;
        private long[] cycleProducts;
        private int[] cycleValues;
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
            var pmax = GetPMax(kmax);

            if (threads == 0)
            {
                ProcessRange(pmax, kmin, kmax, values, offset, sums, m0, null);
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
                tasks[thread] = Task.Factory.StartNew(() => ProcessRange(pmax, kstart, kend, values, offset, sums, m0, null));
            }
            Task.WaitAll(tasks);

            if (sums == null)
                return;

            // Collect summatory function totals for each batch.
            var mabs = new long[threads];
            mabs[0] = 0;
            for (var thread = 1; thread < threads; thread++)
            {
                var last = (long)thread * batchSize - 1;
                if (last < sums.Length)
                    mabs[thread] = mabs[thread - 1] + sums[last] - m0;
            }

            // Convert relative summatory function values into absolute summatory function.
            for (var thread = 1; thread < threads; thread++)
            {
                var index = thread;
                var kstart = (long)thread * batchSize + kmin;
                var kend = Math.Min(kstart + batchSize, kmax);
                tasks[thread] = Task.Factory.StartNew(() => BumpRange(mabs[index], kstart, kend, offset, sums));
            }
            Task.WaitAll(tasks);
        }

        public void GetValues(long kmin, long kmax, long m0, Action<long, long, int[]> action)
        {
            ProcessRange(GetPMax(kmax), kmin, kmax, null, -1, null, m0, action);
        }

        private int GetPMax(long kmax)
        {
            // Determine the number of primes appropriate for values up to kmax.
            var plimit = (int)Math.Ceiling(Math.Sqrt(kmax));
            var pmax = primes.Length;
            while (pmax > 0 && primes[pmax - 1] > plimit)
                --pmax;
            return pmax;
        }

        private void BumpRange(long abs, long kstart, long kend, long offset, long[] sums)
        {
            var klo = (int)(kstart - offset);
            var khi = (int)(kend - offset);
            for (var k = klo; k < khi; k++)
                sums[k] += abs;
        }

        private void CreateCycle()
        {
            // Create pre-sieved product and value cycles of small primes and their squares.
            var dmax = 3;
            cycleLimit = Math.Min(primes.Length, dmax);
            cycleSize = 1;
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = (int)primes[i];
                cycleSize *= p * p;
            }
            cycleProducts = new long[cycleSize];
            cycleValues = new int[cycleSize];
            for (var i = 0; i < cycleSize; i++)
            {
                cycleProducts[i] = 1;
                cycleValues[i] = 1;
            }
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = primes[i];
                for (var k = 0; k < cycleSize; k += p)
                {
                    cycleProducts[k] *= p;
                    cycleValues[k] <<= 1;
                }
                var pSquared = (long)p * p;
                for (var k = (long)0; k < cycleSize; k += pSquared)
                {
                    cycleProducts[k] *= p;
                    cycleValues[k] = cycleValues[k] / 2 * 3;
                }
            }
        }

        private void ProcessRange(int pmax, long kstart, long kend, int[] values, long kmin, long[] sums, long m0, Action<long, long, int[]> action)
        {
            // Acquire resources.
            Data data;
            if (!queue.TryDequeue(out data))
                data = new Data(Math.Max(1, primes.Length));
            var products = data.Products;
            var offsets = data.Offsets;
            var offsetsSquared = data.OffsetsSquared;
            if (action != null)
                values = data.Values;

            // Determine the initial offset and offset squared of each prime divisor.
            for (var i = 0; i < pmax; i++)
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

            // Determine the initial cycle offset.
            var cycleOffset = cycleSize - (int)(kstart % cycleSize);
            if (cycleOffset == cycleSize)
                cycleOffset = 0;
            offsets[0] = cycleOffset;

            // Process the whole range in block-sized batches.
            for (var k = kstart; k < kend; k += blockSize)
            {
                var length = (int)Math.Min(blockSize, kend - k);
                var offset = kmin == -1 ? k : kmin;
                SieveBlock(pmax, k, length, products, values, offsets, offsetsSquared, offset);
                m0 = AddValues(k, length, products, values, sums, offset, m0);

                // Perform action, if any.
                if (action != null)
                    action(k, k + length, values);
            }

            // Release resources.
            queue.Enqueue(data);
        }

        private void SieveBlock(int pmax, long k0, int length, long[] products, int[] values, int[] offsets, long[] offsetsSquared, long kmin)
        {
            // Initialize and pre-sieve product and value arrays from cycles.
            var koffset = k0 - kmin;
            var cycleOffset = offsets[0];
            Array.Copy(cycleProducts, cycleSize - cycleOffset, products, 0, Math.Min(length, cycleOffset));
            Array.Copy(cycleValues, cycleSize - cycleOffset, values, koffset, Math.Min(length, cycleOffset));
            while (cycleOffset < length)
            {
                Array.Copy(cycleProducts, 0, products, cycleOffset, Math.Min(cycleSize, length - cycleOffset));
                Array.Copy(cycleValues, 0, values, koffset + cycleOffset, Math.Min(cycleSize, length - cycleOffset));
                cycleOffset += cycleSize;
            }
            offsets[0] = cycleOffset - length;

            // Handle small primes.
            if (0 < cycleLimit)
            {
                // Handle multiples of 2^2.
                int k;
                for (k = (int)offsetsSquared[0]; k < length; k += 4)
                {
                    var quotient = (k + k0) >> 2;
                    int exponent;
                    for (exponent = 2; (quotient & 1) == 0; exponent++)
                    {
                        products[k] <<= 1;
                        quotient >>= 1;
                    }
                    values[k + koffset] = values[k + koffset] / 3 * (exponent + 1);
                }
                offsetsSquared[0] = k - length;
            }
            if (1 < cycleLimit)
            {
                // Handle multiples of 3^2.
                int k;
                for (k = (int)offsetsSquared[1]; k < length; k += 9)
                {
                    var quotient = (k + k0) / 9;
                    int exponent;
                    for (exponent = 2; quotient % 3 == 0; exponent++)
                    {
                        products[k] *= 3;
                        quotient /= 3;
                    }
                    values[k + koffset] = values[k + koffset] / 3 * (exponent + 1);
                }
                offsetsSquared[1] = k - length;
            }
            if (2 < cycleLimit)
            {
                // Handle multiples of 5^2.
                int k;
                for (k = (int)offsetsSquared[2]; k < length; k += 25)
                {
                    var quotient = (k + k0) / 25;
                    int exponent;
                    for (exponent = 2; quotient % 5 == 0; exponent++)
                    {
                        products[k] *= 5;
                        quotient /= 5;
                    }
                    values[k + koffset] = values[k + koffset] / 3 * (exponent + 1);
                }
                offsetsSquared[2] = k - length;
            }

            // Sieve remaining primes.
            for (var i = cycleLimit; i < pmax; i++)
            {
                var p = primes[i];

                // Handle multiples of p.
                int k;
                for (k = offsets[i]; k < length; k += p)
                {
                    products[k] *= p;
                    values[k + koffset] <<= 1;
                }
                offsets[i] = k - length;

                // Handle multiples of p^2.
                long kk = offsetsSquared[i];
                if (kk < length)
                {
                    var pSquared = (long)p * p;
                    do
                    {
                        products[kk] *= p;
                        var quotient = (kk + k0) / pSquared;
                        int exponent;
                        for (exponent = 2; quotient % p == 0; exponent++)
                        {
                            products[kk] *= p;
                            quotient /= p;
                        }
                        values[kk + koffset] = values[kk + koffset] / 2 * (exponent + 1);
                        kk += pSquared;
                    }
                    while (kk < length);
                }
                offsetsSquared[i] = kk - length;
            }
        }

        private long AddValues(long k0, int length, long[] products, int[] values, long[] sums, long kmin, long m0)
        {
            // Each product can have at most one more prime factor.
            // It has that factor if the value of the product is
            // less than the full value.
            var k = (int)(k0 - kmin);
            if (sums == null)
            {
                for (var i = 0; i < length; i++, k++)
                {
                    if (products[i] < k + kmin)
                        values[k] <<= 1;
                    m0 += values[k];
                }
            }
            else
            {
                for (var i = 0; i < length; i++, k++)
                {
                    if (products[i] < k + kmin)
                        values[k] <<= 1;
                    m0 += values[k];
                    sums[k] = m0;
                }
            }
            return m0;
        }
    }
}
