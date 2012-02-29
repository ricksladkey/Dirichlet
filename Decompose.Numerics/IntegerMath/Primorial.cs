using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int Primorial(int n)
        {
            return (int)Primorial((uint)n);
        }

        public static uint Primorial(uint n)
        {
            var result = (uint)1;
            for (var i = (uint)0; i < n; i++)
                result *= (uint)primes[i];
            return result;
        }

        public static long Primorial(long n)
        {
            return (long)Primorial((ulong)n);
        }

        public static ulong Primorial(ulong n)
        {
            var result = (ulong)1;
            for (var i = (ulong)0; i < n; i++)
                result *= (ulong)primes[i];
            return result;
        }

        public static BigInteger Primorial(BigInteger n)
        {
            return PrimorialCore((int)n);
        }

        private static BigInteger PrimorialCore(int n)
        {
            var result = BigInteger.One;
            for (var i = 0; i < n; i++)
                result *= primes[i];
            return result;
        }
    }
}
