using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class MobiusRange
    {
        private struct Offsets
        {
            public long Offset;
            public long OffsetSquared;
        }

        private const int blockSize = 1 << 15;

        private long size;
        private int threads;
        private uint[] primes;
        private int cycleLimit;
        private uint cycleSize;
        private long[] cycle;

        public long Size { get { return size; } }

        public MobiusRange(long size, int threads)
        {
            this.size = size;
            this.threads = threads;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            primes = new PrimeCollection(limit, 0).ToArray();
            CreateCycle();
        }

        public void GetValues(long kmin, long kmax, sbyte[] values)
        {
            if (threads == 0)
            {
                ProcessRange(kmin, kmax, kmin, values);
                return;
            }
            var tasks = new Task[threads];
            var length = kmax - kmin;
            var batchSize = ((length + threads - 1) / threads + 1) & ~1;
            for (var thread = 0; thread < threads; thread++)
            {
                var kstart = (long)thread * batchSize + kmin;
                var kend = Math.Min(kstart + batchSize, kmax);
                tasks[thread] = Task.Factory.StartNew(() => ProcessRange(kstart, kend, kmin, values));
            }
            Task.WaitAll(tasks);
            if (kmin <= 1)
                values[1 - kmin] = 1;
        }


        private void CreateCycle()
        {
            // Create pre-sieved cycle of the squares of small primes.
            var dmax = 3;
            cycleLimit = Math.Min(primes.Length, dmax);
            cycleSize = 1;
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = primes[i];
                cycleSize *= p * p;
            }
            cycle = new long[cycleSize];
            for (var i = 0; i < cycleSize; i++)
                cycle[i] = -1;
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = primes[i];
                var pMinus = -p;
                for (var k = (uint)0; k < cycleSize; k += p)
                    cycle[k] *= pMinus;
                var pSquared = (long)p * p;
                for (var k = (long)0; k < cycleSize; k += pSquared)
                    cycle[k] = 0;
            }
        }

        private void ProcessRange(long kstart, long kend, long kmin, sbyte[] values)
        {
            var products = new long[blockSize];
            var offsets = new Offsets[primes.Length];
            var cycleOffset = cycleSize - kstart % cycleSize;
            if (cycleOffset == cycleSize)
                cycleOffset = 0;
            offsets[0].Offset = cycleOffset;
            for (var i = 1; i < primes.Length; i++)
            {
                var p = primes[i];
                var offset = p - kstart % p;
                if (offset == p)
                    offset = 0;
                offsets[i].Offset = offset;
                var pSquared = (long)p * p;
                var offsetSquared = pSquared - kstart % pSquared;
                if (offsetSquared == pSquared)
                    offsetSquared = 0;
                offsets[i].OffsetSquared = offsetSquared;
            }
            for (var k = kstart; k < kend; k += blockSize)
            {
                var length = (int)Math.Min(blockSize, kend - k);
                SieveBlock(k, length, products, offsets);
                AddValues(k, length, products, kmin, values);
            }
        }

        private void SieveBlock(long k0, int length, long[] products, Offsets[] offsets)
        {
            var cycleOffset = offsets[0].Offset;
            Array.Copy(cycle, cycleSize - cycleOffset, products, 0, Math.Min(length, cycleOffset));
            while (cycleOffset < length)
            {
                Array.Copy(cycle, 0, products, cycleOffset, Math.Min(cycleSize, length - cycleOffset));
                cycleOffset += cycleSize;
            }
            offsets[0].Offset = cycleOffset - length;

            for (var i = cycleLimit; i < primes.Length; i++)
            {
                var p = (int)primes[i];
                var pMinus = -p;
                long k;
                for (k = offsets[i].Offset; k < length; k += p)
                    products[k] *= pMinus;
                offsets[i].Offset = k - length;
                var pSquared = (long)p * p;
                for (k = offsets[i].OffsetSquared; k < length; k += pSquared)
                    products[k] = 0;
                offsets[i].OffsetSquared = k - length;
            }
        }

        private void AddValues(long k0, int length, long[] products, long kmin, sbyte[] values)
        {
            // Each product that is square-free can have at most one more
            // prime factor.  It has that factor if the absolute value of
            // the product is not equal to the full value.
            var k = k0;
            for (var i = 0; i < length; i++, k++)
            {
                var p = products[i];
                var pos = -p >> 63; // pos = -1 if p > 0, zero otherwise
                var neg = p >> 63; // neg = -1 if p is < 0, zero otherwise
                var abs = (p + neg) ^ neg; // abs = |p|
                var flip = ~(abs - k) >> 63; // flip = -1 if abs >= k, zero otherwise
                values[k - kmin] = (sbyte)(((neg - pos) ^ flip) - flip); // values[k] = pos - neg if flip = -1, neg - pos otherwise
                Debug.Assert(values[k - kmin] == Math.Sign(p) * (Math.Abs(p) == k ? -1 : 1));
            }
        }
    }
}
