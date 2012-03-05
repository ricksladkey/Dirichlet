using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class PrimeCollection
    {
        private const int blockSize = 1 << 20;
        private const int bucketSize = 1 << 20;
        private const int maxBuckets = 1 << 10;

        private int size;
        private int limit;
        private bool[] block;
        private int[][] primes;
        private int[] divisors;
        private int currentBucket;
        private int currentIndex;
        private int numberOfDivisors;

        public int Size { get { return size; } }
        public int Count { get { return currentBucket * bucketSize + currentIndex; } }

        public PrimeCollection(int size)
        {
            this.size = size;
            limit = (int)Math.Ceiling(Math.Sqrt(size));
            currentBucket = 0;
            currentIndex = 0;
            block = new bool[blockSize];
            primes = new int[maxBuckets][];
            primes[0] = new int[bucketSize];
            divisors = primes[0];
            GetDivisors();
            if (limit == 2 && limit < size)
                AddPrime(2);
            for (var k = limit & ~1; k < size; k += blockSize)
                GetPrimes(k, Math.Min(blockSize, size - k));
        }

        public int this[int index]
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
                    AddPrime(i);
                }
            }
            for (var i = sublimit | 1; i < limit; i += 2)
            {
                if (!block[i])
                    AddPrime(i);
            }
            numberOfDivisors = currentIndex;
        }

        private void GetPrimes(int k0, int length)
        {
            Array.Clear(block, 0, length);
            for (var d = 1; d < numberOfDivisors; d++)
            {
                var i = divisors[d];
                var j0 = (-k0 % i + i) % i;
                for (var j = j0; j < length; j += i)
                    block[j] = true;
            }
            for (int i = 1; i < length; i += 2)
            {
                if (!block[i])
                    AddPrime(i + k0);
            }
        }

        private void AddPrime(int p)
        {
            primes[currentBucket][currentIndex++] = p;
            if (currentIndex == bucketSize)
            {
                primes[++currentBucket] = new int[bucketSize];
                currentIndex = 0;
            }
        }
    }
}
