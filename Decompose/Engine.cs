using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Decompose.Numerics;

namespace Decompose
{
    public enum TraceFlags
    {
        Path,
    }

    public class Engine
    {
        private class OperatorMap<T>
        {
            private Dictionary<Op, Func<T, T, object>> binaryOps = new Dictionary<Op, Func<T, T, object>>();
            public OperatorMap()
            {
                var ops = Operations.Get<T>();
                binaryOps.Add(Op.Plus, (a, b) => ops.Add(a, b));
                binaryOps.Add(Op.Minus, (a, b) => ops.Subtract(a, b));
                binaryOps.Add(Op.Times, (a, b) => ops.Multiply(a, b));
                binaryOps.Add(Op.Divide, (a, b) => ops.Divide(a, b));
                binaryOps.Add(Op.Mod, (a, b) => ops.Modulus(a, b));
                binaryOps.Add(Op.BitwiseAnd, (a, b) => ops.And(a, b));
                binaryOps.Add(Op.BitwiseOr, (a, b) => ops.Or(a, b));
                binaryOps.Add(Op.BitwiseXor, (a, b) => ops.ExclusiveOr(a, b));
                binaryOps.Add(Op.LeftShift, (a, b) => ops.LeftShift(a, ops.ToInt32(b)));
                binaryOps.Add(Op.RightShift, (a, b) => ops.RightShift(a, ops.ToInt32(b)));
                binaryOps.Add(Op.Equals, (a, b) => ops.Equals(a, b));
                binaryOps.Add(Op.NotEquals, (a, b) => !ops.Equals(a, b));
                binaryOps.Add(Op.LessThan, (a, b) => ops.Compare(a, b) < 0);
                binaryOps.Add(Op.LessThanOrEqual, (a, b) => ops.Compare(a, b) <= 0);
                binaryOps.Add(Op.GreaterThan, (a, b) => ops.Compare(a, b) > 0);
                binaryOps.Add(Op.GreaterThanOrEqual, (a, b) => ops.Compare(a, b) >= 0);
            }
            public object Operator(Op op, params T[] args)
            {
                if (binaryOps.ContainsKey(op))
                    return binaryOps[op](args[0], args[1]);
                throw new NotImplementedException();
            }
        }

        public static string AttachedKey { get { return "@Attached"; } }
        public static string ContextKey { get { return "@Context"; } }
        public static string AssociatedObjectKey { get { return "@AssociatedObject"; } }
        public object Throw(string message) { throw new Exception(message); }
        public void Trace(TraceFlags flags, string message, params object[] args) { }

        public Engine()
        {
            SetVariable(ContextKey, new Dictionary<string, object>());
        }

        private OperatorMap<BigInteger> opsBigInteger = new OperatorMap<BigInteger>();

        public object Operator(Op op, params object[] args)
        {
            if (args[0] is BigInteger)
                return opsBigInteger.Operator(op, args.Select(arg => (BigInteger)arg).ToArray());
            throw new NotImplementedException();
        }

        public object Operator(AssignmentOp op, params object[] args)
        {
            return null;
        }

        private Dictionary<string, object> variables = new Dictionary<string, object>();

        public object GetVariable(string name)
        {
            return variables[name];
        }

        public object SetVariable(string name, object value)
        {
            return variables[name] = value;
        }

        public object GetProperty(object context, string name)
        {
            if (context is Dictionary<string, object>)
                return (context as Dictionary<string, object>)[name];
            throw new NotImplementedException();
        }

        public object SetProperty(object context, string name, object value)
        {
            if (context is Dictionary<string, object>)
                return (context as Dictionary<string, object>)[name] = value;
            throw new NotImplementedException();
        }
    }
}
