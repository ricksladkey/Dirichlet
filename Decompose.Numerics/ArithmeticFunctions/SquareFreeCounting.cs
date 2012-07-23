using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class SquareFreeCounting
    {
        private static BigInteger[] data10 = new BigInteger[]
        {
            BigInteger.Parse("1"),
            BigInteger.Parse("7"),
            BigInteger.Parse("61"),
            BigInteger.Parse("608"),
            BigInteger.Parse("6083"),
            BigInteger.Parse("60794"),
            BigInteger.Parse("607926"),
            BigInteger.Parse("6079291"),
            BigInteger.Parse("60792694"),
            BigInteger.Parse("607927124"),
            BigInteger.Parse("6079270942"),
            BigInteger.Parse("60792710280"),
            BigInteger.Parse("607927102274"),
            BigInteger.Parse("6079271018294"),
            BigInteger.Parse("60792710185947"),
            BigInteger.Parse("607927101854103"),
            BigInteger.Parse("6079271018540405"),
            BigInteger.Parse("60792710185403794"),
            BigInteger.Parse("607927101854022750"),
            BigInteger.Parse("6079271018540280875"),
            BigInteger.Parse("60792710185402613302"),
            BigInteger.Parse("607927101854026645617"),
            BigInteger.Parse("6079271018540266153468"),
            BigInteger.Parse("60792710185402662868753"),
            BigInteger.Parse("607927101854026628773299"),
            BigInteger.Parse("6079271018540266286424910"),
            BigInteger.Parse("60792710185402662866945299"),
            BigInteger.Parse("607927101854026628664226541"),
            BigInteger.Parse("6079271018540266286631251028"),
            BigInteger.Parse("60792710185402662866327383816"),
            BigInteger.Parse("607927101854026628663278087296"),
            BigInteger.Parse("6079271018540266286632795633943"),
            BigInteger.Parse("60792710185402662866327694188957"),
            BigInteger.Parse("607927101854026628663276901540346"),
            BigInteger.Parse("6079271018540266286632767883637220"),
            BigInteger.Parse("60792710185402662866327677953999263"),
            BigInteger.Parse("607927101854026628663276779463775476"),
        };

        public static BigInteger PowerOfTen(int n)
        {
            return data10[n];
        }

        private const long tmax = (long)1 << 62;
        private const long tmin = -tmax;

        private int threads;
        private BigInteger n;
        private BigInteger sum;
        private int sqrt;
        private MobiusCollection mobius;

        public SquareFreeCounting(int threads)
        {
            this.threads = threads;
        }

        public BigInteger Evaluate(BigInteger n)
        {
            this.n = n;
            if (n == 1)
                return 1;
            sum = 0;
            sqrt = (int)IntegerMath.FloorSquareRoot(n);
            mobius = new MobiusCollection(sqrt + 1, threads);
#if false
            EvaluateSlow(1, sqrt);
#else
            var x = S1(1, sqrt);
            EvaluateSlow(1, x);
#endif
            return sum;
        }

        private void EvaluateSlow(long x1, long x2)
        {
            for (var x = x1; x <= x2; x++)
            {
                var mu = mobius[(int)x];
                if (mu == 1)
                    sum += n / ((long)x * x);
                else if (mu == -1)
                    sum -= n / ((long)x * x);
            }
        }

        private long S1(long x1, long x2)
        {
            var s = (UInt128)n;
            var t = (long)0;
            var x = x2;
            var beta = (long)(n / (x + 1));
            var eps = (long)(n % (x + 1));
            var delta = (long)(n / x - beta);
            var gamma = (long)beta - x * delta;
            var alpha = beta / (x + 1);
            var alphax = (alpha + 1) * (x + 1);
            while (x >= x1)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += delta + delta;
                beta += delta;
                alphax -= alpha + 1;
                if (alphax <= beta)
                {
                    ++alpha;
                    alphax += x;
                    if (alphax <= beta)
                    {
                        ++alpha;
                        alphax += x;
                    }
                }

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 1));
                Debug.Assert(gamma == beta - (BigInteger)(x - 1) * delta);
                Debug.Assert(alpha == n / ((BigInteger)x * x));

                var mu = mobius[(int)x];
                if (mu == -1)
                    t -= alpha;
                else if (mu == 1)
                    t += alpha;
                if (t > tmax)
                {
                    s += (ulong)t;
                    t = 0;
                }
                else if (t < tmin)
                {
                    s -= (ulong)-t;
                    t = 0;
                }
                --x;
            }
            if (t > 0)
                s += (ulong)t;
            else if (t < 0)
                s -= (ulong)-t;
            AddToSum(s < n ? -(n - s) : (s - n));
            return x;
        }

        private void AddToSum(BigInteger s)
        {
            if (!s.IsZero)
            {
                lock (this)
                    sum += s;
            }
        }
    }
}
