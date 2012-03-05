using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class PrimeCollection
    {
        private const int blockSize = 1 << 16;
        private const int bucketSize = 1 << 20;
        private const int maxBuckets = 1 << 10;

        private long size;
        private int limit;
        private bool[] block;
        private uint[][] primes;
        private uint[] divisors;
        private int[] offsets;
        private int currentBucket;
        private int currentIndex;
        private int numberOfDivisors;

        public long Size { get { return size; } }
        public int Count { get { return currentBucket * bucketSize + currentIndex; } }

        public PrimeCollection(long size)
        {
            this.size = size;
            limit = (int)Math.Ceiling(Math.Sqrt(size));
            currentBucket = 0;
            currentIndex = 0;
            block = new bool[Math.Max(blockSize, limit)];
            primes = new uint[maxBuckets][];
            primes[0] = new uint[bucketSize];
            divisors = primes[0];
            GetDivisors();
            if (limit == 2 && limit < size)
                AddPrime(2);
            var k0 = (uint)(limit & ~1);
            offsets = new int[numberOfDivisors];
            for (var i = 0; i < numberOfDivisors; i++)
            {
                var d = divisors[i];
                offsets[i] = (int)((-k0 % d + d) % d);
            }
            for (var k = (long)k0; k < size; k += blockSize)
                GetPrimes((uint)k, (int)Math.Min(blockSize, size - k), offsets);
        }

        public uint this[int index]
        {
            get { return primes[index / bucketSize][index % bucketSize]; }
        }

        private void GetDivisors()
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

        private void GetPrimes(uint k0, int length, int[] offsets)
        {
            Array.Clear(block, 0, length);
            for (var d = 1; d < numberOfDivisors; d++)
            {
                var i = (int)divisors[d];
                var j = offsets[d];
                while (j < length)
                {
                    block[j] = true;
                    j += i;
                }
                offsets[d] = j - length;
            }
            for (int i = 1; i < length; i += 2)
            {
                if (!block[i])
                    AddPrime((uint)i + k0);
            }
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
