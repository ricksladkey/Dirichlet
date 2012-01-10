using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public class MersenneTwister
    {
        private const int N = 624;
        private UInt32[] MT;
        private int index;

        private object syncRoot = new object();

        public object SyncRoot { get { return syncRoot; } }

        public MersenneTwister(UInt32 seed)
        {
            // Reference: http://en.wikipedia.org/wiki/Mersenne_twister
            // Create a length 624 array to store the state of the generator.
            MT = new UInt32[N];

            // Initialize the generator from a seed.
            MT[0] = seed;
            for (int i = 1; i < N; i++)
            {
                MT[i] = 0x6c078965u * (MT[i - 1] ^ ((MT[i - 1] >> 30))) + (UInt32)i;
            }

            index = 0;
        }

        public UInt32 Next()
        {
            // Extract a tempered pseudorandom number based on the index-th value,
            // calling GenerateNumbers() every 624 numbers.

            if (index == 0)
                GenerateNumbers();

            UInt32 y = MT[index];
            y ^= y >> 11;
            y ^= (y << 7) & 0x9d2c5680u;
            y ^= (y << 15) & 0xefc60000u;
            y ^= y >> 18;

            index = (index + 1) % N;

            return y;
        }

        private void GenerateNumbers()
        {
            // Generate an array of 624 untempered numbers.
            for (int i = 0; i < N; i++)
            {
                UInt32 y = (0x80000000u & MT[i]) | (0x7fffffffu & MT[(i + 1) % N]);
                MT[i] = MT[(i + 397) % N] ^ (y >> 1);
                if (y % 2 == 1)
                {
                    MT[i] ^= 0x9908b0dfu;
                }
            }
        }
    }

    public class MersenneTwister32 : IRandomNumberAlgorithm<uint>
    {
        private MersenneTwister random;

        public MersenneTwister32(uint seed)
        {
            random = new MersenneTwister(seed);
        }

        public UInt32 Next(uint n)
        {
            lock (random.SyncRoot)
            {
                var next = random.Next();
                return n == 0 ? next : next % n;
            }
        }
    }

    public class MersenneTwister64 : IRandomNumberAlgorithm<ulong>
    {
        private MersenneTwister random;

        public MersenneTwister64(uint seed)
        {
            random = new MersenneTwister(seed);
        }

        public ulong Next(ulong n)
        {
            lock (random.SyncRoot)
            {
                var next = (ulong)random.Next() << 32 | random.Next();
                return n == 0 ? next : next % n;
            }
        }
    }

    public class MersenneTwisterBigInteger : IRandomNumberAlgorithm<BigInteger>
    {
        private MersenneTwister random;

        public MersenneTwisterBigInteger(uint seed)
        {
            random = new MersenneTwister(seed);
        }

        public BigInteger Next(BigInteger n)
        {
            lock (random.SyncRoot)
            {
                var c = (n.ToByteArray().Length + 3) / 4 * 4;
                var bytes = new byte[c + 1];
                for (int i = 0; i < c; i += 4)
                    BitConverter.GetBytes(random.Next()).CopyTo(bytes, i);
                return new BigInteger(bytes) % n;
            }
        }
    }
}
