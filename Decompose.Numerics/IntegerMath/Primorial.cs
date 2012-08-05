using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static int PrimorialCount(int n)
        {
            return (int)PrimorialCount((uint)n);
        }

        public static uint PrimorialCount(uint n)
        {
            var result = (uint)1;
            for (var i = (uint)0; i < n; i++)
                result *= (uint)primes[i];
            return result;
        }

        public static long PrimorialCount(long n)
        {
            return (long)PrimorialCount((ulong)n);
        }

        public static ulong PrimorialCount(ulong n)
        {
            var result = (ulong)1;
            for (var i = (ulong)0; i < n; i++)
                result *= (ulong)primes[i];
            return result;
        }

        public static BigInteger PrimorialCount(BigInteger n)
        {
            return PrimorialCountCore((int)n);
        }

        private static BigInteger PrimorialCountCore(int n)
        {
            var result = BigInteger.One;
            for (var i = 0; i < n; i++)
                result *= primes[i];
            return result;
        }

        public static int PrimorialUpTo(int n)
        {
            return (int)PrimorialUpTo((uint)n);
        }

        public static uint PrimorialUpTo(uint n)
        {
            var result = (uint)1;
            for (var i = (uint)0; (uint)primes[i] <= n; i++)
                result *= (uint)primes[i];
            return result;
        }

        public static long PrimorialUpTo(long n)
        {
            return (long)PrimorialUpTo((ulong)n);
        }

        public static ulong PrimorialUpTo(ulong n)
        {
            var result = (ulong)1;
            for (var i = (ulong)0; (ulong)primes[i] <= n; i++)
                result *= (ulong)primes[i];
            return result;
        }

        public static BigInteger PrimorialUpTo(BigInteger n)
        {
            return PrimorialUpToCore((int)n);
        }

        private static BigInteger PrimorialUpToCore(int n)
        {
            var result = BigInteger.One;
            for (var i = 0; primes[i] <= n; i++)
                result *= primes[i];
            return result;
        }
    }
}
