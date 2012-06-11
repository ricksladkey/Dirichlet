using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Decompose.Numerics
{
    public static partial class IntegerMath
    {
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

        public static int NumberOfDivisors(BigInteger n)
        {
            if (n < int.MaxValue)
                return NumberOfDivisors((int)n);
            throw new NotImplementedException();
        }

        public static int NumberOfDivisors(int n, int i)
        {
            if (i == 0)
                return 0;
            if (i == 1)
                return 1;
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
    }
}
