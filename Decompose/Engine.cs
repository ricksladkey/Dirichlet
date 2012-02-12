using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
                binaryOps.Add(Op.And, (a, b) => a & b);
                binaryOps.Add(Op.Or, (a, b) => a | b);
                binaryOps.Add(Op.ExclusiveOr, (a, b) => a ^ b);
                binaryOps.Add(Op.Equals, (a, b) => a == b);
                binaryOps.Add(Op.NotEquals, (a, b) => a != b);
            }
            public override object Operator(Op op, params bool[] args)
            {
                if (binaryOps.ContainsKey(op))
                    return binaryOps[op](args[0], args[1]);
                if (op == Op.Not)
                    return !args[0];
                throw new NotImplementedException();
            }
        }

        private class IntegerOperatorMap<T> : OperatorMap<T>
        {
            private Dictionary<Op, Func<T, object>> unaryOps = new Dictionary<Op, Func<T, object>>();
            private Dictionary<Op, Func<T, T, object>> binaryOps = new Dictionary<Op, Func<T, T, object>>();
            private Dictionary<Op, Func<T, T, T, object>> ternaryOps = new Dictionary<Op, Func<T, T, T, object>>();
            public IntegerOperatorMap(IRandomNumberGenerator generator)
            {
                var ops = Operations.Get<T>();
                var rand = generator.Create<T>();
                unaryOps.Add(Op.Negate, a => ops.Negate(a));
                unaryOps.Add(Op.OnesComplement, a => ops.OnesComplement(a));
                unaryOps.Add(Op.Random, a => rand.Next(a));
                binaryOps.Add(Op.Plus, (a, b) => ops.Add(a, b));
                binaryOps.Add(Op.Minus, (a, b) => ops.Subtract(a, b));
                binaryOps.Add(Op.Times, (a, b) => ops.Multiply(a, b));
                binaryOps.Add(Op.Divide, (a, b) => ops.Divide(a, b));
                binaryOps.Add(Op.Mod, (a, b) => ops.Modulus(a, b));
                binaryOps.Add(Op.Power, (a, b) => ops.Power(a, b));
                binaryOps.Add(Op.And, (a, b) => ops.And(a, b));
                binaryOps.Add(Op.Or, (a, b) => ops.Or(a, b));
                binaryOps.Add(Op.ExclusiveOr, (a, b) => ops.ExclusiveOr(a, b));
                binaryOps.Add(Op.LeftShift, (a, b) => ops.LeftShift(a, ops.ToInt32(b)));
                binaryOps.Add(Op.RightShift, (a, b) => ops.RightShift(a, ops.ToInt32(b)));
                binaryOps.Add(Op.Equals, (a, b) => ops.Equals(a, b));
                binaryOps.Add(Op.NotEquals, (a, b) => !ops.Equals(a, b));
                binaryOps.Add(Op.LessThan, (a, b) => ops.Compare(a, b) < 0);
                binaryOps.Add(Op.LessThanOrEqual, (a, b) => ops.Compare(a, b) <= 0);
                binaryOps.Add(Op.GreaterThan, (a, b) => ops.Compare(a, b) > 0);
                binaryOps.Add(Op.GreaterThanOrEqual, (a, b) => ops.Compare(a, b) >= 0);
                binaryOps.Add(Op.Modulo, (a, b) =>
                    {
                        var result = ops.Modulus(a, b);
                        if (ops.Compare(result, ops.Zero) < 0)
                            result = ops.Add(result, b);
                        return result;
                    });
                binaryOps.Add(Op.GreatestCommonDivisor, (a, b) => ops.GreatestCommonDivisor(a, b));
                binaryOps.Add(Op.Divides, (a, b) => ops.IsZero(ops.Modulus(b, a)));
                binaryOps.Add(Op.NotDivides, (a, b) => !ops.IsZero(ops.Modulus(b, a)));
                binaryOps.Add(Op.ModularNegate, (a, b) => ops.ModularDifference(ops.Zero, a, b));
                ternaryOps.Add(Op.ModularSum, (a, b, c) => ops.ModularSum(a, b, c));
                ternaryOps.Add(Op.ModularDifference, (a, b, c) => ops.ModularDifference(a, b, c));
                ternaryOps.Add(Op.ModularProduct, (a, b, c) => ops.ModularProduct(a, b, c));
                ternaryOps.Add(Op.ModularQuotient, (a, b, c) => ops.ModularProduct(a, ops.ModularInverse(b, c), c));
                ternaryOps.Add(Op.ModularPower, (a, b, c) =>
                    {
                        if (ops.Equals(b, ops.Negate(ops.One)))
                        {
                            if (!ops.GreatestCommonDivisor(a, c).Equals(ops.One))
                                throw new InvalidOperationException("not relatively prime");
                            return ops.ModularInverse(a, c);
                        }
                        return ops.ModularPower(a, b, c);
                    });
            }
            public override object Operator(Op op, params T[] args)
            {
                if (unaryOps.ContainsKey(op))
                    return unaryOps[op](args[0]);
                if (binaryOps.ContainsKey(op))
                    return binaryOps[op](args[0], args[1]);
                if (ternaryOps.ContainsKey(op))
                    return ternaryOps[op](args[0], args[1], args[2]);
                throw new NotImplementedException();
            }
        }

        public static string AttachedKey { get { return "@Attached"; } }
        public static string ContextKey { get { return "@Context"; } }
        public static string AssociatedObjectKey { get { return "@AssociatedObject"; } }
        public object Throw(string message) { throw new Exception(message); }
        public void Trace(TraceFlags flags, string message, params object[] args) { }

        private object globalContext;
        private RandomNumberGenerator generator;
        private OperatorMap<bool> opMapBoolean;
        private OperatorMap<int> opMapInt32;
        private OperatorMap<uint> opMapUInt32;
        private OperatorMap<long> opMapInt64;
        private OperatorMap<ulong> opMapUInt64;
        private OperatorMap<BigInteger> opMapBigInteger;
        private Dictionary<string, object> variables;
        private Dictionary<string, Func<object[], object>> globalMethods;

        public Engine()
        {
            globalContext = new Dictionary<string, object>();
            generator = new MersenneTwister(0);
            opMapBoolean = new BooleanOperatorMap();
            opMapInt32 = new IntegerOperatorMap<int>(generator);
            opMapUInt32 = new IntegerOperatorMap<uint>(generator);
            opMapInt64 = new IntegerOperatorMap<long>(generator);
            opMapUInt64 = new IntegerOperatorMap<ulong>(generator);
            opMapBigInteger = new IntegerOperatorMap<BigInteger>(generator);
            variables = new Dictionary<string, object>();
            globalMethods = new Dictionary<string, Func<object[], object>>();

            SetVariable(ContextKey, globalContext);
            AddGlobalMethods();
        }

        private Dictionary<Op, Func<object, object>> opMapCast = new Dictionary<Op, Func<object, object>>
        {
            { Op.Int32, a => ToInt32(a) },
            { Op.UInt32, a => ToUInt32(a) },
            { Op.Int64, a => ToInt64(a) },
            { Op.UInt64, a => ToUInt64(a) },
            { Op.BigInteger, a => ToBigInteger(a) },
        };


        public object Operator(Op op, params object[] args)
        {
            if (opMapCast.ContainsKey(op))
                return opMapCast[op](args[0]);
            var n = args.Length;
            if (args[0] is BigInteger || n >= 2 && args[1] is BigInteger)
                return opMapBigInteger.Operator(op, args.Select(arg => ToBigInteger(arg)).ToArray());
            if (args[0] is ulong || n >= 2 && args[1] is ulong)
                return opMapUInt64.Operator(op, args.Select(arg => ToUInt64(arg)).ToArray());
            if (args[0] is long || n >= 2 && args[1] is long)
                return opMapInt64.Operator(op, args.Select(arg => ToInt64(arg)).ToArray());
            if (args[0] is uint || n >= 2 && args[1] is uint)
                return opMapUInt32.Operator(op, args.Select(arg => ToUInt32(arg)).ToArray());
            if (args[0] is int || n >= 2 && args[1] is int)
                return opMapInt32.Operator(op, args.Select(arg => ToInt32(arg)).ToArray());
            if (args[0] is bool || n >= 2 && args[1] is bool)
                return opMapBoolean.Operator(op, args.Select(arg => (bool)arg).ToArray());
            throw new NotImplementedException();
        }

        public static BigInteger ToBigInteger(object value)
        {
            if (value is int)
                return (BigInteger)(int)value;
            if (value is uint)
                return (BigInteger)(uint)value;
            if (value is long)
                return (BigInteger)(long)value;
            if (value is ulong)
                return (BigInteger)(ulong)value;
            if (value is BigInteger)
                return (BigInteger)(BigInteger)value;
            throw new NotImplementedException();
        }

        public static int ToInt32(object value)
        {
            if (value is int)
                return (int)(int)value;
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

        public static uint ToUInt32(object value)
        {
            if (value is int)
                return (uint)(int)value;
            if (value is uint)
                return (uint)(uint)value;
            if (value is long)
                return (uint)(long)value;
            if (value is ulong)
                return (uint)(ulong)value;
            if (value is BigInteger)
                return (uint)(BigInteger)value;
            throw new NotImplementedException();
        }

        public static long ToInt64(object value)
        {
            if (value is int)
                return (long)(int)value;
            if (value is uint)
                return (long)(uint)value;
            if (value is long)
                return (long)(long)value;
            if (value is ulong)
                return (long)(ulong)value;
            if (value is BigInteger)
                return (long)(BigInteger)value;
            throw new NotImplementedException();
        }

        public static ulong ToUInt64(object value)
        {
            if (value is int)
                return (ulong)(int)value;
            if (value is uint)
                return (ulong)(uint)value;
            if (value is long)
                return (ulong)(long)value;
            if (value is ulong)
                return (ulong)(ulong)value;
            if (value is BigInteger)
                return (ulong)(BigInteger)value;
            throw new NotImplementedException();
        }

        public object Operator(AssignmentOp op, params object[] args)
        {
            return null;
        }

        public object GetVariable(string name)
        {
            if (variables.ContainsKey(name))
                return variables[name];
            throw new InvalidOperationException("unknown variable: " + name);
        }

        public object SetVariable(string name, object value)
        {
            return variables[name] = value;
        }

        public object GetProperty(object context, string name)
        {
            if (context is Dictionary<string, object>)
            {
                var dict = context as Dictionary<string, object>;
                if (dict.ContainsKey(name))
                    return dict[name];
                throw new InvalidOperationException("unknown property: " + name);
            }
            if (name == "Type")
                return context.GetType().Name;
            throw new NotImplementedException();
        }

        public object SetProperty(object context, string name, object value)
        {
            if (context is Dictionary<string, object>)
                return (context as Dictionary<string, object>)[name] = value;
            throw new NotImplementedException();
        }

        public object CallMethod(object context, string name, params object[] args)
        {
            if (context == globalContext)
            {
                if (globalMethods.ContainsKey(name))
                {
                    var method = globalMethods[name];
                    return method(args);
                }
            }
            throw new InvalidOperationException("unknown function: " + name);
        }

        private void AddGlobalMethods()
        {
            globalMethods.Add("exit", Exit);
            globalMethods.Add("print", Print);
            globalMethods.Add("isprime", IsPrime);
            globalMethods.Add("nextprime", NextPrime);
            globalMethods.Add("factor", Factor);
        }

        public object Exit(params object[] args)
        {
            Environment.Exit(args.Length > 0 ? ToInt32(args[0]) : 0);
            return null;
        }

        public object Print(params object[] args)
        {
            var value = args[0];
            if (value is IEnumerable)
                Console.WriteLine(string.Join(", ", (value as IEnumerable).Cast<object>().Select(item => item.ToString())));
            else if (value is bool)
                Console.WriteLine((bool)value ? "true" : "false");
            else
                Console.WriteLine(value);
            return value;
        }

        public object IsPrime(params object[] args)
        {
            return IntegerMath.IsPrime(ToBigInteger(args[0]));
        }

        public object NextPrime(params object[] args)
        {
            return IntegerMath.NextPrime(ToBigInteger(args[0]));
        }

        public object Factor(params object[] args)
        {
            var algorithm = new HybridPollardRhoQuadraticSieve(8, 10000, new QuadraticSieve.Config());
            return algorithm.Factor(ToBigInteger(args[0])).ToArray();
        }
    }
}
