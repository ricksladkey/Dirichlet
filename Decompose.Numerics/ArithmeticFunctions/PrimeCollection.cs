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

        public PrimeCollection(int size)
        {
            this.size = size;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            int count = 0;
            var composite = new bool[size];
            for (int i = 2; i < limit; i++)
            {
                if (!composite[i])
                {
                    for (int j = i * i; j < size; j += i)
                        composite[j] = true;
                    ++count;
                }
            }
            primes = new int[count];
            for (int i = 2; i < limit; i++)
            {
                if (!composite[i])
                    primes[i] = i;
            }
        }

        public int this[int index]
        {
            get { return primes[index]; }
        }
    }
}
