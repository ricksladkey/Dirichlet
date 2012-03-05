using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class PrimeCollection
    {
        private int size;
        private int[] primes;

        public int Size { get { return size; } }
        public int Count { get { return primes.Length; } }

        public PrimeCollection(int size)
        {
            this.size = size;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            int count = 0;
            var composite = new bool[size];
            for (var i = (uint)2; i < limit; i++)
            {
                if (!composite[(int)i])
                {
                    for (var j = i * i; j < size; j += i)
                        composite[(int)j] = true;
                    ++count;
                }
            }
            for (var i = limit; i < size; i++)
            {
                if (!composite[i])
                    ++count;
            }
            primes = new int[count];
            var next = 0;
            for (int i = 2; i < size; i++)
            {
                if (!composite[i])
                    primes[next++] = i;
            }
        }

        public int this[int index]
        {
            get { return primes[index]; }
        }
    }
}
