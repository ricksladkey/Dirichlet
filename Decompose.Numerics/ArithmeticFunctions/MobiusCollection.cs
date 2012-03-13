using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class MobiusCollection
    {
        private const int blockSize = 1 << 15;

        private uint[] primes;
        private int size;
        private byte[] values;
        private int cycleLimit;
        private int cycleSize;
        private int[] cycle;

        public int Size { get { return size; } }
        public int this[int index] { get { return values[index] - 1; } }

        public MobiusCollection(int size, int threads)
        {
            this.size = size;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            primes = new PrimeCollection(limit, 0).ToArray();
            values = new byte[size];
            CreateCycle();
            GetValues(threads);
            values[1] = 2;
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
                cycleSize *= p * p;
            }
            cycle = new int[cycleSize];
            for (var i = 0; i < cycleSize; i++)
                cycle[i] = -1;
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = (int)primes[i];
                var pMinus = -p;
                for (var k = 0; k < cycleSize; k += p)
                    cycle[k] *= pMinus;
                var pSquared = p * p;
                for (var k = 0; k < cycleSize; k += pSquared)
                    cycle[k] = 0;
            }
        }

        private void GetValues(int threads)
        {
            if (threads == 0)
            {
                ProcessRange(1, size);
                return;
            }
            var tasks = new Task[threads];
            var batchSize = ((size + threads - 1) / threads + 1) & ~1;
            for (var thread = 0; thread < threads; thread++)
            {
                var kstart = thread * batchSize + 1;
                var kend = Math.Min(kstart + batchSize, size);
                tasks[thread] = Task.Factory.StartNew(() => ProcessRange(kstart, kend));
            }
            Task.WaitAll(tasks);
        }

        private void ProcessRange(int kstart, int kend)
        {
            var products = new int[blockSize];
            var offsets = new int[primes.Length];
            var offsetsSquared = new int[primes.Length];
            var cycleOffset = cycleSize - kstart % cycleSize;
            if (cycleOffset == cycleSize)
                cycleOffset = 0;
            offsets[0] = cycleOffset;
            for (var i = 1; i < primes.Length; i++)
            {
                var p = (int)primes[i];
                var offset = p - kstart % p;
                if (offset == p)
                    offset = 0;
                offsets[i] = offset;
                var pSquared = p * p;
                var offsetSquared = pSquared - kstart % pSquared;
                if (offsetSquared == pSquared)
                    offsetSquared = 0;
                offsetsSquared[i] = offsetSquared;
            }
            for (var k = kstart; k < kend; k += blockSize)
                SieveBlock(k, Math.Min(blockSize, kend - k), products, offsets, offsetsSquared);
        }

        private void SieveBlock(int k0, int length, int[] products, int[] offsets, int[] offsetsSquared)
        {
            var cycleOffset = offsets[0];
            Array.Copy(cycle, cycleSize - cycleOffset, products, 0, cycleOffset);
            while (cycleOffset < length)
            {
                Array.Copy(cycle, 0, products, cycleOffset, Math.Min(cycleSize, length - cycleOffset));
                cycleOffset += cycleSize;
            }
            offsets[0] = cycleOffset - length;

            for (var i = cycleLimit; i < primes.Length; i++)
            {
                var p = (int)primes[i];
                var pMinus = -p;
                int k;
                for (k = offsets[i]; k < length; k += p)
                    products[k] *= pMinus;
                offsets[i] = k - length;
                var pSquared = p * p;
                for (k = offsetsSquared[i]; k < length; k += pSquared)
                    products[k] = 0;
                offsetsSquared[i] = k - length;
            }

            // Each product that is square-free can have at most one more
            // prime factor.  It has that factor if the absolute value of
            // the product is not equal to the full value.
            for (int i = 0, k = k0; i < length; i++, k++)
            {
                var p = products[i];
                var pos = (-p >> 31) & 1; // pos = 1 if p > 0, zero otherwise
                var neg = p >> 31; // neg = -1 if p is < 0, zero otherwise
                var abs = (p + neg) ^ neg; // abs = |p|
                var flip = ~((abs - k) >> 31) & 2; // flip = 2 if abs == k, zero otherwise
                values[k] = (byte)((pos + neg + flip + 1) & 3); // values[k] = mu(k) + 1
                Debug.Assert(values[k] == Math.Sign(p) * (Math.Abs(p) == k ? -1 : 1) + 1);
            }
        }
    }
}
