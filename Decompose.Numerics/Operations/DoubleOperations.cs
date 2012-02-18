using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class DoubleOperations : IOperations<double>
    {
        public Type Type { get { return typeof(double); } }
        public double Zero { get { return 0; } }
        public double One { get { return 1; } }
        public double Two { get { return 2; } }
        public bool IsUnsigned { get { return false; } }
        public double Convert(int a) { return (double)a; }
        public double Convert(BigInteger a) { return (double)a; }
        public double Convert(double a) { return a; }
        public int ToInt32(double a) { if ((int)a != a) throw new InvalidCastException("not an integer"); return (int)a; }
        public BigInteger ToBigInteger(double a) { return (BigInteger)a; }
        public double ToDouble(double a) { return a; }
        public double Add(double a, double b) { return a + b; }
        public double Subtract(double a, double b) { return a - b; }
        public double Multiply(double a, double b) { return a * b; }
        public double Divide(double a, double b) { return a / b; }
        public double Remainder(double a, double b) { return a % b; }
        public double Modulo(double a, double b) { var result = a % b; if (result < 0) result += b; return result; }
        public double Negate(double a) { return 0 - a; }
        public double LeftShift(double a, int n) { return a * Math.Pow(2, n); }
        public double RightShift(double a, int n) { return a / Math.Pow(2, n); }
        public double And(double a, double b) { return (double)(ToBigInteger(a) & ToBigInteger(b)); }
        public double Or(double a, double b) { return (double)(ToBigInteger(a) | ToBigInteger(b)); }
        public double ExclusiveOr(double a, double b) { return (double)(ToBigInteger(a) ^ ToBigInteger(b)); }
        public double OnesComplement(double a) { return (double)~ToBigInteger(a); }
        public bool IsZero(double a) { return a == 0; }
        public bool IsOne(double a) { return a == 1; }
        public bool IsEven(double a) { return a % 2 == 0; }
        public bool Equals(double x, double y) { return x.Equals(y); }
        public int GetHashCode(double obj) { return obj.GetHashCode(); }
        public int Compare(double x, double y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(double a) { return (uint)(ToBigInteger(a) & uint.MaxValue); }

        public double Power(double a, double b) { return Math.Pow(a, b); }
        public double Root(double a, double b) { return Math.Pow(a, 1 / b); }
        public double GreatestCommonDivisor(double a, double b) { return (double)IntegerMath.GreatestCommonDivisor(ToBigInteger(a), ToBigInteger(b)); }
        public double ModularSum(double a, double b, double modulus) { return (double)IntegerMath.ModularSum(ToBigInteger(a), ToBigInteger(b), ToBigInteger(modulus)); }
        public double ModularDifference(double a, double b, double modulus) { return (double)IntegerMath.ModularDifference(ToBigInteger(a), ToBigInteger(b), ToBigInteger(modulus)); }
        public double ModularProduct(double a, double b, double modulus) { return (double)IntegerMath.ModularProduct(ToBigInteger(a), ToBigInteger(b), ToBigInteger(modulus)); }
        public double ModularQuotient(double a, double b, double modulus) { return (double)IntegerMath.ModularQuotient(ToBigInteger(a), ToBigInteger(b), ToBigInteger(modulus)); }
        public double ModularPower(double value, double exponent, double modulus) { return (double)IntegerMath.ModularPower(ToBigInteger(value), ToBigInteger(exponent), ToBigInteger(modulus)); }
        public double ModularRoot(double value, double exponent, double modulus) { return (double)IntegerMath.ModularRoot(ToBigInteger(value), ToBigInteger(exponent), ToBigInteger(modulus)); }
        public double ModularInverse(double value, double modulus) { return (double)IntegerMath.ModularInverse(ToBigInteger(value), ToBigInteger(modulus)); }

        public Complex Log(double a) { return Math.Log(a); }
    }
}
