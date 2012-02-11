using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Decompose
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
                case Op.BitwiseNot:
                    return 1;
                case Op.New:
                case Op.FirstNonNull:
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
                case Op.Plus:
                case Op.Minus:
                case Op.Times:
                case Op.Mod:
                case Op.Divide:
                case Op.AndAnd:
                case Op.OrOr:
                case Op.BitwiseAnd:
                case Op.BitwiseOr:
                case Op.BitwiseXor:
                case Op.LeftShift:
                case Op.RightShift:
                    return true;
            }
            return false;
        }
    }
}
