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
    public class MathUtilsTests
    {
        [TestMethod]
        public void TestSqrt1()
        {
            var p = BigInteger.Parse("12345678901234567890");
            var n = p * p;
            var p2 = MathUtils.Sqrt(n);
            Assert.AreEqual(p, p2);
        }

        [TestMethod]
        public void TestSqrt2()
        {
            Assert.AreEqual(0, (int)MathUtils.Sqrt(0));
            Assert.AreEqual(1, (int)MathUtils.Sqrt(1));
            Assert.AreEqual(1, (int)MathUtils.Sqrt(2));
            Assert.AreEqual(1, (int)MathUtils.Sqrt(3));
            Assert.AreEqual(2, (int)MathUtils.Sqrt(4));
            Assert.AreEqual(2, (int)MathUtils.Sqrt(5));
            Assert.AreEqual(2, (int)MathUtils.Sqrt(8));
            Assert.AreEqual(3, (int)MathUtils.Sqrt(9));
            Assert.AreEqual(3, (int)MathUtils.Sqrt(10));        
        }

        [TestMethod]
        public void TestPrimality()
        {
            var primes = Enumerable.Range(937, 1000 - 937)
                .Where(n => MathUtils.IsPrimeMillerRabin(n, 20))
                .ToArray();
            var expected = new[] { 937, 941, 947, 953, 967, 971, 977, 983, 991, 997 };
            Assert.IsTrue(((IStructuralEquatable)primes).Equals(expected, EqualityComparer<int>.Default));
        }

        [TestMethod]
        public void TestPollard1()
        {
            var expected = new[] { BigInteger.Parse("274177"), BigInteger.Parse("67280421310721") };
            var n = BigInteger.Parse("18446744073709551617");
            var factors = MathUtils.FactorPollard(n).OrderBy(factor => factor).ToArray();
            var product = factors.Aggregate((sofar, current) => sofar * current);
            Assert.AreEqual(n, product);
            Assert.IsTrue(((IStructuralEquatable)factors).Equals(expected, EqualityComparer<BigInteger>.Default));
        }

        [TestMethod]
        public void TestPollard2()
        {
            var expected = new[] { BigInteger.Parse("91739369"), BigInteger.Parse("266981831") };
            var n = expected.Aggregate(BigInteger.One, (sofar, factor) => sofar * factor);
            var factors = MathUtils.FactorPollard(n).OrderBy(factor => factor).ToArray();
            var product = factors.Aggregate((sofar, current) => sofar * current);
            Assert.AreEqual(n, product);
            Assert.IsTrue(((IStructuralEquatable)factors).Equals(expected, EqualityComparer<BigInteger>.Default));
        }
    }
}
