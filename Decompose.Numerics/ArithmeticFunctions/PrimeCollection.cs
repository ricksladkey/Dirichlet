using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class PrimeCollection : IEnumerable<uint>
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
        private bool[] cycle;
        private int dlimit;
        private int cycleSize;
        private int currentBucket;
        private int currentIndex;
        private int numberOfDivisors;

        public long Size { get { return size; } }
        public int Count { get { return currentBucket * bucketSize + currentIndex; } }

        public uint this[int index]
        {
            get { return primes[index / bucketSize][index % bucketSize]; }
        }

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
            CreateCycle();
            if (threads == 0)
                GetPrimes();
            else
                GetPrimes(threads);
        }

        public IEnumerator<uint> GetEnumerator()
        {
            var count = Count;
            for (int i = 0; i < count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void GetPrimes()
        {
            var k0 = (long)limit & ~1;
            var offsets = new int[numberOfDivisors];
            var cycleOffset = cycleSize - (int)(k0 % cycleSize);
            cycleOffset = (cycleOffset + (((cycleOffset & 1) - 1) & cycleSize)) >> 1;
            offsets[0] = cycleOffset;
            for (var i = 1; i < numberOfDivisors; i++)
            {
                var d = divisors[i];
                var offset = (uint)(d - k0 % d) % d;
                if ((offset & 1) == 0)
                    offset += d;
                offsets[i] = (int)offset >> 1;
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

        private void CreateCycle()
        {
            dlimit = Math.Min(numberOfDivisors, 6);
            cycleSize = 1;
            for (var d = 2; d < dlimit; d++)
                cycleSize *= (int)divisors[d];
            cycle = new bool[cycleSize];
            for (var d = 2; d < dlimit; d++)
            {
                var i = (int)divisors[d];
                for (var j = 0; j < cycleSize; j += i)
                    cycle[j] = true;
            }
        }

        private void SieveBlock(uint k0, int length, bool []block, int[] offsets)
        {
            var length2 = length >> 1;

            var cycleOffset = offsets[0];
            Array.Copy(cycle, cycleSize - cycleOffset, block, 0, cycleOffset);
            while (cycleOffset < length2)
            {
                Array.Copy(cycle, 0, block, cycleOffset, Math.Min(cycleSize, length2 - cycleOffset));
                cycleOffset += cycleSize;
            }
            offsets[0] = cycleOffset - length2;

            for (var d = dlimit; d < numberOfDivisors; d++)
            {
                var i = (int)divisors[d];
                var j = offsets[d];
                while (j < length2)
                {
                    block[j] = true;
                    j += i;
                }
                offsets[d] = j - length2;
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

        private void AddPrimes(uint k0, int length, bool[] block)
        {
            var k1 = k0 + 1;
            var length2 = length >> 1;
            var imin = 3 - k0 % 3;
            imin = (imin + (((imin & 1) - 1) & 3)) >> 1;
            var imax = length2 - (length2 - imin) % 3;
            if (0 < imin && !block[0])
                AddPrime((uint)(0 << 1) + k1);
            if (1 < imin && !block[1])
                AddPrime((uint)(1 << 1) + k1);
            var i = imin + 1;
            while (i < imax)
            {
                if (!block[i])
                    AddPrime((uint)(i << 1) + k1);
                if (!block[i + 1])
                    AddPrime((uint)((i + 1) << 1) + k1);
                i += 3;
            }
            if (i < length2 && !block[i])
                AddPrime((uint)(i << 1) + k1);
        }

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

