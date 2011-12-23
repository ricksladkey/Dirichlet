using System;
using System.Numerics;

namespace Decompose.Numerics
{
    // Reference: http://en.wikipedia.org/wiki/Mersenne_twister

    //// Create a length 624 array to store the state of the generator
    // int[0..623] MT
    // int index = 0

    // // Initialize the generator from a seed
    // function initializeGenerator(int seed) {
    //     MT[0] := seed
    //     for i from 1 to 623 { // loop over each other element
    //         MT[i] := last 32 bits of(1812433253 * (MT[i-1] xor (right shift by 30 bits(MT[i-1]))) + i) // 0x6c078965
    //     }
    // }

    // // Extract a tempered pseudorandom number based on the index-th value,
    // // calling generateNumbers() every 624 numbers
    // function extractNumber() {
    //     if index == 0 {
    //         generateNumbers()
    //     }

    //     int y := MT[index]
    //     y := y xor (right shift by 11 bits(y))
    //     y := y xor (left shift by 7 bits(y) and (2636928640)) // 0x9d2c5680
    //     y := y xor (left shift by 15 bits(y) and (4022730752)) // 0xefc60000
    //     y := y xor (right shift by 18 bits(y))

    //     index := (index + 1) mod 624
    //     return y
    // }

    // // Generate an array of 624 untempered numbers
    // function generateNumbers() {
    //     for i from 0 to 623 {
    //         int y := 32nd bit of(MT[i]) + last 31 bits of(MT[(i+1) mod 624])
    //         MT[i] := MT[(i + 397) mod 624] xor (right shift by 1 bit(y))
    //         if (y mod 2) == 1 { // y is odd
    //             MT[i] := MT[i] xor (2567483615) // 0x9908b0df
    //         }
    //     }
    // }

    public class MersenneTwister32
    {
        private const int N = 624;
        private UInt32[] MT;
        private int index;

        protected object sync = new object();

        public MersenneTwister32(UInt32 seed)
        {
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

        protected UInt32 Next32()
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

        public UInt32 Next()
        {
            lock (sync)
                return Next32();
        }

        public BigInteger Next(BigInteger n)
        {
            lock (sync)
            {
                var c = (n.ToByteArray().Length + 3) / 4 * 4;
                var bytes = new byte[c + 1];
                for (int i = 0; i < c; i += 4)
                    BitConverter.GetBytes(Next()).CopyTo(bytes, i);
                return new BigInteger(bytes) % n;
            }
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

    public class MersenneTwister64 : MersenneTwister32
    {
        public MersenneTwister64(UInt32 seed)
            : base(seed)
        {
        }

        public new UInt64 Next()
        {
            lock (sync)
                return (((UInt64)Next32()) << 32) | ((UInt64)Next32());
        }
    }
}
