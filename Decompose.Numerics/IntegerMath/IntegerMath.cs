using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
        static IntegerMath()
        {
            CachePrimes();
            CreateSmallDivisorBatches();
            CreateModuliMap();
        }

        private static int[] primes;

        private static void CachePrimes()
        {
            primes = new SieveOfErostothones()
                .TakeWhile(p => p < 1000000)
                .ToArray();
        }

        public static uint TwosComplement(uint a)
        {
            return 0 - a;
        }

        public static ulong TwosComplement(ulong a)
        {
            return 0 - a;
        }

        public static int Abs(int a)
        {
            return a >= 0 ? a : -a;
        }

        public static uint Abs(uint a)
        {
            return a;
        }

        public static long Abs(long a)
        {
            return a >= 0 ? a : -a;
        }

        public static ulong Abs(ulong a)
        {
            return a;
        }

        public static BigInteger Abs(BigInteger a)
        {
            return a.Sign != -1 ? a : -a;
        }

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static uint Min(uint a, uint b)
        {
            return a < b ? a : b;
        }

        public static long Min(long a, long b)
        {
            return a < b ? a : b;
        }

        public static ulong Min(ulong a, ulong b)
        {
            return a < b ? a : b;
        }

        public static BigInteger Min(BigInteger a, BigInteger b)
        {
            return BigInteger.Min(a, b);
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static uint Max(uint a, uint b)
        {
            return a > b ? a : b;
        }

        public static long Max(long a, long b)
        {
            return a > b ? a : b;
        }

        public static ulong Max(ulong a, ulong b)
        {
            return a > b ? a : b;
        }

        public static BigInteger Max(BigInteger a, BigInteger b)
        {
            return BigInteger.Max(a, b);
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

        public static BigInteger Modulus(Rational n, BigInteger p)
        {
            if (n.IsInteger)
                return IntegerMath.Modulus((BigInteger)n, p);
            return IntegerMath.ModularQuotient(n.Numerator, n.Denominator, p);
        }

        public static bool IsSquareFree<T>(IEnumerable<T> factors)
        {
            return factors
                .OrderBy(factor => factor)
                .GroupBy(factor => factor)
                .All(grouping => grouping.Count() < 2);
        }

        private static IFactorizationAlgorithm<int> factorerInt = new TrialDivisionFactorization();

        public static int Mobius(int n)
        {
            var factors = factorerInt.Factor(n).ToArray();
            if (!IsSquareFree(factors))
                return 0;
            return factors.Length % 2 == 0 ? 1 : -1;
        }

        public static int Mobius(BigInteger n)
        {
            if (n < int.MaxValue)
                return Mobius((int)n);
            throw new NotImplementedException();
        }

        public static int NumberOfDivisors(int n)
        {
            return factorerInt.Factor(n)
                .GroupBy(p => p)
                .Select(grouping => grouping.Count() + 1)
                .Product();
        }

        public static int NumberOfDivisors(int n, int i)
        {
            return factorerInt.Factor(n)
                .GroupBy(p => p)
                .Select(grouping => grouping.Count())
                .Select(k => Binomial(k + i - 1, k))
                .Product();
        }

        public static BigInteger NumberOfDivisors(BigInteger n, int i)
        {
            if (n < int.MaxValue)
                return NumberOfDivisors((int)n, i);
            throw new NotImplementedException();
        }

        public static int SumOfNumberOfDivisors(int n, int i)
        {
            var sum = (int)0;
            for (var j = (int)1; j <= n; j++)
                sum += NumberOfDivisors(j, i);
            return sum;
        }

        public static BigInteger SumOfNumberOfDivisors(BigInteger n, int i)
        {
            var sum = (BigInteger)0;
            for (var j = (BigInteger)1; j <= n; j++)
                sum += NumberOfDivisors(j, i);
            return sum;
        }

#if false
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
#endif

        public static int GetDigitLength(BigInteger n, int b)
        {
            return (int)Math.Ceiling(BigInteger.Log(n, b));
        }

        public static bool IsPowerOfTwo(int a)
        {
            return (a & (a - 1)) == 0;
        }

        public static bool IsPowerOfTwo(uint a)
        {
            return (a & (a - 1)) == 0;
        }

        public static bool IsPowerOfTwo(long a)
        {
            return (a & (a - 1)) == 0;
        }

        public static bool IsPowerOfTwo(ulong a)
        {
            return (a & (a - 1)) == 0;
        }

        public static bool IsPowerOfTwo(BigInteger a)
        {
            return a.IsPowerOfTwo;
        }
    }
}
