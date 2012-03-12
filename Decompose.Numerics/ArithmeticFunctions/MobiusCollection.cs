using System;
using System.Collections.Generic;
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

        public int Size { get { return size; } }
        public int this[int index] { get { return values[index] - 1; } }

        public MobiusCollection(int size, int threads)
        {
            this.size = size;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            primes = new PrimeCollection(limit, 0).ToArray();
            values = new byte[size];
            GetValues(threads);
            values[1] = 2;
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
            for (var k = kstart; k < kend; k += blockSize)
                SieveBlock(k, Math.Min(blockSize, kend - k), products);
        }

        private void SieveBlock(int k0, int length, int[] products)
        {
            for (var i = 0; i < length; i++)
                products[i] = -1;

            for (var i = 0; i < primes.Length; i++)
            {
                var p = primes[i];
                var pMinus = -(int)p;
                var j0 = p - k0 % p;
                if (j0 == p)
                    j0 = 0;
                for (var j = j0; j < length; j += p)
                    products[j] *= pMinus;
                var pSquared = p * p;
                var j1 = pSquared - k0 % pSquared;
                if (j1 == pSquared)
                    j1 = 0;
                for (var j = j1; j < length; j += pSquared)
                    products[j] = 0;
            }

            byte value;
            for (int i = 0, k = k0; i < length; i++, k++)
            {
                var p = products[i];
                if (p > 0)
                    value = p == k ? (byte)0 : (byte)2;
                else if (p < 0)
                    value = p == -k ? (byte)2 : (byte)0;
                else
                    value = 1;
                values[k] = value;
            }
        }
    }
}
