using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class QuadraticSieve : IFactorizationAlgorithm<BigInteger>
    {
        protected MersenneTwister32 random = new MersenneTwister32(0);
        protected int threads;

        public QuadraticSieve(int threads)
        {
            this.threads = threads;
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            var factors = new List<BigInteger>();
            FactorCore(n, factors);
            return factors;
        }

        private void FactorCore(BigInteger n, List<BigInteger> factors)
        {
            if (n == 1)
                return;
            if (BigIntegerUtils.IsPrime(n))
            {
                factors.Add(n);
                return;
            }
            var divisor = GetDivisor(n);
            if (!divisor.IsZero)
            {
                FactorCore(divisor, factors);
                FactorCore(n / divisor, factors);
            }
        }

        private BigInteger GetDivisor(BigInteger n)
        {
            if (n.IsEven)
                return BigIntegerUtils.Two;
            var sqrtn = BigIntegerUtils.Sqrt(n);
            var factorBaseCandidates = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };
            var factorBase = factorBaseCandidates.Where(factor => BigIntegerUtils.JacobiSymbol(n, factor) == 1).ToArray();
            foreach (var p in factorBase)
            {
                var r1 = BigIntegerUtils.ModularSquareRoot(n, p);
                var r2 = p - r1;
                Console.WriteLine("MSR({0}, {1}) = ({2}, {3})", n, p, r1, r2);
            }
            int found = 0;
            int factorBaseSize = factorBase.Length;
            int desired = factorBase.Length + 1;
            int interval = 30 * 2;
            int interval2 = interval / 2;
            var matrix = new List<List<int>>();
            for (int i = 0; i <= factorBaseSize; i++)
                matrix.Add(new List<int>());
            var candidates = new List<BigInteger>();
            for (int k = -interval2; k <= interval2; k++)
            {
                var x = sqrtn + k;
                var y = x * x - n;
                var exponents = new int[factorBaseSize + 1];
                if (y < 0)
                {
                    exponents[0] = 1;
                    y = -y;
                }
                for (int i = 0; i < factorBaseSize; i++)
                {
                    var p = factorBase[i];
                    while (y % p == 0)
                    {
                        ++exponents[i + 1];
                        y /= p;
                    }
                }
                if (y.IsOne)
                {
                    candidates.Add(x);
                    for (int i = 0; i < exponents.Length; i++)
                        matrix[i].Add(exponents[i] % 2);
                    ++found;
                    if (found == desired)
                        break;
                }
            }
            for (int i = 0; i < matrix.Count; i++)
                Console.WriteLine(string.Join(" ", matrix[i].ToArray()));
            int rows = matrix.Count;
            int cols = found;
            var c = new List<int>();
            for (int i = 0; i < cols; i++)
                c.Add(-1);
            for (int k = 0; k < cols; k++)
            {
                int j = -1;
                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i][k] != 0 && c[i] < 0)
                    {
                        j = i;
                        break;
                    }
                }
                if (j != -1)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        if (i == j || matrix[i][k] == 0)
                            continue;
                        for (int l = 0; l < cols; l++)
                            matrix[i][l] ^= matrix[j][l];
                    }
                    c[j] = k;
                    Console.WriteLine("c[{0}] = {1}", j, c[j]);
                }
                else
                {
                    var v = new List<int>();
                    for (int i = 0; i < c.Count; i++)
                        v.Add(0);
                    for (int jj = 0; jj < c.Count; jj++)
                    {
                        int js = -1;
                        for (int s = 0; s < c.Count; s++)
                        {
                            if (c[s] == jj)
                            {
                                js = s;
                                break;
                            }
                        }
                        if (js != -1)
                            v[jj] = matrix[js][k];
                        else if (jj == k)
                            v[jj] = 1;
                        else
                            v[jj] = 0;
                    }
                    Console.WriteLine("v = {0}", string.Join(" ", v.ToArray()));
                    var xSet = candidates
                        .Zip(v, (x, exponent) => new { X = x, Exponent = exponent })
                        .Where(pair => pair.Exponent == 1)
                        .Select(pair => pair.X)
                        .ToArray();
                    var xPrime = xSet.Aggregate((sofar, current) => sofar * current) % n;
                    var yPrime = BigIntegerUtils.Sqrt(xSet
                        .Aggregate(BigInteger.One, (sofar, current) => sofar * (current * current - n))) % n;
                    var factor = BigInteger.GreatestCommonDivisor(xPrime + yPrime, n);
                    if (!factor.IsOne || factor == n)
                        return factor;
                }
                for (int i = 0; i < matrix.Count; i++)
                    Console.WriteLine(string.Join(" ", matrix[i].ToArray()));
                Console.WriteLine();
                var solution0 = new[] { 1, 1, 1, 0, 1, 0 };
                for (int ii = 0; ii < rows; ii++)
                {
                    int row = 0;
                    for (int jj = 0; jj < cols; jj++)
                    {
                        row ^= solution0[jj] * matrix[ii][jj];
                    }
                    if (row != 0)
                    {
                        Debugger.Break();
                    }
                }
            }
            return BigInteger.Zero;
        }
    }
}
