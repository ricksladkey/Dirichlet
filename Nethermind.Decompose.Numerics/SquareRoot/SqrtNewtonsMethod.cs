using System.Diagnostics;
using System.Numerics;

namespace Nethermind.Decompose.Numerics
{
    public class SqrtNewtonsMethod : ISqrtAlgorithm<BigInteger>
    {
        public BigInteger Sqrt(BigInteger n)
        {
            if (n < 2)
                return n;
            var x0 = BigInteger.One << (n.GetBitLength() / 2 + 1);
            var x1 = x0;
            do
            {
                x0 = x1;
                x1 = (x0 + n / x0) >> 1;
            }
            while (x1 < x0);
            Debug.Assert(x0 * x0 <= n && (x0 + 1) * (x0 + 1) > n);
            return x0;
        }
    }
}
