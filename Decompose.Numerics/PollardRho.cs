using System.Numerics;
using System.Threading;

namespace Decompose.Numerics
{
    public class PollardRho : PollardRhoBase
    {
        const int iterations = 100;

        public PollardRho(int threads)
            : base(threads)
        {
        }

        protected override BigInteger Rho(BigInteger n, CancellationToken cancellationToken)
        {
            if (n % BigIntegerUtils.Two == 0)
                return BigIntegerUtils.Two;

            var c = random.Next(n - BigInteger.One) + BigInteger.One;
            var x = random.Next(n);
            var y = x;

            var divisor = BigInteger.One;
            var x0 = x;
            var xx0 = y;
            do
            {
                var z = BigInteger.One;
                x0 = x;
                xx0 = y;
                for (int i = 0; i < iterations; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return BigInteger.Zero;
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    z = z * (x - y) % n;
                }
                divisor = BigInteger.GreatestCommonDivisor(z, n);
            } while (divisor.IsOne);

            if (divisor == n)
            {
                x = x0;
                y = xx0;
                do
                {
                    if (cancellationToken.IsCancellationRequested)
                        return BigInteger.Zero;
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    divisor = BigInteger.GreatestCommonDivisor(y - x, n);
                } while (divisor.IsOne);
            }

            return divisor;
        }
    }
}
