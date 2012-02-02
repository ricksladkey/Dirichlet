using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public struct Integer<T>
    {
        private T value;
        private IOperations<T> ops;
        public Integer(T value, IOperations<T> ops) { this.value = value; this.ops = ops; }
        public IOperations<T> Operations { get { return ops; } }
        public T Value { get { return value; } }
        public override string ToString() { return value.ToString(); }
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
        public static bool operator ==(Integer<T> a, Integer<T> b) { return a.ops.Equals(a.value, b.value); }
        public static bool operator !=(Integer<T> a, Integer<T> b) { return !a.ops.Equals(a.value, b.value); }
        public static bool operator <(Integer<T> a, Integer<T> b) { return a.ops.Compare(a.value, b.value) < 0; }
        public static bool operator <=(Integer<T> a, Integer<T> b) { return a.ops.Compare(a.value, b.value) <= 0; }
        public static bool operator >(Integer<T> a, Integer<T> b) { return a.ops.Compare(a.value, b.value) > 0; }
        public static bool operator >=(Integer<T> a, Integer<T> b) { return a.ops.Compare(a.value, b.value) >= 0; }
        public override bool Equals(object obj) { return value.Equals(obj); }
        public override int GetHashCode() { return value.GetHashCode(); }
    }
}
