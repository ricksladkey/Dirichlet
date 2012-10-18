using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using Dirichlet.Numerics;

namespace Decompose.Numerics
{
    public class DivisorSummatoryFunctionOdd : IDivisorSummatoryFunction<BigInteger>
    {
        private IDivisorSummatoryFunction<ulong> hyperbolicSumUInt64;
        private IDivisorSummatoryFunction<UInt128> hyperbolicSumUInt128;
        private IDivisorSummatoryFunction<BigInteger> hyperbolicSumBigInteger;

        public DivisorSummatoryFunctionOdd(int threads, bool mod2)
        {
            hyperbolicSumUInt64 = new DivisorSummatoryFunctionOddUInt64(threads, mod2);
            hyperbolicSumUInt128 = new DivisorSummatoryFunctionOddUInt128(threads, mod2);
            hyperbolicSumBigInteger = new DivisorSummatoryFunctionOddBigInteger(threads);
        }

        #region IDivisorSummatoryFunction<BigInteger> Members

        public BigInteger Evaluate(BigInteger n)
        {
            if (n <= ((ulong)1 << 60))
                return hyperbolicSumUInt64.Evaluate((ulong)n);
            if (n <= UInt128.MaxValue)
                return hyperbolicSumUInt128.Evaluate((UInt128)n);
            return hyperbolicSumBigInteger.Evaluate(n);
        }

        public BigInteger Evaluate(BigInteger n, BigInteger x1, BigInteger x2)
        {
            if (n <= ((ulong)1 << 60))
                return hyperbolicSumUInt64.Evaluate((ulong)n, (ulong)x1, (ulong)x2);
            if (n <= UInt128.MaxValue)
                return hyperbolicSumUInt128.Evaluate((UInt128)n, (UInt128)x1, (UInt128)x2);
            return hyperbolicSumBigInteger.Evaluate(n);
        }

        #endregion
    }
}
