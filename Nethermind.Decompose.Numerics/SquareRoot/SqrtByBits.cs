using System.Diagnostics;
using System.Numerics;

namespace Nethermind.Decompose.Numerics
{
    public class SqrtByBits : ISqrtAlgorithm<BigInteger>
    {
        public BigInteger Sqrt(BigInteger n)
        {
            if (n.IsZero)
                return n;
            var k = (n.GetBitLength() - 1) / 2;
            var m = BigInteger.One << k;
            var mSquared = BigInteger.One << (2 * k);
            for (var i = k - 1; i >= 0; i--)
            {
                var mSquaredTest = mSquared + (m << (i + 1)) + (BigInteger.One << (2 * i));
                if (mSquaredTest <= n)
                {
                    m += BigInteger.One << i;
                    mSquared = mSquaredTest;
                }
            }
            return m;
        }
    }
}
