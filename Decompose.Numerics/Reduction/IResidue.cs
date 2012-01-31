using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public interface IResidue<T> : IComparable<IResidue<T>>, IEquatable<IResidue<T>>
    {
        bool IsZero { get; }
        bool IsOne { get; }
        IResidue<T> Set(T x);
        IResidue<T> Set(IResidue<T> x);
        IResidue<T> Copy();
        IResidue<T> Multiply(IResidue<T> x);
        IResidue<T> Add(IResidue<T> x);
        IResidue<T> Subtract(IResidue<T> x);
        T Value();
    }
}
