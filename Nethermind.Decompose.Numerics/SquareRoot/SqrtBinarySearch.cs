using System.Diagnostics;
using System.Numerics;

namespace Nethermind.Decompose.Numerics
{
    public class SqrtBinarySearch : ISqrtAlgorithm<BigInteger>
    {
        public BigInteger Sqrt(BigInteger n)
        {
            if (n < 2)
                return n;
            var m0 = (BigInteger)1;
            var m2 = n / 2 + 1;
            var n0 = m0 * m0;
            var n2 = m2 * m2;
            while (m2 - m0 > 1)
            {
                var m1 = (m0 + m2) / 2;
                var n1 = m1 * m1;
                if (n1 == n)
                    return m1;
                else if (n1 > n)
                {
                    m2 = m1;
                    n2 = n1;
                }
                else if (n1 < n)
                {
                    m0 = m1;
                    n0 = n1;
                }
            }
            Debug.Assert(m0 * m0 <= n && (m0 + 1) * (m0 + 1) > n);
            return m0;
        }

    }
}
