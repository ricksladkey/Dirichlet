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
            var p2 = IntegerMath.Sqrt(n);
            Assert.AreEqual(p, p2);
        }

        [TestMethod]
        public void TestSqrt2()
        {
            Assert.AreEqual(0, (int)IntegerMath.Sqrt(0));
            Assert.AreEqual(1, (int)IntegerMath.Sqrt(1));
            Assert.AreEqual(1, (int)IntegerMath.Sqrt(2));
            Assert.AreEqual(1, (int)IntegerMath.Sqrt(3));
            Assert.AreEqual(2, (int)IntegerMath.Sqrt(4));
            Assert.AreEqual(2, (int)IntegerMath.Sqrt(5));
            Assert.AreEqual(2, (int)IntegerMath.Sqrt(8));
            Assert.AreEqual(3, (int)IntegerMath.Sqrt(9));
            Assert.AreEqual(3, (int)IntegerMath.Sqrt(10));        
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
        public void TestQuadraticSieve1()
        {
            var expected = new[] { BigInteger.Parse("274177"), BigInteger.Parse("67280421310721") };
            var n = BigInteger.Parse("18446744073709551617");
            var algorithm = new QuadraticSieve(8, 0, 0);
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
            IntegerMath.ExtendedGreatestCommonDivisor(a, b, out c, out d);
            Assert.AreEqual(cExpected, c);
            Assert.AreEqual(dExpected, d);
        }

        [TestMethod]
        public void TestBigIntegerReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new BigIntegerReduction());
        }

        [TestMethod]
        public void TestRadix32IntegerReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new Word32IntegerReduction());
        }

        [TestMethod]
        public void TestMontgomeryReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new MontgomeryReduction());
        }

        [TestMethod]
        public void TestBarrettReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new BarrettReduction());
        }

        private void TestReduction(BigInteger p, IReductionAlgorithm reduction)
        {
            var reducer = reduction.GetReducer(p);
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
            var store = new Word32IntegerStore(length);
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

                x.SetSum(a, c);
                Assert.AreEqual(aPrime + c, x.ToBigInteger());

                x.SetProduct(a, b);
                Assert.AreEqual(aPrime * bPrime, x.ToBigInteger());

                x.SetSquare(a);
                Assert.AreEqual(aPrime * aPrime, x.ToBigInteger());

                x.SetProduct(a, c);
                Assert.AreEqual(c * aPrime, x.ToBigInteger());

                x.SetQuotient(a, b, reg1);
                Assert.AreEqual(aPrime / bPrime, x.ToBigInteger());

                x.SetRemainder(a, b);
                Assert.AreEqual(aPrime % bPrime, x.ToBigInteger());

                x.SetQuotient(a, c, reg1);
                Assert.AreEqual(aPrime / c, x.ToBigInteger());

                x.SetRemainder(a, c);
                Assert.AreEqual(aPrime % c, x.ToBigInteger());

                x.SetSquare(a);
                Assert.AreEqual(aPrime * aPrime, x.ToBigInteger());

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

                x.SetGreatestCommonDivisor(a, b, reg1);
                Assert.AreEqual(BigInteger.GreatestCommonDivisor(aPrime, bPrime), x.ToBigInteger());
            }
        }

        [TestMethod]
        public void ModularSquareRootTest1()
        {
            var random = new MersenneTwister32(0);
            var limit = BigInteger.Parse("10023859281455311421");
            for (int i = 0; i < 100; i++)
            {
                var p = IntegerMath.NextPrime(random.Next(limit));
                var n = random.Next(p);
                if (IntegerMath.JacobiSymbol(n, p) == 1)
                {
                    var r1 = IntegerMath.ModularSquareRoot(n, p);
                    var r2 = p - r1;
                    Assert.AreEqual(n, r1 * r1 % p);
                    Assert.AreEqual(n, r2 * r2 % p);
                }
            }
        }

        [TestMethod]
        public void DivModTest1()
        {
            // Triggers "borrow != 0" special case.
            Word32IntegerStore store = new Word32IntegerStore(20);
            var a = store.Create();
            var b = store.Create();
            var c = store.Create();
            var x = store.Create();
            a.Set(BigInteger.Parse("851968723384911158384830467125731460171903460330379450819468842227482878637917031244505597763225"));
            b.Set(BigInteger.Parse("2200761205517100656206929789365760219952611739831"));
            x.SetRemainder(a, b);
            Assert.AreEqual(a.ToBigInteger() % b.ToBigInteger(), x.ToBigInteger());
        }

        [TestMethod]
        public void SieveOfErostothonesTest1()
        {
            int n = 0;
            int i = 0;
            int iterations = 1000;
            foreach (int p in new SieveOfErostothones())
            {
                while (n < p)
                {
                    Assert.IsFalse(IntegerMath.IsPrime(n));
                    ++n;
                }
                Assert.IsTrue(IntegerMath.IsPrime(p));
                n = p + 1;
                if (++i >= iterations)
                    break;
            }
            Assert.AreEqual(i, iterations);
        }

        [TestMethod]
        public void TrialDivisionTest1()
        {
            var algorithm = new TrialDivision();
            for (int n = 2; n < 10000; n++)
            {
                var factors = algorithm.Factor(n).ToArray();
                var product = factors.Aggregate((sofar, current) => sofar * current);
                Assert.AreEqual(n, product);
                Assert.IsTrue(factors.All(factor => IntegerMath.IsPrime(factor)));
                Assert.AreEqual(IntegerMath.IsPrime((BigInteger)n), algorithm.IsPrime(n));
            }
        }
    }
}
