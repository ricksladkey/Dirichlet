using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decompose.Numerics.Test
{
    [TestClass]
    public class NumericsTests
    {
        [TestMethod]
        public void TestSqrt1()
        {
            var p = BigInteger.Parse("12345678901234567890");
            var n = p * p;
            var p2 = BigIntegerUtils.Sqrt(n);
            Assert.AreEqual(p, p2);
        }

        [TestMethod]
        public void TestSqrt2()
        {
            Assert.AreEqual(0, (int)BigIntegerUtils.Sqrt(0));
            Assert.AreEqual(1, (int)BigIntegerUtils.Sqrt(1));
            Assert.AreEqual(1, (int)BigIntegerUtils.Sqrt(2));
            Assert.AreEqual(1, (int)BigIntegerUtils.Sqrt(3));
            Assert.AreEqual(2, (int)BigIntegerUtils.Sqrt(4));
            Assert.AreEqual(2, (int)BigIntegerUtils.Sqrt(5));
            Assert.AreEqual(2, (int)BigIntegerUtils.Sqrt(8));
            Assert.AreEqual(3, (int)BigIntegerUtils.Sqrt(9));
            Assert.AreEqual(3, (int)BigIntegerUtils.Sqrt(10));        
        }

        [TestMethod]
        public void TestPrimality()
        {
            var algorithm = new MillerRabin(16);
            var primes = Enumerable.Range(937, 1000 - 937)
                .Where(n => algorithm.IsPrime(n))
                .ToArray();
            var expected = new[] { 937, 941, 947, 953, 967, 971, 977, 983, 991, 997 };
            Assert.IsTrue(((IStructuralEquatable)primes).Equals(expected, EqualityComparer<int>.Default));
        }

        [TestMethod]
        public void TestPollard1()
        {
            var expected = new[] { BigInteger.Parse("274177"), BigInteger.Parse("67280421310721") };
            var n = BigInteger.Parse("18446744073709551617");
            var algorithm = new PollardRho(1);
            var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
            var product = factors.Aggregate((sofar, current) => sofar * current);
            Assert.AreEqual(n, product);
            Assert.IsTrue(((IStructuralEquatable)factors).Equals(expected, EqualityComparer<BigInteger>.Default));
        }

        [TestMethod]
        public void TestPollard2()
        {
            var expected = new[] { BigInteger.Parse("91739369"), BigInteger.Parse("266981831") };
            var n = expected.Aggregate(BigInteger.One, (sofar, factor) => sofar * factor);
            var algorithm = new PollardRho(1);
            var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
            var product = factors.Aggregate((sofar, current) => sofar * current);
            Assert.AreEqual(n, product);
            Assert.IsTrue(((IStructuralEquatable)factors).Equals(expected, EqualityComparer<BigInteger>.Default));
        }

        [TestMethod]
        public void TestExtendedGreatestCommonDivisor()
        {
            var a = (BigInteger)120;
            var b = (BigInteger)23;
            var cExpected = (BigInteger)(-9);
            var dExpected = (BigInteger)47;
            var results = BigIntegerUtils.ExtendedGreatestCommonDivisor(a, b);
            BigInteger c = results[0];
            BigInteger d = results[1];
            Assert.AreEqual(cExpected, c);
            Assert.AreEqual(dExpected, d);
        }

        [TestMethod]
        public void TestBigIntegerReduction()
        {
            var a = (BigInteger)24;
            var b = (BigInteger)74;
            var n = (BigInteger)1201;
            var expected = a * b % n;
            var reducer = new BigIntegerReduction().GetReducer(n);
            var aPrime = reducer.ToResidue(a);
            var bPrime = reducer.ToResidue(b);
            var cPrime = aPrime.Copy().Multiply(bPrime);
            var result = cPrime.ToBigInteger();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMontgomeryReduction1()
        {
            var a = (BigInteger)24;
            var b = (BigInteger)74;
            var n = (BigInteger)1201;
            var expected = a * b % n;
            var reducer = new MontgomeryReduction().GetReducer(n);
            var aPrime = reducer.ToResidue(a);
            var bPrime = reducer.ToResidue(b);
            var cPrime = aPrime.Copy().Multiply(bPrime);
            var result = cPrime.ToBigInteger();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMontgomeryReduction2()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(new MontgomeryReduction().GetReducer(p));
        }

        [TestMethod]
        public void TestBarrettReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(new BarrettReduction().GetReducer(p));
        }

        private void TestReduction(IReducer reducer)
        {
            var p = reducer.Modulus;
            var random = new MersenneTwister32(0);
            for (int i = 0; i < 100; i++)
            {
                var x = random.Next(p);
                var y = random.Next(p);
                var z = x * y;
                var expected = z % p;

                var actual = reducer.ToResidue(z).ToBigInteger();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestRadix32()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var random = new MersenneTwister32(0);  
            var length = (BigIntegerUtils.GetBitLength(n) * 2 + 31) / 32;
            var bits = new uint[3 * length];
            var a = new Radix32Integer(bits, 0 * length, length);
            var b = new Radix32Integer(bits, 1 * length, length);
            var x = new Radix32Integer(bits, 2 * length, length);
            for (int i = 0; i < 100; i++)
            {
                var aPrime = random.Next(n);
                var bPrime = random.Next(n);
                a.Set(aPrime);
                b.Set(bPrime);

                x.SetSum(a, b);
                var sum = x.ToBigInteger();
                Assert.AreEqual(aPrime + bPrime, sum);

                x.SetProduct(a, b);
                var product = x.ToBigInteger();
                Assert.AreEqual(aPrime * bPrime, product);

                if (aPrime > bPrime)
                {
                    x.SetDifference(a, b);
                    var difference = x.ToBigInteger();
                    Assert.AreEqual(aPrime - bPrime, difference);
                }
                else
                {
                    x.SetDifference(b, a);
                    var difference = x.ToBigInteger();
                    Assert.AreEqual(bPrime - aPrime, difference);
                }
            }
        }
    }
}
