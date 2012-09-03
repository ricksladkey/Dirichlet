using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class MertensFunctionWheel
    {
        private const long maximumSmallBatchSize = (long)1 << 18;
        private const long maximumBatchSize = (long)1 << 26;
        private const long tmax = (long)1 << 62;
        private const long tmin = -tmax;
        private const long C1 = 1;
        private const long C2 = 3;
        private const long C3 = 1;
        private const long C4 = 1;
        private const long C5 = 1;
        private const long C6 = 1;
        private const long C7 = 10;

        private int threads;
        private IArithmeticRange<sbyte, int> mobius;
        private BigInteger n;
        private long u;
        private int imax;
        private sbyte[] mu;
        private int[] m;
        private BigInteger[] mx;
        private int[] r;
        private long[] niSmall;
        private BigInteger[] niLarge;
        private int[][] bucketsSmall;
        private int[][] bucketsLarge;

#if false
        private const int wheelSize = 2;
        private const int wheelCount = (2 - 1);
#endif
#if false
        private const int wheelSize = 2 * 3;
        private const int wheelCount = (2 - 1) * (3 - 1);
#endif
#if false
        private const int wheelSize = 2 * 3 * 5;
        private const int wheelCount = (2 -1) * (3 - 1) * (5 - 1);
#endif
#if false
        private const int wheelSize = 2 * 3 * 5 * 7;
        private const int wheelCount = (2 -1) * (3 - 1) * (5 - 1) * (7 - 1);
#endif
#if false
        private const int wheelSize = 2 * 3 * 5 * 7 * 11;
        private const int wheelCount = (2 -1) * (3 - 1) * (5 - 1) * (7 - 1) * (11 - 1);
#endif
#if false
        private const int wheelSize = 2 * 3 * 5 * 7 * 11 * 13;
        private const int wheelCount = (2 -1) * (3 - 1) * (5 - 1) * (7 - 1) * (11 - 1) * (13 - 1);
#endif
#if true
        private const int wheelSize = 2 * 3 * 5 * 7 * 11 * 13 * 17;
        private const int wheelCount = (2 - 1) * (3 - 1) * (5 - 1) * (7 - 1) * (11 - 1) * (13 - 1) * (17 - 1);
#endif
#if false
        private const int wheelSize = 2 * 3 * 5 * 7 * 11 * 13 * 17 * 19;
        private const int wheelCount = (2 - 1) * (3 - 1) * (5 - 1) * (7 - 1) * (11 - 1) * (13 - 1) * (17 - 1) * (19 - 1);
#endif
        private const int wheelSize2 = wheelSize >> 1;

        private int[] wheelSubtotal;
        private bool[] wheelInclude;
        private int[] wheelNext;

        public MertensFunctionWheel(int threads)
        {
            this.threads = threads;
            wheelSubtotal = new int[wheelSize];
            wheelInclude = new bool[wheelSize >> 1];
            wheelNext = new int[wheelSize >> 1];
            var total = 0;
            var first = -1;
            for (var i = 0; i < wheelSize; i++)
            {
                var include = IntegerMath.GreatestCommonDivisor(i, wheelSize) == 1;
                if (include)
                {
                    if (first == -1)
                        first = i;
                    ++total;
                }
                wheelSubtotal[i] = total;
                wheelInclude[i >> 1] = include;
            }
            Debug.Assert(total == wheelCount);
            var next = first - 1;
            for (var i = (wheelSize >> 1) - 1; i >= 0; i--)
            {
                next += 2;
                wheelNext[i] = next;
                if (wheelInclude[i])
                    next = 0;
            }
        }

        public BigInteger Evaluate(BigInteger n)
        {
            if (n <= 0)
                return 0;

            this.n = n;
            u = (long)IntegerMath.Max(IntegerMath.FloorPower(n, 2, 3) * C1 / C2, IntegerMath.CeilingSquareRoot(n));

            if (u <= wheelSize)
                return new MertensFunctionDR(threads).Evaluate((long)n);

            imax = (int)(n / u);
            mobius = new MobiusRangeAdditive(u + 1, threads);
            var batchSize = Math.Min(u, maximumBatchSize);
            mu = new sbyte[maximumSmallBatchSize];
            m = new int[batchSize];
            mx = new BigInteger[imax + 1];
            r = new int[imax + 1];
            var lmax = 0;
            for (var i = 1; i <= imax; i += 2)
            {
                if (wheelInclude[(i % wheelSize) >> 1])
                    r[lmax++] = i;
            }
            Array.Resize(ref r, lmax);

            niLarge = new BigInteger[imax + 1];
            niSmall = new long[imax + 1];
            var buckets = Math.Max(1, threads);
            var costs = new double[buckets];
            var bucketListsLarge = new List<int>[buckets];
            for (var bucket = 0; bucket < buckets; bucket++)
                bucketListsLarge[bucket] = new List<int>();
            var bucketListsSmall = new List<int>[buckets];
            for (var bucket = 0; bucket < buckets; bucket++)
                bucketListsSmall[bucket] = new List<int>();
            for (var l = 0; l < lmax; l++)
            {
                var i = r[l];
                var ni = n / (uint)i;
                var large = ni > long.MaxValue;
                var cost = Math.Sqrt((double)n / i) * (large ? C7 : 1);
                var addto = 0;
                var mincost = costs[0];
                for (var bucket = 0; bucket < buckets; bucket++)
                {
                    if (costs[bucket] < mincost)
                    {
                        mincost = costs[bucket];
                        addto = bucket;
                    }
                }
                if (large)
                {
                    niLarge[i] = ni;
                    bucketListsLarge[addto].Add(i);
                }
                else
                {
                    niSmall[i] = (long)ni;
                    bucketListsSmall[addto].Add(i);
                }
                costs[addto] += cost;
            }
            bucketsLarge = bucketListsLarge.Select(bucket => bucket.ToArray()).ToArray();
            bucketsSmall = bucketListsSmall.Select(bucket => bucket.ToArray()).ToArray();

            var m0 = 0;
            var xmed = Math.Min((long)IntegerMath.FloorRoot(n, 2) * C5 / C6, u);
            for (var x = (long)1; x <= xmed; x += maximumSmallBatchSize)
            {
                var xstart = x;
                var xend = Math.Min(xstart + maximumSmallBatchSize - 1, xmed);
                m0 = mobius.GetValuesAndSums(xstart, xend + 1, mu, m, m0);
                ProcessBatch(xstart, xend);
            }
            for (var x = xmed + 1; x <= u; x += maximumBatchSize)
            {
                var xstart = x;
                var xend = Math.Min(xstart + maximumBatchSize - 1, u);
                m0 = mobius.GetSums(xstart, xend + 1, m, m0);
                ProcessBatch(xstart, xend);
            }
            ComputeMx();
            return mx[1];
        }

        private void ProcessBatch(long x1, long x2)
        {
            if (threads <= 1)
                UpdateMx(x1, x2, 0);
            else
            {
                var tasks = new Task[threads];
                for (var thread = 0; thread < threads; thread++)
                {
                    var bucket = thread;
                    tasks[thread] = Task.Factory.StartNew(() => UpdateMx(x1, x2, bucket));
                }
                Task.WaitAll(tasks);
            }
        }

        private void UpdateMx(long x1, long x2, int bucket)
        {
            UpdateMxLarge(x1, x2, bucketsLarge[bucket]);
            UpdateMxSmall(x1, x2, bucketsSmall[bucket]);
        }

        private void UpdateMxLarge(long x1, long x2, int[] r)
        {
            for (var l = 0; l < r.Length; l++)
            {
                var i = r[l];
                var x = niLarge[i];
                var sqrt = (long)IntegerMath.FloorSquareRoot(x);
                var xover = (long)IntegerMath.Min(sqrt * C3 / C4, x);
                xover = (long)(x / (x / (ulong)xover));
                var s = (BigInteger)0;

                var jmin = UpToOdd(IntegerMath.Max(imax / i + 1, (long)IntegerMath.Min(xover + 1, x / ((ulong)x2 + 1) + 1)));
                var jmax = DownToOdd((long)IntegerMath.Min(xover, x / (ulong)x1));
                s += JSum1(x, jmin, ref jmax, x1);
                s += JSum2(x, jmin, jmax, x1);

                var kmin = IntegerMath.Max(1, x1);
                var kmax = (long)IntegerMath.Min(x / (ulong)xover - 1, x2);
                //s += KSum1M(x, kmin, ref kmax, x1);
                s += KSum1Mu(x, kmin, ref kmax, x1);
                s += KSum2(x, kmin, kmax, x1);

                mx[i] -= s;
            }
        }

        private void UpdateMxSmall(long x1, long x2, int[] r)
        {
            for (var l = 0; l < r.Length; l++)
            {
                var i = r[l];
                var x = niSmall[i];
                var sqrt = IntegerMath.FloorSquareRoot(x);
                var xover = Math.Min(sqrt * C3 / C4, x);
                xover = x / (x / xover);
                var s = (long)0;

                var jmin = UpToOdd(Math.Max(imax / i + 1, x / (x2 + 1) + 1));
                var jmax = DownToOdd(Math.Min(xover, x / x1));
                //s += JSum1(x, jmin, ref jmax, x1);
                s += JSum2(x, jmin, jmax, x1);

                var kmin = Math.Max(1, x1);
                var kmax = Math.Min(x / xover - 1, x2);
                s += KSum1(x, kmin, ref kmax, x1);
                s += KSum2(x, kmin, kmax, x1);

                mx[i] -= s;
            }
        }

        private long JSum2(long x, long jmin, long jmax, long x1)
        {
            var s = (long)0;
            var j = UpToWheel(jmin);
            var mod = j % wheelSize;
            while (j <= jmax)
            {
                s += m[x / j - x1];
                var skip = wheelNext[mod >> 1];
                j += skip;
                mod += skip;
                if (mod >= wheelSize)
                    mod -= wheelSize;
            }
            return s;
        }

        private long KSum2(long x, long kmin, long kmax, long x1)
        {
            var s = (long)0;
            var current = T1Wheel(x / kmin);
            for (var k = kmin; k <= kmax; k++)
            {
                var next = T1Wheel(x / (k + 1));
                s += (current - next) * m[k - x1];
                current = next;
            }
            return s;
        }

        private long JSum1(long x, long j1, ref long j, long offset)
        {
            var s = (long)0;
            var beta = x / (j + 2);
            var eps = x % (j + 2);
            var delta = x / j - beta;
            var gamma = 2 * beta - j * delta;
            var mod = (j % wheelSize) >> 1;
            while (j >= j1)
            {
                eps += gamma;
                if (eps >= j)
                {
                    ++delta;
                    gamma -= j;
                    eps -= j;
                    if (eps >= j)
                    {
                        ++delta;
                        gamma -= j;
                        eps -= j;
                        if (eps >= j)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += j;
                    eps += j;
                }
                gamma += 4 * delta;
                beta += delta;

                Debug.Assert(eps == x % j);
                Debug.Assert(beta == x / j);
                Debug.Assert(delta == beta - x / (j + 2));
                Debug.Assert(gamma == 2 * beta - (BigInteger)(j - 2) * delta);

                if (wheelInclude[mod])
                    s += m[beta - offset];
                if (--mod < 0)
                    mod += wheelSize2;
                j -= 2;
            }
            return s;
        }

        private long KSum1(long x, long k1, ref long k2, long offset)
        {
            var k = k2;
            if (k == 0 || k < k1)
                return 0;
            var s = (long)0;
            var beta = x / (k + 1);
            var eps = x % (k + 1);
            var delta = x / k - beta;
            var gamma = beta - k * delta;
            var firstBeta = beta;
            var betaOffset = beta / wheelSize * wheelCount;
            beta %= wheelSize;
            while (k >= k1)
            {
                eps += gamma;
                if (eps >= k)
                {
                    ++delta;
                    gamma -= k;
                    eps -= k;
                    if (eps >= k)
                    {
                        ++delta;
                        gamma -= k;
                        eps -= k;
                        if (eps >= k)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += k;
                    eps += k;
                }
                gamma += delta << 1;
                beta += delta;
                if (beta >= wheelSize)
                {
                    betaOffset += wheelCount;
                    beta -= wheelSize;
                }
                var wheel = betaOffset + wheelSubtotal[beta];

                Debug.Assert(eps == x % k);
                Debug.Assert(beta == x / k % wheelSize);
                Debug.Assert(delta == x / k - x / (k + 1));
                Debug.Assert(gamma == x / k - (BigInteger)(k - 1) * delta);
                Debug.Assert(betaOffset == x / k / wheelSize * wheelCount);
                Debug.Assert(wheel == T1Wheel(x / k));

#if true
                s += wheel * mu[k - offset];
#endif
#if false
                s += T1Wheel(beta) * mu[k - offset];
#endif
#if false
                var value = (int)mu[k - offset];
                var flip = value >> 31;
                s += ((T1Wheel(beta) ^ flip) - flip) & (value << 31 >> 31);
#endif
#if false
                var value = mu[k - offset];
                if (value != 0)
                {
                    var flip = value >> 31;
                    s += (T1Wheel(beta) ^ flip) - flip;
                }
#endif
                --k;
            }
            s -= T1Wheel(firstBeta) * m[k2 - offset];
            s += (betaOffset + wheelSubtotal[beta]) * (m[k + 1 - offset] - mu[k + 1 - offset]);
            k2 = k;
            return s;
        }

        private long JSum2(BigInteger x, long jmin, long jmax, long x1)
        {
            var s = (long)0;
            var j = UpToWheel(jmin);
            var mod = j % wheelSize;
            var xRep = (UInt128)x;
            while (j <= jmax)
            {
                s += m[(int)(xRep / (ulong)j - (ulong)x1)];
                var skip = wheelNext[mod >> 1];
                j += skip;
                mod += skip;
                if (mod >= wheelSize)
                    mod -= wheelSize;
            }
            return s;
        }

        private BigInteger KSum2(BigInteger x, long kmin, long kmax, long x1)
        {
            var s = (BigInteger)0;
            var current = T1Wheel(x / kmin);
            for (var k = kmin; k <= kmax; k++)
            {
                var next = T1Wheel(x / (ulong)(k + 1));
                s += (current - next) * m[k - x1];
                current = next;
            }
            return s;
        }

        private BigInteger JSum1(BigInteger x, long j1, ref long j, long offset)
        {
            var s = (long)0;
            var beta = (long)(x / ((ulong)j + 2));
            var eps = (long)(x % ((ulong)j + 2));
            var delta = (long)(x / (ulong)j) - beta;
            var gamma = 2 * beta - j * delta;
            var mod = (j % wheelSize) >> 1;
            while (j >= j1)
            {
                eps += gamma;
                if (eps >= j)
                {
                    ++delta;
                    gamma -= j;
                    eps -= j;
                    if (eps >= j)
                    {
                        ++delta;
                        gamma -= j;
                        eps -= j;
                        if (eps >= j)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += j;
                    eps += j;
                }
                gamma += 4 * delta;
                beta += delta;

                Debug.Assert(eps == (BigInteger)x % j);
                Debug.Assert(beta == (BigInteger)x / j);
                Debug.Assert(delta == beta - (BigInteger)x / (j + 2));
                Debug.Assert(gamma == 2 * beta - (BigInteger)(j - 2) * delta);

                if (wheelInclude[mod])
                    s += m[beta - offset];
                if (--mod < 0)
                    mod += wheelSize2;
                j -= 2;
            }
            return s;
        }

        private long KSum1M(long x, long k1, ref long k, long offset)
        {
            if (k == 0)
                return 0;
            var s = (long)0;
            var beta = x / (k + 1);
            var eps = x % (k + 1);
            var delta = x / k - beta;
            var gamma = beta - k * delta;
            var lastCount = T1Wheel(beta);
            while (k >= k1)
            {
                eps += gamma;
                if (eps >= k)
                {
                    ++delta;
                    gamma -= k;
                    eps -= k;
                    if (eps >= k)
                    {
                        ++delta;
                        gamma -= k;
                        eps -= k;
                        if (eps >= k)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += k;
                    eps += k;
                }
                gamma += delta << 1;
                beta += delta;

                Debug.Assert(eps == x % k);
                Debug.Assert(beta == x / k);
                Debug.Assert(delta == beta - x / (k + 1));
                Debug.Assert(gamma == beta - (BigInteger)(k - 1) * delta);

                var count = T1Wheel(beta);
                s += (count - lastCount) * m[k - offset];
                lastCount = count;
                --k;
            }
            return s;
        }

        private BigInteger KSum1Mu(BigInteger x, long k1, ref long k, long offset)
        {
            if (k == 0)
                return 0;
            var s = (BigInteger)0;
            var t = (long)0;
            var beta = (long)(x / (k + 1));
            var eps = (long)(x % (k + 1));
            var delta = (long)(x / k - beta);
            var gamma = (long)(beta - k * delta);
            var lastCount = T1Wheel(beta);
            while (k >= k1)
            {
                eps += gamma;
                if (eps >= k)
                {
                    ++delta;
                    gamma -= k;
                    eps -= k;
                    if (eps >= k)
                    {
                        ++delta;
                        gamma -= k;
                        eps -= k;
                        if (eps >= k)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += k;
                    eps += k;
                }
                gamma += delta << 1;
                beta += delta;

                Debug.Assert(eps == (BigInteger)x % k);
                Debug.Assert(beta == (BigInteger)x / k);
                Debug.Assert(delta == beta - (BigInteger)x / (k + 1));
                Debug.Assert(gamma == beta - (BigInteger)(k - 1) * delta);

                var count = T1Wheel(beta);
                t += (count - lastCount) * m[k - offset];
                if (t > tmax || t < tmin)
                {
                    s += t;
                    t = 0;
                }
                lastCount = count;
                --k;
            }
            s += t;
            return s;
        }

        private void ComputeMx()
        {
            for (var l = r.Length - 1; l >= 0; l--)
            {
                var i = r[l];
                var s = (BigInteger)0;
                for (var ij = 2 * i; ij <= imax; ij += i)
                    s += mx[ij];
                mx[i] -= s;
            }
        }

        private long UpToOdd(long a)
        {
            return a | 1;
        }

        private long DownToOdd(long a)
        {
            return (a - 1) | 1;
        }

        private long UpToWheel(long a)
        {
            var b = (a % wheelSize) >> 1;
            return wheelInclude[b] ? a : a + wheelNext[b];
        }

        private long T1Wheel(long a)
        {
            var b = a / wheelSize;
            var c = (int)(a - b * wheelSize);
            return wheelCount * b + wheelSubtotal[c];
        }

        private BigInteger T1Wheel(BigInteger a)
        {
            var b = a / wheelSize;
            var c = (int)(a - b * wheelSize);
            return (uint)wheelCount * b + (uint)wheelSubtotal[c];
        }
    }
}
