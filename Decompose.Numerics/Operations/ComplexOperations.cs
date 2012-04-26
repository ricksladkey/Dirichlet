using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class ComplexOperations : IOperations<Complex>
    {
        public Type Type { get { return typeof(Complex); } }
        public Complex Zero { get { return 0; } }
        public Complex One { get { return 1; } }
        public Complex Two { get { return 2; } }
        public bool IsUnsigned { get { return false; } }
        public Complex Convert(int a) { return (Complex)a; }
        public Complex Convert(BigInteger a) { return (Complex)a; }
        public Complex Convert(double a) { return a; }
        public int ToInt32(Complex a) { return (int)ToDouble(a); }
        public BigInteger ToBigInteger(Complex a) { return (BigInteger)ToDouble(a); }
        public double ToDouble(Complex a) { if (a != a.Real) throw new InvalidCastException("not real"); return a.Real; }
        public Complex Add(Complex a, Complex b) { return a + b; }
        public Complex Subtract(Complex a, Complex b) { return a - b; }
        public Complex Multiply(Complex a, Complex b) { return a * b; }
        public Complex Divide(Complex a, Complex b) { return a / b; }
        public Complex Remainder(Complex a, Complex b) { return ToDouble(a) % ToDouble(b); }
        public Complex Modulo(Complex a, Complex b) { var result = ToDouble(a) % ToDouble(b); if (result < 0) result += ToDouble(b); return result; }
        public Complex Negate(Complex a) { return 0 - a; }
        public Complex LeftShift(Complex a, int n) { return a * Math.Pow(2, n); }
        public Complex RightShift(Complex a, int n) { return a / Math.Pow(2, n); }
        public Complex And(Complex a, Complex b) { return (Complex)(ToBigInteger(a) & ToBigInteger(b)); }
        public Complex Or(Complex a, Complex b) { return (Complex)(ToBigInteger(a) | ToBigInteger(b)); }
        public Complex ExclusiveOr(Complex a, Complex b) { return (Complex)(ToBigInteger(a) ^ ToBigInteger(b)); }
        public Complex OnesComplement(Complex a) { return (Complex)~ToBigInteger(a); }
        public int Sign(Complex a) { if (a != a.Real) throw new InvalidCastException("not real"); return Math.Sign(a.Real); }
        public bool IsZero(Complex a) { return a == 0; }
        public bool IsOne(Complex a) { return a == 1; }
        public bool IsEven(Complex a) { return ToDouble(a) % 2 == 0; }
        public bool Equals(Complex x, Complex y) { return x.Equals(y); }
        public int GetHashCode(Complex obj) { return obj.GetHashCode(); }
        public int Compare(Complex x, Complex y) { return x.Magnitude.CompareTo(y.Magnitude); }
        public uint LeastSignificantWord(Complex a) { return (uint)(ToBigInteger(a) & uint.MaxValue); }

        public Complex Power(Complex a, Complex b) { return Complex.Pow(a, b); }
        public Complex Root(Complex a, Complex b) { return Complex.Pow(a, 1 / b); }
        public Complex GreatestCommonDivisor(Complex a, Complex b) { return (Complex)IntegerMath.GreatestCommonDivisor(ToBigInteger(a), ToBigInteger(b)); }
        public Complex ModularSum(Complex a, Complex b, Complex modulus) { return (Complex)IntegerMath.ModularSum(ToBigInteger(a), ToBigInteger(b), ToBigInteger(modulus)); }
        public Complex ModularDifference(Complex a, Complex b, Complex modulus) { return (Complex)IntegerMath.ModularDifference(ToBigInteger(a), ToBigInteger(b), ToBigInteger(modulus)); }
        public Complex ModularProduct(Complex a, Complex b, Complex modulus) { return (Complex)IntegerMath.ModularProduct(ToBigInteger(a), ToBigInteger(b), ToBigInteger(modulus)); }
        public Complex ModularQuotient(Complex a, Complex b, Complex modulus) { return (Complex)IntegerMath.ModularQuotient(ToBigInteger(a), ToBigInteger(b), ToBigInteger(modulus)); }
        public Complex ModularPower(Complex value, Complex exponent, Complex modulus) { return (Complex)IntegerMath.ModularPower(ToBigInteger(value), ToBigInteger(exponent), ToBigInteger(modulus)); }
        public Complex ModularRoot(Complex value, Complex exponent, Complex modulus) { return (Complex)IntegerMath.ModularRoot(ToBigInteger(value), ToBigInteger(exponent), ToBigInteger(modulus)); }
        public Complex ModularInverse(Complex value, Complex modulus) { return (Complex)IntegerMath.ModularInverse(ToBigInteger(value), ToBigInteger(modulus)); }

        public Complex AbsoluteValue(Complex a) { return Complex.Abs(a); }
        public Complex Log(Complex a) { return Complex.Log(a); }
        public Complex Factorial(Complex a) { return IntegerMath.Factorial(ToInt32(a)); }
    }
}
