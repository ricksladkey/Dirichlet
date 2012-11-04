using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Scripting
{
    public class BooleanOperatorMap : IOperatorMap
    {
        private Dictionary<Op, Func<bool, bool, bool>> binaryOps = new Dictionary<Op, Func<bool, bool, bool>>();

        public BooleanOperatorMap()
        {
            binaryOps.Add(Op.And, (a, b) => a & b);
            binaryOps.Add(Op.Or, (a, b) => a | b);
            binaryOps.Add(Op.ExclusiveOr, (a, b) => a ^ b);
            binaryOps.Add(Op.Equals, (a, b) => a == b);
            binaryOps.Add(Op.NotEquals, (a, b) => a != b);
        }

        public object Operator(Op op, params object[] args)
        {
            if (binaryOps.ContainsKey(op))
                return binaryOps[op]((bool)args[0], (bool)args[1]);
            if (op == Op.Not)
                return !(bool)args[0];
            throw new NotImplementedException();
        }
    }

}
