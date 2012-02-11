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
        private abstract class OperatorMap<T>
        {
            public abstract object Operator(Op op, params T[] args);
        }

        private class BooleanOperatorMap : OperatorMap<bool>
        {
            private Dictionary<Op, Func<bool, bool, bool>> binaryOps = new Dictionary<Op, Func<bool, bool, bool>>();
            public BooleanOperatorMap()
            {
                binaryOps.Add(Op.BitwiseAnd, (a, b) => a & b);
                binaryOps.Add(Op.BitwiseOr, (a, b) => a | b);
                binaryOps.Add(Op.BitwiseXor, (a, b) => a ^ b);
                binaryOps.Add(Op.Equals, (a, b) => a == b);
                binaryOps.Add(Op.NotEquals, (a, b) => a != b);
            }
            public override object Operator(Op op, params bool[] args)
            {
                if (binaryOps.ContainsKey(op))
                    return binaryOps[op](args[0], args[1]);
                throw new NotImplementedException();
            }
        }

        private class IntegerOperatorMap<T> : OperatorMap<T>
        {
            private Dictionary<Op, Func<T, T, object>> binaryOps = new Dictionary<Op, Func<T, T, object>>();
            public IntegerOperatorMap()
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
            public override object Operator(Op op, params T[] args)
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

        private OperatorMap<bool> opMapBoolean = new BooleanOperatorMap();
        private OperatorMap<int> opMapInt32 = new IntegerOperatorMap<int>();
        private OperatorMap<BigInteger> opMapBigInteger = new IntegerOperatorMap<BigInteger>();

        public object Operator(Op op, params object[] args)
        {
            if (args[0] is BigInteger || args[1] is BigInteger)
                return opMapBigInteger.Operator(op, args.Select(arg => ToBigInteger(arg)).ToArray());
            if (args[0] is int || args[1] is int)
                return opMapInt32.Operator(op, args.Select(arg => ToInt32(arg)).ToArray());
            if (args[0] is bool || args[1] is bool)
                return opMapBoolean.Operator(op, args.Select(arg => (bool)arg).ToArray());
            throw new NotImplementedException();
        }

        public int ToInt32(object value)
        {
            if (value is int)
                return (int)value;
            if (value is uint)
                return (int)(uint)value;
            if (value is long)
                return (int)(long)value;
            if (value is ulong)
                return (int)(ulong)value;
            if (value is BigInteger)
                return (int)(BigInteger)value;
            throw new NotImplementedException();
        }

        public BigInteger ToBigInteger(object value)
        {
            if (value is int)
                return (int)value;
            if (value is uint)
                return (uint)value;
            if (value is long)
                return (long)value;
            if (value is ulong)
                return (ulong)value;
            if (value is BigInteger)
                return (BigInteger)value;
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
