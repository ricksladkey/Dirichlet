using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public abstract class PollardRhoBase : IFactorizationAlgorithm
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
                return Rho(n, CancellationToken.None);
            var cancellationToken = new CancellationTokenSource();
            var tasks = new Task<BigInteger>[threads];
            for (int i = 0; i < threads; i++)
                tasks[i] = Task.Factory.StartNew(() => Rho(n, cancellationToken.Token));
            var index = Task.WaitAny(tasks);
            cancellationToken.Cancel();
            return tasks[index].Result;
        }

        protected abstract BigInteger Rho(BigInteger n, CancellationToken cancellationToken);
    }
}
