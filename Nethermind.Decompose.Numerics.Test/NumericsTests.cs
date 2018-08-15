using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.Decompose.Numerics.Test
{
    [TestClass]
    public class NumericsTests
    {
        [TestMethod]
        public void TestSqrt1()
        {
            var p = BigInteger.Parse("12345678901234567890");
            var n = p * p;
            var p2 = IntegerMath.FloorSquareRoot(n);
            Assert.AreEqual(p, p2);
        }

        [TestMethod]
        public void TestSqrt2()
        {
            Assert.AreEqual(0, (int)IntegerMath.FloorSquareRoot(0));
            Assert.AreEqual(1, (int)IntegerMath.FloorSquareRoot(1));
            Assert.AreEqual(1, (int)IntegerMath.FloorSquareRoot(2));
            Assert.AreEqual(1, (int)IntegerMath.FloorSquareRoot(3));
            Assert.AreEqual(2, (int)IntegerMath.FloorSquareRoot(4));
            Assert.AreEqual(2, (int)IntegerMath.FloorSquareRoot(5));
            Assert.AreEqual(2, (int)IntegerMath.FloorSquareRoot(8));
            Assert.AreEqual(3, (int)IntegerMath.FloorSquareRoot(9));
            Assert.AreEqual(3, (int)IntegerMath.FloorSquareRoot(10));        
        }

        [TestMethod]
        public void TestSqrtByBits1()
        {
            var algorithm = new SqrtByBits();
            var p = BigInteger.Parse("12345678901234567890");
            var n = p * p;
            var p2 = algorithm.Sqrt(n);
            Assert.AreEqual(p, p2);
        }

        [TestMethod]
        public void TestPrimality1()
        {
            var algorithm = MillerRabin.Create(16, new Int32Reduction());
            var primes = Enumerable.Range(937, 1000 - 937)
                .Where(n => algorithm.IsPrime(n))
                .ToArray();
            var expected = new[] { 937, 941, 947, 953, 967, 971, 977, 983, 991, 997 };
            Assert.IsTrue(((IStructuralEquatable)primes).Equals(expected, EqualityComparer<int>.Default));
        }

        [TestMethod]
        public void TestPrimality2()
        {
            var algorithm = MillerRabin.Create(16, new UInt32MontgomeryReduction());
            Assert.IsTrue(new SieveOfEratosthenes().Take(10000).All(prime => algorithm.IsPrime((uint)prime)));
        }

        [TestMethod]
        public void TestPrimality3()
        {
            var primalityOld = new OldMillerRabin(16);
            TestPrimality3<int>(0, 10000, primalityOld, new Int32Reduction());
            //TestPrimality3<int>(0, 10000, primalityOld, new Int32MontgomeryReduction());
            TestPrimality3<int>(int.MaxValue - 10000 - 1, 10000, primalityOld, new Int32Reduction());
            //TestPrimality3<int>(int.MaxValue - 10000 - 1, 10000, primalityOld, new Int32MontgomeryReduction());
            TestPrimality3<uint>(0, 10000, primalityOld, new UInt32Reduction());
            TestPrimality3<uint>(0, 10000, primalityOld, new UInt32MontgomeryReduction());
            TestPrimality3<uint>(uint.MaxValue - 10000 - 1, 10000, primalityOld, new UInt32Reduction());
            TestPrimality3<uint>(uint.MaxValue - 10000 - 1, 10000, primalityOld, new UInt32MontgomeryReduction());
            TestPrimality3<long>(0, 10000, primalityOld, new Int64Reduction());
            //TestPrimality3<long>(0, 10000, primalityOld, new Int64MontgomeryReduction());
            TestPrimality3<long>(long.MaxValue - 10000 - 1, 10000, primalityOld, new Int64Reduction());
            //TestPrimality3<long>(long.MaxValue - 10000 - 1, 10000, primalityOld, new Int64MontgomeryReduction());
            TestPrimality3<ulong>(0, 10000, primalityOld, new UInt64Reduction());
            TestPrimality3<ulong>(0, 10000, primalityOld, new UInt64MontgomeryReduction());
            TestPrimality3<ulong>(ulong.MaxValue - 10000 - 1, 10000, primalityOld, new UInt64Reduction());
            TestPrimality3<ulong>(ulong.MaxValue - 10000 - 1, 10000, primalityOld, new UInt64MontgomeryReduction());
            TestPrimality3<UInt128>(0, 10000, primalityOld, new UInt128Reduction());
            TestPrimality3<UInt128>(0, 10000, primalityOld, new UInt128MontgomeryReduction());
            TestPrimality3<UInt128>(UInt128.MaxValue - 10000 - 1, 10000, primalityOld, new UInt128Reduction());
            TestPrimality3<UInt128>(UInt128.MaxValue - 10000 - 1, 10000, primalityOld, new UInt128MontgomeryReduction());
            TestPrimality3<BigInteger>(0, 10000, primalityOld, new BigIntegerReduction());
            TestPrimality3<BigInteger>(0, 10000, primalityOld, new BigIntegerMontgomeryReduction());
            TestPrimality3<BigInteger>((BigInteger)UInt128.MaxValue * UInt128.MaxValue, 10000, primalityOld, new BigIntegerReduction());
            TestPrimality3<BigInteger>((BigInteger)UInt128.MaxValue * UInt128.MaxValue, 10000, primalityOld, new BigIntegerMontgomeryReduction());
        }

        private void TestPrimality3<T>(T startValue, T countValue, IPrimalityAlgorithm<T> primalityOld, IReductionAlgorithm<T> reduction)
        {
            var start = (Number<T>)startValue;
            var count = (Number<T>)countValue;
            var primalityNew = MillerRabin.Create(16, reduction);
            for (var i = (Number<T>)0; i < count; i++)
            {
                var n = start + i;
                var isPrimeOld = primalityOld.IsPrime(n);
                var isPrimeNew = primalityNew.IsPrime(n);
                Assert.AreEqual(isPrimeOld, isPrimeNew);
            }
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
            var expected = new[] { ulong.Parse("400433141"), ulong.Parse("646868797") };
            var n = expected[0] * expected[1];
            var algorithm = new UInt64PollardRhoReduction(new UInt64Reduction());
            var factors = algorithm.Factor(n).OrderBy(factor => factor).ToArray();
            var product = factors.Aggregate((sofar, current) => sofar * current);
            Assert.AreEqual(n, product);
            Assert.IsTrue(((IStructuralEquatable)factors).Equals(expected, EqualityComparer<ulong>.Default));
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
        public void TestExtendedEuclideanAlgorithm()
        {
            var a = (BigInteger)120;
            var b = (BigInteger)23;
            var cExpected = (BigInteger)(-9);
            var dExpected = (BigInteger)47;
            BigInteger c;
            BigInteger d;
            IntegerMath.ExtendedEuclideanAlgorithm(a, b, out c, out d);
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
        public void TestMutableIntegerIntegerReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new MutableIntegerReduction());
        }

        [TestMethod]
        public void TestMontgomeryReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new BigIntegerMontgomeryReduction());
        }

        [TestMethod]
        public void TestBarrettReduction()
        {
            var p = BigInteger.Parse("10023859281455311421");
            TestReduction(p, new BarrettReduction());
        }

        [TestMethod]
        public void TestUInt64Reduction()
        {
            var p = ulong.Parse("10023859281455311421");
            TestReduction(p, new UInt64Reduction());
        }

        [TestMethod]
        public void TestUInt32MontgomeryReduction()
        {
            TestReduction(uint.MaxValue, new UInt32MontgomeryReduction());
            TestReduction((uint)1 << 16, new UInt32MontgomeryReduction());
            TestReduction((uint)1 << 8, new UInt32MontgomeryReduction());
        }

        [TestMethod]
        public void TestUInt64MontgomeryReduction()
        {
            TestReduction(ulong.Parse("259027704197601377"), new UInt64MontgomeryReduction());
        }

        private void TestReduction<T>(Number<T> max, IReductionAlgorithm<T> reduction)
        {
            var random = new RandomInteger<T>(0);
            for (int j = 0; j < 100; j++)
            {
                var p = random.Next(max) | 1;
                var reducer = reduction.GetReducer(p);
                var xBar = reducer.ToResidue(p);
                var yBar = reducer.ToResidue(p);
                var zBar = reducer.ToResidue(p);
                for (int i = 0; i < 100; i++)
                {
                    var x = random.Next(p);
                    var y = random.Next(p);

                    xBar.Set(x);
                    yBar.Set(y);
                    zBar.Set(xBar).Multiply(yBar);
                    Assert.AreEqual((BigInteger)x * y % p, (Number<T>)zBar.Value);

                    xBar.Set(x);
                    zBar.Set(xBar).Power(y);
                    Assert.AreEqual(BigInteger.ModPow(x, y, p), (Number<T>)zBar.Value);
                }
            }
        }

        [TestMethod]
        public void TestMutableInteger1()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var store = new MutableIntegerStore(1);
            var a = store.Allocate();
            var b = store.Allocate();
            var x = store.Allocate();
            for (int i = -10; i < 10; i++)
            {
                for (int j = -10; j < 10; j++)
                {
                    a.Set(i);
                    Assert.AreEqual(i, (int)a);
                    Assert.AreEqual((BigInteger)i, (BigInteger)a);
                    Assert.AreEqual(i.GetBitLength(), a.GetBitLength());
                    b.Set(j);
                    Assert.AreEqual(j, (int)b);
                    Assert.AreEqual((BigInteger)j, (BigInteger)b);
                    Assert.AreEqual(j.GetBitLength(), j.GetBitLength());

                    x.Set((BigInteger)i);
                    Assert.AreEqual(i, (int)x);

                    for (int k = 0; k < 10; k++)
                    {
                        x.Set(a).LeftShift(k);
                        Assert.AreEqual(i << k, (int)x);
                        x.Set(a).RightShift(k);
                        Assert.AreEqual(i >> k, (int)x);
                    }

                    x.SetAnd(a, b, store);
                    Assert.AreEqual(i & j, (int)x);
                    x.SetOr(a, b, store);
                    Assert.AreEqual(i | j, (int)x);
                    x.SetExclusiveOr(a, b, store);
                    Assert.AreEqual(i ^ j, (int)x);
                    x.SetNot(a);
                    Assert.AreEqual(~i, (int)x);

                    x.SetSum(a, b);
                    Assert.AreEqual(i + j, (int)x);
                    x.SetDifference(a, b);
                    Assert.AreEqual(i - j, (int)x);
                    x.SetProduct(a, b);
                    Assert.AreEqual(i * j, (int)x);
                    if (j != 0)
                    {
                        x.SetQuotient(a, b, store);
                        Assert.AreEqual(i / j, (int)x);
                        x.SetRemainder(a, b);
                        Assert.AreEqual(i % j, (int)x);
                    }
                    x.Set(a).Negate();
                    Assert.AreEqual(-i, (int)x);
                    x.Set(a).AbsoluteValue();
                    Assert.AreEqual(Math.Abs(i), (int)x);

                    x.Set(a).Increment();
                    Assert.AreEqual(i + 1, (int)x);
                    x.Set(a).Decrement();
                    Assert.AreEqual(i - 1, (int)x);
                    x.SetSum(a, j);
                    Assert.AreEqual(i + j, (int)x);
                    x.SetDifference(a, j);
                    Assert.AreEqual(i - j, (int)x);
                    x.SetProduct(a, j);
                    Assert.AreEqual(i * j, (int)x);
                    x.SetProduct(a, j);
                    Assert.AreEqual(i * j, (int)x);
                    if (j != 0)
                    {
                        x.SetQuotient(a, j, store);
                        Assert.AreEqual(i / j, (int)x);
                        x.SetQuotient(a, j, store);
                        Assert.AreEqual(i / j, (int)x);
                        var remainder = a.GetRemainder(j);
                        Assert.AreEqual(i % j, remainder);
                    }

                    Assert.AreEqual(i == j, a == b);
                    Assert.AreEqual(i != j, a != b);
                    Assert.AreEqual(i < j, a < b);
                    Assert.AreEqual(i <= j, a <= b);
                    Assert.AreEqual(i > j, a > b);
                    Assert.AreEqual(i >= j, a >= b);

                    Assert.AreEqual(i == j, a == j);
                    Assert.AreEqual(i != j, a != j);
                    Assert.AreEqual(i < j, a < j);
                    Assert.AreEqual(i <= j, a <= j);
                    Assert.AreEqual(i > j, a > j);
                    Assert.AreEqual(i >= j, a >= j);

                    Assert.AreEqual(i == j, i == b);
                    Assert.AreEqual(i != j, i != b);
                    Assert.AreEqual(i < j, i < b);
                    Assert.AreEqual(i <= j, i <= b);
                    Assert.AreEqual(i > j, i > b);
                    Assert.AreEqual(i >= j, i >= b);
                }
            }
        }

        [TestMethod]
        public void TestMutableInteger2()
        {
            for (int i = -10; i < 10; i++)
            {
                for (int j = -10; j < 10; j++)
                {
                    Assert.AreEqual((BigInteger)i & j, (int)((MutableInteger)i & j));
                    Assert.AreEqual((BigInteger)i | j, (int)((MutableInteger)i | j));
                    Assert.AreEqual((BigInteger)i ^ j, (int)((MutableInteger)i ^ j));
                    Assert.AreEqual(~(BigInteger)i, (int)(~(MutableInteger)i));

                    Assert.AreEqual((BigInteger)i + j, (int)((MutableInteger)i + j));
                    Assert.AreEqual((BigInteger)i - j, (int)((MutableInteger)i - j));
                    Assert.AreEqual((BigInteger)i * j, (int)((MutableInteger)i * j));
                    if (j != 0)
                    {
                        Assert.AreEqual((BigInteger)i / j, (int)((MutableInteger)i / j));
                        Assert.AreEqual((BigInteger)i % j, (int)((MutableInteger)i % j));
                    }
                    Assert.AreEqual(-(BigInteger)i, (int)(-(MutableInteger)i));
                }
            }
        }

        [TestMethod]
        public void TestMutableInteger3()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var generator = new MersenneTwister(0);
            var random = generator.Create<BigInteger>();
            var smallRandom = generator.Create<uint>();
            var length = (n.GetBitLength() * 2 + 31) / 32 + 3;
            var store = new MutableIntegerStore(1);
            var a = store.Allocate();
            var b = store.Allocate();
            var x = store.Allocate();
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
                    Assert.AreEqual(aPrime << j, (BigInteger)x);

                    x.Set(a).RightShift(j);
                    Assert.AreEqual(aPrime >> j, (BigInteger)x);
                }

                x.SetSum(a, b);
                Assert.AreEqual(aPrime + bPrime, (BigInteger)x);

                x.SetSum(a, c);
                Assert.AreEqual(aPrime + c, (BigInteger)x);

                x.SetProduct(a, b);
                Assert.AreEqual(aPrime * bPrime, (BigInteger)x);

                x.SetSquare(a);
                Assert.AreEqual(aPrime * aPrime, (BigInteger)x);

                x.SetProduct(a, c);
                Assert.AreEqual(c * aPrime, (BigInteger)x);

                x.SetQuotient(a, b, store);
                Assert.AreEqual(aPrime / bPrime, (BigInteger)x);

                x.SetRemainder(a, b);
                Assert.AreEqual(aPrime % bPrime, (BigInteger)x);

                x.SetQuotient(a, c, store);
                Assert.AreEqual(aPrime / c, (BigInteger)x);

                x.SetRemainder(a, c);
                Assert.AreEqual(aPrime % c, (BigInteger)x);

                x.SetSquare(a);
                Assert.AreEqual(aPrime * aPrime, (BigInteger)x);

                x.SetDifference(a, b);
                Assert.AreEqual(aPrime - bPrime, (BigInteger)x);

                x.SetGreatestCommonDivisor(a, b, store);
                Assert.AreEqual(BigInteger.GreatestCommonDivisor(aPrime, bPrime), (BigInteger)x);

                if (x.IsOne)
                {
                    x.SetModularInverse(a, b, store);
                    Assert.AreEqual(IntegerMath.ModularInverse(aPrime, bPrime), (BigInteger)x);
                }
            }
        }

        [TestMethod]
        public void ModularSquareRootTest1()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
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
            int length = 20;
            var a = new MutableInteger(length);
            var b = new MutableInteger(length);
            var c = new MutableInteger(length);
            var x = new MutableInteger(length);
            a.Set(BigInteger.Parse("851968723384911158384830467125731460171903460330379450819468842227482878637917031244505597763225"));
            b.Set(BigInteger.Parse("2200761205517100656206929789365760219952611739831"));
            x.SetRemainder(a, b);
            Assert.AreEqual((BigInteger)a % (BigInteger)b, (BigInteger)x);
        }

        [TestMethod]
        public void SieveOfEratosthenesTest1()
        {
            var primes = new SieveOfEratosthenes();
            int iterations = 1 << 18;
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
            ModularInverseTest(int.MaxValue);
            ModularInverseTest(uint.MaxValue);
            ModularInverseTest(long.MaxValue);
            ModularInverseTest(ulong.MaxValue);
            ModularInverseTest(BigInteger.Parse("10023859281455311421"));
        }

        public void ModularInverseTest<T>(T max)
        {
            var random = new RandomInteger<T>(0);
            for (int i = 0; i < 1000; i++)
            {
                var q = random.Next(max);
                var p = random.Next(q);
                while (!Number<T>.GreatestCommonDivisor(p, q).IsOne)
                {
                    q = random.Next(max);
                    p = random.Next(q);
                }
                var r = Number<T>.FloorRoot(random.Next(q), 2);
                r -= r % p;
                var pInv = Number<T>.ModularInverse(p, q);
                var result = Number<T>.ModularProduct(p, pInv, q);
                Assert.AreEqual(Number<T>.One, result);
                Assert.AreEqual(r / p % q, Number<T>.ModularProduct(r, pInv, q));
            }
        }

        [TestMethod]
        public void ModularOperationsTest()
        {
            ModularOperationsTest((ulong)1 << 20, (ulong)1 << 20);
            ModularOperationsTest((ulong)1 << 40, (ulong)1 << 20);
            ModularOperationsTest((ulong)1 << 60, (ulong)1 << 20);

            ModularOperationsTest((ulong)1 << 20, (ulong)1 << 40);
            ModularOperationsTest((ulong)1 << 40, (ulong)1 << 40);
            ModularOperationsTest((ulong)1 << 60, (ulong)1 << 40);

            ModularOperationsTest((ulong)1 << 20, (ulong)1 << 60);
            ModularOperationsTest((ulong)1 << 40, (ulong)1 << 60);
            ModularOperationsTest((ulong)1 << 60, (ulong)1 << 60);

            ModularOperationsTest(ulong.MaxValue, ulong.MaxValue);
        }

        private void ModularOperationsTest(ulong factorMax, ulong modulusMax)
        {
            var random = new MersenneTwister(0).Create<ulong>();
            for (int i = 0; i < 10000; i++)
            {
                var n = random.Next(modulusMax - 1) + 1;
                var a = random.Next(factorMax) % n;
                var b = random.Next(factorMax) % n;
                var s = (int)(b % 32);
                Assert.AreEqual(((BigInteger)a + b) % n, IntegerMath.ModularSum(a, b, n));
                Assert.AreEqual((((BigInteger)a - b) % n + n) % n, IntegerMath.ModularDifference(a, b, n));
                Assert.AreEqual((BigInteger)a * b % n, IntegerMath.ModularProduct(a, b, n));
            }
            var sum = 0;
            for (int i = 0; i < 1000; i++)
            {
                var n = random.Next(modulusMax - 1) + 1;
                var a = random.Next(factorMax) % n;
                var b = random.Next(factorMax) % n;
                Assert.AreEqual(BigInteger.ModPow(a, b, n), IntegerMath.ModularPower(a, b, n));
                ++sum;
            }
        }

        [TestMethod]
        public void ModularPowerOfTwoBug()
        {
            var value = (ulong)310322;
            var exponent = (ulong)647414;
            var s = 5;
            var result = IntegerMath.ModularPowerPowerOfTwoModulus(value, exponent, s);
            Assert.AreEqual((ulong)0, result);
        }

        [TestMethod]
        public void ModularPowerEvenBug()
        {
            var a = (ulong)310322;
            var b = (ulong)647414;
            var n = (ulong)507446567392;
            var result = IntegerMath.ModularPower(a, b, n);
            Assert.AreEqual((ulong)448890944160, result);
        }

        [TestMethod]
        public void ModularPowerOfTwo()
        {
            var random = new MersenneTwister(0).Create<ulong>();
            var modulusMax = ulong.MaxValue;
            for (int i = 0; i < 10000; i++)
            {
                var modulus = random.Next(modulusMax - 1);
                var exponent = random.Next(modulus);
                Assert.AreEqual(BigInteger.ModPow(2, exponent, modulus), IntegerMath.ModularPowerOfTwo(exponent, modulus));
            }
        }

        [TestMethod]
        public void FloorRootTest1()
        {
            var max = IntegerMath.Power((BigInteger)2, 3 * 64);
            var random = new MersenneTwister(0).Create<BigInteger>();
            for (var i = 0; i < 100000; i++)
            {
                var x = random.Next(max);
                var n = (int)random.Next(20 - 2) + 2;
                var root = IntegerMath.FloorRoot(x, n);
                Assert.IsTrue(IntegerMath.Power(root, n) <= x && IntegerMath.Power(root + 1, n) > x);
            }
        }

        [TestMethod]
        public void FloorRootTest2()
        {
            // FloorRootCore guesses do not converge:
            // 1) 4
            // 2) 3
            // 3) 207
            var n = IntegerMath.Power((BigInteger)2, 52) - 1;
            var root = IntegerMath.FloorRoot(n, 26);
            Assert.AreEqual((BigInteger)3, root);
        }

        [TestMethod]
        public void PerfectPowerTest1()
        {
            var max = IntegerMath.Power((BigInteger)2, 3 * 64);
            var random = new MersenneTwister(0).Create<BigInteger>();
            for (var i = 0; i < 1000; i++)
            {
                var n = (int)random.Next(20 - 1) + 1;
                var x = IntegerMath.NextPrime(IntegerMath.FloorRoot(random.Next(max), n));
                var y = IntegerMath.Power(x, n);
                var power = IntegerMath.PerfectPower(y);
                Assert.AreEqual(n, power);
            }
        }

        private BigInteger[] piData =
        {
            BigInteger.Parse("0"),
            BigInteger.Parse("1"),
            BigInteger.Parse("2"),
            BigInteger.Parse("4"),
            BigInteger.Parse("6"),
            BigInteger.Parse("11"),
            BigInteger.Parse("18"),
            BigInteger.Parse("31"),
            BigInteger.Parse("54"),
            BigInteger.Parse("97"),
            BigInteger.Parse("172"),
            BigInteger.Parse("309"),
            BigInteger.Parse("564"),
            BigInteger.Parse("1028"),
            BigInteger.Parse("1900"),
            BigInteger.Parse("3512"),
            BigInteger.Parse("6542"),
            BigInteger.Parse("12251"),
            BigInteger.Parse("23000"),
            BigInteger.Parse("43390"),
            BigInteger.Parse("82025"),
            BigInteger.Parse("155611"),
            BigInteger.Parse("295947"),
            BigInteger.Parse("564163"),
            BigInteger.Parse("1077871"),
            BigInteger.Parse("2063689"),
            BigInteger.Parse("3957809"),
            BigInteger.Parse("7603553"),
            BigInteger.Parse("14630843"),
            BigInteger.Parse("28192750"),
            BigInteger.Parse("54400028"),
            BigInteger.Parse("105097565"),
            BigInteger.Parse("203280221"),
            BigInteger.Parse("393615806"),
            BigInteger.Parse("762939111"),
            BigInteger.Parse("1480206279"),
            BigInteger.Parse("2874398515"),
            BigInteger.Parse("5586502348"),
            BigInteger.Parse("10866266172"),
            BigInteger.Parse("21151907950"),
            BigInteger.Parse("41203088796"),
            BigInteger.Parse("80316571436"),
            BigInteger.Parse("156661034233"),
            BigInteger.Parse("305761713237"),
            BigInteger.Parse("597116381732"),
            BigInteger.Parse("1166746786182"),
            BigInteger.Parse("2280998753949"),
            BigInteger.Parse("4461632979717"),
            BigInteger.Parse("8731188863470"),
            BigInteger.Parse("17094432576778"),
            BigInteger.Parse("33483379603407"),
            BigInteger.Parse("65612899915304"),
            BigInteger.Parse("128625503610475"),
            BigInteger.Parse("252252704148404"),
            BigInteger.Parse("494890204904784"),
            BigInteger.Parse("971269945245201"),
            BigInteger.Parse("1906879381028850"),
            BigInteger.Parse("3745011184713964"),
            BigInteger.Parse("7357400267843990"),
            BigInteger.Parse("14458792895301660"),
            BigInteger.Parse("28423094496953330"),
            BigInteger.Parse("55890484045084135"),
            BigInteger.Parse("109932807585469973"),
            BigInteger.Parse("216289611853439384"),
            BigInteger.Parse("425656284035217743"),
            BigInteger.Parse("837903145466607212"),
            BigInteger.Parse("1649819700464785589"),
            BigInteger.Parse("3249254387052557215"),
            BigInteger.Parse("6400771597544937806"),
            BigInteger.Parse("12611864618760352880"),
            BigInteger.Parse("24855455363362685793"),
            BigInteger.Parse("48995571600129458363"),
            BigInteger.Parse("96601075195075186855"),
            BigInteger.Parse("190499823401327905601"),
            BigInteger.Parse("375744164937699609596"),
            BigInteger.Parse("741263521140740113483"),
        };

        [TestMethod]
        public void PrimeCollectionTest1()
        {
            for (var j = 0; j <= 24; j++)
            {
                var n = 1 << j;
                var primes = new PrimeCollection(n + 1, 0);
                Assert.AreEqual(piData[j], primes.Count);
            }
        }

        [TestMethod]
        public void PrimeCollectionTest2()
        {
            var algorithm = new PrimeCounting(0);
            for (var n = 1 << 10; n <= (1 << 11); n++)
            {
                var parity = algorithm.ParityOfPi(n);
                var primes = new PrimeCollection(n + 1, 0);
                Assert.AreEqual(parity, primes.Count % 2);
            }
        }

        [TestMethod]
        public void PrimeCountingTest1()
        {
            var algorithm = new PrimeCounting(0);
            for (var j = 0; j <= 24; j++)
            {
                var n = 1 << j;
                Assert.AreEqual(piData[j], (BigInteger)algorithm.Pi(n));
            }
        }

        [TestMethod]
        public void PrimeCountingTest2()
        {
            var algorithm = new PrimeCounting(0);
            for (var j = 0; j <= 24; j++)
            {
                var n = 1 << j;
                Assert.AreEqual(piData[j] % 2, algorithm.ParityOfPi(n));
            }
        }

        [TestMethod]
        public void PrimeCountingTest3()
        {
            var algorithm = new PrimeCounting(0);
            for (var j = 0; j <= 40; j++)
            {
                var n = (BigInteger)1 << j;
                Assert.AreEqual(piData[j] % 2, algorithm.ParityOfPi(n));
            }
        }

        [TestMethod]
        public void PrimeCountingTest4()
        {
            var algorithm = new PrimeCounting(8);
            for (var j = 0; j <= 40; j++)
            {
                var n = (BigInteger)1 << j;
                Assert.AreEqual(piData[j] % 2, algorithm.ParityOfPi(n));
            }
        }

        [TestMethod]
        public void MobiusCollectionTest1()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusCollection(n, threads);
                for (int i = 1; i < n; i++)
                    Assert.AreEqual(IntegerMath.Mobius(i), mobius[i]);
            }
        }

        [TestMethod]
        public void MobiusRangeTest1()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusRange(n, threads);
                var values = new sbyte[mobius.Size];
                mobius.GetValues(1, mobius.Size, values);
                for (int i = 1; i < n; i++)
                    Assert.AreEqual(IntegerMath.Mobius(i), values[i - 1]);
            }
        }

        [TestMethod]
        public void MobiusRangeTest2()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusRange(n, threads);
                var batchSize = 100;
                var values = new sbyte[batchSize];
                for (var k = 1; k < mobius.Size; k += batchSize)
                {
                    var kmax = Math.Min(k + batchSize, mobius.Size);
                    mobius.GetValues(k, kmax, values);
                    var length = kmax - k;
                    for (int i = 0; i < length; i++)
                        Assert.AreEqual(IntegerMath.Mobius(i + k), values[i]);
                }
            }
        }

        [TestMethod]
        public void MobiusRangeTest3()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusRange(n + 1, 8);
                var batchSize = 1 << 16;
                var values = new sbyte[batchSize];
                var sum = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    mobius.GetValues(kmin, kmax, values);
                    var length = kmax - kmin;
                    for (var i = 0; i < length; i++)
                        sum += values[i];
                }
                Assert.AreEqual(sum, MertensFunction.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void MobiusOddRangeTest1()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusOddRange(n | 1, threads);
                var values = new sbyte[n >> 1];
                mobius.GetValues(1, n | 1, values);
                for (int i = 1; i < n; i += 2)
                    Assert.AreEqual(IntegerMath.Mobius(i), values[i >> 1]);
            }
        }

        [TestMethod]
        public void MobiusOddRangeTest2()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusOddRange(n | 1, threads);
                var batchSize = 100;
                var values = new sbyte[batchSize >> 1];
                for (var k = 1; k < n; k += batchSize)
                {
                    var kmin = k;
                    var kmax = Math.Min(kmin + batchSize, n | 1);
                    mobius.GetValues(k, kmax, values);
                    var length = kmax - kmin;
                    for (int i = 0; i < length; i += 2)
                        Assert.AreEqual(IntegerMath.Mobius(i + kmin), values[i >> 1]);
                }
            }
        }

        [TestMethod]
        public void MobiusOddRangeTest3()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusOddRange(n + 1, 8);
                var batchSize = 1 << 16;
                var values = new sbyte[batchSize >> 1];
                var sum = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    mobius.GetValues(kmin, kmax | 1, values);
                    var length = kmax - kmin;
                    for (var i = 0; i < length; i += 2)
                        sum += values[i >> 1];
                }
                Assert.AreEqual(sum, MertensFunctionOdd.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void MobiusOddRangeTest4()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusOddRange(n + 1, 8);
                var batchSize = 1 << 16;
                var values = new sbyte[batchSize >> 1];
                var sums = new int[batchSize >> 1];
                var m0 = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    m0 = mobius.GetValuesAndSums(kmin, kmax | 1, values, sums, m0);
                }
                Assert.AreEqual(m0, MertensFunctionOdd.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void MobiusOddRangeTest5()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusOddRange(n + 1, 8);
                var batchSize = 1 << 16;
                var sums = new int[batchSize >> 1];
                var m0 = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    m0 = mobius.GetSums(kmin, kmax | 1, sums, m0);
                }
                Assert.AreEqual(m0, MertensFunctionOdd.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void MobiusRangeAdditiveTest1()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusRangeAdditive(n, threads);
                var values = new sbyte[mobius.Size];
                mobius.GetValues(1, mobius.Size, values);
                for (int i = 1; i < n; i++)
                    Assert.AreEqual(IntegerMath.Mobius(i), values[i - 1]);
            }
        }

        [TestMethod]
        public void MobiusRangeAdditiveTest2()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusRangeAdditive(n, threads);
                var batchSize = 100;
                var values = new sbyte[batchSize];
                for (var k = 1; k < mobius.Size; k += batchSize)
                {
                    var kmax = Math.Min(k + batchSize, mobius.Size);
                    mobius.GetValues(k, kmax, values);
                    var length = kmax - k;
                    for (int i = 0; i < length; i++)
                        Assert.AreEqual(IntegerMath.Mobius(i + k), values[i]);
                }
            }
        }

        [TestMethod]
        public void MobiusRangeAdditiveTest3()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusRangeAdditive(n + 1, 8);
                var batchSize = 1 << 16;
                var values = new sbyte[batchSize];
                var sum = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    mobius.GetValues(kmin, kmax, values);
                    var length = kmax - kmin;
                    for (var i = 0; i < length; i++)
                        sum += values[i];
                }
                Assert.AreEqual(sum, MertensFunction.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void MobiusRangeAdditiveTest4()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusRangeAdditive(n + 1, 8);
                var batchSize = 1 << 16;
                var values = new sbyte[batchSize];
                var sums = new int[batchSize];
                var m0 = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    m0 = mobius.GetValuesAndSums(kmin, kmax, values, sums, m0);
                }
                Assert.AreEqual(m0, MertensFunction.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void MobiusOddRangeAdditiveTest1()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusOddRangeAdditive(n | 1, threads);
                var values = new sbyte[n >> 1];
                mobius.GetValues(1, n | 1, values);
                for (int i = 1; i < n; i += 2)
                    Assert.AreEqual(IntegerMath.Mobius(i), values[i >> 1]);
            }
        }

        [TestMethod]
        public void MobiusOddRangeAdditiveTest2()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var mobius = new MobiusOddRangeAdditive(n | 1, threads);
                var batchSize = 100;
                var values = new sbyte[batchSize >> 1];
                for (var k = 1; k < n; k += batchSize)
                {
                    var kmin = k;
                    var kmax = Math.Min(kmin + batchSize, n | 1);
                    mobius.GetValues(k, kmax, values);
                    var length = kmax - kmin;
                    for (int i = 0; i < length; i += 2)
                        Assert.AreEqual(IntegerMath.Mobius(i + kmin), values[i >> 1]);
                }
            }
        }

        [TestMethod]
        public void MobiusOddRangeAdditiveTest3()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusOddRangeAdditive(n + 1, 8);
                var batchSize = 1 << 16;
                var values = new sbyte[batchSize >> 1];
                var sum = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    mobius.GetValues(kmin, kmax | 1, values);
                    var length = kmax - kmin;
                    for (var i = 0; i < length; i += 2)
                        sum += values[i >> 1];
                }
                Assert.AreEqual(sum, MertensFunctionOdd.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void MobiusOddRangeAdditiveTest4()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusOddRangeAdditive(n + 1, 8);
                var batchSize = 1 << 16;
                var values = new sbyte[batchSize >> 1];
                var sums = new int[batchSize >> 1];
                var m0 = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    m0 = mobius.GetValuesAndSums(kmin, kmax | 1, values, sums, m0);
                }
                Assert.AreEqual(m0, MertensFunctionOdd.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void MobiusOddRangeAdditiveTest5()
        {
            for (var j = 6; j <= 8; j++)
            {
                var n = IntegerMath.Power((long)10, j);
                var mobius = new MobiusOddRangeAdditive(n + 1, 8);
                var batchSize = 1 << 16;
                var sums = new int[batchSize >> 1];
                var m0 = 0;
                for (var kmin = (long)1; kmin < n; kmin += batchSize)
                {
                    var kmax = Math.Min(kmin + batchSize, mobius.Size);
                    m0 = mobius.GetSums(kmin, kmax | 1, sums, m0);
                }
                Assert.AreEqual(m0, MertensFunctionOdd.PowerOfTen(j));
            }
        }

        [TestMethod]
        public void DivisorCollectionTest1()
        {
            var n = 1 << 10;
            var divisors = new DivisorsCollection(n);
            for (int i = 1; i < n; i++)
                Assert.AreEqual(IntegerMath.NumberOfDivisors(i), divisors[i]);
        }

        [TestMethod]
        public void DivisorRangeTest1()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var divisors = new DivisorRange(n, threads);
                var values = new ushort[n];
                divisors.GetValues(1, n, values);
                for (int i = 1; i < n; i++)
                    Assert.AreEqual(IntegerMath.NumberOfDivisors(i), values[i - 1]);
            }
        }

        [TestMethod]
        public void DivisorRangeTest2()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var divisors = new DivisorRange(n, threads);
                var batchSize = 100;
                var values = new ushort[batchSize];
                for (var k = 1; k < n; k += batchSize)
                {
                    var kmin = k;
                    var kmax = Math.Min(kmin + batchSize, n);
                    divisors.GetValues(k, kmax, values);
                    var length = kmax - kmin;
                    for (int i = 0; i < length; i++)
                        Assert.AreEqual(IntegerMath.NumberOfDivisors(i + kmin), values[i]);
                }
            }
        }

        [TestMethod]
        public void DivisorRangeTest3()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var divisors = new DivisorRange(n, threads);
                var batchSize = 100;
                var values = new ushort[batchSize];
                var sums = new ulong[batchSize];
                var sum0 = (ulong)0;
                for (var k = 1; k < n; k += batchSize)
                {
                    var kmin = k;
                    var kmax = Math.Min(kmin + batchSize, n);
                    sum0 = divisors.GetValuesAndSums(kmin, kmax, values, sums, sum0);
                    var length = kmax - kmin;
                    for (int i = 0; i < length; i++)
                    {
                        Assert.AreEqual((ushort)IntegerMath.NumberOfDivisors(i + kmin), values[i]);
                        Assert.AreEqual((ulong)IntegerMath.SumOfNumberOfDivisors(i + kmin), sums[i]);
                    }
                }
            }
        }

        [TestMethod]
        public void DivisorRangeTest4()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var divisors = new DivisorRange(n, threads);
                var batchSize = 100;
                var values = new ushort[batchSize];
                var sums = new ulong[batchSize];
                var sum0 = (ulong)0;
                for (var k = 1; k < n; k += batchSize)
                {
                    var kmin = k;
                    var kmax = Math.Min(kmin + batchSize, n);
                    sum0 = divisors.GetSums(k, kmax, sums, sum0);
                    var length = kmax - kmin;
                    for (int i = 0; i < length; i++)
                        Assert.AreEqual((ulong)IntegerMath.SumOfNumberOfDivisors(i + kmin), sums[i]);
                }
            }
        }

        [TestMethod]
        public void DivisorOddRangeTest1()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var divisors = new DivisorOddRange(n | 1, threads);
                var values = new ushort[n];
                divisors.GetValues(1, n | 1, values);
                for (int i = 1; i < n; i += 2)
                    Assert.AreEqual(IntegerMath.NumberOfDivisors(i), values[i >> 1]);
            }
        }

        [TestMethod]
        public void DivisorOddRangeTest2()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var divisors = new DivisorOddRange(n | 1, threads);
                var sums = new ulong[n];
                divisors.GetSums(1, n | 1, sums, 0);
                for (int i = 1; i < n; i += 2)
                    Assert.AreEqual((ulong)IntegerMath.SumOfNumberOfDivisorsOdd(i), sums[i >> 1]);
            }
        }

        [TestMethod]
        public void DivisorOddRangeAdditiveTest1()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var divisors = new DivisorOddRangeAdditive(n | 1, threads);
                var values = new ushort[n >> 1];
                divisors.GetValues(1, n | 1, values);
                for (int i = 1; i < n; i += 2)
                    Assert.AreEqual(IntegerMath.NumberOfDivisors(i), values[i >> 1]);
            }
        }

        [TestMethod]
        public void DivisorOddRangeAdditiveTest2()
        {
            for (var threads = 0; threads < 4; threads++)
            {
                var n = 1 << 10;
                var divisors = new DivisorOddRangeAdditive(n | 1, threads);
                var sums = new ulong[n >> 1];
                divisors.GetSums(1, n | 1, sums, 0);
                for (int i = 1; i < n; i += 2)
                    Assert.AreEqual((ulong)IntegerMath.SumOfNumberOfDivisorsOdd(i), sums[i >> 1]);
            }
        }
    }
}
