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
        public void TestBigIntegerReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(new BigIntegerReduction().GetReducer(p));
        }

        [TestMethod]
        public void TestRadix32IntegerReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(new Radix32IntegerReduction().GetReducer(p));
        }

        [TestMethod]
        public void TestMontgomeryReduction()
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
            var xPrime = reducer.ToResidue(0);
            var yPrime = reducer.ToResidue(0);
            var zPrime = reducer.ToResidue(0);
            var random = new MersenneTwister32(0);
            for (int i = 0; i < 100; i++)
            {
                var x = random.Next(p);
                var y = random.Next(p);
                var z = x * y;
                var expected = z % p;

                xPrime.Set(x);
                yPrime.Set(y);
                zPrime.Set(xPrime).Multiply(yPrime);
                var actual = zPrime.ToBigInteger();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestRadix32()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var random = new MersenneTwister32(0);
            var length = (n.GetBitLength() * 2 + 31) / 32 + 3;
            var store = new Radix32Store(length);
            var a = store.Create();
            var b = store.Create();
            var x = store.Create();
            var reg1 = store.Create();
            var reg2 = store.Create();
            for (int i = 0; i < 1000; i++)
            {
                var aPrime = random.Next(n);
                var bPrime = random.Next(n);
                uint c = random.Next();
                a.Set(aPrime);
                b.Set(bPrime);

                for (int j = 0; j <= 65; j++)
                {
                    x.Set(a).LeftShift(j);
                    Assert.AreEqual(aPrime << j, x.ToBigInteger());

                    x.Set(a).RightShift(j);
                    Assert.AreEqual(aPrime >> j, x.ToBigInteger());
                }

                x.SetSum(a, b);
                Assert.AreEqual(aPrime + bPrime, x.ToBigInteger());

                x.SetProduct(a, b);
                Assert.AreEqual(aPrime * bPrime, x.ToBigInteger());

                x.SetSquare(a);
                Assert.AreEqual(aPrime * aPrime, x.ToBigInteger());

                x.SetProduct(a, c);
                Assert.AreEqual(c * aPrime, x.ToBigInteger());

                x.SetQuotient(a, b, reg1);
                Assert.AreEqual(aPrime / bPrime, x.ToBigInteger());

                x.SetRemainder(a, b, reg1);
                Assert.AreEqual(aPrime % bPrime, x.ToBigInteger());

                x.SetQuotient(a, c, reg1);
                Assert.AreEqual(aPrime / c, x.ToBigInteger());

                x.SetRemainder(a, c, reg1);
                Assert.AreEqual(aPrime % c, x.ToBigInteger());

                if (aPrime > bPrime)
                {
                    x.SetDifference(a, b);
                    Assert.AreEqual(aPrime - bPrime, x.ToBigInteger());
                }
                else
                {
                    x.SetDifference(b, a);
                    Assert.AreEqual(bPrime - aPrime, x.ToBigInteger());
                }

                x.SetGreatestCommonDivisor(a, b, reg1, reg2);
                Assert.AreEqual(BigInteger.GreatestCommonDivisor(aPrime, bPrime), x.ToBigInteger());
            }
        }

        [TestMethod]
        public void DivModTest1()
        {
            Radix32Store store = new Radix32Store(20);
            var a = store.Create();
            var b = store.Create();
            var c = store.Create();
            var x = store.Create();
            var reg1 = store.Create();
            var reg2 = store.Create();
            a.Set(BigInteger.Parse("9580940093428730948"));
            b.Set(BigInteger.Parse("9460544844897193437"));
            c.Set(BigInteger.Parse("120395248531537511"));
            x.SetRemainder(a, b, reg1);
            Assert.AreEqual(a.ToBigInteger() % b.ToBigInteger(), x.ToBigInteger());
            Assert.AreEqual(c, x);
        }
    }
}
