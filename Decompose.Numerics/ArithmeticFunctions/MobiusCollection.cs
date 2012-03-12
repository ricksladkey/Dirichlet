using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class MobiusCollection
    {
        private const int blockSize = 1 << 16;

        private uint[] primes;
        private int size;
        private byte[] values;
        private int dlimit;
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
            // Create pre-sieved cycle of small primes.
            var dmax = 5;
            dlimit = Math.Min(primes.Length, dmax);
            cycleSize = 1;
            for (var d = 0; d < dlimit; d++)
                cycleSize *= (int)primes[d];
            cycleSize *= 2;
            cycle = new int[cycleSize];
            for (var i = 0; i < cycleSize; i++)
                cycle[i] = -1;
            for (var i = 0; i < dlimit; i++)
            {
                var p = (int)primes[i];
                var pMinus = -p;
                for (var j = 0; j < cycleSize; j += p)
                    cycle[j] *= pMinus;
            }
            for (var j = 0; j < cycleSize; j += 4)
                cycle[j] = 0;
        }

        private void GetValues(int threads)
        {
            if (threads == 0)
            {
                ProcessRange(0, size);
                return;
            }
            var tasks = new Task[threads];
            var batchSize = ((size + threads - 1) / threads + 1) & ~1;
            for (var thread = 0; thread < threads; thread++)
            {
                var kstart = thread * batchSize;
                var kend = Math.Min(kstart + batchSize, size);
                tasks[thread] = Task.Factory.StartNew(() => ProcessRange(kstart, kend));
            }
            Task.WaitAll(tasks);
        }

        private void ProcessRange(int kstart, int kend)
        {
            var products = new int[blockSize];
            var offsets = new int[primes.Length];
            var cycleOffset = cycleSize - kstart % cycleSize;
            if (cycleOffset == cycleSize)
                cycleOffset = 0;
            offsets[0] = cycleOffset;
            for (var i = dlimit; i < primes.Length; i++)
            {
                var p = (int)primes[i];
                var offset = p - kstart % p;
                if (offset == p)
                    offset = 0;
                offsets[i] = offset;
            }
            for (var k = kstart; k < kend; k += blockSize)
                SieveBlock(k, Math.Min(blockSize, kend - k), products, offsets);
        }

        private void SieveBlock(int k0, int length, int[] products, int[] offsets)
        {
            var cycleOffset = offsets[0];
            Array.Copy(cycle, cycleSize - cycleOffset, products, 0, cycleOffset);
            while (cycleOffset < length)
            {
                Array.Copy(cycle, 0, products, cycleOffset, Math.Min(cycleSize, length - cycleOffset));
                cycleOffset += cycleSize;
            }
            offsets[0] = cycleOffset - length;

            for (var i = 1; i < dlimit; i++)
            {
                var p = (int)primes[i];
                var pSquared = p * p;
                var j1 = pSquared - k0 % pSquared;
                if (j1 == pSquared)
                    j1 = 0;
                for (var j = j1; j < length; j += pSquared)
                    products[j] = 0;
            }

            for (var i = dlimit; i < primes.Length; i++)
            {
                var p = (int)primes[i];
                var pMinus = -p;
                int j;
                for (j = offsets[i]; j < length; j += p)
                    products[j] *= pMinus;
                offsets[i] = j - length;
                var pSquared = p * p;
                var j1 = pSquared - k0 % pSquared;
                if (j1 == pSquared)
                    j1 = 0;
                for (j = j1; j < length; j += pSquared)
                    products[j] = 0;
            }

            for (int i = 0, k = k0; i < length; i++, k++)
            {
                var p = products[i];
                //if (p > 0)
                //    values[k] = p == k ? (byte)0 : (byte)2;
                //else if (p < 0)
                //    values[k] = p == -k ? (byte)2 : (byte)0;
                //else
                //    values[k] = 1;
                var pos = ((p - 1) >> 31) + 1; // pos = 1 if p > 0, zero otherwise
                var neg = p >> 31; // neg = -1 if p is < 0, zero otherwise
                var abs = (p + neg) ^ neg; // abs = Math.Abs(p)
                var flip = ~((abs - k) >> 31) & 2; // flip = 2 if abs == k, zero otherwise
                values[k] = (byte)((pos + neg + flip + 1) & 3);
            }
        }
    }
}
