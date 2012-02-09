using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public struct Integer<T> : IComparable<Integer<T>>, IEquatable<Integer<T>>
    {
        private static Integer<T> zero;
        private static Integer<T> one;
        private static Integer<T> two;

        static Integer()
        {
            var ops = Operations.Get<T>();
            zero = new Integer<T>(ops.Zero, ops);
            one = new Integer<T>(ops.One, ops);
            two = new Integer<T>(ops.Two, ops);
        }

        private T value;
        private IOperations<T> ops;
        public Integer(T value) { this.value = value; this.ops = Operations.Get<T>(); }
        public Integer(T value, IOperations<T> ops) { this.value = value; this.ops = ops; }
        public T Value { get { return value; } }
        public static Integer<T> Zero { get { return zero; } }
        public static Integer<T> One { get { return one; } }
        public static Integer<T> Two { get { return two; } }
        public bool IsZero { get { return ops.IsZero(value); } }
        public bool IsOne { get { return ops.IsOne(value); } }
        public bool IsEven { get { return ops.IsEven(value); } }
        public static implicit operator Integer<T>(int value) { var ops = Operations.Get<T>(); return new Integer<T>(ops.Convert(value), ops); }
        public static implicit operator Integer<T>(T value) { return new Integer<T>(value); }
        public static implicit operator T(Integer<T> integer) { return integer.value; }
        public static implicit operator BigInteger(Integer<T> integer) { return integer.ops.ToBigInteger(integer.value); }
        public static Integer<T> operator +(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.Add(a.value, b.value), a.ops); }
        public static Integer<T> operator -(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.Subtract(a.value, b.value), a.ops); }
        public static Integer<T> operator *(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.Multiply(a.value, b.value), a.ops); }
        public static Integer<T> operator /(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.Divide(a.value, b.value), a.ops); }
        public static Integer<T> operator %(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.Modulus(a.value, b.value), a.ops); }
        public static Integer<T> operator -(Integer<T> a) { return new Integer<T>(a.ops.Negate(a.value), a.ops); }
        public static Integer<T> operator ++(Integer<T> a) { return new Integer<T>(a.ops.Add(a, a.ops.One), a.ops); }
        public static Integer<T> operator --(Integer<T> a) { return new Integer<T>(a.ops.Subtract(a, a.ops.One), a.ops); }
        public static Integer<T> operator <<(Integer<T> a, int b) { return new Integer<T>(a.ops.LeftShift(a.value, b), a.ops); }
        public static Integer<T> operator >>(Integer<T> a, int b) { return new Integer<T>(a.ops.RightShift(a.value, b), a.ops); }
        public static Integer<T> operator &(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.And(a.value, b.value), a.ops); }
        public static Integer<T> operator |(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.Or(a.value, b.value), a.ops); }
        public static Integer<T> operator ^(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.ExclusiveOr(a.value, b.value), a.ops); }
        public static Integer<T> operator ~(Integer<T> a) { return new Integer<T>(a.ops.Not(a.value), a.ops); }
        public static bool operator ==(Integer<T> a, Integer<T> b) { return a.ops.Equals(a.value, b.value); }
        public static bool operator !=(Integer<T> a, Integer<T> b) { return !a.ops.Equals(a.value, b.value); }
        public static bool operator <(Integer<T> a, Integer<T> b) { return a.ops.Compare(a.value, b.value) < 0; }
        public static bool operator <=(Integer<T> a, Integer<T> b) { return a.ops.Compare(a.value, b.value) <= 0; }
        public static bool operator >(Integer<T> a, Integer<T> b) { return a.ops.Compare(a.value, b.value) > 0; }
        public static bool operator >=(Integer<T> a, Integer<T> b) { return a.ops.Compare(a.value, b.value) >= 0; }
        public static Integer<T> SquareRoot(Integer<T> a) { return new Integer<T>(a.ops.SquareRoot(a.value), a.ops); }
        public static Integer<T> GreatestCommonDivisor(Integer<T> a, Integer<T> b) { return new Integer<T>(a.ops.GreatestCommonDivisor(a.value, b.value), a.ops); }
        public static Integer<T> ModularSum(Integer<T> a, Integer<T> b, Integer<T> modulus) { return new Integer<T>(a.ops.ModularSum(a.value, b.value, modulus.value), a.ops); }
        public static Integer<T> ModularDifference(Integer<T> a, Integer<T> b, Integer<T> modulus) { return new Integer<T>(a.ops.ModularDifference(a.value, b.value, modulus.value), a.ops); }
        public static Integer<T> ModularProduct(Integer<T> a, Integer<T> b, Integer<T> modulus) { return new Integer<T>(a.ops.ModularProduct(a.value, b.value, modulus.value), a.ops); }
        public static Integer<T> ModularPower(Integer<T> a, Integer<T> exponent, Integer<T> modulus) { return new Integer<T>(a.ops.ModularPower(a.value, exponent.value, modulus.value), a.ops); }
        public static Integer<T> ModularInverse(Integer<T> a, Integer<T> modulus) { return new Integer<T>(a.ops.ModularInverse(a.value, modulus.value), a.ops); }
        public int CompareTo(Integer<T> other) { return ops.Compare(value, other.value); }
        public bool Equals(Integer<T> other) { return ops.Equals(value, other.value); }
        public override string ToString() { return value.ToString(); }
        public override bool Equals(object obj) { return obj is Integer<T> ? ops.Equals(value, ((Integer<T>)obj).value) : false; }
        public override int GetHashCode() { return value.GetHashCode(); }
    }
}
