using System;
using System.Linq;
using System.Numerics;
using Decompose.Numerics;

namespace Decompose.Scripting
{
    public static class Cast
    {
        public static bool ToBoolean(object value)
        {
            if (value is bool)
                return (bool)value;
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
            if (value is double)
                return (int)(double)value;
            if (value is Rational)
                return (int)(Rational)value;
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
            if (value is double)
                return (uint)(double)value;
            if (value is Rational)
                return (uint)(Rational)value;
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
            if (value is double)
                return (long)(double)value;
            if (value is Rational)
                return (long)(Rational)value;
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
            if (value is double)
                return (ulong)(double)value;
            if (value is Rational)
                return (ulong)(Rational)value;
            throw new NotImplementedException();
        }

        public static Complex ToComplex(object value)
        {
            if (value is int)
                return (Complex)(int)value;
            if (value is uint)
                return (Complex)(uint)value;
            if (value is long)
                return (Complex)(long)value;
            if (value is ulong)
                return (Complex)(ulong)value;
            if (value is BigInteger)
                return (Complex)(BigInteger)value;
            if (value is double)
                return (Complex)(double)value;
            if (value is Rational)
                return (Complex)(Rational)value;
            if (value is Complex)
                return (Complex)(Complex)value;
            throw new NotImplementedException();
        }

        public static double ToDouble(object value)
        {
            if (value is int)
                return (double)(int)value;
            if (value is uint)
                return (double)(uint)value;
            if (value is long)
                return (double)(long)value;
            if (value is ulong)
                return (double)(ulong)value;
            if (value is BigInteger)
                return (double)(BigInteger)value;
            if (value is double)
                return (double)(double)value;
            if (value is Rational)
                return (double)(Rational)value;
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
            if (value is double)
                return (BigInteger)(double)value;
            if (value is Rational)
                return (BigInteger)(Rational)value;
            throw new NotImplementedException();
        }

        public static Rational ToRational(object value)
        {
            if (value is int)
                return (Rational)(int)value;
            if (value is uint)
                return (Rational)(uint)value;
            if (value is long)
                return (Rational)(long)value;
            if (value is ulong)
                return (Rational)(ulong)value;
            if (value is BigInteger)
                return (Rational)(BigInteger)value;
            if (value is double)
                return (Rational)(double)value;
            if (value is Rational)
                return (Rational)(Rational)value;
            throw new NotImplementedException();
        }

        public static object[] ConvertToCompatibleTypes(object[] args)
        {
            if (args.Length < 2)
                return args;
            var types = args.Select(arg => arg.GetType()).ToArray();
            if (types.Any(type => type == typeof(Complex)))
                return args.Select(arg => Cast.ToComplex(arg)).Cast<object>().ToArray();
            if (types.Any(type => type == typeof(double)))
                return args.Select(arg => Cast.ToDouble(arg)).Cast<object>().ToArray();
            if (types.Any(type => type == typeof(Rational)))
                return args.Select(arg => Cast.ToRational(arg)).Cast<object>().ToArray();
            if (types.Any(type => type == typeof(BigInteger)))
                return args.Select(arg => Cast.ToBigInteger(arg)).Cast<object>().ToArray();
            if (types.Any(type => type == typeof(ulong)))
                return args.Select(arg => Cast.ToUInt64(arg)).Cast<object>().ToArray();
            if (types.Any(type => type == typeof(long)))
                return args.Select(arg => Cast.ToInt64(arg)).Cast<object>().ToArray();
            if (types.Any(type => type == typeof(uint)))
                return args.Select(arg => Cast.ToUInt32(arg)).Cast<object>().ToArray();
            if (types.Any(type => type == typeof(int)))
                return args.Select(arg => Cast.ToInt32(arg)).Cast<object>().ToArray();
            if (types.Any(type => type == typeof(bool)))
                return args.Select(arg => Cast.ToBoolean(arg)).Cast<object>().ToArray();
            throw new NotImplementedException();
        }

        public static object ToTypedArray(Type type, object[] args)
        {
            var array = Array.CreateInstance(type, args.Length);
            for (int i = 0; i < args.Length; i++)
                array.SetValue(args[i], i);
            return array;
        }
    }
}
