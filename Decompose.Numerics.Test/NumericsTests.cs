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
            var algorithm = new PollardRho(1, 0);
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
            var algorithm = new PollardRho(1, 0);
            var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
            var product = factors.Aggregate((sofar, current) => sofar * current);
            Assert.AreEqual(n, product);
            Assert.IsTrue(((IStructuralEquatable)factors).Equals(expected, EqualityComparer<BigInteger>.Default));
        }

        [TestMethod]
        public void TestPollardLong()
        {
            var expected = new[] { long.Parse("400433141"), long.Parse("646868797") };
            var n = expected[0] * expected[1];
            var algorithm = new PollardRhoLong();
            var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
            var product = factors.Aggregate((sofar, current) => sofar * current);
            Assert.AreEqual(n, product);
            Assert.IsTrue(((IStructuralEquatable)factors).Equals(expected, EqualityComparer<long>.Default));
        }

        [TestMethod]
        public void TestShanksSquareForms()
        {
            var expected = new[] { long.Parse("91739369"), long.Parse("266981831") };
            var n = expected.Aggregate((long)1, (sofar, factor) => sofar * factor);
            var algorithm = new ShanksSquareForms();
            var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
            var product = factors.Aggregate((sofar, current) => sofar * current);
            Assert.AreEqual(n, product);
            Assert.IsTrue(((IStructuralEquatable)factors).Equals(expected, EqualityComparer<long>.Default));
        }

        [TestMethod]
        public void TestQuadraticSieve1()
        {
            var expected = new[] { BigInteger.Parse("274177"), BigInteger.Parse("67280421310721") };
            var n = BigInteger.Parse("18446744073709551617");
            var algorithm = new QuadraticSieve(new QuadraticSieve.Config { Threads = 8 });
            var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
            Assert.IsTrue(factors.Length == 2);
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
            TestReduction(p, new BigIntegerReduction(),
                new MersenneTwisterBigInteger(0), value => value);
        }

        [TestMethod]
        public void TestRadix32IntegerReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new Word32IntegerReduction(),
                new MersenneTwisterBigInteger(0), value => value);
        }

        [TestMethod]
        public void TestMontgomeryReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new MontgomeryReduction(),
                new MersenneTwisterBigInteger(0), value => value);
        }

        [TestMethod]
        public void TestBarrettReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new BarrettReduction(),
                new MersenneTwisterBigInteger(0), value => value);
        }

        [TestMethod]
        public void TestUInt128Reduction()
        {
            var p = ulong.Parse("10023859281455311421");
            TestReduction(p, new UInt128Reduction(),
                new MersenneTwister64(0), value => (BigInteger)value);
        }

        [TestMethod]
        public void TestUInt128MontgomeryReduction()
        {
            TestReduction(ulong.Parse("46234103"), new UInt128MontgomeryReduction(),
                new MersenneTwister64(0), value => (BigInteger)value);
            TestReduction(ulong.Parse("259027704197601377"), new UInt128MontgomeryReduction(),
                new MersenneTwister64(0), value => (BigInteger)value);
        }

        private void TestReduction<TInteger>(TInteger p, IReductionAlgorithm<TInteger> reduction, IRandomNumberAlgorithm<TInteger> random, Func<TInteger, BigInteger> toBigInteger)
        {
            var reducer = reduction.GetReducer(p);
            var xPrime = reducer.ToResidue(p);
            var yPrime = reducer.ToResidue(p);
            var zPrime = reducer.ToResidue(p);
            for (int i = 0; i < 100; i++)
            {
                var x = random.Next(p);
                var y = random.Next(p);
                var z = toBigInteger(x) * toBigInteger(y);
                var expected = z % toBigInteger(p);

                xPrime.Set(x);
                yPrime.Set(y);
                zPrime.Set(xPrime).Multiply(yPrime);
                var actual = toBigInteger(zPrime.ToInteger());
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestRadix32()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var random = new MersenneTwisterBigInteger(0);
            var smallRandom = new MersenneTwister32(0);
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
                uint c = smallRandom.Next(0);
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
            var random = new MersenneTwisterBigInteger(0);
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
            var primes = new SieveOfErostothones();
            int iterations = 1000;
            for (int repetition = 1; repetition <= 2; repetition++)
            {
                int n = 0;
                int i = 0;
                foreach (int p in primes)
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
        }

        [TestMethod]
        public void TrialDivisionTest1()
        {
            var primalityAlgorithm = new TrialDivisionPrimality();
            var factorizationAlgorithm = new TrialDivisionFactorization();
            for (int n = 2; n < 10000; n++)
            {
                var factors = factorizationAlgorithm.Factor(n).ToArray();
                var product = factors.Aggregate((sofar, current) => sofar * current);
                Assert.AreEqual(n, product);
                Assert.IsTrue(factors.All(factor => IntegerMath.IsPrime(factor)));
                Assert.AreEqual(IntegerMath.IsPrime((BigInteger)n), primalityAlgorithm.IsPrime(n));
            }
        }

        const string matrix1 = @"
            1111111
            0000000
            0000011
            0011000
            ";

        const string matrix2 = @"
            11111111111111111000000000000000000000000000
            00000000000000000000000000000000000000000000
            00000111011100011001000010111100011100001110
            00110000010110101010011001110001101101000001
            00101001000000100000001100000100010000011100
            00000100100010000000100000000001000000101100
            01000000101000110000000000000010101011000010
            01010000000001000011000001100100001000010010
            00001000110000010010010010000001001000100010
            00000001100001000000101000000110000000100000
            10001000000000000000000000100000000001000000
            10100100000001000000000000001010000000000000
            00100001101000000100000000000010000000000000
            01010001001100001100000100000000100000000000
            00001000011100000001000100000000000000000000
            00000000000000000000000101000000000100001000
            00000100000000100000000001000000000000000001
            10000000000000000000000000000000000000000000
            10000000000000001001000000001000000100000000
            00000000000010010010000000000000000000000000
            01000001000011000000001000000000000000000000
            00010000000010000000000000000000000000000000
            00000010000000000000000000000000000000000010
            00000100000000001000000000000000000001000000
            00000000000000100000000000000000000100000000
            00000010000000000000000000100000000000010000
            00100010000000000000000000000000010000000000
            00010000000000000000000000010000000000100000
            00000000000000000000000000010000000000000000
            00000000010000000000000000001000000000000000
            00000000000000000000000000000001000000000000
            00000000000000010000000000000000000000001000
            00000000000000000000000000000000000000000000
            00000000000100000000100000000000100000000000
            ";

        [TestMethod]
        public void GaussianEliminationTest()
        {
            var solver = new GaussianElimination<BoolBitArray>(1);
            foreach (var text in new[] { matrix1, matrix2 })
            {
                var origMatrix = GetBitMatrix(GetLines(text));
                var matrix = new BoolBitMatrix(origMatrix);
                foreach (var v in solver.Solve(matrix))
                {
                    Assert.IsTrue(GaussianElimination<BoolBitArray>.IsSolutionValid(origMatrix, v));
                    Assert.IsTrue(GaussianElimination<BoolBitArray>.IsSolutionValid(matrix, v));
                }
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void StructuredGaussianEliminationTest()
        {
            var solver = new StructuredGaussianElimination<BoolBitArray, BoolBitMatrix>(1, 0, false);
            foreach (var text in new[] { matrix1, matrix2 })
            {
                var origMatrix = GetBitMatrix(GetLines(text));
                var matrix = new BoolBitMatrix(origMatrix);
                foreach (var v in solver.Solve(matrix))
                {
                    Assert.IsTrue(GaussianElimination<BoolBitArray>.IsSolutionValid(origMatrix, v));
                    Assert.IsTrue(GaussianElimination<BoolBitArray>.IsSolutionValid(matrix, v));
                }
                Console.WriteLine();
            }
        }

        private string[] GetLines(string text)
        {
            return text.Split('\n').Select(row => row.Trim()).Where(row => row != "").ToArray();
        }

        private IBitMatrix GetBitMatrix(string[] lines)
        {
            int rows = lines.Length;
            int cols = lines[0].Length;
            var matrix = new BoolBitMatrix(rows, cols);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for (int j = 0; j < line.Length; j++)
                    matrix[i, j] = line[j] == '1';
            }
            return matrix;
        }

        [TestMethod]
        public void BitLengthTest()
        {
            for (int i = 0; i < 256; i++)
            {
                ulong value = (ulong)i;
                int count = 0;
                while (value != 0)
                {
                    value >>= 1;
                    ++count;
                }
                Assert.AreEqual(count, i.GetBitLength());
                count += 0;
            }
        }

        [TestMethod]
        public void BitCountTest()
        {
            for (int i = 0; i < 256; i++)
            {
                ulong value = (ulong)i;
                int count = 0;
                while (value != 0)
                {
                    if ((value & 1) != 0)
                        ++count;
                    value >>= 1;
                }
                Assert.AreEqual(count, i.GetBitCount());
                count += 0;
            }
        }

        [TestMethod]
        public void ModularInverseTest()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var random = new MersenneTwisterBigInteger(0);
            for (int i = 0; i < 1000; i++)
            {
                var p = random.Next(n);
                var q = random.Next(n);
                while (!BigInteger.GreatestCommonDivisor(p, q).IsOne)
                {
                    p = random.Next(n);
                    q = random.Next(n);
                }
                var pInv = IntegerMath.ModularInverse(p, q);
                var result = p * pInv % q;
                Assert.AreEqual(BigInteger.One, result);
            }
        }

        [TestMethod]
        public void UInt128Test()
        {
            UInt128Test(20, 20);
            UInt128Test(20, 40);
            UInt128Test(20, 60);
            UInt128Test(40, 20);
            UInt128Test(40, 40);
            UInt128Test(40, 60);
            UInt128Test(60, 20);
            UInt128Test(60, 40);
            UInt128Test(60, 60);
        }

        private void UInt128Test(int factorSize, int modulusSize)
        {
            var random = new MersenneTwister64(0);
            var factorMax = (ulong)1 << factorSize;
            var modulusMax = (ulong)1 << modulusSize;
            for (int i = 0; i < 10000; i++)
            {
                var a = random.Next(factorMax);
                var b = random.Next(factorMax);
                var n = random.Next(modulusMax - 1) + 1;
                if ((n & 1) == 0)
                    ++n;
                var c = (UInt128)a * b;
                Assert.AreEqual((BigInteger)a * b, (BigInteger)c);
                Assert.AreEqual((BigInteger)a * b % n, c % n);
                Assert.AreEqual(BigInteger.ModPow(a, b, n), (BigInteger)IntegerMath.ModularPower(a, b, n));
            }
        }
    }
}
