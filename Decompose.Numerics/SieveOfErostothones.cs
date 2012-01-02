using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class SieveOfErostothones : IEnumerable<int>
    {
        private const int initialSize = 1024;
        private BitArray bits;

        private IEnumerable<int> Sieve()
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
                for (int i = 0; i < bits.Length; i++)
                {
                    if (!bits[i])
                        yield return i;
                }
                m = bits.Length;
                bits.Length = m * 2;
            }
            while (true)
            {
                int n = bits.Length;
                int p = 2;
                while (true)
                {
                    if (p >= m)
                        yield return p;
                    if (2 * p >= n)
                    {
                        for (p++; p < n; p++)
                        {
                            if (!bits[p])
                                yield return p;
                        }
                        break;
                    }
                    int q = Math.Max(m + (p - m % p) % p, 2 * p);
                    Debug.Assert(q % p == 0);
                    for (int i = q; i < n; i += p)
                        bits[i] = true;
                    ++p;
                    while (p < n && bits[p])
                        ++p;
                    if (p >= n)
                        break;
                }
                m = bits.Length;
                bits.Length = m * 2;
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return Sieve().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
