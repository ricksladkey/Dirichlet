using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose
{
    /// <summary>
    /// An Op represents the corresponding C# operator
    /// or method.
    /// </summary>
    public enum Op
    {
        Plus = 1,
        Minus,
        Times,
        Mod,
        Divide,
        Negate,

        AndAnd,
        OrOr,

        Not,

        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        BitwiseNot,
        LeftShift,
        RightShift,

        Equals,
        NotEquals,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,

        Int32,
        UInt32,
        Int64,
        UInt64,
        BigInteger,

        Comma,
        Conditional,
        FirstNonNull,
        As,
        Is,
        New,
    }

    /// <summary>
    /// The OperatorExtensions class extends the Operator
    /// enumeration to to include methods to perform those operations.
    /// </summary>
    public static class OperatorExtensions
    {
        public static int GetArity(this Op op)
        {
            return OperatorHelper.GetArity(op);
        }
    }
}
