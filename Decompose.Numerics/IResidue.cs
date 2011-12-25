using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IResidue : IComparable<IResidue>
    {
        bool IsZero { get; }
        bool IsOne { get; }
        IResidue Set(IResidue x);
        IResidue Copy();
        IResidue Multiply(IResidue x);
        IResidue Add(IResidue x);
        IResidue Subtract(IResidue x);
        IResidue SetGreatestCommonDivisor(IResidue a, IResidue b);
        BigInteger ToBigInteger();
    }
}
