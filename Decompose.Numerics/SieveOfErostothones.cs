using System.Collections;
using System.Collections.Generic;
using System;

namespace Decompose.Numerics
{
    public class SieveOfErostothones
    {
        private const int initialSize = 10;
        private BitArray bits;

        public IEnumerable<int> Sieve()
        {
            int m = 0;
            if (bits == null)
            {
                bits = new BitArray(initialSize);
                bits[0] = true;
                bits[1] = true;
            }
            else
            {
                m = bits.Length;
                bits.Length = m * 2;
            }
            int n = bits.Length;
            int p = 2;
            while (true)
            {
                yield return p;
                int q = Math.Max(m + (m - m % p) % p, 2 * p);
                for (int i = q; i < n; i += p)
                    bits[i] = true;
                ++p;
                while (p < n && bits[p])
                    ++p;
                if (p >= n)
                    break;
            }
        }
    }
}
