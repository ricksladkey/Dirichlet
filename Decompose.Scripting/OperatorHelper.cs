using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Decompose.Scripting
{
    /// <summary>
    /// The OperatorHelper class is used to perform operations
    /// the same way that the C# language would.
    /// </summary>
    public static class OperatorHelper
    {
        public static int GetArity(Op op)
        {
            switch (op)
            {
                case Op.Not:
                case Op.Negate:
                case Op.OnesComplement:
                case Op.Int32:
                case Op.UInt32:
                case Op.Int64:
                case Op.UInt64:
                case Op.BigInteger:
                case Op.Double:
                case Op.Complex:
                case Op.Rational:
                case Op.Random:
                    return 1;
                case Op.New:
                case Op.NullCoalescing:
                    return 0;
                default:
                    return 2;
            }
        }

        public static int GetArity(AssignmentOp op)
        {
            switch (op)
            {
                case AssignmentOp.Increment:
                case AssignmentOp.Decrement:
                    return 1;
                default:
                    return 2;
            }
        }

        private static bool IsTypeTransitive(Op op)
        {
            switch (op)
            {
                case Op.Add:
                case Op.Subtract:
                case Op.Multiply:
                case Op.Remainder:
                case Op.Divide:
                case Op.AndAnd:
                case Op.OrOr:
                case Op.And:
                case Op.Or:
                case Op.ExclusiveOr:
                case Op.LeftShift:
                case Op.RightShift:
                    return true;
            }
            return false;
        }
    }
}
