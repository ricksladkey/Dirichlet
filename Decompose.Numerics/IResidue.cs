using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IResidue
    {
        IResidue Copy();
        IResidue Multiply(IResidue x);
        IResidue Add(IResidue x);
        BigInteger ToBigInteger();
    }
}
