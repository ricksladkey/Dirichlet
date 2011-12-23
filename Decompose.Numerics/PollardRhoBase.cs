using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public abstract class PollardRhoBase : IFactorizationAlgorithm<BigInteger>
    {
        protected MersenneTwister32 random = new MersenneTwister32(0);
        protected int threads;

        protected PollardRhoBase(int threads)
        {
            this.threads = threads;
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            var factors = new ConcurrentQueue<BigInteger>();
            FactorInternal(n, factors);
            return factors;
        }

        private void FactorInternal(BigInteger n, ConcurrentQueue<BigInteger> factors)
        {
            if (n == 1)
                return;
            if (BigIntegerUtils.IsPrime(n))
            {
                factors.Enqueue(n);
                return;
            }
            var divisor = RhoParallel(n);
            FactorInternal(divisor, factors);
            FactorInternal(n / divisor, factors);
        }

        private BigInteger RhoParallel(BigInteger n)
        {
            if (threads == 1)
            {
                var xInit = random.Next(n);
                var c = random.Next(n - BigInteger.One) + BigInteger.One;
                return Rho(n, xInit, c, CancellationToken.None);
            }
            var cancellationToken = new CancellationTokenSource();
            var tasks = new Task<BigInteger>[threads];
            for (int i = 0; i < threads; i++)
            {
                var xInit = random.Next(n);
                var c = random.Next(n - BigInteger.One) + BigInteger.One;
                tasks[i] = Task.Factory.StartNew(() => Rho(n, xInit, c, cancellationToken.Token));
            }
            var index = Task.WaitAny(tasks);
            cancellationToken.Cancel();
            return tasks[index].Result;
        }

        protected static BigInteger F(BigInteger x, BigInteger c, BigInteger n)
        {
#if true
            return (x * x + c) % n;
#else
            return BigIntegerUtils.AddMod(BigInteger.ModPow(x, BigIntegerUtils.Two, n), c, n);
#endif
        }

        protected abstract BigInteger Rho(BigInteger n, BigInteger xInit, BigInteger c, CancellationToken cancellationToken);
    }
}
