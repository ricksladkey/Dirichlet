using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IResidue<TInteger> : IComparable<IResidue<TInteger>>, IEquatable<IResidue<TInteger>>
    {
        bool IsZero { get; }
        bool IsOne { get; }
        IResidue<TInteger> Set(TInteger x);
        IResidue<TInteger> Set(IResidue<TInteger> x);
        IResidue<TInteger> Copy();
        IResidue<TInteger> Multiply(IResidue<TInteger> x);
        IResidue<TInteger> Add(IResidue<TInteger> x);
        IResidue<TInteger> Subtract(IResidue<TInteger> x);
        TInteger ToInteger();
    }
}
