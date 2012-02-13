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
            opMaps.Add(typeof(int), new IntegerOperatorMap<int>(generator));
            opMaps.Add(typeof(uint), new IntegerOperatorMap<uint>(generator));
            opMaps.Add(typeof(long), new IntegerOperatorMap<long>(generator));
            opMaps.Add(typeof(ulong), new IntegerOperatorMap<ulong>(generator));
            opMaps.Add(typeof(BigInteger), new IntegerOperatorMap<BigInteger>(generator));
            opMaps.Add(typeof(double), new IntegerOperatorMap<double>(generator));
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

        public object Operator(Op op, params object[] args)
        {
            if (opMapCast.ContainsKey(op))
                return opMapCast[op](args[0]);
            args = Cast.ConvertToCompatibleTypes(args);
            if (opMaps.ContainsKey(args[0].GetType()))
                return opMaps[args[0].GetType()].Operator(op, args);
            throw new NotImplementedException();
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
            args = Cast.ConvertToCompatibleTypes(args);
            var types = args.Select(arg => arg.GetType()).ToArray();
            var method = GetType().GetMethod(name, types);
            if (method == null)
            {
                method = GetType().GetMethods()
                    .Where(item => item.Name == name && item.IsGenericMethod)
                    .FirstOrDefault();
                if (method != null)
                    method = method.MakeGenericMethod(types[0]);
            }
            if (method != null)
                return method.Invoke(this, args);
            throw new NotImplementedException();
        }

        private void AddGlobalMethods()
        {
            globalMethods.Add("exit", Exit);
            globalMethods.Add("print", Print);
            globalMethods.Add("jacobi", Jacobi);
            globalMethods.Add("isprime", IsPrime);
            globalMethods.Add("nextprime", NextPrime);
            globalMethods.Add("factor", Factor);
            globalMethods.Add("sqrt", args => Invoke("Sqrt", args));
        } 

        public object Exit(params object[] args)
        {
            Environment.Exit(args.Length > 0 ? Cast.ToInt32(args[0]) : 0);
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
            return null;
        }

        public object Jacobi(params object[] args)
        {
            return IntegerMath.JacobiSymbol(Cast.ToBigInteger(args[0]), Cast.ToBigInteger(args[1]));
        }

        public object IsPrime(params object[] args)
        {
            return IntegerMath.IsPrime(Cast.ToBigInteger(args[0]));
        }

        public double Sqrt(double a)
        {
            return Math.Sqrt(a);
        }

        public T Sqrt<T>(T a)
        {
            return Integer<T>.SquareRoot(a);
        }

        public object NextPrime(params object[] args)
        {
            return IntegerMath.NextPrime(Cast.ToBigInteger(args[0]));
        }

        public object Factor(params object[] args)
        {
            var algorithm = new HybridPollardRhoQuadraticSieve(8, 1000000, new QuadraticSieve.Config { Threads = 8 });
            return algorithm.Factor(Cast.ToBigInteger(args[0])).ToArray();
        }
    }
}
