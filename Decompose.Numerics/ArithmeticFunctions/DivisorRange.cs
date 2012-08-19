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
            GetValuesAndSums(kmin, kmax, values, null, 0, kmin);
        }

        public void GetValues(long kmin, long kmax, int[] values, long offset)
        {
            GetValuesAndSums(kmin, kmax, values, null, 0, offset);
        }

        public long GetSums(long kmin, long kmax, long[] sums, long sum0)
        {
            return GetValuesAndSums(kmin, kmax, null, sums, sum0, kmin);
        }

        public long GetSums(long kmin, long kmax, long[] sums, long sum0, long offset)
        {
            return GetValuesAndSums(kmin, kmax, null, sums, sum0, offset);
        }

        public long GetValuesAndSums(long kmin, long kmax, int[] values, long[] sums, long sum0)
        {
            return GetValuesAndSums(kmin, kmax, values, sums, sum0, kmin);
        }

        public long GetValuesAndSums(long kmin, long kmax, int[] values, long[] sums, long sum0, long offset)
        {
            var pmax = GetPMax(kmax);
            var voffset = values == null ? -1 : offset;
            var soffset = sums == null ? -1 : offset;
            var slast = kmax - soffset - 1;

            if (threads == 0)
            {
                ProcessRange(pmax, kmin, kmax, values, voffset, sums, soffset, sum0, null);
                return sums == null ? 0 : sums[slast];
            }

            // Choose batch size such that: batchSize*threads >= length and batchSize is even.
            var tasks = new Task[threads];
            var length = kmax - kmin;
            var batchSize = ((length + threads - 1) / threads + 1) & ~1;
            for (var thread = 0; thread < threads; thread++)
            {
                var kstart = (long)thread * batchSize + kmin;
                var kend = Math.Min(kstart + batchSize, kmax);
                tasks[thread] = Task.Factory.StartNew(() => ProcessRange(pmax, kstart, kend, values, voffset, sums, soffset, sum0, null));
            }
            Task.WaitAll(tasks);

            if (sums == null)
                return 0;

            // Collect summatory function totals for each batch.
            var mabs = new long[threads];
            mabs[0] = 0;
            for (var thread = 1; thread < threads; thread++)
            {
                var last = (long)thread * batchSize - 1;
                if (last < sums.Length)
                    mabs[thread] = mabs[thread - 1] + sums[last] - sum0;
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

            return sums[slast];
        }

        public void GetValues(long kmin, long kmax, Action<long, long, int[]> action)
        {
            ProcessRange(GetPMax(kmax), kmin, kmax, null, -1, null, -1, 0, action);
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

        private void ProcessRange(int pmax, long kstart, long kend, int[] values, long kmin, long[] sums, long smin, long sum0, Action<long, long, int[]> action)
        {
            // Acquire resources.
            Data data;
            if (!queue.TryDequeue(out data))
                data = new Data(Math.Max(1, primes.Length));
            var products = data.Products;
            var offsets = data.Offsets;
            var offsetsSquared = data.OffsetsSquared;
            if (values == null)
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
                var voffset = kmin == -1 ? k : kmin;
                var soffset = smin == -1 ? k : smin;
                var length = (int)Math.Min(blockSize, kend - k);
                SieveBlock(pmax, k, length, products, values, offsets, offsetsSquared, voffset);
                sum0 = AddValues(k, length, products, values, voffset, sums, soffset, sum0);

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
                const int i = 0;
                const int p = 2;
                const int pSquared = p * p;
                int kk;
                for (kk = (int)offsetsSquared[i]; kk < length; kk += pSquared)
                {
                    Debug.Assert((k0 + kk) % pSquared == 0);
                    var quotient = (k0 + kk) / pSquared;
                    int exponent;
                    for (exponent = 2; quotient % p == 0; exponent++)
                    {
                        products[kk] *= p;
                        quotient /= p;
                    }
                    values[kk + koffset] = values[kk + koffset] / 3 * (exponent + 1);
                }
                offsetsSquared[i] = kk - length;
            }
            if (1 < cycleLimit)
            {
                // Handle multiples of 3^2.
                const int i = 1;
                const int p = 3;
                const int pSquared = p * p;
                int kk;
                for (kk = (int)offsetsSquared[i]; kk < length; kk += pSquared)
                {
                    Debug.Assert((k0 + kk) % pSquared == 0);
                    var quotient = (k0 + kk) / pSquared;
                    int exponent;
                    for (exponent = 2; quotient % p == 0; exponent++)
                    {
                        products[kk] *= p;
                        quotient /= p;
                    }
                    values[kk + koffset] = values[kk + koffset] / 3 * (exponent + 1);
                }
                offsetsSquared[i] = kk - length;
            }
            if (2 < cycleLimit)
            {
                // Handle multiples of 5^2.
                const int i = 2;
                const int p = 5;
                const int pSquared = p * p;
                int kk;
                for (kk = (int)offsetsSquared[i]; kk < length; kk += pSquared)
                {
                    Debug.Assert((k0 + kk) % pSquared == 0);
                    var quotient = (k0 + kk) / pSquared;
                    int exponent;
                    for (exponent = 2; quotient % p == 0; exponent++)
                    {
                        products[kk] *= p;
                        quotient /= p;
                    }
                    values[kk + koffset] = values[kk + koffset] / 3 * (exponent + 1);
                }
                offsetsSquared[i] = kk - length;
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
                        var quotient = (k0 + kk) / pSquared;
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

        private long AddValues(long k0, int length, long[] products, int[] values, long kmin, long[] sums, long smin, long sum0)
        {
            // Each product can have at most one more prime factor.
            // It has that factor if the value of the product is
            // less than the full value.
            var deltai = (int)(k0 - kmin);
            var deltas = (int)(smin - kmin);
            var kmax = (int)(deltai + length);
            if (sums == null)
            {
                for (var k = deltai; k < kmax; k++)
                {
                    if (products[k - deltai] < k + kmin)
                        values[k] <<= 1;
                    sum0 += values[k];
                }
            }
            else
            {
                for (var k = deltai; k < kmax; k++)
                {
                    if (products[k - deltai] < k + kmin)
                        values[k] <<= 1;
                    sum0 += values[k];
                    sums[k - deltas] = sum0;
                }
            }
            return sum0;
        }
    }
}
