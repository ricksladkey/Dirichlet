using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class PrimeCollection
    {
        private const int blockSizeSingleThreaded = 1 << 16;
        private const int blockSizeMultiThreaded = 1 << 18;
        private const int bucketSize = 1 << 20;
        private const int maxBuckets = 1 << 10;

        private long size;
        private int limit;
        private bool[] block;
        private uint[][] primes;
        private uint[] divisors;
        private int currentBucket;
        private int currentIndex;
        private int numberOfDivisors;

        public long Size { get { return size; } }
        public int Count { get { return currentBucket * bucketSize + currentIndex; } }

        public PrimeCollection(long size, int threads)
        {
            this.size = Math.Min((long)uint.MaxValue + 1, size);
            limit = (int)Math.Ceiling(Math.Sqrt(size));
            currentBucket = 0;
            currentIndex = 0;
            block = new bool[Math.Max(blockSizeSingleThreaded >> 1, limit)];
            primes = new uint[maxBuckets][];
            primes[0] = new uint[bucketSize];
            divisors = primes[0];
            if (size <= 13)
            {
                if (size > 2)
                    AddPrime(2);
                if (size > 3)
                    AddPrime(3);
                if (size > 5)
                    AddPrime(5);
                if (size > 7)
                    AddPrime(7);
                if (size > 11)
                    AddPrime(11);
                return;
            }
            GetDivisors(block);
            if (threads == 0)
                GetPrimes();
            else
                GetPrimes(threads);
        }

        public uint this[int index]
        {
            get { return primes[index / bucketSize][index % bucketSize]; }
        }

        public void GetPrimes()
        {
            var k0 = (long)limit & ~1;
            var offsets = new int[numberOfDivisors];
            for (var i = 0; i < numberOfDivisors; i++)
            {
                var d = divisors[i];
                var offset = (uint)(d - k0 % d) % d;
                if ((offset & 1) == 0)
                    offset += d;
                offsets[i] = (int)offset;
            }
            for (var k = k0; k < size; k += blockSizeSingleThreaded)
            {
                var kstart = (uint)k;
                var length = (int)Math.Min(blockSizeSingleThreaded, size - k);
                SieveBlock(kstart, length, block, offsets);
                AddPrimes(kstart, length, block);
            }
        }

        private void GetPrimes(int threads)
        {
            var k0 = (long)limit & ~1;
            var batchSize = threads * blockSizeMultiThreaded;
            var tasks = new Task[threads];
            var blocks = new bool[threads][];
            for (var thread = 0; thread < threads; thread++)
                blocks[thread] = new bool[blockSizeMultiThreaded >> 1];
            for (var l = k0; l < size; l += batchSize)
            {
                var kmax = l + Math.Min(batchSize, size - l);
                var thread = 0;
                for (var k = l; k < kmax; k += blockSizeMultiThreaded)
                {
                    var kstart = (uint)k;
                    var length = (int)Math.Min(blockSizeMultiThreaded, kmax - k);
                    var block = blocks[thread];
                    tasks[thread] = Task.Factory.StartNew(() => SieveBlock(kstart, length, block));
                    ++thread;
                }
                thread = 0;
                for (var k = l; k < kmax; k += blockSizeMultiThreaded)
                {
                    var kstart = (uint)k;
                    var length = (int)Math.Min(blockSizeMultiThreaded, kmax - k);
                    tasks[thread].Wait();
                    var block = blocks[thread];
                    AddPrimes(kstart, length, block);
                    ++thread;
                }
            }
        }

        private void GetDivisors(bool[] block)
        {
            // Sieve for all primes < sqrt(size).
            var sublimit = (int)Math.Ceiling(Math.Sqrt(limit));
            if (2 < limit)
                AddPrime(2);
            for (var i = 3; i < sublimit; i += 2)
            {
                if (!block[i])
                {
                    for (var j = i * i; j < limit; j += i)
                        block[j] = true;
                    AddPrime((uint)i);
                }
            }
            for (var i = sublimit | 1; i < limit; i += 2)
            {
                if (!block[i])
                    AddPrime((uint)i);
            }
            numberOfDivisors = currentIndex;
        }

        private void SieveBlock(uint k0, int length, bool []block, int[] offsets)
        {
            var length2 = length >> 1;
            var dlimit = Math.Min(numberOfDivisors, 6);
            var cycleSize = 1;
            for (var d = 2; d < dlimit; d++)
                cycleSize *= (int)divisors[d];
            Array.Clear(block, 0, cycleSize);
            for (var d = 2; d < dlimit; d++)
            {
                var i = (int)divisors[d];
                var j = i - k0 % i;
                j = (j + (((j & 1) - 1) & i)) >> 1;
                while (j < cycleSize)
                {
                    block[j] = true;
                    j += i;
                }
            }
            for (var i = cycleSize; i < length2; i += cycleSize)
                Array.Copy(block, 0, block, i, Math.Min(cycleSize, length2 - i));
            for (var d = dlimit; d < numberOfDivisors; d++)
            {
                var i = (int)divisors[d];
                var j = offsets[d] >> 1;
                while (j < length2)
                {
                    block[j] = true;
                    j += i;
                }
                offsets[d] = ((j << 1) | 1) - length;
            }
        }

        private void SieveBlock(uint k0, int length, bool[] block)
        {
            var length2 = length >> 1;
            Array.Clear(block, 0, length2);
            for (var d = 2; d < numberOfDivisors; d++)
            {
                var i = divisors[d];
                var j = i - k0 % i;
                j = (j + (((j & 1) - 1) & i)) >> 1;
                while (j < length2)
                {
                    block[j] = true;
                    j += i;
                }
            }
        }

#if false
        private void AddPrimes(uint k0, int length, bool[] block)
        {
            var length2 = length >> 1;
            for (var i = 0; i < length2; i++)
            {
                if (!block[i])
                    AddPrime((uint)((i << 1) | 1) + k0);
            }
        }
#endif

#if true
        private void AddPrimes(uint k0, int length, bool[] block)
        {
            var length2 = length >> 1;
            var imin = 3 - k0 % 3;
            imin = (imin + (((imin & 1) - 1) & 3)) >> 1;
            var i = 0;
            while (i < imin)
            {
                if (!block[i])
                    AddPrime((uint)((i << 1) | 1) + k0);
                ++i;
            }
            while (true)
            {
                if (++i >= length2)
                    break;
                if (!block[i])
                    AddPrime((uint)((i << 1) | 1) + k0);
                if (++i >= length2)
                    break;
                if (!block[i])
                    AddPrime((uint)((i << 1) | 1) + k0);
                ++i;
            }
        }
#endif

        private void AddPrime(uint p)
        {
            primes[currentBucket][currentIndex++] = p;
            if (currentIndex == bucketSize)
            {
                primes[++currentBucket] = new uint[bucketSize];
                currentIndex = 0;
            }
        }
    }
}
