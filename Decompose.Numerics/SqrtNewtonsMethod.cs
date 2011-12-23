using System.Numerics;

namespace Decompose.Numerics
{
    public class SqrtNewtonsMethod : ISqrtAlgorithm
    {
        public BigInteger Sqrt(BigInteger n)
        {
            if (n < BigIntegerUtils.Two)
                return n;
            var x0 = n;
            var x1 = x0;
            do
            {
                x0 = x1;
                x1 = (x0 + n / x0) / BigIntegerUtils.Two;
            } while (x1 < x0);
            return x0;
        }
    }
}
