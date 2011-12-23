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
            BigInteger c;
            BigInteger d;
            BigIntegerUtils.ExtendedGreatestCommonDivisor(a, b, out c, out d);
            Assert.AreEqual(cExpected, c);
            Assert.AreEqual(dExpected, d);
        }

        [TestMethod]
        public void TestMontgomery()
        {
            var a = (BigInteger)24;
            var b = (BigInteger)74;
            var n = (BigInteger)1201;
            var expected = a * b % n;
            var residue = new MontgomeryReduction(n);
            var aPrime = residue.ToResidue(a);
            var bPrime = residue.ToResidue(b);
            var cPrime = residue.Multiply(aPrime, bPrime);
            var result = residue.FromResidue(cPrime);
            Assert.AreEqual(expected, result);
        }
    }
}
