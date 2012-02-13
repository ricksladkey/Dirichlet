using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Decompose.Numerics;

namespace Decompose.Scripting
{
    public enum TraceFlags
    {
        Path,
    }

    public class Engine
    {
        public static string AttachedKey { get { return "@Attached"; } }
        public static string ContextKey { get { return "@Context"; } }
        public static string AssociatedObjectKey { get { return "@AssociatedObject"; } }
        public object Throw(string message) { throw new Exception(message); }
        public void Trace(TraceFlags flags, string message, params object[] args) { }

        private object globalContext;
        private RandomNumberGenerator generator;
        private Dictionary<string, object> variables;
        private Dictionary<string, Func<object[], object>> globalMethods;
        private List<Frame> stack;

        private Dictionary<Op, Func<object, object>> opMapCast = new Dictionary<Op, Func<object, object>>
        {
            { Op.Double, a => Cast.ToDouble(a) },
            { Op.Int32, a => Cast.ToInt32(a) },
            { Op.UInt32, a => Cast.ToUInt32(a) },
            { Op.Int64, a => Cast.ToInt64(a) },
            { Op.UInt64, a => Cast.ToUInt64(a) },
            { Op.BigInteger, a => Cast.ToBigInteger(a) },
            { Op.Rational, a => Cast.ToRational(a) },
        };

        private Dictionary<Type, IOperatorMap> opMaps = new Dictionary<Type, IOperatorMap>();

        private class Frame
        {
            public Dictionary<string, object> Variables { get; set; }
        }

        public Engine()
        {
            globalContext = new object();
            generator = new MersenneTwister(0);
            opMaps.Add(typeof(bool), new BooleanOperatorMap());
            opMaps.Add(typeof(int), new NumericOperatorMap<int>(generator));
            opMaps.Add(typeof(uint), new NumericOperatorMap<uint>(generator));
            opMaps.Add(typeof(long), new NumericOperatorMap<long>(generator));
            opMaps.Add(typeof(ulong), new NumericOperatorMap<ulong>(generator));
            opMaps.Add(typeof(BigInteger), new NumericOperatorMap<BigInteger>(generator));
            opMaps.Add(typeof(Rational), new NumericOperatorMap<Rational>(generator));
            opMaps.Add(typeof(double), new NumericOperatorMap<double>(generator));
            variables = new Dictionary<string, object>();
            globalMethods = new Dictionary<string, Func<object[], object>>();
            stack = new List<Frame>();

            SetVariable(ContextKey, globalContext);
            AddGlobalMethods();
        }

        public void PushFrame()
        {
            stack.Add(new Frame { Variables = new Dictionary<string, object>() });
        }

