using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public struct Integer<T> : IComparable, IComparable<Integer<T>>, IEquatable<Integer<T>>
    {
        private static IOperations<T> ops;
        private static Integer<T> zero;
        private static Integer<T> one;
        private static Integer<T> two;

        static Integer()
        {
            ops = Operations.Get<T>();
            zero = new Integer<T>(ops.Zero);
            one = new Integer<T>(ops.One);
            two = new Integer<T>(ops.Two);
        }

        private T value;
        public Integer(T value) { this.value = value; }
        public T Value { get { return value; } }
        public static Integer<T> Zero { get { return zero; } }
        public static Integer<T> One { get { return one; } }
        public static Integer<T> Two { get { return two; } }
        public bool IsZero { get { return ops.IsZero(value); } }
        public bool IsOne { get { return ops.IsOne(value); } }
        public bool IsEven { get { return ops.IsEven(value); } }
        public static implicit operator Integer<T>(int value) { return new Integer<T>(ops.Convert(value)); }
        public static explicit operator Integer<T>(BigInteger value) { return new Integer<T>(ops.Convert(value)); }
        public static implicit operator Integer<T>(double value) { return new Integer<T>(ops.Convert(value)); }
        public static implicit operator Integer<T>(T value) { return new Integer<T>(value); }
        public static implicit operator T(Integer<T> integer) { return integer.value; }
        public static explicit operator int(Integer<T> integer) { return ops.ToInt32(integer.value); }
        public static implicit operator BigInteger(Integer<T> integer) { return ops.ToBigInteger(integer.value); }
        public static explicit operator double(Integer<T> integer) { return ops.ToDouble(integer.value); }
        public static Integer<T> operator +(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Add(a.value, b.value)); }
        public static Integer<T> operator +(T a, Integer<T> b) { return new Integer<T>(ops.Add(a, b.value)); }
        public static Integer<T> operator +(Integer<T> a, T b) { return new Integer<T>(ops.Add(a.value, b)); }
        public static Integer<T> operator -(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Subtract(a.value, b.value)); }
        public static Integer<T> operator -(T a, Integer<T> b) { return new Integer<T>(ops.Subtract(a, b.value)); }
        public static Integer<T> operator -(Integer<T> a, T b) { return new Integer<T>(ops.Subtract(a.value, b)); }
        public static Integer<T> operator *(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Multiply(a.value, b.value)); }
        public static Integer<T> operator *(T a, Integer<T> b) { return new Integer<T>(ops.Multiply(a, b.value)); }
        public static Integer<T> operator *(Integer<T> a, T b) { return new Integer<T>(ops.Multiply(a.value, b)); }
        public static Integer<T> operator /(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Divide(a.value, b.value)); }
        public static Integer<T> operator /(T a, Integer<T> b) { return new Integer<T>(ops.Divide(a, b.value)); }
        public static Integer<T> operator /(Integer<T> a, T b) { return new Integer<T>(ops.Divide(a.value, b)); }
        public static Integer<T> operator %(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Remainder(a.value, b.value)); }
        public static Integer<T> operator %(T a, Integer<T> b) { return new Integer<T>(ops.Remainder(a, b.value)); }
        public static Integer<T> operator %(Integer<T> a, T b) { return new Integer<T>(ops.Remainder(a.value, b)); }
        public static Integer<T> operator +(Integer<T> a) { return a; }
        public static Integer<T> operator -(Integer<T> a) { return new Integer<T>(ops.Negate(a.value)); }
        public static Integer<T> operator ++(Integer<T> a) { return new Integer<T>(ops.Add(a.value, one.value)); }
        public static Integer<T> operator --(Integer<T> a) { return new Integer<T>(ops.Subtract(a.value, one.value)); }
        public static Integer<T> operator <<(Integer<T> a, int b) { return new Integer<T>(ops.LeftShift(a.value, b)); }
        public static Integer<T> operator >>(Integer<T> a, int b) { return new Integer<T>(ops.RightShift(a.value, b)); }
        public static Integer<T> operator &(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.And(a.value, b.value)); }
        public static Integer<T> operator &(T a, Integer<T> b) { return new Integer<T>(ops.And(a, b.value)); }
        public static Integer<T> operator &(Integer<T> a, T b) { return new Integer<T>(ops.And(a.value, b)); }
        public static Integer<T> operator |(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Or(a.value, b.value)); }
        public static Integer<T> operator |(T a, Integer<T> b) { return new Integer<T>(ops.Or(a, b.value)); }
        public static Integer<T> operator |(Integer<T> a, T b) { return new Integer<T>(ops.Or(a.value, b)); }
        public static Integer<T> operator ^(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.ExclusiveOr(a.value, b.value)); }
        public static Integer<T> operator ^(T a, Integer<T> b) { return new Integer<T>(ops.ExclusiveOr(a, b.value)); }
        public static Integer<T> operator ^(Integer<T> a, T b) { return new Integer<T>(ops.ExclusiveOr(a.value, b)); }
        public static Integer<T> operator ~(Integer<T> a) { return new Integer<T>(ops.OnesComplement(a.value)); }
        public static bool operator ==(Integer<T> a, Integer<T> b) { return ops.Equals(a.value, b.value); }
        public static bool operator ==(T a, Integer<T> b) { return ops.Equals(a, b.value); }
        public static bool operator ==(Integer<T> a, T b) { return ops.Equals(a.value, b); }
        public static bool operator !=(Integer<T> a, Integer<T> b) { return !ops.Equals(a.value, b.value); }
        public static bool operator !=(T a, Integer<T> b) { return !ops.Equals(a, b.value); }
        public static bool operator !=(Integer<T> a, T b) { return !ops.Equals(a.value, b); }
        public static bool operator <(Integer<T> a, Integer<T> b) { return ops.Compare(a.value, b.value) < 0; }
        public static bool operator <(T a, Integer<T> b) { return ops.Compare(a, b.value) < 0; }
        public static bool operator <(Integer<T> a, T b) { return ops.Compare(a.value, b) < 0; }
        public static bool operator <=(Integer<T> a, Integer<T> b) { return ops.Compare(a.value, b.value) <= 0; }
        public static bool operator <=(T a, Integer<T> b) { return ops.Compare(a, b.value) <= 0; }
        public static bool operator <=(Integer<T> a, T b) { return ops.Compare(a.value, b) <= 0; }
        public static bool operator >(Integer<T> a, Integer<T> b) { return ops.Compare(a.value, b.value) > 0; }
        public static bool operator >(T a, Integer<T> b) { return ops.Compare(a, b.value) > 0; }
        public static bool operator >(Integer<T> a, T b) { return ops.Compare(a.value, b) > 0; }
        public static bool operator >=(Integer<T> a, Integer<T> b) { return ops.Compare(a.value, b.value) >= 0; }
        public static bool operator >=(T a, Integer<T> b) { return ops.Compare(a, b.value) >= 0; }
        public static bool operator >=(Integer<T> a, T b) { return ops.Compare(a.value, b) >= 0; }
        public static Integer<T> Power(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Power(a.value, b.value)); }
        public static Integer<T> Root(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Root(a.value, b.value)); }
        public static Integer<T> FloorRoot(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.Convert(Math.Floor(Math.Exp(BigInteger.Log(a) / ops.ToDouble(b))))); }
        public static Integer<T> SquareRoot(Integer<T> a) { return new Integer<T>(ops.Root(a.value, ops.Convert(2))); }
        public static Integer<T> GreatestCommonDivisor(Integer<T> a, Integer<T> b) { return new Integer<T>(ops.GreatestCommonDivisor(a.value, b.value)); }
        public static Integer<T> ModularSum(Integer<T> a, Integer<T> b, Integer<T> modulus) { return new Integer<T>(ops.ModularSum(a.value, b.value, modulus.value)); }
        public static Integer<T> ModularDifference(Integer<T> a, Integer<T> b, Integer<T> modulus) { return new Integer<T>(ops.ModularDifference(a.value, b.value, modulus.value)); }
        public static Integer<T> ModularProduct(Integer<T> a, Integer<T> b, Integer<T> modulus) { return new Integer<T>(ops.ModularProduct(a.value, b.value, modulus.value)); }
        public static Integer<T> ModularPower(Integer<T> a, Integer<T> exponent, Integer<T> modulus) { return new Integer<T>(ops.ModularPower(a.value, exponent.value, modulus.value)); }
        public static Integer<T> ModularInverse(Integer<T> a, Integer<T> modulus) { return new Integer<T>(ops.ModularInverse(a.value, modulus.value)); }
        public int CompareTo(object obj) { if (obj is Integer<T>) return ops.Compare(value, ((Integer<T>)obj).value); throw new ArgumentException("obj"); }
        public int CompareTo(T other) { return ops.Compare(value, other); }
        public int CompareTo(Integer<T> other) { return ops.Compare(value, other.value); }
        public bool Equals(T other) { return ops.Equals(value, other); }
        public bool Equals(Integer<T> other) { return ops.Equals(value, other.value); }
        public static Integer<T> Min(Integer<T> a, Integer<T> b) { return ops.Compare(a.value, b.value) < 0 ? a : b; }
        public static Integer<T> Max(Integer<T> a, Integer<T> b) { return ops.Compare(a.value, b.value) > 0 ? a : b; }
        public static Integer<T> Abs(Integer<T> a) { return ops.Compare(a.value, Zero) < 0 ? new Integer<T>(ops.Negate(a.value)): a; }
        public static Complex Log(Integer<T> a) { return ops.Log(a.value); }
        public override bool Equals(object obj) { return obj is Integer<T> && ops.Equals(value, ((Integer<T>)obj).value); }
        public override int GetHashCode() { return value.GetHashCode(); }
        public override string ToString() { return value.ToString(); }
    }
}
