using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        public static uint TwosComplement(uint a)
        {
            return 0 - a;
        }

        public static ulong TwosComplement(ulong a)
        {
            return 0 - a;
        }

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static BigInteger Min(BigInteger a, BigInteger b)
        {
            return a < b ? a : b;
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static BigInteger Max(BigInteger a, BigInteger b)
        {
            return a > b ? a : b;
        }

        public static int QuotientFloor(int a, int b)
        {
            return a / b;
        }

        public static BigInteger QuotientFloor(BigInteger a, BigInteger b)
        {
            return a / b;
        }

        public static int QuotientCeiling(int a, int b)
        {
            return (a + b - 1) / b;
        }

        public static BigInteger QuotientCeiling(BigInteger a, BigInteger b)
        {
            return (a + b - 1) / b;
        }

        public static int MultipleOfFloor(int a, int b)
        {
            return a / b * b;
        }

        public static BigInteger MultipleOfFloor(BigInteger a, BigInteger b)
        {
            return a / b * b;
        }

        public static int MultipleOfCeiling(int a, int b)
        {
            return (a + b - 1) / b * b;
        }

        public static BigInteger MultipleOfCeiling(BigInteger a, BigInteger b)
        {
            return (a + b - 1) / b * b;
        }

        public static int Modulus(int n, int p)
        {
            return (n % p + p) % p;
        }

        public static uint Modulus(uint n, uint p)
        {
            return n % p;
        }

        public static long Modulus(long n, long p)
        {
            var result = n % p;
            if (result < 0)
                result += p;
            return result;
        }

        public static ulong Modulus(ulong n, ulong p)
        {
            return n % p;
        }

        public static int Modulus(BigInteger n, int p)
        {
            var result = (int)(n % p);
            if (result < 0)
                result += p;
            return result;
        }

        public static uint Modulus(BigInteger n, uint p)
        {
            var result = (long)(n % p);
            if (result < 0)
                result += p;
            return (uint)result;
        }

        public static long Modulus(BigInteger n, long p)
        {
            var result = (long)(n % p);
            if (result < 0)
                result += p;
            return result;
        }

        public static ulong Modulus(BigInteger n, ulong p)
        {
            if (n.Sign != -1)
                return (ulong)(n % p);
            var result = p - (ulong)(-n % p);
            if (result == p)
                result = 0;
            return result;
        }

        public static BigInteger Modulus(BigInteger n, BigInteger p)
        {
            var result = n % p;
            if (result.Sign == -1)
                result += p;
            return result;
        }

        public static bool IsSquareFree<T>(IEnumerable<T> factors)
        {
            return factors
                .OrderBy(factor => factor)
                .GroupBy(factor => factor)
                .All(grouping => grouping.Count() < 2);
        }

        public static int FloorSquareRoot(int n)
        {
            return (int)Math.Floor(Math.Sqrt(n));
        }

        public static uint FloorSquareRoot(uint n)
        {
            return (uint)Math.Floor(Math.Sqrt(n));
        }

        public static long FloorSquareRoot(long n)
        {
            return (long)Math.Floor(Math.Sqrt(n));
        }

        public static ulong FloorSquareRoot(ulong n)
        {
            return (ulong)Math.Floor(Math.Sqrt(n));
        }

        private static ISqrtAlgorithm<BigInteger> sqrt = new SqrtNewtonsMethod();

        public static BigInteger FloorSquareRoot(BigInteger n)
        {
            return sqrt.Sqrt(n);
        }

        public static int GetDigitLength(BigInteger n, int b)
        {
            return (int)Math.Ceiling(BigInteger.Log(n, b));
        }

        public static bool IsQuadraticResidue(BigInteger n, BigInteger p)
        {
            return ModularPower(n, (p - 1) / 2, p).IsOne;
        }
    }
}
