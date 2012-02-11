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
        private class OperatorMap<T> : Dictionary<Op, Func<T, T, T>>
        {
            public OperatorMap()
            {
                var ops = Operations.Get<T>();
                Add(Op.Plus, ops.Add);
                Add(Op.Minus, ops.Subtract);
                Add(Op.Times, ops.Multiply);
                Add(Op.Divide, ops.Divide);
                Add(Op.Mod, ops.Modulus);
            }
        }

        public static string AttachedKey { get { return "@Attached"; } }
        public static string ContextKey { get { return "@Context"; } }
        public static string AssociatedObjectKey { get { return "@AssociatedObject"; } }
        public object Throw(string message) { throw new Exception(message); }
        public void Trace(TraceFlags flags, string message, params object[] args) { }

        private OperatorMap<BigInteger> opsBigInteger = new OperatorMap<BigInteger>();

        public object Operator(Op op, params object[] args)
        {
            if (args[0] is BigInteger)
                return opsBigInteger[op]((BigInteger)args[0], (BigInteger)args[1]);
            return null;
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
    }
}