        public void PopFrame()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        public object GetVariable(string name)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                if (stack[i].Variables.ContainsKey(name))
                    return stack[i].Variables[name];
            }
            if (variables.ContainsKey(name))
                return variables[name];
            throw new InvalidOperationException("unknown variable: " + name);
        }

        public object SetGlobalVariable(string name, object value)
        {
            return variables[name] = value;
        }

        public object NewVariable(string name, object value)
        {
            if (!stack[stack.Count - 1].Variables.ContainsKey(name))
                return stack[stack.Count - 1].Variables[name] = value;
            throw new InvalidOperationException("variable exists");
        }

        public object SetVariable(string name, object value)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                if (stack[i].Variables.ContainsKey(name))
                    return stack[i].Variables[name] = value;
            }
            return variables[name] = value;
        }

        public object GetProperty(object context, string name)
        {
            if (context == globalContext)
                return GetVariable(name);
            if (name == "Type")
                return context.GetType().Name;
            throw new NotImplementedException();
        }

        public object SetProperty(object context, string name, object value)
        {
            if (context == globalContext)
                return SetVariable(name, value);
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

        private object Invoke(string name, params object[] args)
        {
            var type = typeof(Engine);
            args = Cast.ConvertToCompatibleTypes(args);
            var types = args.Select(arg => arg.GetType()).ToArray();
            var method = type.GetMethod(name, types);
            if (method == null && args.Length != 0)
            {
                method = type.GetMethods()
                    .Where(item => item.Name == name && item.IsGenericMethod)
                    .FirstOrDefault();
                if (method != null)
                    method = method.MakeGenericMethod(types[0]);
            }
            if (method != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != 0 && parameters[0].ParameterType.IsArray)
                    return method.Invoke(this, new object[] { Cast.ToTypedArray(args[0].GetType(), args) });
                return method.Invoke(this, args);
            }
            throw new NotImplementedException();
        }

        public object Operator(Op op, params object[] args)
        {
            if (opMapCast.ContainsKey(op))
                return opMapCast[op](args[0]);
            args = Cast.ConvertToCompatibleTypes(args);
            if (opMaps.ContainsKey(args[0].GetType()))
                return Simplify(opMaps[args[0].GetType()].Operator(op, args));
            throw new NotImplementedException();
        }

        public object Simplify(object value)
        {
            if (value is Rational && ((Rational)value).IsInteger)
                return ((Rational)value).Numerator;
            return value;
        }

        private void AddGlobalMethods()
        {
            globalMethods.Add("exit", Exit);
            globalMethods.Add("print", Print);
            globalMethods.Add("jacobi", args => Invoke("Jacobi", args));
            globalMethods.Add("isprime", args => Invoke("IsPrime", args));
            globalMethods.Add("nextprime", args => Invoke("NextPrime", args));
            globalMethods.Add("factor", args => Invoke("Factor", args));
            globalMethods.Add("sqrt", args => Invoke("Sqrt", args));
            globalMethods.Add("log", args => Invoke("Log", args));
            globalMethods.Add("exp", args => Invoke("Exp", args));
            globalMethods.Add("floor", args => Invoke("Floor", args));
            globalMethods.Add("ceiling", args => Invoke("Ceiling", args));
            globalMethods.Add("min", args => Invoke("Min", args));
            globalMethods.Add("max", args => Invoke("Max", args));
        } 

        public object Exit(params object[] args)
        {
            Environment.Exit(args.Length > 0 ? Cast.ToInt32(args[0]) : 0);
            return null;
        }

        public object Print(params object[] args)
        {
            var value = args[0];
            if (value is IEnumerable && !(value is string))
                Console.WriteLine(string.Join(", ", (value as IEnumerable).Cast<object>().Select(item => item.ToString())));
            else if (value is bool)
                Console.WriteLine((bool)value ? "true" : "false");
            else
                Console.WriteLine(value);
            return null;
        }

        public BigInteger Jacobi(BigInteger a, BigInteger b)
        {
            return IntegerMath.JacobiSymbol(a, b);
        }

        public bool IsPrime(BigInteger a)
        {
            return IntegerMath.IsPrime(a);
        }

        public T Sqrt<T>(T a)
        {
            return Integer<T>.Root(a, 2);
        }

        public double Log(object a)
        {
            if (a is double)
                return Math.Log((double)a);
            return BigInteger.Log(Cast.ToBigInteger(a));
        }

        public double Exp(object a)
        {
            return Math.Exp(Cast.ToDouble(a));
        }

        public T Max<T>(params T[] args)
        {
            var result = args[0];
            foreach (var arg in args)
                result = Integer<T>.Max(result, arg);
            return result;
        }

        public T Min<T>(params T[] args)
        {
            var result = args[0];
            foreach (var arg in args)
                result = Integer<T>.Min(result, arg);
            return result;
        }

        public object Floor(object a)
        {
            if (a is double)
                return (BigInteger)Math.Floor((double)a);
            if (a is Rational)
                return Rational.Floor((Rational)a);
            return a;
        }

        public object Ceiling(object a)
        {
            if (a is double)
                return (BigInteger)Math.Ceiling((double)a);
            if (a is Rational)
                return Rational.Ceiling((Rational)a);
            return a;
        }

        public BigInteger NextPrime(BigInteger a)
        {
            return IntegerMath.NextPrime(a);
        }

        public IEnumerable<BigInteger> Factor(BigInteger a)
        {
            var algorithm = new HybridPollardRhoQuadraticSieve(8, 1000000, new QuadraticSieve.Config { Threads = 8 });
            return algorithm.Factor(a).ToArray();
        }
    }
}
