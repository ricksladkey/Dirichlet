﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Decompose.Numerics;
#if GNU_MP
using BigInt = Gnu.MP.Integer;
#endif
using System.Threading.Tasks;
using System.Threading;
using Dirichlet.Numerics;

namespace Sandbox
{
    class Program
    {
        static TextWriter output;

        static void Main(string[] args)
        {
            output = new ConsoleLogger("Decompose.log");
            try
            {
                //Montgomery128Test();
                //ParityTest();
                //Modular128Test();
                //SquareFreeCountingTest();
                //Operator128PerformanceTest();
                //GreatestCommonDivisorPerformanceTest();
                //DivisorsPerformanceTest();
                //ModularSumTest();
                //DivisorSummatoryFunctionOddTest();
                //MertensPerformanceTest();
                //MertensFormulaTest();
                //PiMod2PerformanceTest();
                //PiMod2PerformanceTestPowerOfTwo();
                //PiMod3PerformanceTest();
                //PerfectPowerTest();
                //FloorRootTest();
                //FindPrimeTest1();
                //BarrettReductionTest1();
                //BarrettReductionTest2();
                //MutableIntegerTest1();
                //FactorTest1();
                //FactorTest2();
                //FactorTest3();
                //FactorTest4();
                //MsieveTest();
                //FactorTest6();
                //QuadraticSieveParametersTest();
                //QuadraticSieveStandardTest();
                //QuadraticSieveDebugTest();
                //QuadraticSieveFactorTest();
                //CunninghamTest();
                //GaussianEliminationTest1();
                //CreateSamplesTest();
                //GraphTest();
                //UInt128Test();
                //ModularInverseTest1();
                //ModularInverseTest2();
                //PrimalityTest();
                //OperationsTest();
                //DivisionTest1();
                //DivisionTest2();
                //MertensPrimorialTest();
                //DivisorCubeTest1();
                //DivisorCubeTest2();
                DivisorCubeTest3();
            }
            catch (AggregateException ex)
            {
                HandleException(ex);
                ex.Handle(HandleException);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        static bool HandleException(Exception ex)
        {
            output.WriteLine("Exception: {0}", ex.Message);
            output.WriteLine("Stack trace:");
            output.WriteLine(ex.StackTrace);
            return true;
        }

        static void DivisorCubeTest1()
        {
            for (var n = 1; n <= 50; n++)
            {
                var sqrtn = IntegerMath.FloorSquareRoot(n);
                var cubed = IntegerMath.Cube(n);
                var h = IntegerMath.NumberOfDivisors(cubed);
                var f1 = 0;
                foreach (var factor in IntegerMath.Factors(n))
                {
                    f1 += IntegerMath.Square(IntegerMath.Mobius(factor)) *
                        IntegerMath.NumberOfDivisors(factor) *
                        IntegerMath.NumberOfDivisors(n / factor);
                }
                var f2 = 0;
                for (var d = 1; d <= sqrtn; d++)
                {
                    var dsquared = d * d;
                    if (n % dsquared == 0)
                    {
                        var mu = IntegerMath.Mobius(d);
                        if (mu != 0)
                        {
                            var m = n / dsquared;
                            var sum = 0;
                            foreach (var factor in IntegerMath.Factors(m))
                            {
                                sum += IntegerMath.NumberOfDivisors(factor * d * d) *
                                    IntegerMath.NumberOfDivisors(m / factor);
                            }
                            f2 += mu * sum;
                        }
                    }
                }
                Console.WriteLine("n = {0}, h(n) = {1}, f1(n) = {2}, f2(n) = {3}", n, h, f1, f2);
            }
        }

        static void DivisorCubeTest2()
        {
            var threads = 8;
            var n = IntegerMath.Power((long)10, 10);
            var divisors = new DivisorRange(n + 1, threads);
            var batchSize = 1 << 24;
            var sums = new ulong[batchSize];
            var sum0 = (ulong)0;
            for (var k = (long)1; k <= n; k += batchSize)
            {
                var kmin = k;
                var kmax = Math.Min(kmin + batchSize, n + 1);
                sum0 = divisors.GetSums(k, kmax, sums, sum0);
            }

            Console.WriteLine("sieve: {0}", sum0);

            var algorithm = new DivisorSummatoryFunction();
            var sum1 = (ulong)algorithm.Evaluate(n);

            Console.WriteLine("algorithm: {0}", sum1);
        }

        static void DivisorCubeTest3()
        {
            var x = 1000;
            var sum = 0;
            for (var n = 1; n <= x; n++)
            {
                sum += IntegerMath.NumberOfDivisors(IntegerMath.Cube(n));
            }
            Console.WriteLine("direct sum = {0}", sum);

            sum = 0;
            var sqrtx = IntegerMath.FloorSquareRoot(x);
            for (var e = 1; e <= sqrtx; e++)
            {
                var mu = IntegerMath.Mobius(e);
                if (mu != 0)
                {
                    var fmax = x / (e * e);
                    var fsum = 0;
                    for (var f = 1; f <= fmax; f++)
                    {
                        var t1 = IntegerMath.NumberOfDivisors(e * e * f);
                        var t2 = IntegerMath.SumOfNumberOfDivisors(x / (e * e * f));
                        fsum += t1 * t2;
                    }
                    sum += mu * fsum;
                }
            }
            Console.WriteLine("formula sum = {0}", sum);
        }

        static void MertensPrimorialTest()
        {
            var m = (int)IntegerMath.PrimorialCount((BigInteger)7);
            for (var i = 1; i <= 2 * m; i += 10000)
            {
                var x = i;
                var sum = 0;
                for (var n = 1; n <= x; n++)
                {
                    if (IntegerMath.GreatestCommonDivisor(n, m) == 1)
                        sum += IntegerMath.Mertens(x / n);
                }

                var expected = 0;
                foreach (var factor in IntegerMath.Factors(m))
                    expected += IntegerMath.Mobius(factor) * (x >= factor ? 1 : 0);
                sum -= expected;

                Console.WriteLine("i = {0}, sum = {1}", i, sum);
            }
        }

        static void Modular128Test()
        {
            var timer = new Stopwatch();
            var iterations = 100000;
            {
                var random = new MersenneTwister(0).Create<UInt128>();
                timer.Restart();
                for (int i = 0; i < iterations; i++)
                {
                    var a = random.Next(0);
                    var b = random.Next(0);
                    var c = random.Next(0);
                    var a2 = (BigInteger)a;
                    var b2 = (BigInteger)b;
                    var c2 = (BigInteger)c;
                }
                Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
            {
                var random = new MersenneTwister(0).Create<UInt128>();
                timer.Restart();
                for (int i = 0; i < iterations; i++)
                {
                    var a = random.Next(0);
                    var b = random.Next(0);
                    var c = random.Next(0);
                    var a2 = (BigInteger)a;
                    var b2 = (BigInteger)b;
                    var c2 = (BigInteger)c;
                    UInt128 result;
                    UInt128.ModPow(out result, ref a, ref b, ref c);
                }
                Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
            {
                var random = new MersenneTwister(0).Create<UInt128>();
                timer.Restart();
                for (int i = 0; i < iterations; i++)
                {
                    var a = random.Next(0);
                    var b = random.Next(0);
                    var c = random.Next(0);
                    var a2 = (BigInteger)a;
                    var b2 = (BigInteger)b;
                    var c2 = (BigInteger)c;
                    var result2 = BigInteger.ModPow(a2, b2, c2);
                }
                Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
        }

        static void DivisorSummatoryFunctionOddTest()
        {
#if true
            var threads = 8;
            int repetitions = 1;
            for (var j = 0; j < 1; j++)
            {
                for (var i = 16; i <= 24; i++)
                {
                    var algorithm1 = new DivisorSummatoryFunctionOddUInt64(threads, false);
                    var algorithm2 = new DivisorSummatoryFunctionOddUInt128(threads, false);
                    var n = IntegerMath.Power((BigInteger)10, i);
                    var xmax = IntegerMath.FloorSquareRoot(n);
#if false
                    var xmin = IntegerMath.Min(600 * IntegerMath.CeilingRoot(2 * n, 3), xmax);
#else
                    var xmin = 1;
#endif
#if false
                    var s1 = EvaluateAndTime(() => algorithm1.Evaluate(n, xmin, xmax), repetitions);
#else
                    var s1 = 0;
#endif
                    var s2 = EvaluateAndTime(() => algorithm2.Evaluate(n, xmin, xmax), repetitions);
                    Console.WriteLine("i = {0}, s1 = {1}, s2 = {2}", i, s1, s2);
                }
            }
#endif

#if false
            var threads = 8;
            for (int i = 30; i <= 40; i++)
            {
                var algorithm1 = new DivisionFreeDivisorSummatoryFunction(threads, false, true);
                var algorithm2 = new DivisorSummatoryFunctionOddUInt128(threads, false);
                var n = IntegerMath.Power((BigInteger)2, i);
                var xmax = IntegerMath.FloorSquareRoot(n);
#if false
                var xmin = IntegerMath.Min(600 * IntegerMath.CeilingRoot(2 * n, 3), xmax);
#else
                var xmin = 1;
#endif
#if true
                var s1 = EvaluateAndTime(() => algorithm1.Evaluate(n, xmin, xmax));
#else
                var s1 = 0;
#endif
                var s2 = EvaluateAndTime(() => algorithm2.Evaluate(n, xmin, xmax));
                Console.WriteLine("i = {0}, s1 = {1}, s2 = {2}", i, s1, s2);
            }
#endif
        }

        static void MertensPerformanceTest()
        {
            var threads = 8;
            var timer = new Stopwatch();
            for (var power = 16; power <= 16; power++)
            {
                var n = IntegerMath.Power((BigInteger)10, power);
#if false
                var algorithm1 = new MertensFunctionDR(threads);
                Console.WriteLine("{{ {0}, {1} }},", power, EvaluateAndTime(() => algorithm1.Evaluate((long)n)));
#endif
#if true
                var algorithm2 = new MertensFunctionWheel(threads);
                Console.WriteLine("{{ {0}, {1} }},", power, EvaluateAndTime(() => algorithm2.Evaluate(n)));
#endif
            }
        }

        static void MertensFormulaTest()
        {
            for (var power = 1; power <= 5; power++)
            {
                var n = IntegerMath.Power(10, power);
                var sum = IntegerMath.MertensOdd(n / 2);
                for (var i = 3; i <= n; i += 2)
                    sum += IntegerMath.MertensOdd(n / i);
                sum = 1 - sum;
                Console.WriteLine("power = {0}, M({1}) = {2}, sum = {3}", power, n, IntegerMath.Mertens(n), sum);
            }
        }

        static void PiMod2PerformanceTest()
        {
            var threads = 8;
            var algorithm1 = new PrimeCounting(threads);
            var algorithm2 = new PrimeCountingMod2Odd(threads);
            var timer = new Stopwatch();
            timer.Restart();
            for (var i = 24; i <= 27; i++)
            {
                var n = IntegerMath.Power((BigInteger)10, i);
                var p0 = i <= 24 ? PrimeCounting.PiPowerOfTen(i) % 2 : -1;
#if false
                var p1 = algorithm1.ParityOfPi(n);
#else
                var p1 = -1;
#endif
#if true
                var p2 = EvaluateAndTime(() => algorithm2.Evaluate(n));
#else
                var p2 = -1;
#endif
                Console.WriteLine("i = {0}, p0 = {1}, p1 = {2}, p2 = {3}", i, p0, p1, p2);
                if (p0 != -1 && p1 != -1 && p0 != p1)
                {
                    Console.WriteLine("mismatch!");
                    break;
                }
                if (p0 != -1 && p2 != -1 && p0 != p2)
                {
                    Console.WriteLine("mismatch!");
                    break;
                }
            }
        }

        static void PiMod2PerformanceTestPowerofTwo()
        {
            var threads = 8;
            var algorithm1 = new PrimeCountingMod2Odd(threads);
            var algorithm2 = new PrimeCounting(threads);
            var timer = new Stopwatch();
            timer.Restart();
            for (var i = 59; i <= 72; i++)
            {
                var n = IntegerMath.Power((BigInteger)2, i);
                var p0 = i <= 80 ? PrimeCountingMod2.PowerOfTwo(i) : -1;
#if true
                timer.Restart();
#if false
                var p1 = EvaluateAndTime(() => algorithm1.Evaluate(n));
#else
                var p1 = algorithm1.Evaluate(n);
#endif
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#else
                var p1 = -1;
#endif
#if false
                timer.Restart();
                var p2 = algorithm2.ParityOfPi(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#else
                var p2 = -1;
#endif
                Console.WriteLine("i = {0}, p0 = {1}, p1 = {2}, p2 = {3}", i, p0, p1, p2);
                if (p0 != -1 && p0 != p1)
                {
                    Console.WriteLine("mismatch!");
                    break;
                }
            }
        }

        static void PiMod3PerformanceTest()
        {
            var threads = 8;
            var algorithm1 = new PrimeCountingMod3Odd(threads, true);
            var algorithm2 = new PrimeCountingMod3Odd(threads, false);
            var timer = new Stopwatch();
            timer.Restart();
            for (var i = 16; i <= 19; i++)
            {
                timer.Restart();
                for (var iterations = 0; iterations < 1; iterations++)
                {
                    var n = IntegerMath.Power((BigInteger)10, i);
                    var p1 = PrimeCounting.PiPowerOfTen(i) % 3;
#if false
                    var p2 = EvaluateAndTime(() => algorithm1.Evaluate(n));
#else
                    var p2 = -1;
#endif
                    var p3 = EvaluateAndTime(() => algorithm2.Evaluate(n));
                    if (iterations == 0)
                        Console.WriteLine("i = {0}, p1 = {1}, p2 = {2}, p3 = {3}", i, p1, p2, p3);
                }
            }
        }

        static Rational SawToothStar(BigInteger xn, BigInteger xd)
        {
            var mod = IntegerMath.Modulus(xn, xd);
            if (mod == 0)
                return 0;
            return new Rational(mod, xd) - new Rational(1, 2);
        }

        static Rational SawTooth(BigInteger xn, BigInteger xd)
        {
            return new Rational(IntegerMath.Modulus(xn, xd), xd) - new Rational(1, 2);
        }

        static Rational DedekindSum(BigInteger a, BigInteger b, BigInteger c)
        {
            var sum = (Rational)0;
            for (BigInteger k = 0; k < b; k++)
                sum += SawToothStar(k * a + c, b) * SawToothStar(k, b);
            return sum;
        }

        static BigInteger ScaledSawToothStar(BigInteger xn, BigInteger xd)
        {
            Debug.Assert(xn >= 0);
            var mod = xn % xd;
            if (mod == 0)
                return 0;
            return 2 * mod - xd;
        }

        static BigInteger ScaledSawTooth(BigInteger xn, BigInteger xd)
        {
            Debug.Assert(xn >= 0);
            return 2 * (xn % xd) - xd;
        }

        static BigInteger ScaledDedekindSumSlow(BigInteger a, BigInteger b, BigInteger c)
        {
            var sum = (BigInteger)0;
            for (var k = (BigInteger)1; k < b; k++)
                sum += ScaledSawToothStar(k * a + c, b) * (2 * k - b);
            Debug.Assert(3 * sum % b == 0);
            return 3 * sum / b;
        }

        static BigInteger ScaledDedekindSum(BigInteger a, BigInteger b, BigInteger c, out BigInteger aInv)
        {
            // See Knuth TAOCP Volume 2, 2nd edition, 3.3.3 Theorem D.
            // See also http://matwbn.icm.edu.pl/ksiazki/aa/aa33/aa3341.pdf
            var aPrime = a % b;
            var m1 = b;
            var m2 = aPrime;
            var c1 = c % b;
            var a1 = m1 / m2;
            var p0 = (BigInteger)1;
            var p1 = a1;
            var sumWhole = (BigInteger)0;
            var sumFraction = aPrime;
            var t = 1;
            while (true)
            {
                var b1 = c1 / m2;
                var c2 = c1 - b1 * m2;
                if (c2 == 0 && c1 != 0)
                    sumWhole += 3 * t;
                var sixb1 = 6 * b1;
                var termWhole = a1 - sixb1;
                var termFraction = sixb1 * (c1 + c2) * p0;
                if (t > 0)
                {
                    sumWhole += termWhole;
                    sumFraction += termFraction;
                }
                else
                {
                    sumWhole -= termWhole;
                    sumFraction -= termFraction;
                }
                if (m2.IsOne)
                    break;
                var tmp1 = m1;
                m1 = m2;
                m2 = tmp1 - a1 * m2;
                c1 = c2;
                a1 = m1 / m2;
                var tmp2 = p0;
                p0 = p1;
                p1 = a1 * p1 + tmp2;
                t = -t;
            }
            aInv = t > 0 ? p0 : b - p0;
            Debug.Assert(aInv == IntegerMath.ModularInverse(a, b));
            return b * (sumWhole + t - 2) + aInv + sumFraction;
        }

        static BigInteger GetLatticeCount(BigInteger p, BigInteger q, BigInteger t)
        {
            Debug.Assert(IntegerMath.GreatestCommonDivisor(p, q) == 1);
            BigInteger qInv;
            BigInteger pInv;
            var sumPQC = ScaledDedekindSum(p, q, t, out pInv);
            var sumQPC = ScaledDedekindSum(q, p, t, out qInv);
            if (pInv < 0)
                pInv += q;
            if (qInv < 0)
                qInv += p;
            var threePQ = 3 * p * q;
            var tModP = t % p;
            var tModQ = t % q;
            var n = 6 * t * (t + p + q + 1) + 3 * threePQ + 1
                + p * (p - sumPQC - 6 * tModQ)
                + q * (q - sumQPC - 6 * tModP);
            if (tModP != 0)
                n -= 6 * q * (t * qInv % p) - threePQ;
            if (tModQ != 0)
                n -= 6 * p * (t * pInv % q) - threePQ;
            Debug.Assert(n % (12 * p * q) == 0);
            return n / threePQ >> 2;
        }

        static int T2Odd(int n)
        {
            var sqrt = IntegerMath.FloorRoot(n, 2);
            var sum = 0;
            for (var i = 1; i <= sqrt; i += 2)
            {
                var ni = n / i;
                sum += ni + (ni & 1);
            }
            sum -= IntegerMath.Power((sqrt + 1) / 2, 2);
            return sum;
        }

        static int T2(int n)
        {
            var sqrt = IntegerMath.FloorRoot(n, 2);
            var sum = 0;
            for (var i = 1; i <= sqrt; i++)
                sum += n / i;
            sum = 2 * sum - IntegerMath.Power(sqrt, 2);
            return sum;
        }

        static int t2(int n)
        {
            return IntegerMath.NumberOfDivisors(n, 2);
        }

        static int f2(int n, MobiusCollection mu)
        {
            var sum = 0;
            for (int i = 1; i <= n; i++)
            {
                if (n % (i * i) == 0)
                    sum += mu[i] * t2(n / (i * i));
            }
            return sum;
        }

        static int F2(int n, MobiusCollection mu)
        {
            var sqrt = IntegerMath.FloorRoot(n, 2);
            var sum = 0;
            for (var i = 1; i <= sqrt; i += 2)
            {
                if (mu[i] != 0)
                    sum += mu[i] * T2Odd(n / (i * i));
            }
            return (sum - 1) / 2;
        }

        static int ParityOfPi(int n)
        {
            var sqrt = IntegerMath.FloorRoot(n, 2);
            var kmax = IntegerMath.FloorLog(n, 2);
            var mu = new MobiusCollection(IntegerMath.Max(sqrt, kmax) + 1, 0);
            var sum = 0;
            for (var k = 1; k <= kmax; k++)
            {
                if (mu[k] != 0)
                    sum += mu[k] * F2(IntegerMath.FloorRoot(n, k), mu);
            }
            return (sum + (n >= 2 ? 1 : 0)) % 2;
        }

        static void MuSummationTest()
        {
#if false
            var n = 1000;
            var sum = (double)0;
            var mu = new MobiusCollection(n + 1, 8);
            for (var a = 1; a <= n; a++)
            {
                for (var b = 1; b <= n; b++)
                {
                    var gcd = IntegerMath.GreatestCommonDivisor(a, b);
                    var term = (Rational)(mu[a] * mu[b]) / IntegerMath.Square((BigInteger)a * b) * IntegerMath.Square((BigInteger)gcd);
                    sum += (double)term;
                }
            }
            Console.WriteLine("sum = {0}", sum);
#endif

#if false
            var n = 300;
            var sum = (double)0;
            var mu = new MobiusCollection(n + 1, 8);
            for (var a = 1; a <= n; a++)
            {
                for (var b = 1; b <= n; b++)
                {
                    for (var c = 1; c <= n; c++)
                    {
                        var gcd = IntegerMath.GreatestCommonDivisor(a, b);
                        if (gcd == 1)
                            continue;
                        gcd = IntegerMath.GreatestCommonDivisor(gcd, c);
                        var term = (Rational)(mu[a] * mu[b] * mu[c]) / IntegerMath.Square((BigInteger)a * b * c) * IntegerMath.Power((BigInteger)gcd, 3);
                        sum += (double)term;
                    }
                }
            }
            Console.WriteLine("sum = {0}", sum);
#endif

#if false
            var n = 70;
            var sum = (double)0;
            var mu = new MobiusCollection(n + 1, 8);
            for (var a = 1; a <= n; a++)
            {
                for (var b = 1; b <= n; b++)
                {
                    for (var c = 1; c <= n; c++)
                    {
                        for (var d = 1; d <= n; d++)
                        {
                            var gcd = IntegerMath.GreatestCommonDivisor(a, b);
                            gcd = IntegerMath.GreatestCommonDivisor(gcd, c);
                            gcd = IntegerMath.GreatestCommonDivisor(gcd, d);
                            var term = (Rational)(mu[a] * mu[b] * mu[c] * mu[d]) / IntegerMath.Square((BigInteger)a * b * c * d) * IntegerMath.Power((BigInteger)gcd, 4);
                            sum += (double)term;
                        }
                    }
                }
            }
            Console.WriteLine("sum = {0}", sum);
#endif
        }

        static void SumOfPrimePowersPrime()
        {
            var pmax = 100000;
            var kmax = 20;
            for (var p = (BigInteger)2; p <= pmax; p++)
            {
                if (!IntegerMath.IsPrime(p))
                    continue;
#if true
                for (var k = (BigInteger)2; k <= kmax; k++)
                {
                    var s = (BigInteger)0;
                    for (var i = (BigInteger)0; i <= k; i++)
                        s += IntegerMath.Power(p, i);
                    var n = IntegerMath.PerfectPower(s);
                    if (n == 1)
                        continue;
                    var q = n.IsOne ? s : IntegerMath.Root(s, n);
                    if (!IntegerMath.IsPrime(q))
                        continue;
                    Console.WriteLine("p = {0}, q = {1}, k = {2}, n = {3}", p, q, k, n);
                }
#endif
            }
        }

        static T EvaluateAndTime<T>(Func<T> expr, int repetitions = 1)
        {
            var timer = new Stopwatch();
            var process = Process.GetCurrentProcess();
            var cpu1 = process.TotalProcessorTime;
            timer.Restart();
            T result = expr();
            for (var i = 1; i < repetitions; i++)
                result = expr();
            var elapsed = timer.ElapsedTicks;
            var cpu2 = process.TotalProcessorTime;
            output.WriteLine("elapsed = {0:F3} msec, cpu = {1:F3}, ratio = {2:F3}",
                (double)elapsed / Stopwatch.Frequency * 1000, (cpu2 - cpu1).TotalMilliseconds,
                (cpu2 - cpu1).TotalMilliseconds / ((double)elapsed / Stopwatch.Frequency * 1000));
            return result;
        }

        static int P(int x, int p, int a)
        {
            var y = x / IntegerMath.Power(p, a);
            return (a + 1) * IntegerMath.SumOfNumberOfDivisors(y) - a * IntegerMath.SumOfNumberOfDivisors(y / p);
        }

        static int P(int x, int p, int a, int q, int b)
        {
            var y = x / (IntegerMath.Power(p, a) * IntegerMath.Power(q, b));
            return (a + 1) * (b + 1) * IntegerMath.SumOfNumberOfDivisors(y)
                - a * (b + 1) * IntegerMath.SumOfNumberOfDivisors(y / p)
                - (a + 1) * b * IntegerMath.SumOfNumberOfDivisors(y / q)
                + a * b * IntegerMath.SumOfNumberOfDivisors(y / (p * q));
        }

        static int P(int x, int p, int a, int q, int b, int r, int c)
        {
            var y = x / (IntegerMath.Power(p, a) * IntegerMath.Power(q, b) * IntegerMath.Power(r, c));
            return (a + 1) * (b + 1) * (c + 1) * IntegerMath.SumOfNumberOfDivisors(y)
                - a * (b + 1) * (c + 1) * IntegerMath.SumOfNumberOfDivisors(y / p)
                - (a + 1) * b * (c + 1) * IntegerMath.SumOfNumberOfDivisors(y / q)
                - (a + 1) * (b + 1) * c * IntegerMath.SumOfNumberOfDivisors(y / r)
                + a * b * (c + 1) * IntegerMath.SumOfNumberOfDivisors(y / (p * q))
                + a * (b + 1) * c * IntegerMath.SumOfNumberOfDivisors(y / (p * r))
                + (a + 1) * b * c * IntegerMath.SumOfNumberOfDivisors(y / (q * r))
                - a * b * c * IntegerMath.SumOfNumberOfDivisors(y / (p * q * r));
        }

        static int PRecursive(int x, int p, int a, int q, int b)
        {
            var y = x / (IntegerMath.Power(p, a) * IntegerMath.Power(q, b));
            if (y == 0)
                return 0;
            Console.WriteLine("x = {0}, p = {1}, a = {2}, q = {3}, b = {4}", x, p, a, q, b);
            return (a + 1) * (b + 1)
                * (IntegerMath.SumOfNumberOfDivisors(y) - P(y, p, 1) - P(y, q, 1) + PRecursive(y, p, 1, q, 1))
                + PRecursive(x, p, a + 1, q, b) + PRecursive(x, p, a, q, b + 1) - PRecursive(x, p, a + 1, q, b + 1);
        }

        class CoefficientMap
        {
            private Dictionary<Tuple<int, int>, int> map = new Dictionary<Tuple<int, int>, int>();

            public int this[int a, int b]
            {
                get { return map.ContainsKey(Tuple.Create(a, b)) ? map[Tuple.Create(a, b)] : 0; }
                set { map[Tuple.Create(a, b)] = value; }
            }

            public Dictionary<Tuple<int, int>, int> Map
            {
                get { return map; }
            }
        }

        static void P(CoefficientMap map, int sign, int depth, int a0, int b0, int a, int b)
        {
            if (--depth == 0)
                return;
            var coef = sign * (a + 1) * (b + 1);
            map[a0 + a, b0 + b] += coef;
            map[a0 + a + 1, b0 + b] -= 2 * coef;
            map[a0 + a + 2, b0 + b] += coef;
            map[a0 + a, b0 + b + 1] -= 2 * coef;
            map[a0 + a, b0 + b + 2] += coef;
            P(map, coef, depth, a0 + a, b0 + b, 1, 1);
            P(map, sign, depth, a0, b0, a + 1, b);
            P(map, sign, depth, a0, b0, a, b + 1);
            P(map, -sign, depth, a0, b0, a + 1, b + 1);
        }

        static int UpToOdd(int a)
        {
            return a | 1;
        }

        static int DownToOdd(int a)
        {
            return (a - 1) | 1;
        }

        static int T1Odd(int a)
        {
            return (a + (a & 1)) >> 1;
        }

        static void NotUsed()
        {
#if false
            var n = 100000000;
            var algorithm = new DivisorRange(n + 1, 8);
            var values = new int[n];
            algorithm.GetValues(1, n + 1, values);
            var min = new int[n + 1];
            for (var i = 1; i <= n; i++)
            {
                var d = values[i - 1];
                if (min[d] == 0)
                    min[d] = i;
            }
            for (var i = 1; i <= n; i++)
            {
                if (min[i] != 0)
                {
                    var iseq = IntegerMath.PrimeFactors(i).Reverse();
                    var minseq = IntegerMath.PrimeFactors(min[i]).GroupBy(factor => factor).Select(group => group.Count());
                    if (iseq.Select(factor => factor - 1).SequenceEqual(minseq))
                        continue;
                    Console.WriteLine("i = {0} =>{1}, min = {2} =>{3}",
                        i,
                        iseq.Aggregate("", (sofar, current) => sofar + " " + current),
                        min[i],
                        minseq.Aggregate("", (sofar, current) => sofar + " " + current));
                }
            }
#endif

        }

        static void SquareFreeCountingTest()
        {
            var threads = 8;
            var algorithm = new SquareFreeCounting(threads, false);
            var timer = new Stopwatch();
            for (var i = 12; i <= 24; i++)
            {
                var n = IntegerMath.Power((BigInteger)10, i);
                var sum1 = SquareFreeCounting.PowerOfTen(i);
                timer.Restart();
                var sum2 = algorithm.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}", i, sum1, sum2);
            }
        }

        static void Operator128PerformanceTest()
        {
            var sum1 = EvaluateAndTime(() =>
            {
                var a = (UInt128)(ulong.MaxValue / 1000);
                var b = (UInt128)(ulong.MaxValue / 1000);
                var s = (UInt128)0;
                for (var i = 0; i < 1000000000; i++)
                    s += a;
                return s;
            });
            var sum2 = EvaluateAndTime(() =>
            {
                var a = (Int128)(ulong.MaxValue / 1000);
                var b = (Int128)(ulong.MaxValue / 1000);
                var s = (Int128)0;
                for (var i = 0; i < 1000000000; i++)
                    s += a;
                return s;
            });
            Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
        }

        static void GreatestCommonDivisorPerformanceTest()
        {
            for (var j = 0; j < 1; j++)
            {
                var timer = new Stopwatch();
                var count = 5000000;
                var random1 = new MersenneTwister(0).Create<UInt128>();
                var random2 = new MersenneTwister(0).Create<UInt128>();
                var sum1 = (UInt128)0;
                {
                    timer.Restart();
                    for (var i = 0; i < count; i++)
                    {
                        var a = random1.Next(0);
                        var b = random1.Next(0);
                        var a1 = (BigInteger)a;
                        var b1 = (BigInteger)b;
                        var c = UInt128.GreatestCommonDivisor(a, b);
                        sum1 += c;
                    }
                    Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
                var sum2 = (BigInteger)0;
                {
                    timer.Restart();
                    for (var i = 0; i < count; i++)
                    {
                        var a = random2.Next(0);
                        var b = random2.Next(0);
                        var a1 = (BigInteger)a;
                        var b1 = (BigInteger)b;
                        var c = BigInteger.GreatestCommonDivisor(a1, b1);
                        sum2 += c;
                    }
                    Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
                Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
            }
        }

        public struct Unsigned
        {
#if false
            public static explicit operator Unsigned(long a) { return new Unsigned(); }
#endif
#if true
            public static implicit operator Unsigned(long a) { return new Unsigned(); }
#endif
            public static implicit operator Unsigned(uint a) { return new Unsigned(); }
            public static bool operator >(Unsigned a, long b) { return true; }
            public static bool operator <(Unsigned a, long b) { return true; }
            public static bool operator <(Unsigned a, Unsigned b) { return true; }
            public static bool operator >(Unsigned a, Unsigned b) { return true; }

            public static void Test(Unsigned a)
            {
#if true
                if (a < 0)
                    Console.WriteLine();
#endif
            }
        }

        static void DivisorsPerformanceTest()
        {
            var foo = (BigInteger)1;
            if (foo > 1)
                Console.WriteLine("xyzzy");
            var timer = new Stopwatch();
            var nref = IntegerMath.Power((BigInteger)10, 22);
            var limitref = IntegerMath.FloorRoot(nref, 3);
            for (var j = 0; j < 1; j++)
            {
                var sum1 = (BigInteger)0;
#if true
                {
                    var n = (BigInteger)nref;
                    var limit = (long)limitref;
                    timer.Restart();
                    for (var i = (long)1; i <= limitref; i++)
                        sum1 += n / i;
                    Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
#endif
                var sum2 = (UInt128)0;
                {
                    var n = (UInt128)nref;
                    var limit = (ulong)limitref;
                    timer.Restart();
                    for (var i = (ulong)1; i <= limit; i++)
                        sum2 += n / i;
                    Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
                var sum3 = (Int128)0;
#if true
                {
                    var n = (Int128)nref;
                    var limit = (long)limitref;
                    timer.Restart();
                    for (var i = (long)1; i <= limit; i++)
                        sum3 += n / i;
                    Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
#endif
                var sum4 = (MutableInteger)0;
#if true
                {
                    var n = (MutableInteger)nref;
                    var limit = (long)limitref;
                    var ni = (MutableInteger)0;
                    var store = new MutableIntegerStore(4);
                    timer.Restart();
                    for (var i = (long)1; i <= limit; i++)
                        sum4.Add(ni.SetQuotient(n, i, store));
                    Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
#endif
                var sum5 = (Int128)0;
#if true
                {
                    var n = -(Int128)nref;
                    var limit = -(long)limitref;
                    timer.Restart();
                    for (var i = -(long)1; i >= limit; i--)
                        sum5 += n / i;
                    Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
#endif
                Console.WriteLine("n = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}, sum4 = {4}, sum5 = {5}", nref, sum1, sum2, sum3, sum4, sum5);
            }
        }

        static void ModularSumTest()
        {
            for (var k = 0; k < 2; k++)
            {
                EvaluateAndTime(() =>
                {
                    var random = new MersenneTwister(0).Create<ulong>();
                    var sum = (ulong)0;
                    for (var i = 0; i < 1000000; i++)
                    {
                        var a = random.Next(long.MaxValue);
                        var b = random.Next(long.MaxValue);
                        var c = random.Next(long.MaxValue);
                        a %= c;
                        b %= c;
                        for (var j = 0; j < 1000; j++)
                            sum += IntegerMath.ModularSum(a, b, c);
                    }
                    Console.WriteLine("sum = {0}", sum);
                    return 0;
                });
                EvaluateAndTime(() =>
                {
                    var random = new MersenneTwister(0).Create<ulong>();
                    var sum = (ulong)0;
                    for (var i = 0; i < 1000000; i++)
                    {
                        var a = random.Next(long.MaxValue);
                        var b = random.Next(long.MaxValue);
                        var c = random.Next(long.MaxValue);
                        a %= c;
                        b %= c;
                        for (var j = 0; j < 1000; j++)
                            sum += IntegerMath.ModularDifference(a, b, c);
                    }
                    Console.WriteLine("sum = {0}", sum);
                    return 0;
                });
            }
        }

        static void Montgomery128Test()
        {
            var repetitions = 10000000;
            var random = new MersenneTwister(0).Create<UInt128>();
            var max = (UInt128)1 << 127;
            var modulus = random.Next(max) | 1;
            var a = random.Next(max) % modulus;
            var b = random.Next(max) % modulus;
            Console.WriteLine("a       = {0}", a);
            Console.WriteLine("b       = {0}", b);
            Console.WriteLine("modulus = {0}", modulus);

            var c0 = EvaluateAndTime(() => UInt128.ModMul(a, b, modulus), repetitions);
            Console.WriteLine("c0      = {0}", c0);

            var a1 = (BigInteger)a;
            var b1 = (BigInteger)b;
            var modulus1 = (BigInteger)modulus;
            var c1 = EvaluateAndTime(() => a1 * b1 % modulus1, repetitions);
            Console.WriteLine("c1      = {0}", c1);

            int rLength = 128;
            var rMinusOne = UInt128.MaxValue;
            var rModN = rMinusOne % modulus + 1;
            var rSquaredModN = UInt128.ModMul(rModN, rModN, modulus);
            var nInv = IntegerMath.ModularInversePowerOfTwoModulus(modulus, rLength);
            var k0 = (ulong)IntegerMath.TwosComplement(nInv);
            var aBar = UInt128.Reduce(a, rSquaredModN, modulus, k0);
            var bBar = UInt128.Reduce(b, rSquaredModN, modulus, k0);
            var cBar = EvaluateAndTime(() => UInt128.Reduce(aBar, bBar, modulus, k0), repetitions);
            var c2 = UInt128.Reduce(cBar, modulus, k0);
            Console.WriteLine("c2      = {0}", c2);
        }

        static void ParityTest()
        {
#if false
            var timer = new Stopwatch();
            var threads = 8;
            var n = IntegerMath.Power((long)10, 10) | 1;
            var batchSize = 1 << 24;
#if false
            var algorithm1 = new DivisorOddRange(n | 1, threads);
            var sums1 = new ulong[batchSize >> 1];
            timer.Restart();
            for (var x = (long)1; x < n; x += batchSize)
                algorithm1.GetSums(x, Math.Min(x + batchSize, n), sums1, 0);
            Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
            var algorithm2 = new DivisorOddRangeAdditive(n | 1, threads);
            var sums2a = new ulong[batchSize >> 1];
            var sums2b = new ulong[batchSize >> 1];
            var sums2 = sums2a;
            timer.Restart();
            for (var x = (long)1; x < n; x += batchSize)
            {
                sums2 = sums2 == sums2a ? sums2b : sums2a;
                algorithm2.GetSums(x, Math.Min(x + batchSize, n), sums2, 0);
            }
            Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#if false
            var algorithm3 = new DivisorOddRangeAdditive(n | 1, threads);
            timer.Restart();
            algorithm1.GetValues(1, n, (kstart, kend, values) => { });
            Console.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#endif

#if false
            // Confirm that low-precision logs reliably detect a factor larger than the square root.
            // Specifically, a1 should be greater than b for all n and if a2 is greater than b
            // then n should not have a factor larger than the square root.
            var odd = 1;
            for (var n = 1; n <= 10000000; n++)
            {
                var sqrt = IntegerMath.FloorSquareRoot(n);
                var factors = IntegerMath.PrimeFactors(n);
                var small = factors.Where(p => p <= sqrt).ToArray();
                var a1 = factors.Sum(p => IntegerMath.CeilingLogBaseTwo(p) | odd);
                var a2 = small.Sum(p => IntegerMath.CeilingLogBaseTwo(p) | odd);
                var b = IntegerMath.CeilingLogBaseTwo(n) - 1;
                if (!(a1 > b) || (a2 > b) != (factors.Length == small.Length))
                    Console.WriteLine("i = {0}, a = {1}, b = {2}", n, a1, b);
            }
#endif

#if false
            var n = 1000000;
            var algorithm = new MobiusOddRange(n + 1, 8);
            var values = new sbyte[n >> 1];
            algorithm.GetValues(1, UpToOdd(n), values);
            for (var i = 1; i <= n; i += 2)
            {
                if (values[i >> 1] != IntegerMath.Mobius(i))
                    Console.WriteLine("values[{0}] = {1}, mu({2}) = {3}", i >> 1, values[i >> 1], i, IntegerMath.Mobius(i));
            }
            var m = new long[n >> 1];
            algorithm.GetValues(1, UpToOdd(n), null, 1, m, 0);
            var sum = (long)0;
            for (var i = 1; i <= n; i += 2)
            {
                sum += values[i >> 1];
                if (sum != m[i >> 1])
                    Console.WriteLine("m[{0}] = {1}, sum(values,1,{2}) = {3}", i >> 1, m[i >> 1], i, sum);
            }
#endif

#if false
            var threads = 8;
            var timer = new Stopwatch();
            var n = IntegerMath.Power((long)10, 10);
            var batchSize = (long)1 << 24;
            var algorithm1 = new MobiusRangeAdditive(n + 1, threads);
            var values1 = new sbyte[batchSize];
            var sums1 = new int[batchSize];
            timer.Restart();
            var sum1 = 0;
            for (var x = (long)1; x <= n; x += batchSize)
                sum1 = algorithm1.GetValuesAndSums(x, Math.Min(x + batchSize, n + 1), values1, sums1, sum1);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#if false
            var algorithm2 = new MobiusRange(n + 1, threads);
            var values2 = new sbyte[batchSize];
            var sums2 = new int[batchSize];
            timer.Restart();
            var sum2 = 0;
            for (var x = (long)1; x <= n; x += batchSize)
                sum2 = algorithm2.GetValuesAndSums(x, Math.Min(x + batchSize, n + 1), values2, sums2, sum2);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#else
            var sum2 = -1;
#endif
            Console.WriteLine("n = {0}, sum1 = {1}, sum2 = {2}", n, sum1, sum2);
#endif

#if false
            var n = IntegerMath.Power(10, 5);
            var algorithm1 = new DivisorRange(n, 0);
            var algorithm2 = new DivisionFreeDivisorSummatoryFunction(0, false, true);
            var values = new int[n];
            algorithm1.GetValues(1, n + 1, values);
            for (var i = 1; i <= n; i += 2)
            {
                var sum = 0;
                for (var j = 1; j <= i; j += 2)
                    sum += values[j - 1];
                if (sum != algorithm2.Evaluate(i))
                {
                    Debugger.Break();
                    Console.WriteLine();
                }
            }
#endif

#if false
            for (var power = 1; power <= 16; power++)
            {
                var n = IntegerMath.Power((long)2, power);
                var sqrt = IntegerMath.FloorSquareRoot(n);
                var sum = (long)0;
                for (var d = (long)1; d <= sqrt; d += 2)
                    sum += IntegerMath.Mobius(d) * IntegerMath.Square((IntegerMath.FloorSquareRoot(n / IntegerMath.Power(d, 2)) + 1) / 2);
                if (sum % 2 != 1)
                    Console.WriteLine("not odd\n");
                sum = (sum - 1) / 2 % 2;
                Console.WriteLine("power = {0}, sum = {1}", power, sum);
            }
#endif

#if false
            var threads = 8;
            var power = 9;
            var n = IntegerMath.Power((long)10, power);
            var batchSize = 1 << 24;
            var algorithmDivisor = new DivisorRange(n + 1, threads);
            var algorithmMobius = new MobiusRange(n + 1, threads);
            var dsums = new long[batchSize];
            var msums = new int[batchSize];
            var timer = new Stopwatch();
            timer.Restart();
            var sum0 = (long)0;
            for (var k0 = (long)1; k0 <= n; k0 += batchSize)
            {
                var kstart = k0;
                var kend = Math.Min(n + 1, k0 + batchSize);
                sum0 = algorithmDivisor.GetSums(kstart, kend, dsums, sum0);
#if false
                for (var x = kstart; x < kend; x++)
                {
                    var tau = divisor[x - kstart];
                    if (tau != IntegerMath.NumberOfDivisors(x))
                    {
                        Debugger.Break();
                        Console.WriteLine();
                    }
                }
#endif
            }
            output.WriteLine("elapsed1 = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#if false
            timer.Restart();
            algorithmDivisor.GetValues(1, n + 1, 0, (kstart, kend, values) =>
                {
#if false
                for (var x = kstart; x < kend; x++)
                {
                    var tau = values[x - kstart];
                    if (tau != IntegerMath.NumberOfDivisors(x))
                    {
                        Debugger.Break();
                        Console.WriteLine();
                    }
                }
#endif
                });
            output.WriteLine("elapsed1 = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif

#if true
            timer.Restart();
            var m0 = 0;
            for (var k0 = (long)1; k0 <= n; k0 += batchSize)
            {
                var kstart = k0;
                var kend = Math.Min(n + 1, k0 + batchSize);
                m0 = algorithmMobius.GetSums(kstart, kend, msums, m0);
            }
            output.WriteLine("elapsed1 = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#endif

#if false
            {
                var sum1 = EvaluateAndTime(() =>
                {
                    var a = (UInt128)10;
                    a = UInt128.Square(UInt128.Square(a));
                    Console.WriteLine("a = {0}", a);
                    var s = (UInt128)0;
                    for (var i = 0; i < 100000000; i++)
                        s += UInt128.Double(a);
                    return s;
                });
                var sum2 = EvaluateAndTime(() =>
                {
                    var a = (UInt128)10;
                    a = UInt128.Square(UInt128.Square(a));
                    Console.WriteLine("a = {0}", a);
                    var s = (UInt128)0;
                    for (var i = 0; i < 100000000; i++)
                        s += a << 1;
                    return s;
                });
                Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
            }
#endif

#if false
            {
                var sum1 = EvaluateAndTime(() =>
                    {
                        var a = (ulong)10;
                        Console.WriteLine("a = {0}", a);
                        var s = (UInt128)0;
                        for (var i = 0; i < 100000000; i++)
                            s += UInt128.Square(a);
                        return s;
                    });
                var sum2 = EvaluateAndTime(() =>
                    {
                        var a = (ulong)10;
                        Console.WriteLine("a = {0}", a);
                        var s = (UInt128)0;
                        for (var i = 0; i < 100000000; i++)
                            s += UInt128.Multiply(a, a);
                        return s;
                    });
                Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
            }
#endif

#if false
            {
                var sum1 = EvaluateAndTime(() =>
                {
                    var a = (ulong)10;
                    Console.WriteLine("a = {0}", a);
                    var s = (UInt128)0;
                    for (var i = 0; i < 100000000; i++)
                        s += a << 10;
                    return s;
                });
                var sum2 = EvaluateAndTime(() =>
                {
                    var a = (UInt128)10;
                    Console.WriteLine("a = {0}", a);
                    var s = (UInt128)0;
                    for (var i = 0; i < 100000000; i++)
                        s += a << 10;
                    return s;
                });
                Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
            }
#endif

#if false
            var threads = 8;
            for (var i = 48; i <= 48; i++)
            {
                var n = IntegerMath.Power((BigInteger)2, i);
                var mertens1 = new MertensFunctionWheel64(threads);
                var sum1 = EvaluateAndTime(() => mertens1.Evaluate((long)n));
                Console.WriteLine("i = {0}, sum = {1}", i, sum1);
            }
#endif

#if false
            var threads = 1;
            for (var i = 13; i <= 13; i++)
            {
                for (var loop = 0; loop < 3; loop++)
                {
                    var n = IntegerMath.Power((BigInteger)10, i);
                    var mertens1 = new MertensFunctionWheel(threads);
                    var sum1 = EvaluateAndTime(() => mertens1.Evaluate((long)n));
#if true
                    var mertens2 = new MertensFunctionWheel64(threads);
                    var sum2 = EvaluateAndTime(() => mertens2.Evaluate((long)n));
#else
                    var sum2 = (long)0;
#endif
                    var sum3 = i <= 19 ? MertensFunction.PowerOfTen(i) : 0;
                    Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}", i, sum1, sum2, sum3);
                }
            }
#endif

#if false
            var threads = 8;
            var timer = new Stopwatch();
            for (var power = 9; power <= 9; power++)
            {
                var n = IntegerMath.Power((long)10, power);
                var algorithm1 = new MobiusRange(n + 1, threads);
                var sum1 = (long)0;
                timer.Restart();
                var batchSize = (n + threads - 1) / threads;
                var tasks = new Task[threads];
                for (var thread = 0; thread < threads; thread++)
                {
                    var kmin = 1 + thread * batchSize;
                    var kmax = Math.Min(kmin + batchSize, n + 1);
                    tasks[thread] = Task.Factory.StartNew(() =>
                        algorithm1.GetValues(kmin, kmax, 0, (k1, k2, values) =>
                        {
                            var s = 0;
                            int length = (int)(k2 - k1);
                            for (var i = 0; i < length; i++)
                                s += values[i];
                            Interlocked.Add(ref sum1, s);
                        }));
                }
                Task.WaitAll(tasks);
                output.WriteLine("elapsed1 = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#if false
                var algorithm2 = new MertensFunctionDR(threads);
                timer.Restart();
                var sum2 = algorithm2.Evaluate(n);
                output.WriteLine("elapsed1 = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#else
                var sum2 = 0;
#endif
                Console.WriteLine("power = {0}, sum1 = {1}, sum2 = {2}", power, sum1, sum2);
            }
#endif

#if false
            var timer = new Stopwatch();
#if true
            {
                var i = 24;
                var n = IntegerMath.Power((BigInteger)10, i);
                var algorithm = new SquareFreeCounting(0, false);
                timer.Restart();
                var sum1 = algorithm.Evaluate(n);
                output.WriteLine("elapsed1 = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                var sum2 = SquareFreeCounting.PowerOfTen(i);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}", i, sum1, sum2);
            }
#endif
#if false
            {
                var i = 14.8;
                var n = (long)Math.Pow(10, i);
                timer.Restart();
                var mertens1 = new MertensFunctionWheel64(8);
                var sum1 = mertens1.Evaluate(n);
                output.WriteLine("elapsed2 = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("i = {0}, sum1 = {1}", i, sum1);
            }
#endif
#endif

#if false
            for (var power = 1; power <= 7; power++)
            {
                var n = IntegerMath.Power(10, power);
                var sqrt = IntegerMath.FloorSquareRoot(n);
                var sum1 = 0;
                for (var m = 1; m <= sqrt; m++)
                    sum1 += IntegerMath.Power(-1, m + 1) * IntegerMath.Mertens(n / m);
                var sum2 = -1 + IntegerMath.Mertens(sqrt) * (sqrt % 2);
                for (var m = 1; m <= sqrt; m++)
                    sum2 -= n / m % 2 * IntegerMath.Mobius(m);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}", power, sum1, sum2);
            }
#endif

#if false
            var n = 100;
            var algorithm = new MobiusOddRange(n + 1, 0);
            var values = new sbyte[n >> 1];
            algorithm.GetValues(1, UpToOdd(n), values);
            for (var i = 1; i <= n; i += 2)
                Console.WriteLine("values[{0}] = {1}, mu({2}) = {3}", i >> 1, values[i >> 1], i, IntegerMath.Mobius(i));
#endif

#if false
            for (var power = 4; power <= 4; power++)
            {
                var n = IntegerMath.Power(2, power) - 1;
                var u = (int)IntegerMath.FloorPower((BigInteger)n, 2, 3);
                var imax = n / u;
                if (imax < 2)
                    continue;
                var sum1 = 0;
                for (var i = 1; i <= imax; i++)
                {
                    var mu = IntegerMath.Mobius(i);
                    if (mu == 0)
                        continue;
                    var ni = n / i;
                    var jmin = (imax / i + 1) | 1;
                    var jmax = ni;
                    var s = 0;
                    for (var j = jmin; j <= jmax; j += 2)
                        s += IntegerMath.MertensOdd(ni / j);
                    sum1 += mu * s;
                }
                sum1 = IntegerMath.MertensOdd(imax) - IntegerMath.MertensOdd(imax / 2) - sum1;
                var sum2 = IntegerMath.Mertens(n);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}", power, sum1, sum2);
            }
#endif

#if false
            var n = 10000;
            var count = 0;
            for (var i = 1; i <= n; i += 2)
            {
                if (IntegerMath.Mobius(i) != 0)
                    ++count;
            }
            Console.WriteLine("n = {0}, % = {1}", n, (double)count / n * 2 * 100);
#endif

#if false
            for (var power = 1; power <= 20; power++)
            {
                var n = IntegerMath.Power(2, power) - 1;
                var imax = IntegerMath.FloorRoot(n, 3);
                var sum1 = 0;
                for (var i = 1; i <= imax; i++)
                {
                    if (i % 2 == 0)
                        continue;
                    if (i % 3 != 0)
                        continue;
                    var mu = IntegerMath.Mobius(i);
                    if (mu == 0)
                        continue;
                    var ni = n / i;
                    var jmin = 1;
                    var jmax = (imax / i - 1) | 1;
                    var s = 0;
                    for (var j = jmin; j <= jmax; j += 2)
                        s += IntegerMath.Mertens(ni / j);
                    sum1 += mu * s;
                }
                var sum2 = (long)0;
                for (var k = 3; k <= imax; k *= 3)
                    sum2 -= IntegerMath.Mertens(n / k);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}", power, sum1, sum2);
            }
#endif

#if false
            for (var power = 3; power <= 7; power++)
            {
                var n = IntegerMath.Power(10, power);
                var imax = IntegerMath.FloorRoot(n, 3) * 2;
                var sqrt = IntegerMath.FloorSquareRoot(n);
                var sum1 = 0;
                for (var k = 1; k <= sqrt; k++)
                {
                    var ilast = IntegerMath.Min(imax, n / (k * k));
                    var nk1 = n / k;
                    var nk2 = n / (k + 1);
                    while (ilast > 0 && nk2 / ilast < IntegerMath.FloorSquareRoot(n / ilast))
                        --ilast;
                    var s = 0;
                    for (var i = 1; i <= ilast; i += 2)
                    {
                        var nki2 = nk2 / i;
                        var sqrtni = IntegerMath.FloorSquareRoot(n / i);
                        var max = Math.Max(sqrtni, nki2);
                        if (max != nki2)
                            Debugger.Break();
                        s += IntegerMath.Mobius(i) * (T1Odd(nk1 / i) - T1Odd(nk2 / i));
                    }
                    sum1 += IntegerMath.Mertens(k) * s;
                }
                var sum2 = 0;
                for (var i = 1; i <= imax; i += 2)
                {
                    var ni = n / i;
                    var jmin = (IntegerMath.FloorSquareRoot(ni) + 1) | 1;
                    var s = 0;
                    for (var j = jmin; j <= ni; j += 2)
                        s += IntegerMath.Mertens(ni / j);
                    sum2 += IntegerMath.Mobius(i) * s;
                }
                var sum3 = 0;
                for (var i = 1; i <= imax; i += 2)
                {
                    var x = n / i;
                    var sqrtx = IntegerMath.FloorSquareRoot(x);
                    var s = 0;

                    var kmin = 1;
                    var kmax = x / sqrtx - 1;
                    var current = T1Odd(x / kmin);
                    for (var k = kmin; k <= kmax; k++)
                    {
                        var next = T1Odd(x / (k + 1));
                        s += (current - next) * IntegerMath.Mertens(k);
                        current = next;
                    }
                    sum3 += IntegerMath.Mobius(i) * s;
                }
                Console.WriteLine("imax = {0}", imax);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}", power, sum1, sum2, sum3);
            }
#endif

#if false
            for (var power = 1; power <= 7; power++)
            {
                var n = IntegerMath.Power(10, power);
                var imax = IntegerMath.FloorRoot(n, 3);
                var sum1 = 0;
                for (var i = 1; i <= imax; i += 2)
                {
                    var mu = IntegerMath.Mobius(i);
                    if (mu == 0)
                        continue;
                    var ni = n / i;
                    var jmin = (imax / i + 1) | 1;
                    var jmax = (ni - 1) | 1;
                    var s = 0;
                    for (var j = jmin; j <= jmax; j += 2)
                        s += IntegerMath.Mertens(ni / j);
                    sum1 += mu * s;
                }
                sum1 = -sum1;
                var sum2 = IntegerMath.Mertens(n);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}", power, sum1, sum2);
            }
#endif

#if false
            for (var i = 1; i <= 7; i++)
            {
                var x = IntegerMath.Power(10, i);
                var u = IntegerMath.FloorRoot(x, 3);
                var sum1 = 0;
                for (var m = 1; m <= u; m += 2)
                {
                    var nmax = u / m;
                    var xm = x / m;
                    var s = 0;
                    for (var n = 1; n <= nmax; n += 2)
                        s += IntegerMath.Mertens(xm / n);
                    sum1 += IntegerMath.Mobius(m) * s;
                }
                var sum2 = IntegerMath.Mertens(x);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}", i, sum1, sum2);
            }
#endif

#if false
            for (var i = 1; i <= 5; i++)
            {
                var x = IntegerMath.Power(10, i);
                var u = IntegerMath.FloorRoot(x, 3);
                var sum1 = 0;
                for (var m = 1; m <= u; m++)
                {
                    var nmax = u / m;
                    var xm = x / m;
                    var s = 0;
                    for (var n = 1; n <= nmax; n += 2)
                        s += IntegerMath.Mertens(xm / n);
                    sum1 += IntegerMath.Mobius(m) * s;
                }
                var sum2 = IntegerMath.Mertens(x) - IntegerMath.Mertens(x / 2);
                Console.WriteLine("i = {0}, sum1 = {1}, sum2 = {2}", i, sum1, sum2);
            }
#endif

#if false
            var n = (long)1000000;
            var mobius = new MobiusRange(n + 1, 8);
            var u = IntegerMath.FloorPower(n, 2, 3);
            var v = n / u;
            var array = new long[v + 1];
            var m = new long[u + 1];
            var values = new sbyte[u + 1];
            mobius.GetValues(0, u + 1, values);
            var t = (long)0;
            for (var i = 0; i <= u; i++)
            {
                t += values[i];
                m[i] = t;
            }
            for (var i = v; i >= 1; i--)
            {
                var ni = n / i;
                var s = (long)0;
                var jmax = IntegerMath.FloorSquareRoot(ni);
                var kmax = ni / jmax;
                var jmin = v / i;
                var ijmax = jmin * i;
                for (var ij = 2 * i; ij <= ijmax; ij += i)
                    s += array[ij];
                for (var j = jmin + 1; j <= jmax; j++)
                    s += m[ni / j];
                var current = ni;
                for (var k = 1; k < kmax; k++)
                {
                    var next = ni / (k + 1);
                    s += (current - next) * m[k];
                    current = next;
                }
                array[i] = 1 - s;
            }
            var sum1 = array[1];
            var sum2 = IntegerMath.Mertens(n);
            Console.WriteLine("n = {0}, sum1 = {1}, sum2 = {2}", n, sum1, sum2);
#endif

#if false
            var algorithm1 = new DivisionFreeDivisorSummatoryFunction(1, false, false);
            var algorithm2 = new Divisors();
            var n = 1000;
            //var result = algorithm.primeCountingFunction(100);
            var result1 = algorithm1.Evaluate(n) - 2 * n + 1;
            var result2 = algorithm2.Evaluate(n);
            var result3 = algorithm2.countdivisorsfast(n, 2, 2);
            Console.WriteLine("n = 100, result1 = {0}, result2 = {1}, result3 = {2}", result1, result2, result3);
#endif

#if false
            var n = 3;
            var mu = new MobiusCollection(1000, 8);
            var sqrt = IntegerMath.FloorSquareRoot(n);
            var sum = 0;
            for (var a = 1; a <= sqrt; a++)
            {
                sum += mu[a] * IntegerMath.SumOfNumberOfDivisors(n / (a * a), 3);
            }
            Console.WriteLine("n = {0}, sum = {1}", n, sum);
#endif

#if false
            var map = new CoefficientMap();
            PQ(map, 1, 10, 0, 0, 1, 2);
            foreach (var pair in map.Map.OrderBy(pair => pair.Key.Item1).ThenBy(pair => pair.Key.Item2))
                Console.WriteLine("map[{0}, {1}] = {2}", pair.Key.Item1, pair.Key.Item2, pair.Value);
#endif

#if false
            var p = 2;
            var q = 3;
            var r = 5;
            var a = 2;
            var b = 2;
            var c = 2;
            var d = IntegerMath.Power(p, a) * IntegerMath.Power(q, b) * IntegerMath.Power(r, c);
            for (var x = 0; x <= 10000; x += d)
            {
                var sum1 = 0;
                for (var n = 1; n <= x / d; n++)
                    sum1 += IntegerMath.NumberOfDivisors(d * n);
                var sum2 = 0;
                sum2 += P(x, p, a, q, b, r, c);
                var sum3 = 0;
                var sum4 = 0;
                Console.WriteLine("x = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}, sum4 = {4}", x, sum1, sum2, sum3, sum4);
            }
#endif

#if false
            var mu = new MobiusCollection(100 + 1, 8);
            for (var x = 1; x <= 100; x++)
            {
                var sqrt = IntegerMath.FloorSquareRoot(x);
                var sum1 = 0;
                for (var n = 1; n <= x; n++)
                    sum1 += IntegerMath.NumberOfDivisors(n * n);
                var sum2 = 0;
                for (var a = 1; a <= sqrt; a++)
                    sum2 += IntegerMath.Mobius(a) * IntegerMath.SumOfNumberOfDivisors(x / (a * a), 3);
                var sum3 = 0;
                for (var n = 1; n <= x; n++)
                {
                    for (var a = 1; a <= n; a++)
                    {
                        if (n % a != 0)
                            continue;
                        for (var b = 1; b <= a; b++)
                        {
                            if (a % (b * b) != 0)
                                continue;
                            sum3 += IntegerMath.NumberOfDivisors(n / a) * IntegerMath.Mobius(b);
                        }
                    }
                }
                var sum4 = 0;
                Console.WriteLine("x = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}, sum4 = {4}", x, sum1, sum2, sum3, sum4);
            }
#endif

#if false
            var mu = new MobiusCollection(100 + 1, 8);
            for (var x = 1; x <= 100; x++)
            {
                var sqrt = IntegerMath.FloorSquareRoot(x);
                var sum1 = IntegerMath.NumberOfDivisors(x) * IntegerMath.Liouville(x);
                var sum2 = 0;
                for (var a = 1; a <= x; a++)
                {
                    if (x % a != 0)
                        continue;
                    sum2 += IntegerMath.Liouville(a) * IntegerMath.Liouville(x / a);
                }
                var sum3 = 0;
                var sum4 = 0;
                Console.WriteLine("n = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}, sum4 = {4}", x, sum1, sum2, sum3, sum4);
            }
#endif

#if false
            var mu = new MobiusCollection(100 + 1, 8);
            for (var x = 1; x <= 100; x++)
            {
                var sqrt = IntegerMath.FloorSquareRoot(x);
                var sum1 = 0;
                for (var a = 1; a <= x; a++)
                {
                    var term = IntegerMath.Power(3, IntegerMath.LittleOmega(a));
                    sum1 += term;
                }
                var sum2 = 0;
                for (var a = 1; a <= x; a++)
                {
                    var term = mu[a] * mu[a] * IntegerMath.NumberOfDivisors(a) * (x / a);
                    sum2 += term;
                }
                var sum3 = 0;
                for (var a = 1; a <= sqrt; a++)
                {
                    for (var b = 1; b <= x / (a * a); b++)
                        sum3 += IntegerMath.Mobius(a) * IntegerMath.NumberOfDivisors(a * a * b) * (x / (a * a * b));
                }
                var sum4 = 0;
                for (var a = 1; a <= x; a++)
                {
                    for (var b = 1; b <= x / a; b++)
                        sum4 += IntegerMath.Square(IntegerMath.Mobius(a * b)) * x / (a * b);
                }
                Console.WriteLine("n = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}, sum4 = {4}", x, sum1, sum2, sum3, sum4);
            }
#endif

#if false
            var mu = new MobiusCollection(100 + 1, 8);
            for (var n = 1; n <= 100; n++)
            {
                var sum1 = 0;
                for (var x = 1; x <= n; x++)
                {
                    for (var d = 1; d <= x; d++)
                    {
                        if (x % (d * d) != 0)
                            continue;
                        var term = mu[d];
                        sum1 += term;
                    }
                }
                var sum2 = new PrimeCounting(8).NumberOfSquareFree(n);
                var sum3 = 0;
                for (var x = 1; x <= n; x++)
                {
                    var term = mu[x] * (n / (x * x));
                    sum3 += term;
                }
                Console.WriteLine("n = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}", n, sum1, sum2, sum3);
            }
#endif

#if false
            var mu = new MobiusCollection(100 + 1, 8);
            for (var n = 1; n <= 100; n++)
            {
                var sum1 = 0;
                for (var d = 1; d <= n; d++)
                {
                    if (n % (d * d) != 0)
                        continue;
                    var term = mu[d] * IntegerMath.NumberOfDivisors(n / (d * d));
                    Console.Write(" {0}", term);
                    sum1 += term;
                }
                Console.WriteLine("");
                var sum2 = 0;
                for (var d = 1; d <= n; d++)
                {
                    if (n % d != 0)
                        continue;
                    var term = mu[d] * mu[d];
                    sum2 += term;
                }
                var sum3 = 0;
                for (var d = 1; d <= n; d++)
                {
                    if (n % d != 0)
                        continue;
                    var term = mu[n / d] * IntegerMath.NumberOfDivisors(d * d);
                    Console.Write(" {0}", term);
                    sum3 += term;
                }
                Console.WriteLine("");
                Console.WriteLine("n = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}", n, sum1, sum2, sum3);
            }
#endif

#if false
            for (int i = 17; i <= 20; i++)
            {
                var algorithm1 = new DivisionFreeDivisorSummatoryFunction(0, false, false);
                var algorithm2 = new DivisorSummatoryFunctionArticle();
                var n = IntegerMath.Power((BigInteger)10, i);
                var sqrt = (long)IntegerMath.FloorSquareRoot(n);
                var timer = new Stopwatch();
#if true
                timer.Restart();
                var s1 = algorithm1.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#else
                var s1 = 0;
#endif
                timer.Restart();
                var s2 = algorithm2.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("i = {0}, s1 = {1}, s2 = {2}", i, s1, s2);
            }
#endif

#if false
            var n = IntegerMath.Primorial(6);
            var logn = IntegerMath.FloorLog(n, 2);
            var mu = 0;
            var coef = new int[100];
            for (var k = 1; k <= logn; k++)
            {
                for (var j = 0; j <= k; j++)
                    coef[j] += IntegerMath.Power(-1, 2 * k - j) * IntegerMath.Binomial(k, j);
            }
            for (var j = 0; j <= logn; j++)
            {
                Console.WriteLine("coef[{0}] = {1}, tau{2}({3}) = {4}", j, coef[j], j, n, IntegerMath.NumberOfDivisors(n, j));
                mu += coef[j] * IntegerMath.NumberOfDivisors(n, j);
            }
            Console.WriteLine("mu({0}) = {1}", n, mu);
#endif

#if false
            var nmax = 1000;
            var mu = new MobiusCollection(nmax + 1, 0);
            for (var n = 1; n <= nmax; n++)
            {
                var root3 = IntegerMath.FloorRoot(n, 3);
#if false
                var sum = 0;
                for (var d = 1; d <= root3; d += 2)
                {
                    var m = IntegerMath.FloorRoot(n / (d * d * d), 3);
                    var m2 = (m + 1) / 2;
                    sum += mu[d] * m2 * m2 * m2;
                }
                var mod = sum % 9;
#endif
#if false
                var sum = 0;
                for (var d = 1; d <= root3; d += 2)
                {
                    var m = n / (d * d * d);
                    var zmax = IntegerMath.FloorRoot(m, 3);
                    for (var z = 1; z <= zmax; z += 2)
                    {
                        var value = (IntegerMath.FloorRoot(m / z, 2) + 1) / 2;
                        sum += mu[d] * value * value;
                    }
                }
                var mod = sum % 3;
#endif
#if true
                var z = 1;
                var sum = 0;
                for (var d = 1; d <= root3; d += 2)
                {
                    var m = n / (d * d);
                    var value = (IntegerMath.FloorRoot(m / z, 2) + 1) / 2;
                    sum += mu[d] * value * value;
                }
                var mod = sum % 3;
#endif
#if false
                var sum = 0;
                for (var d = 1; d <= root3; d += 2)
                {
                    var m = n / (d * d * d);
                    var zmax = IntegerMath.FloorRoot(m, 3);
                    for (var z = 1; z <= zmax; z += 2)
                    {
                        var value = (m / (z * z) + 1) / 2;
                        sum += mu[d] * value;
                    }
                }
                var mod = sum % 3;
#endif
#if false
                var sum = 0;
                for (var d = 1; d <= root3; d += 2)
                {
                    var m = n / (d * d * d);
                    var zmax = IntegerMath.FloorRoot(m, 3);
                    for (var z = 1; z <= zmax; z += 2)
                    {
                        var value1 = (IntegerMath.FloorRoot(m / z, 2) + 1) / 2;
                        sum -= mu[d] * value1 * value1;
                        var value2 = (m / (z * z) + 1) / 2;
                        sum += mu[d] * value2;
                    }
                }
                var mod = (sum + (n + (n & 1)) % 3) % 3;
#endif
#if false
                var sum = 0;
                for (var d = 1; d <= root3; d++)
                {
                    var m = root3 / d;
                    sum += mu[d] * m * m * m;
                }
                var mod = sum % 9;
#endif
#if false
                var sum = 0;
                for (var d = 1; d <= n; d++)
                    sum += mu[d] * (n / d) * (n / d);
                var mod = sum % 4;
#endif
#if false
                var sum = 0;
                for (var d = 1; d <= n; d += 2)
                    sum += mu[n / d];
                var mod = sum;
#endif
#if false
                var sum = 0;
                for (var d = 1; d <= n; d++)
                {
                    if (d % 2 == 0)
                        continue;
                    sum += IntegerMath.Power(-1, 0) * IntegerMath.Mertens(n / d);
                }
                var mod = sum % 4;
#endif
#if false
                var sum = 0;
                for (var d = 1; d <= n; d += 2)
                    sum += IntegerMath.MertensOdd(n / d);
                var mod = sum;
#endif
                Console.WriteLine("n = {0}, sum = {1}, mod = {2}", n, sum, mod);
            }
#endif

#if false
            var algorithm1 = new DivisionFreeDivisorSummatoryFunction(0, false, false);
            var algorithm2 = new DivisionFreeDivisorSummatoryFunction(0, false, true);
            var timer = new Stopwatch();
            for (var i = 5; i <= 20; i++)
            {
                var n = IntegerMath.Power((BigInteger)10, i);

                timer.Restart();
                var sum1 = algorithm1.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);

                timer.Restart();
                var sum2 = algorithm2.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);

                Console.WriteLine();
            }
#endif

#if false
            var n = 10000;
            var sqrt = IntegerMath.FloorRoot(n, 2);
            var sum1 = T2Odd(n);
            var algorithm2 = new DivisionFreeDivisorSummatoryFunction(0, false, true);
            var sum2 = algorithm2.Evaluate(n);
            Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
#endif

#if false
            var mu = new MobiusCollection(101, 0);
            for (var n = 1; n <= 100; n++)
            {
                var sum = 0;
                for (var d = 1; d <= n; d++)
                {
                    Console.WriteLine("mu[{0}] = {1}, (n / d)^2 = {2}", d, mu[d], IntegerMath.Power(n / d, 2));
                    sum += mu[d] * IntegerMath.Power(n / d, 2);
                }
                Console.WriteLine("n = {0}, sum = {1}, mod4 = {2}", n, sum, sum % 4);
            }
#endif

#if false
            for (var i = 1; i <= 25; i++)
            {
                var n = IntegerMath.Power(2, i);
                var value1 = PrimeCounting.PiPowerOfTwo(i) % 2;
                var value2 = ParityOfPi(n);
                Console.WriteLine("i = {0}, value1 = {1}, value2 = {2}", i, value1, value2);
            }
#endif

#if false
            for (var n = 30; n <= 100; n += 1)
            {
                var h = new string[n + 1, n + 1];
                for (var i = 1; i <= n; i++)
                {
                    for (var j = 1; j <= n; j++)
                        h[i, j] = i * j <= n ? (i % 2 != 0 && j % 2 != 0 ? "+" : "x") : " ";
                }
                var odd = 0;
                var both = 0;
                for (var i = n; i >= 1; i--)
                {
                    var s = "";
                    for (var j = 1; j <= n; j++)
                    {
                        if (h[i, j] != " ")
                            ++both;
                        if (h[i, j] == "+")
                            ++odd;
                        s += h[i, j];
                    }
                    //Console.WriteLine(s);
                }
                var sum1 = T2Odd(n) + 2 * T2(n / 2) - T2(n / 4);
                var sum2 = 0;
                Console.WriteLine("odd = {0}, even = {1}, both = {2}", odd, both - odd, both);
                Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
            }
#endif

#if false
            var algorithm = new DivisionFreeDivisorSummatoryFunction(0, false);
            var timer = new Stopwatch();
            for (var i = 5; i <= 20; i++)
            {
                timer.Restart();
                for (var iterations = 0; iterations < 1; iterations++)
                {
                    var n = IntegerMath.Power((BigInteger)10, i);
                    var p1 = PrimeCounting.PiPowerOfTen(i) % 2;
                    var limit = (int)IntegerMath.FloorRoot(n, 3);
                    var mu = new MobiusCollection((int)limit + 1, 0);
                    var sum = (BigInteger)0;
                    for (var j = 1; j <= limit; j++)
                        sum += mu[j] * algorithm.Evaluate(n / IntegerMath.Power((BigInteger)j, 3));
                    var t = sum - 1 - 3 * n + 3 * SquareFreeCounting.PowerOfTen(i);
                    var p2 = t / 2 % 2;
                    if (iterations == 0)
                        Console.WriteLine("i = {0}, p1 = {1}, p2 = {2}", i, p1, p2);
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif

#if false
            for (int j = 1; j <= 5; j++)
            {
                var timer = new Stopwatch();
                timer.Restart();
                var mu1 = SimplerMobiusCollection.GetMu(1000000);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                timer.Restart();
                var mu2 = new SimpleMobiusCollection(1000000 + 1);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#if true
                for (int i = 1; i <= 1000000; i++)
                {
                    if (mu1[i] != mu2[i])
                        output.WriteLine("mu1[{0}] = {1}, mu2[{2}] = {3}", i, mu1[i], i, mu2[i]);
                }
#endif
            }
#endif

#if false
            // Test showing that n % 3 is optimized by the JIT compiler.
            var d = new List<int> { 1, 2, 3 }.Count;
            var timer = new Stopwatch();
            timer.Restart();
            var sum1 = (long)0;
            for (var i = (long)1; i < 1000000000; i++)
            {
                sum1 += i % 3;
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            timer.Restart();
            var sum2 = (long)0;
            for (var i = (long)1; i < 1000000000; i++)
            {
                sum2 += i % d;
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
#endif

#if false
            var mu = new MobiusCollection((1 << 20) + 1, 8);
            var p = 5;
#if true
            for (int pow = 1; pow <= 50; pow++)
            {
                var n = (BigInteger)1 << pow;
                var root = IntegerMath.FloorRoot(n, p);
#else
            for (var n = (BigInteger)1; n <= 1000; n++)
            {
                var root = n;
#endif
                var sum = (BigInteger)0;
                for (int i = 1; i <= root; i++)
                    sum += mu[i] * IntegerMath.Power(root / i, p);
                //sum = (sum - 1) / p;
                Debug.Assert((sum - 1) % (p * p) % p == 0);
                sum = (sum - 1) % (p * p) / p;
                Console.WriteLine("n = {0}, sum = {1}", n, sum);
            }
#endif

#if false
            var p = 3;
            var p2 = p * p;
            var mu = new MobiusCollection(1000, 0);
            for (int i = 1; i <= 40; i++)
            {
                var smt = IntegerMath.Binomial(i + p - 1, i);
                if (i >= p)
                    smt -= IntegerMath.Binomial(i - 1, i - p);
                Console.WriteLine("i = {0}, smt(i) = {1}, smt(i)%{2} = {3}", i, smt, p2, smt / p % p);
            }
#endif

#if false
            var algorithm1 = new DivisionFreeDivisorSummatoryFunction(0, false);
            var algorithm2 = new Divisors();
            var timer = new Stopwatch();

            //algorithm.main();
            for (int b = 9; b <= 10; b++)
            {
                var n = (BigInteger)1 << b;
                Console.WriteLine();
                timer.Restart();
                var sum1 = algorithm1.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("2^{0}: sum1 = {1}", b, sum1);
#if true
                timer.Restart();
                var sum2 = algorithm2.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("2^{0}: sum2 = {1}", b, sum2);
#endif
            }
#endif

#if false
            // Experimental calculation of T3(n) by several methods: brute force, hyperbola, 
            var n = (BigInteger)1 << 20;
            {
                var sum = (BigInteger)0;
                for (var z = (BigInteger)1; z <= n; z++)
                {
                    var nz = n / z;
                    for (var x = (BigInteger)1; x <= nz; x++)
                        sum += nz / x;
                }
                Console.WriteLine("n = {0}, T3(n) = {1}", n, sum);
            }
            {
                var sum = (BigInteger)0;
                var root3 = IntegerMath.FloorRoot(n, 3);
                for (var z = (BigInteger)1; z <= root3; z++)
                {
                    var nz = n / z;
                    var sqrtnz = IntegerMath.FloorSquareRoot(nz);
                    var t = (BigInteger)0;
                    for (var x = z + 1; x <= sqrtnz; x++)
                        t += nz / x;
                    sum += 2 * t - sqrtnz * sqrtnz + nz / z;
                }
                sum = 3 * sum + root3 * root3 * root3;
                Console.WriteLine("n = {0}, T3(n) = {1}", n, sum);
            }
            {
                var a = IntegerMath.FloorRoot(n, 3);
                var i = 3;
                var i1 = i - 1;
                var sum = (BigInteger)0;
                for (var j = (BigInteger)1; j <= a; j++)
                {
                    sum += IntegerMath.NumberOfDivisors(j, i1) * (n / j);
                    for (var l = 1; l < i1; l++)
                    {
                        var t = (BigInteger)0;
                        var nj = n / j;
                        for (var k = a / j + 1; k <= nj; k++)
                            t += IntegerMath.SumOfNumberOfDivisors(nj / k, i1 - l); 
                        sum += IntegerMath.NumberOfDivisors(j, l) * t;
                    }
                }
                for (var j = a + 1; j <= n; j++)
                    sum += IntegerMath.SumOfNumberOfDivisors(n / j, i1);
                Console.WriteLine("n = {0}, T3(n) = {1}", n, sum);
            }
#endif

#if false
            var threads = 8;
            var algorithm1 = new DivisionFreeDivisorSummatoryFunction(threads, false);
            var algorithm2 = new DivisionFreeDivisorSummatoryFunction(threads, true);
            var timer = new Stopwatch();
            for (int b = 1; b <= 96; b++)
            {
                var n = (BigInteger)1 << b;
#if false
                Console.WriteLine();
#endif
                timer.Restart();
                var sum1 = algorithm1.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("2^{0}: sum1 = {1}", b, sum1);
#if false
                timer.Restart();
                var sum2 = algorithm2.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("2^{0}: sum2 = {1}", b, sum2);
#endif
            }
#endif

#if false
            var threads = 8;
            var n = (BigInteger)1 << 60;
            var algorithm1 = new PrimeCounting(0);
            var algorithm2 = new DivisionFreeDivisorSummatoryFunction(threads, false);
            var algorithm3 = new DivisionFreeDivisorSummatoryFunction(threads, true);
            var xmax = (ulong)IntegerMath.FloorRoot(n, 2);
            var timer = new Stopwatch();
            for (int i = 1; i <= 1; i++)
            {
                timer.Restart();
                var sum1 = 0;
#if false
                {
                    ulong sqrt = 0;
                    sum1 = algorithm1.TauSumInnerLarge((UInt128)n, out sqrt);
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
                var sum2 = (BigInteger)0;
#if true
                timer.Restart();
                sum2 = algorithm2.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
                var sum3 = (BigInteger)0;
#if true
                timer.Restart();
                sum3 = algorithm3.Evaluate(n);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
                var sum4 = (BigInteger)0;
#if true
                timer.Restart();
                {
                    var t = (BigInteger)0;
                    for (var x = (ulong)1; x <= xmax; x++)
                        t += n / x;
                    sum4 = (t << 1) - (BigInteger)xmax * xmax;
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
                var sum5 = (BigInteger)0;
#if true
                timer.Restart();
                {
                    var store = new MutableIntegerStore(4);
                    var t = store.Allocate();
                    var reg1 = store.Allocate();
                    var reg2 = store.Allocate();
                    var reg3 = store.Allocate();
                    var nRep = store.Allocate().Set(n);
                    for (var x = (ulong)1; x <= xmax; x++)
                    {
                        reg1.Set(nRep).ModuloWithQuotient(reg2.Set(x), reg3);
                        t.Add(reg3);
                    }
                    sum5 = (t << 1) - (MutableInteger)xmax * xmax;
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
                var sum6 = (BigInteger)0;
#if true
                timer.Restart();
                {
                    var t = (UInt128)0;
                    var nRep = (UInt128)n;
                    for (var x = (ulong)1; x <= xmax; x++)
                        t += nRep / x;
                    sum6 = (t << 1) - (UInt128)xmax * xmax;
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
                Console.WriteLine("n = {0}, sum1 = {1}, sum2 = {2}, sum3 = {3}, sum4 = {4}, sum5 = {5}, sum6 = {6}", n, sum1, sum2, sum3, sum4, sum5, sum6);
            }
#endif

#if false
            var algorithm = new DivisorSummatoryFunctionArticle();
            var n = 1 << 30;
            var xmax = IntegerMath.FloorRoot(n, 2);
            var sum = algorithm.S1(n, 1, xmax);
            Console.WriteLine("n = {0}, sum = {1}", n, sum);
#endif

#if false
#if true
            var algorithm = new DivisorSummatoryFunctionArticle();
            var nmax = (BigInteger)1 << 20;
            for (var n = (BigInteger)36; n <= nmax; n++)
            {
                var fast = algorithm.Evaluate(n);
                var slow = (BigInteger)0;
                var imax = IntegerMath.FloorRoot(n, 2);
                for (var i = (BigInteger)1; i <= imax; i++)
                    slow += n / i;
                slow = 2 * slow - imax * imax;
                if (fast != slow)
                {
                    Console.WriteLine("n = {0}, fast = {1}, slow = {2}", n, fast, slow);
                    algorithm.Evaluate(n);
                    break;
                }
                if (n % 1000 == 0)
                    Console.WriteLine("n = {0}...", n);
            }
#else
            var algorithm = new DivisorSummatoryFunctionArticle();
            var n = 3380;
            var fast = algorithm.Evaluate(n);
            var slow = (BigInteger)0;
            var imax = IntegerMath.FloorRoot(n, 2);
            for (var i = (BigInteger)1; i <= imax; i++)
                slow += n / i;
            slow = 2 * slow - imax * imax;
            Console.WriteLine("n = {0}, fast = {1}, slow = {2}", n, fast, slow);
#endif
#endif

#if false
            var diag = false;
            var algorithm = new DivisorSummatoryFunction();
            var algorithm2 = new DivisorSummatoryFunction2(diag);
            var n = (BigInteger)1 << 60;
            var timer = new Stopwatch();
            timer.Restart();
            var sum = algorithm.Evaluate(n);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            timer.Restart();
            var sum2 = algorithm2.Evaluate(n);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            Console.WriteLine("sum = {0}, sum2 = {1}", sum, sum2);
#endif

#if false
            for (var i = BigInteger.Pow(10, 26); i < BigInteger.Pow(10, 27); i++) Console.WriteLine(i);
#endif
#if false
            var algorithm = new PrimeCounting(8);
            for (var e = 0; e < 20; e++)
            {
                var s = algorithm.NumberOfSquareFree(IntegerMath.Power((BigInteger)10, e));
                Console.WriteLine("e = {0}, s = {1}", e, s);
            }
#endif

#if false
#if false
            var algorithm = new DivisorSummatoryFunction(false);
            var nmax = (BigInteger)1 << 20;
            for (var n = (BigInteger)36; n <= nmax; n++)
            {
                var fast = algorithm.Evaluate(n);
                var slow = (BigInteger)0;
                var imax = IntegerMath.FloorRoot(n, 2);
                for (var i = (BigInteger)1; i <= imax; i++)
                    slow += n / i;
                slow = 2 * slow - imax * imax;
                if (fast != slow)
                {
                    Console.WriteLine("n = {0}, fast = {1}, slow = {2}", n, fast, slow);
                    break;
                }
                if (n % 1000 == 0)
                    Console.WriteLine("n = {0}...", n);
            }
#else
            var diag = false;
            var algorithm = new DivisorSummatoryFunction();
            var n = (BigInteger)1 << 60;
            if (!diag)
                algorithm.Evaluate((BigInteger)1 << 30);
            var timer = new Stopwatch();
            timer.Restart();
            var sum = (BigInteger)0;
            sum = algorithm.Evaluate(n);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            timer.Restart();
#if true
            var slow = (BigInteger)0;
            var imax = IntegerMath.FloorSquareRoot(n);
            for (var i = (BigInteger)1; i <= imax; i++)
                slow += n / i;
            slow = 2 * slow - imax * imax;
#else
            var algorithm2 = new PrimeCounting(8);
            var slow = algorithm2.TauSum((UInt128)n);
#endif
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            Console.WriteLine("n = {0}, sum = {1}, slow = {2}", n, sum, slow);
#endif
#endif

#if false
            var maxRep = (BigInteger)1 << 53;
            var n = maxRep * maxRep * 7 / 10;
            var s = (BigInteger)Math.Floor(Math.Sqrt((double)n));
            var r = n - s * s;
            if (r.Sign == -1)
                --s;
            else if (r > (s << 1))
               ++s;
            var sqrt = IntegerMath.FloorRoot(n, 2);
            Console.WriteLine("");
#endif

#if false
            var p = 99;
            var q = 13;
            for (var t = 0; t <= p * q; t++)
            {
                var count1 = 0;
                for (int i = 0; i <= q; i++)
                {
                    for (int j = 0; j <= p; j++)
                    {
                        if (i * p + j * q <= t)
                            ++count1;
                    }
                }
                var count2 = GetLatticeCount(p, q, t);
                Debug.Assert(count1 == count2);
                Console.WriteLine("t = {0}, count1 = {1}, count2 = {2}", t, count1, count2);
            }
#endif

#if false
            var y = IntegerMath.NextPrime((BigInteger)1 << 40);
            var imax = IntegerMath.FloorRoot(y, 2);
            var imin = IntegerMath.FloorRoot(y, 3);
            var step = IntegerMath.FloorRoot(y, 6);
            var count0 = (BigInteger)0;
            var count1 = (BigInteger)0;
            var count2 = (BigInteger)0;
            var timer = new Stopwatch();
#if true
            timer.Restart();
            for (var i0 = imax; i0 > imin; i0 -= step + 1)
            {
                var jmax = -step;
                var i0Squared = i0 * i0;
                for (var j = 0; j >= jmax; j--)
                    count0 += y / (i0 + j);
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#if true
            timer.Restart();
            for (var i0 = imax; i0 > imin; i0 -= step + 1)
            {
                var jmax = -step;
                var i0Squared = i0 * i0;
                for (var j = 0; j >= jmax; j--)
                    count1 += (-j + i0) * y / i0Squared;
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#if true
            timer.Restart();
            for (var i0 = imax; i0 > imin; i0 -= step + 1)
            {
                var jmax = -step;
                var div0 = y / i0;
                var p = y;
                var q = i0 * i0;
                var t = p * (i0 - jmax) - q * div0;
                count2 += GetLatticeCount(p, q, t) + (div0 - 1) * (step + 1);
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
            output.WriteLine("count0 = {0}, count1 = {1}, count2 = {2}", count0, count1, count2);
#endif

#if false
            var primes = new SieveOfEratosthenes().Take(10).ToArray();
            for (var i = 0; i < primes.Length - 1; i++)
            {
                for (var j = i + 1; j < primes.Length; j++)
                {
                    var p = primes[i];
                    var q = primes[j];
                    var parity1 = GetLatticeCount(p, q, p * q) % 2;
                    var parity2 = (((p + 1) * (q + 1) / 2) + 1) % 2;
                    Console.WriteLine("p = {0}, q = {1}, parity1 = {2}, parity2 = {3}", p, q, parity1, parity2);
                }
            }
#endif

#if false
#if false
            var a = IntegerMath.Power((BigInteger)2, 34) + 1;
            var b = IntegerMath.Power((BigInteger)2, 35);
            var c = 1;
#endif
#if true
            var a = 8;
            var b = 7;
            var c = 0;
#endif
            var sum1 = ScaledDedekindSumSlow(a, b, c);
            var sum2 = ScaledDedekindSum(a, b, c);
            Console.WriteLine("sum1 = {0}, sum2 = {1}", sum1, sum2);
#endif
#if false
            double y = (double)IntegerMath.NextPrime(1 << 30);
            double imax = Math.Floor(Math.Sqrt(y));
            double imin = Math.Pow(y, (double)1 / 3);
            double i0 = imax;
            int count = 0;
            int parcount = 0;
            while (i0 > imin)
            {
                double a = y / (i0 * i0 * i0);
                double b = -3 * y / (i0 * i0);
                double c = 3 * y / i0;
                double d = 1 - y;
                double lhs = 2 * b * b * b - 9 * a * b * c + 27 * a * a * d;
                double rhs = Math.Sqrt(lhs * lhs - 4 * Math.Pow(b * b - 3 * a * c, 3));
                double i1 = (-b - Math.Pow((lhs + rhs) / 2, (double)1 / 3) - Math.Pow((lhs - rhs) / 2, (double)1 / 3))/(3*a);
                double e = i1 * (y / i1 - (a * i1 * i1 + b * i1 + c));
                Console.WriteLine("|[{0}, {1})| = {2}, e = {3}", i1, i0, i0 - i1, e);
                var jlimit = Math.Ceiling(i1 - i0);
                for (double j = 0; j > jlimit; j--)
                {
                    var i = i0 + j;
                    if (Math.Floor(y / i) != Math.Floor(a * i * i + b * i + c))
                    {
                        Debugger.Break();
                        Console.WriteLine();
                    }
                    var divlin = Math.Floor(a * (-i0 * j + i0 * i0));
                    var divpar = Math.Floor(a * (j * j - i0 * j + i0 * i0));
                    if (divpar != divlin)
                        ++parcount;
                    if ((divpar - divlin != 0) != ((j - 1) * (j - 1) > y % i))
                    {
                        Debugger.Break();
                        Console.WriteLine("i = {0}, divpar - divlin = {1}", i, divpar - divlin);
                    }
                }
                i0 = Math.Ceiling(i1);
                ++count;
#if false
                if (count == 5)
                    break;
#endif
            }
            Console.WriteLine("C(count) = {0}", count/(Math.Pow(y, (double)1 / 3) * Math.Log(y)));
            Console.WriteLine("parcount/count = {0}", (double)parcount / count);
#endif
#if false
            double y = 1 << 20;
            double sqrty = 1 << 10;
            double a = 1/sqrty;
            double b = -3;
            double c = 3*sqrty;
            double d = 1-y;
            double discriminant = 18*a*b*c*d - 4*b*b*b*d + b*b*c*c - 4*a*c*c*c - 27*a*a*d*d;
            double tmp1 = 2*b*b*b - 9*a*b*c + 27*a*a*d;
            double tmp2 = Math.Sqrt(tmp1*tmp1 - 4*Math.Pow(b*b - 3*a*c, 3));
            double j = Math.Pow(y, (double)1/6);
            double imax = sqrty - j;
            double div = y/imax;
            double divpar = a*imax*imax + b*imax + c;
            double e = (div - divpar) * imax;
            Console.WriteLine();
#endif
#if false
            var y = 1 << 20;
            var imax = (1 << 10) + 1;
            var imin = 1;
            var div = y / imax;
            var d1 = 1;
            for (var i = imax - 1; i >= imin; i--)
            {
                var d2 = (y / i - div) - d1;
                d1 = y / i - div;
                div = y / i;
                Console.WriteLine("{0} / {1} = {2}, {3} % {4} = {5}, d1 = {6}, d2 = {7}", y, i, y / i, y, i, y % i, d1, d2);
            }
#endif
#if false
            var algorithm = new PrimeCounting(8);
            var b = 10;
            for (int i = 24; i <= 24; i++)
            {
                for (int j = 5; j <= 9; j++)
                {
                    Console.WriteLine("Start Time: {0}", DateTime.Now);
                    var timer = new Stopwatch();
                    timer.Start();
                    var n = j * IntegerMath.Power((BigInteger)b, i);
                    Console.WriteLine("{0}*{1}^{2}, parity of pi(n) = {3}", j, b, i, algorithm.ParityOfPi(n));
                    output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
            }
#endif
#if false
            var algorithm = new PrimeCounting(8);
            for (int j = 0; j < 1; j++)
            {
                for (int i = 64; i <= 64; i++)
                {
                    var timer = new Stopwatch();
                    var n = (UInt128)1 << i;
                    timer.Restart();
#if true
                    var tau1 = algorithm.TauSumParallel(n);
#else
                    ulong sqrt;
                    var tau1 = algorithm.TauSumInner(n, out sqrt);
#endif
                    Console.WriteLine("i = {0}, n = {1}, sum(n) = {2}", i, n, tau1);
                    output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#if false
                    timer.Restart();
                    var tau2 = algorithm.TauSum(n);
                    Console.WriteLine("i = {0}, n = {1}, sum(n) = {2}", i, n, tau2);
                    output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#if false
                    if (tau1 != tau2)
                    {
                        Debugger.Break();
                        Console.WriteLine();
                    }
#endif
                }
            }
#endif
#if false
            var timer = new Stopwatch();
            var timer2 = new Stopwatch();
            timer.Start();
            var n = IntegerMath.Power((long)2, 40);
            var mobius = new MobiusRange(n + 1, 8);
            var batchSize = 1 << 30;
            var values = new sbyte[batchSize];
            var kmax = mobius.Size;
            var kmin = kmax - batchSize;
            timer2.Restart();
            mobius.GetValues(kmin, kmax, values);
            output.WriteLine("elapsed2 = {0:F3} msec", (double)timer2.ElapsedTicks / Stopwatch.Frequency * 1000);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#if false
            var timer = new Stopwatch();
            var timer2 = new Stopwatch();
            timer.Start();
            var n = IntegerMath.Power((long)2, 40);
            var mobius = new MobiusRange(n + 1, 8);
            var batchSize = 1 << 30;
            var values = new sbyte[batchSize];
            var sum = 0;
            for (var kmin = (long)1; kmin < n; kmin += batchSize)
            {
                var kmax = Math.Min(kmin + batchSize, mobius.Size);
                timer2.Restart();
                mobius.GetValues(kmin, kmax, values);
                output.WriteLine("elapsed2 = {0:F3} msec", (double)timer2.ElapsedTicks / Stopwatch.Frequency * 1000);
#if false
                var length = kmax - kmin;
                for (var i = 0; i < length; i++)
                    sum += values[i];
#endif
            }
            Console.WriteLine("Sum(mobius) = {0}", sum);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#if false
            Console.WriteLine("procs = {0}", Environment.ProcessorCount);
#endif
#if false
            var timer = new Stopwatch();
            timer.Start();
            var n = 1 << 30;
            var mobius = new MobiusCollection(n + 1, 8);
            Console.WriteLine("|mobius| = {0}", mobius.Size);
#if false
            for (int i = 0; i < primes.Count; i++)
                Console.WriteLine("prime[{0}] = {1}", i, primes[i]);
#endif
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#if false
            var n = 1 << 30;
            var mobius1 = new SimpleMobiusCollection(n + 1);
            var mobius2 = new MobiusCollection(n + 1, 8);
            for (var i = 1; i < n; i++)
            {
                if (mobius1[i] != mobius2[i])
                {
                    Debugger.Break();
                    Console.WriteLine();
                }
            }
#endif
#if false
            var timer = new Stopwatch();
            timer.Start();
            var n = (long)1 << 32;
            var primes = new PrimeCollection(n + 1, 0);
            Console.WriteLine("|primes| = {0}", primes.Count);
#if false
            for (int i = 0; i < primes.Count; i++)
                Console.WriteLine("prime[{0}] = {1}", i, primes[i]);
#endif
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
#if false
            for (var j = 10; j <= 20; j += 2)
            {
                var n = 1 << j;
                var count = 0;
                for (var i = 2; i < n; i++)
                {
                    if (IntegerMath.IsSquareFree(i) && factors.All(factor => factor * factor < i))
                        ++count;
                }
                Console.WriteLine("n = {0}, count = {1}, n / count = {2}", n, count, (double)n / count);
            }
#endif
#if false
            {
                var n = 1023;
                var a = 93;
                var b = 777;
                var sum = 0;
                var timer = new Stopwatch();
                timer.Restart();
                for (var i = 0; i < int.MaxValue; i++)
                {
                    if (++a >= n)
                        a -= n;
                    if (++b >= n)
                        b -= n;
                    var c = a + b;
                    if (c >= n)
                        c -= n;
                    sum += c;
                }
                output.WriteLine("elapsed = {0:F3} msec, sum = {1}", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000, sum);
            }
            {
                var n = 1023;
                var a = 93;
                var b = 777;
                var sum = 0;
                var timer = new Stopwatch();
                timer.Restart();
                for (var i = 0; i < int.MaxValue; i++)
                {
                    if (++a >= n)
                        a -= n;
                    if (++b >= n)
                        b -= n;
                    var c = a + b;
                    c -= ((n - c - 1) >> 31) & n;
                    sum += c;
                }
                output.WriteLine("elapsed = {0:F3} msec, sum = {1}", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000, sum);
            }
#endif
#if false
            var algorithm = new PrimeCounting(4);
            for (int j = 0; j < 1; j++)
            {
                for (int i = 55; i <= 55; i++)
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    var n = (BigInteger)1 << i;
                    Console.WriteLine("i = {0}, n = {1}, parity of pi(n) = {2}", i, n, algorithm.ParityOfPi(n));
                    output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                }
            }
#if false
            foreach (var pair in algorithm.TauSumMap.OrderBy(pair => pair.Key))
                Console.WriteLine("key = {0}, value = {1}", pair.Key, pair.Value);
#endif
#endif
#if false
            for (var n = 0; n <= 30; n++)
            {
#if false
                if (n != 1 && !IntegerMath.IsPrime(n))
                    continue;
#endif
                Console.Write("{0,3}: ", n);
                //Console.WriteLine(string.Join(" ", Enumerable.Range(1, 100).Where(i => IntegerMath.IsPrime(i)).Select(i => string.Format("{0,2}", n % (2 * i)))));
                //Console.WriteLine(string.Join(" ", Enumerable.Range(1, 30).Select(i => string.Format("{0,2}", n % (2 * i)))));
                //Console.WriteLine(string.Join(" ", Enumerable.Range(1, Math.Min(30, (int)Math.Floor(Math.Sqrt(n)))).Select(i => string.Format("{0,2}", n % (2 * i) >= i ? 1 : 0))));
                //Console.WriteLine(string.Join(" ", Enumerable.Range(1, Math.Min(30, (int)Math.Floor(Math.Sqrt(n)))).Select(i => string.Format("{0,3}", n % (2 * i) - i))));
                Console.WriteLine(string.Join(" ", Enumerable.Range(1, 22).Select(i => string.Format("{0,3}", n % (2 * i) - i))));
            }
#endif
#if false
            TauSumInner(20);
#endif
#if false
            var reps = 100000;
            var timer = new Stopwatch();
            for (int i = 2; i <= 20; i += 2)
            {
                var algorithm = new PrimeCounting();
#if false
                timer.Restart();
                for (int rep = 0; rep < reps; rep++)
                    algorithm.TauSumInner(1 << i);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
                timer.Restart();
                for (int rep = 0; rep < reps; rep++)
                    TauSumInner(i);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                output.WriteLine();
            }
#endif
#if false
            var algorithm = new PrimeCounting();
            for (int n = 1; n < 25; n++)
                Console.WriteLine("n = {0}, TauSumInner(n) = {1}", n, algorithm.TauSumInner(n));
#endif
#if false
            var algorithm = new PrimeCounting();
            for (int i = 2; i <= 2; i++)
            {
                var n = 1 << i;
                Console.WriteLine("i = {0}, n = {1}, parity of pi(n) = {2}", i, n, algorithm.ParityOfPi(n));
            }
#endif
#if false
            for (int n = 1; n <= 100; n++)
            {
                //Console.WriteLine("n = {0}, 2^w(n)0 = {1}, 2^w(n)1 = {2}", n, TwoToTheOmega0(n), TwoToTheOmega1(n));
                //Console.WriteLine("n = {0}, Sum(2^w(n))0 = {1}, Sum(2^w(n))1 = {2}, Sum(2^w(n)) = {3}", n, SumTwoToTheOmega0(n), SumTwoToTheOmega1(n), SumTwoToTheOmega(n));
                //Console.WriteLine("pi'(n + 1) = {0}, Sum(2^w(n)) / 2 % 2 = {1}", PiWithPowers(n + 1), SumTwoToTheOmega(n) / 2 % 2);
                Console.WriteLine("pi(n) % 2 = {0}, parity pi(n) = {1}", Pi(n) % 2, ParityOfPi(n));
                Console.WriteLine();
            }
#endif
#if false
            for (int i = 1; i <= 10; i++)
                Console.WriteLine("tau({0}) = {1}, sum = {2}", i, Tau(i), TauSum(i));
            for (int i = 0; i < 1; i++)
            {
                var n = (i + 1) * 10;
                Console.WriteLine("n = {0}", n);
                var bruteForcePi = BruteForcePi(n);
                Console.WriteLine("BruteForcePi(n) = {0}", bruteForcePi);
                var bruteForce1 = BruteForce1(n);
                Console.WriteLine("BruteForce1(n) = {0}, % 4 / 2 = {1}", bruteForce1, bruteForce1 % 4 / 2);
                var bruteForce2 = BruteForce2(n);
                Console.WriteLine("BruteForce2(n) = {0}, % 4 / 2 = {1}", bruteForce2, bruteForce2 % 4 / 2);
                var pi = Pi(n);
                Console.WriteLine("Pi(n) = {0}", pi);
                var tauSlow = SlowTauSum(n);
                var mu = MuSum(n);
                Console.WriteLine("MuSum(n) = {0}, % 4 / 2 = {1}", mu, mu % 4 / 2);
            }
#endif
#if false
                var tau = TauSum(n);
                Console.WriteLine("TauSumSlow(n) = {0}, TauSum(n) = {1}", tauSlow, tau);
#endif
#if false
            for (var power = 1; power <= 15; power++)
            {
                var n = IntegerMath.Power((BigInteger)10, power);
                Console.WriteLine("TauSum({0}) = {1}", n, TauSum(n));
            }
#endif
        }

        private static BigInteger TauSumInner(long n)
        {
            var limit = (int)Math.Floor(Math.Sqrt(n));
            var sum1 = 0;
            var current = limit - 1;
            var delta = 1;
            var i = limit;
            while (i > 0)
            {
                var product = (long)(current + delta) * i;
                if (product > n)
                    --delta;
                else if (product + i <= n)
                {
                    ++delta;
                    if (product + 2 * i <= n)
                        break;
                }
                current += delta;
                sum1 ^= current;
                --i;
            }
            sum1 &= 1;
            var sum2 = 0;
            var count2 = 0;
            while (i > 0)
            {
                sum2 ^= (int)(n % (i << 1)) - i;
                --i;
                ++count2;
            }
            sum2 = (sum2 >> 31) & 1;
            if ((count2 & 1) != 0)
                sum2 ^= 1;
            return sum1 ^ sum2;
        }

        private static BigInteger TauSumInner3(long n)
        {
            var limit = (int)Math.Floor(Math.Sqrt(n));
            var sum = 0;
            for (int i = 1; i <= limit; i++)
                sum ^= (int)(n % (i << 1)) - i;
            sum = (sum >> 31) & 1;
            return (limit & 1) == 0 ? sum : sum ^ 1;
        }

        private static int[] div = new int[1024 + 1];
        private static int[] mod = new int[1024 + 1];

        private static int TauSumInner2(int k)
        {
            var limit = 1 << (k / 2);

            for (var i = 1; i <= limit; i++)
                mod[i] = 1;
#if false
            Console.Write("j = {0,2}", 0);
            Console.Write(", div = {0}", string.Join(", ", div.Skip(1).Take(10)));
            Console.WriteLine(", mod = {0}", string.Join(", ", mod.Skip(1).Take(10)));
#endif
            for (var j = 1; j <= k; j++)
            {
                for (var i = 1; i <= limit; i++)
                {
                    mod[i] <<= 1;
                    if (mod[i] >= i << 1)
                        mod[i] -= i << 1;
                }
#if true
                var n = 1 << j;
                for (var i = 1; i <= limit; i++)
                {
                    var d = n / i % 2;
                    var m = n % i;
                    if (d != mod[i] / i || m != mod[i] % i)
                    {
                        Debugger.Break();
                        Console.WriteLine();
                    }
                }
#endif
#if false
                Console.Write("j = {0,2}", j);
                Console.Write(", div = {0}", string.Join(", ", div.Skip(1).Take(10)));
                Console.WriteLine(", mod = {0}", string.Join(", ", mod.Skip(1).Take(10)));
#endif
            }
            var sum = 0;
            for (var i = 1; i <= limit; i++)
                sum ^= div[i];
            Console.WriteLine("div = {0}", string.Concat(mod.Select((m, i) => m >= i ? 1 : 0).Skip(1)));
            return sum;
        }

        private static int TauSumInner1(int k)
        {
            var limit = 1 << (k / 2);

            div[1] = 1;
            for (var i = 2; i <= limit; i++)
                mod[i] = 1;
#if false
            Console.Write("j = {0,2}", 0);
            Console.Write(", div = {0}", string.Join(", ", div.Skip(1).Take(10)));
            Console.WriteLine(", mod = {0}", string.Join(", ", mod.Skip(1).Take(10)));
#endif
            for (var j = 1; j <= k; j++)
            {
                for (var i = limit >> 1; i >= 0; i--)
                {
                    div[i << 1] = div[i];
                    mod[i << 1] = mod[i] << 1;
                }
                for (var i = 1; i < limit; i += 2)
                {
                    div[i] = 0;
                    mod[i] <<= 1;
                    if (mod[i] >= i)
                    {
                        mod[i] -= i;
                        div[i] = 1;
                    }
                }
#if false
                var n = 1 << j;
                for (var i = 1; i <= limit; i++)
                {
                    var d = n / i % 2;
                    var m = n % i;
                    if (d != div[i] || m != mod[i])
                    {
                        Debugger.Break();
                        Console.WriteLine();
                    }
                }
#endif
#if false
                Console.Write("j = {0,2}", j);
                Console.Write(", div = {0}", string.Join(", ", div.Skip(1).Take(10)));
                Console.WriteLine(", mod = {0}", string.Join(", ", mod.Skip(1).Take(10)));
#endif
            }
            var sum = 0;
            for (var i = 1; i <= limit; i++)
                sum ^= div[i];
            Console.WriteLine("div = {0}", string.Concat(div.Skip(1)));
            return sum;
        }

        private static int TwoToTheOmega0(int n)
        {
            var omega = new TrialDivisionFactorization().Factor(n).OrderBy(factor => factor).Distinct().Count();
            return 1 << omega;
        }

        private static int TwoToTheOmega1(int n)
        {
            var sum = 0;
            for (int d = 1; true; d++)
            {
                var dSquared = d * d;
                if (dSquared > n)
                    break;
                if (n % dSquared == 0)
                    sum += IntegerMath.Mobius(d) * Tau(n / dSquared);
            }
            return sum;
        }

        private static int SumTwoToTheOmega0(int x)
        {
            var sum = 0;
            for (int n = 1; n <= x; n++)
                sum += TwoToTheOmega0(n);
            return sum;
        }

        private static int SumTwoToTheOmega1(int x)
        {
            var sum = 0;
            for (int n = 1; n <= x; n++)
                sum += TwoToTheOmega1(n);
            return sum;
        }

        private static int BruteForce1(int x)
        {
            var sum = 0;
            for (int n = 1; n < x; n++)
                sum += Tau(n) * Math.Abs(IntegerMath.Mobius(n));
            return sum;
        }

        private static int BruteForce2(int x)
        {
            var limit = (int)Math.Ceiling(Math.Sqrt(x));
            var sum = 0;
            for (int d = 1; d < limit; d++)
            {
                var dSquared = d * d;
                var tau = 0;
                for (int n = dSquared; n < x; n += dSquared)
                    tau += Tau(n);
                sum += IntegerMath.Mobius(d) * tau;
            }
            return sum;
        }

        private static int BruteForcePi(int x)
        {
            var sum = 0;
            for (int n = 1; n < x; n++)
                sum += Tau(n) * Math.Abs(IntegerMath.Mobius(n)) % 4;
            return (sum - 1) / 2;
        }

        private static int SlowTauSum(int y)
        {
            var sum = 0;
            for (int i = 1; i <= y; i++)
                sum += Tau(i);
            return sum;
        }

        private static int Tau(int y)
        {
            var tau = 0;
            for (int i = 1; i <= y; i++)
            {
                if (y % i == 0)
                    ++tau;
            }
            return tau;
        }

        private static BigInteger TauSum(BigInteger y)
        {
            var sum = (BigInteger)0;
            var n = (BigInteger)1;
            while (true)
            {
                var term = y / n - n;
                if (term < 0)
                    break;
                sum += term;
                ++n;
            }
            sum = 2 * sum + n - 1;
            return sum;
        }

        private static int TauSum2(int y)
        {
            // http://michaelnielsen.org/polymath1/index.php?title=Prime_counting_function
            var limit = (int)Math.Floor(Math.Sqrt(y));
            var sum = 0;
            for (int n = 1; n < limit; n++)
            {
                sum += y / n;
            }
            sum = 2 * sum - limit * limit;
            return sum;
        }

        private static int TauSum3(int y)
        {
            // Brute force search.
            var sum = 0;
            for (int a = 1; a < y; a++)
                for (int b = 1; b < y; b++)
                    if (a * b < y)
                        ++sum;
            return sum;
        }

        static void FloorRootTest()
        {
            var max = IntegerMath.Power(BigInteger.Parse("47021221151697033430710241825459573109"), 3);
            var random = new MersenneTwister(0).Create<BigInteger>();
            for (var i = 0; i < 1000000; i++)
            {
                var x = random.Next(max);
                var n = (int)random.Next(20 - 2) + 2;
                var root = IntegerMath.FloorRoot(x, n);
                if (IntegerMath.Power(root, n) <= x && IntegerMath.Power(root + 1, n) > x)
                    continue;
                Debugger.Break();
                Console.WriteLine("miscalculation");
            }
        }

        static void PerfectPowerTest()
        {
            var timer = new Stopwatch();
            timer.Restart();
            var max = IntegerMath.Power((BigInteger)2, 3 * 64);
            var random = new MersenneTwister(0).Create<BigInteger>();
            var samples = random.Sequence(max).Take(500).ToArray();
            var primes = samples.Select(sample => IntegerMath.NextPrime(sample)).ToArray();
            var average = samples.Zip(primes, (sample, prime) => (double)(prime - sample)).Average();
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            timer.Restart();
            for (var j = 0; j < 100; j++)
            {
                for (var i = 0; i < primes.Length; i++)
                {
                    var x = primes[i];
#if false
                    var n = (int)random.Next(20 - 1) + 1;
#else
                    var n = 1;
#endif
                    var y = IntegerMath.Power(x, n);
                    var power = IntegerMath.PerfectPower(y);
                    if (power == n)
                        continue;
                    Debugger.Break();
                    Console.WriteLine("miscalculation");
                }
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        static void FindPrimeTest1()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
            var limit = BigInteger.One << (32 * 4);
            var x = random.Next(limit);
            while (!IntegerMath.IsPrime(x))
                ++x;
            output.WriteLine("x = {0}", x);
        }

        static void BarrettReductionTest1()
        {
            var p = BigInteger.Parse("10023859281455311421");
            var random = new MersenneTwister(0).Create<BigInteger>();
            var x = random.Next(p);
            var y = random.Next(p);
            var z = x * y;
            var expected = z % p;
            var bLength = 32;
            var b = BigInteger.One << bLength;
            var k = (p.GetBitLength() - 1) / bLength + 1;
            var mu = BigInteger.Pow(b, 2 * k) / p;
            var bToTheKPlusOne = BigInteger.Pow(b, k + 1);

            var qhat = (z >> (bLength * (k - 1))) * mu >> (bLength * (k + 1));
            var r = z % bToTheKPlusOne - qhat * p % bToTheKPlusOne;
            if (r.Sign == -1)
                r += bToTheKPlusOne;
            while (r >= p)
                r -= p;
            if (r != expected)
                throw new InvalidOperationException();

            var reducer = new BarrettReduction().GetReducer(p);
            var actual = reducer.ToResidue(z).Value;
            if (actual != expected)
                throw new InvalidOperationException();
        }

        static void BarrettReductionTest2()
        {
            var n = BigInteger.Parse("10023859281455311421");
            var random1 = new MersenneTwister(0).Create<BigInteger>();
            var random2 = new MersenneTwister(0).Create<BigInteger>();
            var timer1 = new Stopwatch();
            var timer2 = new Stopwatch();
            var iterations1 = 1000;
            var iterations2 = 1000;
            var reducer = new BarrettReduction().GetReducer(n);

            timer1.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var a = reducer.ToResidue(random1.Next(n));
                var b = reducer.ToResidue(random1.Next(n));
                var c = reducer.ToResidue(0);

                for (int j = 0; j < iterations2; j++)
                    c.Set(a).Multiply(b);
            }
            var elapsed1 = timer1.ElapsedMilliseconds;

            timer2.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var a = random2.Next(n);
                var b = random2.Next(n);
                var c = BigInteger.Zero;

                for (int j = 0; j < iterations2; j++)
                {
                    c = a * b;
                    c %= n;
                }
            }
            var elapsed2 = timer1.ElapsedMilliseconds;

            output.WriteLine("elapsed1 = {0}, elapsed2 = {1}", elapsed1, elapsed2);
        }

        static void MutableIntegerTest1()
        {
            for (int i = 0; i < 2; i++)
            {
                MutableIntegerTest1("sum:     ", (c, a, b) => c.SetSum(a, b), (a, b) => a + b);
                MutableIntegerTest1("product: ", (c, a, b) => c.SetProduct(a, b), (a, b) => a * b);
            }
        }

        static void MutableIntegerTest1(string label,
            Action<MutableInteger, MutableInteger, MutableInteger> operation1,
            Func<BigInteger, BigInteger, BigInteger> operation2)
        {
            var n = BigInteger.Parse("10023859281455311421");
            var length = (n.GetBitLength() * 2 + 31) / 32;
            var random1 = new MersenneTwister(0).Create<BigInteger>();
            var random2 = new MersenneTwister(0).Create<BigInteger>();
            var timer1 = new Stopwatch();
            var timer2 = new Stopwatch();
            var iterations1 = 10000;
            var iterations2 = 10000;

            timer1.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var a = new MutableInteger(length);
                var b = new MutableInteger(length);
                var c = new MutableInteger(length);
                a.Set(random1.Next(n));
                b.Set(random1.Next(n));

                for (int j = 0; j < iterations2; j++)
                    operation1(c, a, b);
            }
            var elapsed1 = timer1.ElapsedMilliseconds;

#if false
            timer2.Start();
            for (int i = 0; i < iterations1; i++)
            {
                var a = random2.Next(n);
                var b = random2.Next(n);
                var c = BigInteger.Zero;

                for (int j = 0; j < iterations2; j++)
                    c = operation2(a, b);
            }
            var elapsed2 = timer1.ElapsedMilliseconds;

            output.WriteLine("{0}: elapsed1 = {1}, elapsed2 = {2}", label, elapsed1, elapsed2);
#else
            output.WriteLine("{0}: elapsed = {1}", label, elapsed1);
#endif
        }

        static void FactorTest1()
        {
            for (var k = 14; k <= 14; k++)
                FactorTest1Core(k);
        }

        static void FactorTest1Core(int k)
        {
            //var n = BigInteger.Parse("10023859281455311421");
            output.WriteLine("k = {0}", k);
            var n = samples[k].N;
            var threads = 1;
            var iterations = 1;
            bool debug = false;
            var config = new QuadraticSieve.Config
            {
                DiagnosticsOutput = output,
                IntervalSize = 32 * 1024,
                BlockSize = 32 * 1024,
                Multiplier = 1,
                Diagnostics = QuadraticSieve.Diag.None,
                //Diagnostics = QuadraticSieve.Diag.Verbose,
                ThresholdExponent = 1.05,
                ErrorLimit = 1,
                FactorBaseSize = 80,
                Threads = threads,
            };

            output.WriteLine("bits = {0}", n.GetBitLength());
            output.WriteLine();

            for (var i = 0; i < 1; i++)
            {
                if (n <= long.MaxValue)
                {
                    FactorTest(debug, iterations, (long)n, new ShanksSquareForms(), "ShanksSquareForms");
                }
                if (n <= ulong.MaxValue)
                {
                    FactorTest(debug, iterations, (ulong)n, PollardRhoReduction.Create(new UInt64Reduction()), "PollardRho/UInt64");
                    FactorTest(debug, iterations, (ulong)n, PollardRhoReduction.Create(new UInt64MontgomeryReduction()), "PollardRho/UInt64Montgomery");
                    FactorTest(debug, iterations, (ulong)n, new UInt64PollardRhoReduction(new UInt64Reduction()), "UInt64PollardRho/UInt64");
                    FactorTest(debug, iterations, (ulong)n, new UInt64PollardRhoReduction(new UInt64MontgomeryReduction()), "UInt64PollardRho/UInt64Montgomery");
                }
                if (n <= UInt128.MaxValue)
                {
                    FactorTest(debug, iterations, (UInt128)n, PollardRhoReduction.Create(new UInt128Reduction()), "PollardRho/UInt128");
                    FactorTest(debug, iterations, (UInt128)n, PollardRhoReduction.Create(new UInt128MontgomeryReduction()), "PollardRho/UInt128Montgomery");
                }
#if false
                if (false)
                {
                    FactorTest(debug, iterations, n, PollardRhoReduction.Create(new BigIntegerReduction()), "PollardRho/BigInteger");
                    FactorTest(debug, iterations, n, PollardRhoReduction.Create(new BigIntegerMontgomeryReduction()), "PollardRho/BigIntegerMontgomery");
                    FactorTest(debug, iterations, n, new QuadraticSieve(config), "QuadraticSieve");
                }
#endif
                output.WriteLine();
            }
        }

        static void FactorTest2()
        {
            var p = BigInteger.Parse("287288745765902964785862069919080712937");
            var q = BigInteger.Parse("7660450463");
            var n = p * q;
            int threads = 4;
            bool debug = false;
            FactorTest(debug, 25, n, new PollardRhoBrent(threads, 0));
            FactorTest(debug, 25, n, new BigIntegerPollardRhoReduction(threads, 0, new MutableIntegerReduction()));
            FactorTest(debug, 25, n, new BigIntegerPollardRhoReduction(threads, 0, new BarrettReduction()));
            FactorTest(debug, 25, n, new BigIntegerPollardRhoReduction(threads, 0, new BigIntegerMontgomeryReduction()));
        }

        static void FactorTest3()
        {
            for (int i = 214; i <= 214; i++)
            {
                var plus = true;
                var n = (BigInteger.One << i) + (plus ? 1 : -1);
                output.WriteLine("i = {0}, n = {1}", i, n);
                if (IntegerMath.IsPrime(n))
                    continue;
                int threads = 4;
                var factors = null as BigInteger[];
                //factors = FactorTest(true, 1, n, new PollardRho(threads, 0));
                factors = FactorTest(true, 5, n, new BigIntegerPollardRhoReduction(threads, 0, new MutableIntegerReduction()));
                //factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, 0, new BarrettReduction()));
                //factors = FactorTest(true, 5, n, new PollardRhoReduction(threads, 0, new MontgomeryReduction()));
                foreach (var factor in factors)
                    output.WriteLine("{0}", factor);
            }

        }

        static void FactorTest4()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
            for (int i = 5; i <= 16; i++)
            {
                var limit = BigInteger.Pow(new BigInteger(10), i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                int threads = 8;
                var factors = null as BigInteger[];
                //factors = FactorTest(true, 1, n, new PollardRho(threads, 0));
                //factors = FactorTest(true, 1, n, new PollardRhoReduction(threads, 0, new MutableIntegerReduction()));
                factors = FactorTest(true, 1, n, new BigIntegerPollardRhoReduction(threads, 0, new BigIntegerMontgomeryReduction()));
                //factors = FactorTest(true, 1, n, new QuadraticSieve(new QuadraticSieve.Config { Threads = threads }));
            }
        }

        static void MsieveTest()
        {
            //var n = BigInteger.Parse("87463");
            //var n = BigInteger.Parse("10023859281455311421");
            var n = BigInteger.Parse("5382000000735683358022919837657883000000078236999000000000000063"); // https://sites.google.com/site/shouthillgc/Home/gc1p8qn/factorizing-tool
            //var sample = samples[20]; var n = sample.P * sample.Q;

            output.WriteLine("n = {0}", n);
            //FactorTest(debug, 500, n, new PollardRhoReduction(pollardThreads, new MontgomeryReduction()));
            var config = new QuadraticSieve.Config
            {
                DiagnosticsOutput = output,
                Threads = 8,
                //FactorBaseSize = 5400,
                //LowerBoundPercent = 65,
                //IntervalSize = 12 * 32 * 1024,
                //Multiplier = 3,
                Diagnostics = QuadraticSieve.Diag.Verbose,
            };
            var factors = FactorTest(false, 1, n, new QuadraticSieve(config));
            foreach (var factor in factors)
                output.WriteLine("{0}", factor);
        }

        static void FactorTest6()
        {
            var n = BigInteger.Parse("18446744073709551617");
            //var n = BigInteger.Parse("12345678901");
            FactorTest(false, 100, n, new QuadraticSieve(new QuadraticSieve.Config { Threads = 8 }));
        }

        static void QuadraticSieveParametersTest()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
            int i = 50;
            var sample = samples[i];
            var p = sample.P;
            var q = sample.Q;
            var n = sample.N;
            output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
#if true
            for (int size = 100000; size <= 200000; size += 10000)
            {
                var config = new QuadraticSieve.Config
                {
                    DiagnosticsOutput = output,
                    Threads = 8,
                    FactorBaseSize = size,
                    //Diagnostics = QuadraticSieve.Diag.Verbose,
                    ReportingInterval = 60,
                    SieveTimeLimit = 60,
                    ThresholdExponent = 2.25,
                    //LargePrimeOptimization = true,
                    ProcessPartialPartialRelations = true,
                };
                output.WriteLine("size = {0}", size);
                RunParameterTest(config, n);
            }
#endif
#if false
            for (int percent = 80; percent <= 80; percent += 5)
            {
                var config = new QuadraticSieve.Config
                {
                    DiagnosticsOutput = output,
                    Threads = threads,
                    FactorBaseSize = size,
                    LowerBoundPercent = percent,
                    //Diagnostics = QuadraticSieve.Diag.Verbose,
                };
                output.WriteLine("percent = {0}", percent);
                RunParameterTest(config, n);
            }
#endif
#if false
            var blockSize = 32 * 1024;
            for (int size = 4 * blockSize; size <= 32 * blockSize; size += blockSize)
            {
                var config = new QuadraticSieve.Config
                {
                    DiagnosticsOutput = output,
                    Threads = 1,
                    IntervalSize = size,
                    ReportingInterval = 60,
                    SieveTimeLimit = 60,
                    //Diagnostics = QuadraticSieve.Diag.Verbose,
                };
                output.WriteLine("interval size = {0}", size);
                RunParameterTest(config, n);
            }
#endif
        }

        static void RunParameterTest(QuadraticSieve.Config config, BigInteger n)
        {
            if (config.SieveTimeLimit != 0)
            {
                GC.Collect();
                new QuadraticSieve(config).Factor(n).ToArray();
            }
            else
                FactorTest(false, 1, n, new QuadraticSieve(config));
        }

        static void CreateSamplesTest()
        {
            var random = new MersenneTwister(0).Create<BigInteger>();
            for (int i = 1; i <= 10; i++)
            {
                var limit = BigInteger.Pow(10, i);
                var p = NextPrime(random, limit);
                var q = NextPrime(random, limit);
                var n = p * q;
                output.WriteLine("new SampleComposite");
                output.WriteLine("{");
                output.WriteLine("    Digits = {0},", 2 * i);
                output.WriteLine("    P = BigInteger.Parse(\"{0}\"),", p);
                output.WriteLine("    Q = BigInteger.Parse(\"{0}\"),", q);
                output.WriteLine("},");
            }
        }

        static void QuadraticSieveStandardTest()
        {
            new QuadraticSieve(new QuadraticSieve.Config()).Factor(samples[10].N).ToArray();
            //new QuadraticSieve(new QuadraticSieve.Config()).Factor(35095264073).ToArray();
            var config = new QuadraticSieve.Config
            {
#if !DEBUG
                Threads = 8,
#endif
                DiagnosticsOutput = output,
                //ProcessPartialPartialRelations = true,
            };
            for (int i = 20; i <= 35; i++)
            {
                var sample = samples[i];
                var p = sample.P;
                var q = sample.Q;
                var n = sample.N;
                output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
                //output.WriteLine("n = {0}", n);
                var algorithm = new QuadraticSieve(config);
                FactorTest(false, 1, n, algorithm);
            }
        }

        static void QuadraticSieveDebugTest()
        {
            new QuadraticSieve(new QuadraticSieve.Config()).Factor(samples[10].N).ToArray();
            //new QuadraticSieve(new QuadraticSieve.Config()).Factor(35095264073).ToArray();
            var config = new QuadraticSieve.Config
            {
#if !DEBUG
                Threads = 8,
#else
                Threads = 0,
#endif
                Diagnostics = QuadraticSieve.Diag.Verbose,
                DiagnosticsOutput = output,
                ReportingInterval = 60,
                //MergeLimit = 10,
                //ProcessPartialPartialRelations = true,
                //ThresholdExponent = 2.6,

                //FactorBaseSize = 14000,
                //BlockSize = 64 * 1024,
                //IntervalSize = 64 * 1024,
                //CofactorCutoff = 4096 * 4,
                //ErrorLimit = 1,
                //NumberOfFactors = 12,
                //UseCountTable = true,
                //CofactorCutoff = 1024,
                //SieveTimeLimit = 120,
            };
            var i = 35;
            var sample = samples[i];
            var p = sample.P;
            var q = sample.Q;
            var n = sample.N;
            output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
            output.WriteLine("n = {0}", n);
            var algorithm = new QuadraticSieve(config);
            FactorTest(false, 1, n, algorithm);
        }

        static void QuadraticSieveFactorTest()
        {
            new QuadraticSieve(new QuadraticSieve.Config()).Factor(samples[10].N).ToArray();
            //new QuadraticSieve(new QuadraticSieve.Config()).Factor(35095264073).ToArray();
            var config = new QuadraticSieve.Config
            {
#if !DEBUG
                Threads = 8,
#endif
                Diagnostics = QuadraticSieve.Diag.Verbose,
                DiagnosticsOutput = output,
                ReportingInterval = 60,
                ThresholdExponent = 2.5,
                MergeLimit = 10,
            };
            var i = 50;
            var sample = samples[i];
            var p = sample.P;
            var q = sample.Q;
            var n = sample.N;
            output.WriteLine("i = {0}, p = {1}, q = {2}", i, p, q);
            output.WriteLine("n = {0}", n);
            var algorithm = new QuadraticSieve(config);
            FactorTest(false, 1, n, algorithm);
        }

        static void CunninghamTest()
        {
            var n = BigInteger.Pow(3, 225) - 1;
            output.WriteLine("n = {0}", n);
            var algorithm = new HybridPollardRhoQuadraticSieve(4, 1000000, new QuadraticSieve.Config());
            foreach (var factor in algorithm.Factor(n))
                output.WriteLine("{0}", factor);
        }

        static void GaussianEliminationTest1()
        {
            var threads = 8;
            var mergeLimit = 5;
            //var file = @"..\..\..\..\matrix-12001.txt.gz";
            var file = @"..\..\..\..\matrix-18401.txt.gz";
            //var file = @"..\..\..\..\matrix-150001.txt.gz";
            var lines = GetLinesGzip(file);
            var timer = new Stopwatch();

#if false
            var solver = new GaussianElimination<Word64BitArray>(threads);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<Word64BitMatrix>);
#endif
#if false
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<Word64BitMatrix>);
#endif
#if false
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads, true);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<ByRowSetBitMatrix>);
#endif
#if false
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads, true);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<ByColSetBitMatrix>);
#endif
#if true
            var solver = new StructuredGaussianElimination<Word64BitArray, Word64BitMatrix>(threads, mergeLimit, true);
            var getter = new Func<string[], IBitMatrix>(GetBitMatrix<SetBitMatrix>);
#endif

            timer.Restart();
            var matrix = getter(lines);
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            output.WriteLine("Rows = {0}, Cols = {1}", matrix.Rows, matrix.Cols);

            timer.Restart();
            var solutions = solver.Solve(matrix).ToArray();
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);

            output.WriteLine("solutions = {0}", solutions.Length);
        }

        private static void GraphTest()
        {
            //var file = @"..\..\..\..\pprs-23906.txt.gz";
            var file = @"..\..\..\..\pprs-42726.txt.gz";
            var lines = GetLinesGzip(file);
            var pprs = lines
                .Select(line => line.Split(' ').Select(field => long.Parse(field)).ToArray())
                .Select(pair => Tuple.Create(pair[0], pair[1]))
                .ToArray();

            for (int i = 0; i < 1; i++)
                GraphTest(pprs);
        }

        private static void GraphTest(Tuple<long, long>[] pprs)
        {
            //var algorithm = new QuadraticSieve(new QuadraticSieve.Config());
            //var algorithm = new PollardRhoReduction(1, int.MaxValue, new MutableIntegerReduction());
            //var algorithm = new PollardRhoReduction(1, int.MaxValue, new MontgomeryReduction());
            //var algorithm = new OldPollardRhoLong();
            //var algorithm = new ShanksSquareForms();
            var algorithm = new UInt64PollardRhoReduction(new UInt64MontgomeryReduction());

            var timer = new Stopwatch();
            timer.Restart();
            int processed = 0;
            int found = 0;
            int total = 0;
            int failed = 0;
#if false
            var referenceGraph = new Graph<long, Edge<long>>();
#endif
            //var graph = new Graph<long, Edge<long>>();
            var graph = new PartialRelationGraph<int>();
            foreach (var ppr in pprs)
            {
                var cofactor1 = ppr.Item1;
                var cofactor2 = ppr.Item2;
                var cofactor = cofactor1 * cofactor2;
                if (cofactor2 != 1)
                {
#if false
                for (int i = 0; i < 1000; i++)
                    IntegerMath.IsProbablePrime(cofactor1);
#endif
#if true
                    var factor = algorithm.GetDivisor(cofactor);
                    if (factor != cofactor1 && factor != cofactor2)
                        ++failed;
#endif
                }
                var cycle = graph.FindPath(cofactor1, cofactor2);
#if false
                var referenceCycle = referenceGraph.FindPath(cofactor1, cofactor2);
                if ((referenceCycle == null) != (cycle == null))
                {
                    Debugger.Break();
                    cycle = graph.FindPath(cofactor1, cofactor2);
                }
#endif
                if (cycle == null)
                {
#if false
                    referenceGraph.AddEdge(cofactor1, cofactor2);
#endif
                    graph.AddEdge(cofactor1, cofactor2, ref processed);
                }
                else
                {
#if false
                    if (referenceCycle.Count != cycle.Count)
                    {
                        Debugger.Break();
                        cycle = graph.FindPath(cofactor1, cofactor2);
                    }
                    var referenceCofactors = new[] { cofactor1, cofactor2 }
                        .Concat(referenceCycle.SelectMany(edge => new[] { edge.Vertex1, edge.Vertex2 }))
                        .Distinct()
                        .OrderBy(vertex => vertex)
                        .ToArray();
                    var cofactors = new[] { cofactor1, cofactor2 }
                        .Concat(cycle.SelectMany(edge => new[] { edge.Vertex1, edge.Vertex2 }))
                        .Distinct()
                        .OrderBy(vertex => vertex)
                        .ToArray();
                    if (cofactors.Length != cycle.Count + 1)
                    {
                        Debugger.Break();
                        cycle = graph.FindPath(cofactor1, cofactor2);
                    }
                    else if (!referenceCofactors.SequenceEqual(cofactors))
                    {
                        Debugger.Break();
                        cycle = graph.FindPath(cofactor1, cofactor2);
                    }
#endif
                    total += cycle.Count + 1;
                    //output.WriteLine("cycle = {0}", cycle.Count + 1);
#if false
                    foreach (var edge in referenceCycle)
                        referenceGraph.RemoveEdge(edge);
#endif
                    foreach (var edge in cycle)
                        graph.RemoveEdge(edge);
                    ++found;
                }
                ++processed;
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            output.WriteLine("processed = {0}; found = {1}; total = {2}; average = {3:F3}", processed, found, total, (double)total / found);
            output.WriteLine("failed = {0}", failed);
        }

        private static void UInt128Test()
        {
            var timer = new Stopwatch();
            var random = new MersenneTwister(0).Create<ulong>();
            var max = (ulong)1 << 60;
            timer.Start();
            for (int i = 0; i < 5000000; i++)
            {
                var value = random.Next(max);
                var exponent = random.Next(max);
                var modulus = random.Next(max);
                if ((modulus & 1) == 0)
                    ++modulus;
#if false
                var result = BigInteger.ModPow(value, exponent, modulus);
#else
                var result = IntegerMath.ModularPower(value, exponent, modulus);
#endif
                //var result = IntegerMath.IsPrime(value);
#if false
                if (result != BigInteger.ModPow(value, exponent, modulus))
                    throw new InvalidOperationException("miscalculation");
#endif
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        static void ModularInverseTest1()
        {
            var result0 = IntegerMath.ModularPowerOfTwo(2378270433224750976, 3880873274144447570);
            var result = IntegerMath.ModularPower(375, 249, 388);
            var d = (uint)273;
            var dInv1 = (uint)IntegerMath.ModularInverse(d, (long)1 << 32);
            var result1 = IntegerMath.ModularProduct(d, dInv1, (long)1 << 32);
            var dInv2 = IntegerMath.ModularInversePowerOfTwoModulus(d, 32);
            Console.WriteLine();
        }

        static void ModularInverseTest2()
        {
            //var inverse = new Func<long, long, long>(HybridRSSModularInverse);
            //var inverse = new Func<long, long, long>(RSSSimpleModularInverse);
            var inverse = new Func<long, long, long>(RSSModularInverse);
            //var a = (long)43760554345460349;
            //var m = (long)76055434546034911;
            //var aInv = IntegerMath.ModularInverse(a, m);
            //Console.WriteLine("a = {0}, n = {1}, aInv = {2}, a * aInv % n = {3}", a, m, aInv, IntegerMath.ModularProduct(a, aInv, m));
            //var aInv2 = inverse(a, m);
            //Console.WriteLine("aInv2 = {0}", aInv2);

            var max = (ulong)long.MaxValue;
            var random = new MersenneTwister(0).Create<ulong>();
            var pairs = random.Sequence(max)
                .Zip(random.Sequence(max), (a, b) => new { A = a, B = b })
                .Where(pair => pair.A != 1 && pair.B != 1 && IntegerMath.GreatestCommonDivisor(pair.A, pair.B) == 1)
                .Take(1000)
                .ToArray();
            int count = 10000;

            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < count; i++)
            {
                foreach (var pair in pairs)
                {
                    var aInv = IntegerMath.ModularInverse(pair.A, pair.B);
                    ulong c;
                    ulong d;
                    IntegerMath.ExtendedEuclideanAlgorithm(pair.A, pair.B, out c, out d);
                    if (IntegerMath.ModularProduct(pair.A, c, pair.B) != 1)
                        throw new InvalidOperationException("miscalculation");
                    if (c != aInv)
                        throw new InvalidOperationException("disagreement");
                }
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);

#if false
            timer.Restart();
            for (int i = 0; i < count; i++)
            {
                foreach (var pair in pairs)
                    inverse(pair.A, pair.B);
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
        }

        private static long RSSModularInverse(long a, long m)
        {
            long u = m;
            long v = a;
            long r = 0;
            long s = 1;
            while (v > 0)
            {
                while ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                while ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            if (u > 1)
                return 0;
            while (r < 0)
                r += m;
            while (r >= m)
                r -= m;
            return r;
        }

        private static long HybridRSSModularInverse(long a, long m)
        {
            long u = m;
            long v = a;
            long r = 0;
            long s = 1;
            while (v > int.MaxValue)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            while (v > 0 && u > int.MaxValue)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            return HybridRSSModularInverse((int)u, (int)v, r, s, m);
        }

        private static long HybridRSSModularInverse(int u, int v, long r, long s, long m)
        {
            while (v > 0)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            if (u > 1)
                return 0;
            while (r < 0)
                r += m;
            while (r >= m)
                r -= m;
            return r;
        }

        private static long RSSSimpleModularInverse(long a, long m)
        {
            long u = m;
            long v = a;
            long r = 0;
            long s = 1;
            while (v > 0)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            if (u > 1)
                return 0;
            while (r < 0)
                r += m;
            while (r >= m)
                r -= m;
            return r;
        }

        private static int RSSSimpleModularInverse(int a, int m)
        {
            int u = m;
            int v = a;
            int r = 0;
            int s = 1;
            while (v > 0)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    r -= s;
                }
                else
                {
                    v -= u;
                    s -= r;
                }
            }
            if (u > 1)
                return 0;
            while (r < 0)
                r += m;
            while (r >= m)
                r -= m;
            return r;
        }

        private static ulong RSUModularInverse(ulong a, ulong m)
        {
            ulong u = m;
            ulong v = a;
            ulong r = 0;
            ulong s = 1;
            while (v > 0)
            {
                if ((u & 1) == 0)
                {
                    u >>= 1;
                    if ((r & 1) == 0)
                        r >>= 1;
                    else
                        r = (r + m) >> 1;
                }
                else if ((v & 1) == 0)
                {
                    v >>= 1;
                    if ((s & 1) == 0)
                        s >>= 1;
                    else
                        s = (s + m) >> 1;
                }
                else if (u > v)
                {
                    u -= v;
                    if (r < s)
                        r += m - s;
                    else
                        r -= s;
                }
                else
                {
                    v -= u;
                    if (s < r)
                        s += m - r;
                    else
                        s -= r;
                }
            }
            if (u > 1)
                return 0;
            return r;
        }

        private static ulong SEUModularInverse(ulong a, ulong m)
        {
            ulong u;
            ulong v;
            ulong s;
            ulong r;
            if (a < m)
            {
                u = m;
                v = a;
                r = 0;
                s = 1;
            }
            else
            {
                v = m;
                u = a;
                s = 0;
                r = 1;
            }
            while (v > 1)
            {
                var f = u.GetBitLength() - v.GetBitLength();
                if (u < v << f)
                    --f;
                u -= v << f;
                var t = s;
                for (int i = 0; i < f; i++)
                {
                    t <<= 1;
                    if (t > m)
                        t -= m;
                }
                if (r < t)
                    r += m;
                r -= t;
                if (u < v)
                {
                    t = u; u = v; v = t;
                    t = r; r = s; s = t;
                }
            }
            if (v == 0)
                s = 0;
            return s;
        }

        static void PrimalityTest()
        {
#if false
            {
                int count = 2000000;
                var random = new MersenneTwister(0).Create<ulong>();
                //var algorithm = new OldMillerRabin(16);
                //var algorithm = MillerRabin.Create(16, new UInt64Reduction());
                var algorithm = MillerRabin.Create(16, new UInt64MontgomeryReduction());
                var max = (ulong)1 << 60;
                var timer = new Stopwatch();
                timer.Start();
                for (int i = 0; i < count; i++)
                {
                    var result = algorithm.IsPrime(random.Next(max));
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if false
            {
                int count = 80000;
                var random = new MersenneTwister(0).Create<BigInteger>();
                //var algorithm = new OldMillerRabin(16);
                //var algorithm = MillerRabin.Create(16, new BigIntegerReduction());
                //var algorithm = MillerRabin.Create(16, new MutableIntegerReduction());
                var algorithm = MillerRabin.Create(16, new BigIntegerMontgomeryReduction());
                var max = BigInteger.One << 256;
                var timer = new Stopwatch();
                timer.Start();
                for (int i = 0; i < count; i++)
                {
                    var result = algorithm.IsPrime(random.Next(max) | 1);
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if true
            {
                int count = 1000000;
                var random = new MersenneTwister(0).Create<ulong>();
                var max = ulong.MaxValue;
                var timer = new Stopwatch();
                timer.Start();
                for (int i = 0; i < count; i++)
                {
                    var x = random.Next(max) | 1;
#if false
                    var result = BigInteger.ModPow(2, x - 1, x);
#endif
#if false
                    var result = UInt128.ModularPower(2, x - 1, x);
#endif
#if true
                    var result = IntegerMath.IsProbablePrime(x);
#endif
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
        }

        static void OperationsTest()
        {
            int count = 5000000;
#if false
            var max = (BigInteger)1 << 128;
            var source = new MersenneTwister(0).Create<BigInteger>();
#endif
#if true
            var max = (int)1 << 31;
            var source = new MersenneTwister(0).Create<int>();
#endif
            var pairs = source.Sequence(max)
                .Zip(source.Sequence(max), (a, b) => Tuple.Create(a, b))
                .Where(pair => IntegerMath.GreatestCommonDivisor(pair.Item1, pair.Item2) == 1)
                .Take(count)
                .ToArray();
#if true
            {
                Console.WriteLine("hand coded 32");
                int c;
                int d;
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<int>();
                timer.Restart();
                for (int i = 0; i < pairs.Length; i++)
                {
                    var a = pairs[i].Item1;
                    var b = pairs[i].Item2;
                    ExtendedEuclideanAlgorithm(a, b, out c, out d);
                    if (c < 0)
                        c += b;
                    if (IntegerMath.ModularProduct(a, c, b) != 1)
                        throw new InvalidOperationException("miscalculation");
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if false
            {
                Console.WriteLine("hand coded 64");
                var max = (long)1 << 62;
                long c;
                long d;
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<long>();
                timer.Restart();
                for (int i = 0; i < count; i++)
                    ExtendedEuclideanAlgorithm(random.Next(max), random.Next(max), out c, out d);
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if false
            {
                Console.WriteLine("hand coded BigInteger");
                BigInteger c;
                BigInteger d;
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<BigInteger>();
                timer.Restart();
                for (int i = 0; i < pairs.Length; i++)
                {
                    var a = pairs[i].Item1;
                    var b = pairs[i].Item2;
                    ExtendedEuclideanAlgorithm(a, b, out c, out d);
                    if (c < 0)
                        c += b;
                    if (a * c % b != 1)
                        throw new InvalidOperationException("miscalculation");
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if false
            {
                Console.WriteLine("new BigInteger");
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<BigInteger>();
                timer.Restart();
                for (int i = 0; i < count; i++)
                {
                    var a = pairs[i].Item1;
                    var b = pairs[i].Item2;
                    var c = IntegerMath.ModularInverse(a, b);
                    if (a * c % b != 1)
                        throw new InvalidOperationException("miscalculation");
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if true
            //OperationsTest((int)1 << 30, count);
            //OperationsTest((ulong)1 << 62, count);
            OperationsTest(pairs);
#endif
        }

        static void OperationsTest<T>(Tuple<T, T>[] pairs)
        {
            Console.WriteLine("type = {0}", typeof(T));
#if true
            {
                Console.WriteLine("using Operation<T>");
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<T>();
                T c;
                T d;
                timer.Restart();
                for (int i = 0; i < pairs.Length; i++)
                {
                    var a = pairs[i].Item1;
                    var b = pairs[i].Item2;
                    ExtendedEuclideanAlgorithm(a, b, out c, out d);
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
#if true
            {
                Console.WriteLine("using Number<T>");
                GC.Collect();
                var timer = new Stopwatch();
                var random = new MersenneTwister(0).Create<T>();
                Number<T> c;
                Number<T> d;
                timer.Restart();
                for (int i = 0; i < pairs.Length; i++)
                {
                    var a = pairs[i].Item1;
                    var b = pairs[i].Item2;
                    ExtendedEuclideanAlgorithm(a, b, out c, out d);
                }
                output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
            }
#endif
        }

        static void DivisionTest1()
        {
            var intervalSize = 256 * 1024;
            var count = 5000;
            var samples = new MersenneTwister(0).Create<int>().Sequence(intervalSize).Select(a => a | 1).Take(count).ToArray();
            DivisionTest1(intervalSize, samples);
            DivisionTest2(intervalSize, samples);
            DivisionTest3(intervalSize, samples);
            DivisionTest4(intervalSize, samples);
        }

        private static void DivisionTest1(int intervalSize, int[] samples)
        {
            var divisions = samples.Select(d => new UInt32Division1((uint)d)).ToArray();
            //var divisions = samples.Select(d => new UInt32Division2((uint)d)).ToArray();
            //var divisions = samples.Select(d => new UInt32Division3((uint)d)).ToArray();
            //var divisions = samples.Select(d => new UInt32Division4((uint)d)).ToArray();
            var timer = new Stopwatch();
            var hash = 0;
            timer.Restart();
            for (var k = 0; k < intervalSize; k++)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    var r = divisions[i].Modulus((uint)k);
#if false
                    if (r != k % d)
                        Debugger.Break();
#endif
                    hash = (hash << 2 | hash >> 30) ^ r.GetHashCode();
                }
            }
            output.WriteLine("elapsed = {0:F3} msec, hash = {1}", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000, hash);
        }

        private static void DivisionTest2(int intervalSize, int[] samples)
        {
            var timer = new Stopwatch();
            var hash = 0;
            timer.Restart();
            for (var k = 0; k < intervalSize; k++)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    var d = samples[i];
                    var r = k % d;
                    hash = (hash << 2 | hash >> 30) ^ r.GetHashCode();
                }
            }
            output.WriteLine("elapsed = {0:F3} msec, hash = {1}", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000, hash);
        }

        private static void DivisionTest3(int intervalSize, int[] samples)
        {
            //var divisions = samples.Select(d => new UInt32Division1((uint)d)).ToArray();
            //var divisions = samples.Select(d => new UInt32Division2((uint)d)).ToArray();
            //var divisions = samples.Select(d => new UInt32Division3((uint)d)).ToArray();
            //var divisions = samples.Select(d => new UInt32Division4((uint)d)).ToArray();
            var divisions = samples.Select(d => new UInt32Division5((uint)d)).ToArray();
            var timer = new Stopwatch();
            var hash = 0;
            timer.Restart();
            for (var k = 0; k < intervalSize; k++)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    var p = divisions[i].IsDivisible((uint)k);
#if false
                    if (p != (k % d == 0))
                        Debugger.Break();
#endif
                    hash = (hash << 2 | hash >> 30) ^ p.GetHashCode();
                }
            }
            output.WriteLine("elapsed = {0:F3} msec, hash = {1}", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000, hash);
        }

        private static void DivisionTest4(int intervalSize, int[] samples)
        {
            var timer = new Stopwatch();
            var hash = 0;
            timer.Restart();
            for (var k = 0; k < intervalSize; k++)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    var d = samples[i];
                    var p = k % d == 0;
                    hash = (hash << 2 | hash >> 30) ^ p.GetHashCode();
                }
            }
            output.WriteLine("elapsed = {0:F3} msec, hash = {1}", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000, hash);
        }

        static void DivisionTest2()
        {
            var count = 10000;
            var divisorSize = (uint)1 << 20;
            var dividendSize = (ulong)divisorSize << 32;
            var divisors = new MersenneTwister(0).Create<uint>().Sequence(divisorSize)
                .Select(a => a | 1).Take(count).ToArray();
            var dividends = new MersenneTwister(0).Create<ulong>().Sequence(dividendSize)
                .Zip(divisors, (dividend, divisor) => dividend % (divisor << 32))
                .ToArray();

            //var divisions1 = divisors.Select(d => new UInt64Division0(d)).Cast<IDivisionAlgorithm<ulong, uint>>().ToArray();
            //var divisions1 = divisors.Select(d => new UInt64Division1(d)).Cast<IDivisionAlgorithm<ulong, uint>>().ToArray();
            //var divisions1 = divisors.Select(d => new UInt64Division5(d)).Cast<IDivisionAlgorithm<ulong, uint>>().ToArray();

            var divisions1 = divisors.Select(d => new UInt64Division1(d)).Cast<IDivisionAlgorithm<ulong, uint>>().ToArray();
            var divisions2 = divisors.Select(d => new UInt64Division2(d)).Cast<IDivisionAlgorithm<ulong, uint>>().ToArray();
            var divisions3 = divisors.Select(d => new UInt64Division3(d)).Cast<IDivisionAlgorithm<ulong, uint>>().ToArray();
            var divisions4 = divisors.Select(d => new UInt64Division4(d)).Cast<IDivisionAlgorithm<ulong, uint>>().ToArray();

            var divisions0 = divisors.Select(d => new UInt64Division0(d)).Cast<IDivisionAlgorithm<ulong, uint>>().ToArray();

            DivisionTest2(dividends, divisions0, true);
            DivisionTest2(dividends, divisions1, true);
            DivisionTest2(dividends, divisions2, true);
            DivisionTest2(dividends, divisions3, true);
            DivisionTest2(dividends, divisions4, true);
        }

        private static void DivisionTest2(ulong[] dividends, IDivisionAlgorithm<ulong, uint>[] divisions, bool check)
        {
            var timer = new Stopwatch();
            timer.Restart();
            for (int j = 0; j < dividends.Length; j++)
            {
                var k = dividends[j];
                for (int i = 0; i < divisions.Length; i++)
                {
                    var r = divisions[i].Modulus(k);
                    if (check && r != k % divisions[i].Divisor)
                    {
                        if (Debugger.IsAttached)
                            Debugger.Break();
                        r = divisions[i].Modulus(k);
                        throw new InvalidOperationException("miscalculation");
                    }
                }
            }
            output.WriteLine("elapsed = {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        public static void ExtendedEuclideanAlgorithm(int a, int b, out int c, out int d)
        {
            var x = (int)0;
            var lastx = (int)1;
            var y = (int)1;
            var lasty = (int)0;

            while (b != 0)
            {
                var quotient = a / b;
                var tmpa = a;
                a = b;
                b = tmpa - quotient * b;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        public static void ExtendedEuclideanAlgorithm(long a, long b, out long c, out long d)
        {
            var x = (long)0;
            var lastx = (long)1;
            var y = (long)1;
            var lasty = (long)0;

            while (b != 0)
            {
                var quotient = a / b;
                var tmpa = a;
                a = b;
                b = tmpa - quotient * b;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        public static void ExtendedEuclideanAlgorithm(BigInteger a, BigInteger b, out BigInteger c, out BigInteger d)
        {
            var x = (BigInteger)0;
            var lastx = (BigInteger)1;
            var y = (BigInteger)1;
            var lasty = (BigInteger)0;

            while (b != 0)
            {
                var quotient = a / b;
                var tmpa = a;
                a = b;
                b = tmpa - quotient * b;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        public static void ExtendedEuclideanAlgorithm<T>(T a, T b, out T c, out T d)
        {
            var ops = Operations.Get<T>();
            var x = ops.Zero;
            var lastx = ops.One;
            var y = ops.One;
            var lasty = ops.Zero;

            while (!ops.IsZero(b))
            {
                var quotient = ops.Divide(a, b);
                var tmpa = a;
                a = b;
                b = ops.Subtract(tmpa, ops.Multiply(quotient, b));
                var tmpx = x;
                x = ops.Subtract(lastx, ops.Multiply(quotient, x));
                lastx = tmpx;
                var tmpy = y;
                y = ops.Subtract(lasty, ops.Multiply(quotient, y));
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        public static void ExtendedEuclideanAlgorithm<T>(Number<T> a, Number<T> b, out Number<T> c, out Number<T> d)
        {
            var x = Number<T>.Zero;
            var lastx = Number<T>.One;
            var y = Number<T>.One;
            var lasty = Number<T>.Zero;

            var p = a;
            var q = b;
            while (!q.IsZero)
            {
                var quotient = p / q;
                var tmpa = p;
                p = q;
                q = tmpa - quotient * q;
                var tmpx = x;
                x = lastx - quotient * x;
                lastx = tmpx;
                var tmpy = y;
                y = lasty - quotient * y;
                lasty = tmpy;
            }
            c = lastx;
            d = lasty;
        }

        private static string[] GetLinesGzip(string file)
        {
            using (var stream = new StreamReader(new GZipStream(File.OpenRead(file), CompressionMode.Decompress)))
            {
                var lines = new List<string>();
                while (true)
                {
                    var line = stream.ReadLine();
                    if (line == null)
                        break;
                    lines.Add(line);
                }
                return lines.ToArray();
            }
        }

        private static IBitMatrix GetBitMatrix<TMatrix>(string[] lines) where TMatrix : IBitMatrix
        {
            if (!lines[0].Contains(' '))
            {
                int rows = lines.Length;
                int cols = lines[0].Length;
                var matrix = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), rows, cols);
                for (int i = 0; i < rows; i++)
                {
                    var line = lines[i];
                    for (int j = 0; j < cols; j++)
                    {
                        if (line[j] == '1')
                            matrix[i, j] = true;
                    }
                }
                return matrix;
            }
            else
            {
                var fields = lines[0].Split(' ').Select(field => int.Parse(field)).ToArray();
                int rows = fields[0];
                int cols = fields[1];
                var matrix = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), rows, cols);
                for (int i = 0; i < rows; i++)
                {
                    var indices = lines[i + 1].Split(' ')
                        .Where(field => field != "")
                        .Select(field => int.Parse(field));
                    foreach (var j in indices)
                        matrix[i, j] = true;
                }
                return matrix;
            }
        }

        private static BigInteger NextPrime(IRandomNumberAlgorithm<BigInteger> random, BigInteger limit)
        {
            var digits = IntegerMath.GetDigitLength(limit - 1, 10);
            var n = random.Next(limit);
            while (IntegerMath.GetDigitLength(n, 10) < digits)
                n = random.Next(limit);
            return IntegerMath.NextPrime(n);
        }

        private static T[] FactorTest<T>(bool debug, int iterations, T n, IFactorizationAlgorithm<T> algorithm, string label = "unknown")
        {
            var results = new List<T[]>();
            if (iterations > 0)
                algorithm.Factor(n);
            GC.Collect();
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < iterations; i++)
                results.Add(algorithm.Factor(n).OrderBy(factor => factor).ToArray());
            var elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000;
            if (true)
            {
                foreach (var factors in results)
                {
                    if (factors.Length < 2)
                        throw new InvalidOperationException("too few factors");
                    var product = (Number<T>)factors.Aggregate((sofar, current) => (Number<T>)sofar * current);
                    if (factors.Select(factor => (Number<T>)factor).Any(factor => factor.IsOne || factor == n || !IntegerMath.IsPrime(factor)))
                        throw new InvalidOperationException("invalid factor");
                    if (n != product)
                        throw new InvalidOperationException("validation failure");
                }
            }
            output.WriteLine("{0} iterations in {1:F0} msec, {2:F3} msec/iteration ({3})", iterations, elapsed, elapsed / iterations, label);
            return results[0];
        }

        private class SampleComposite
        {
            public int Digits { get; set; }
            public BigInteger P { get; set; }
            public BigInteger Q { get; set; }
            public BigInteger N { get { return P * Q; } }
        }

        private static SampleComposite[] samples =
        {
            null,
            new SampleComposite
            {
                Digits = 2,
                P = BigInteger.Parse("5"),
                Q = BigInteger.Parse("11"),
            },
            new SampleComposite
            {
                Digits = 4,
                P = BigInteger.Parse("37"),
                Q = BigInteger.Parse("61"),
            },
            new SampleComposite
            {
                Digits = 6,
                P = BigInteger.Parse("967"),
                Q = BigInteger.Parse("379"),
            },
            new SampleComposite
            {
                Digits = 8,
                P = BigInteger.Parse("5431"),
                Q = BigInteger.Parse("8513"),
            },
            new SampleComposite
            {
                Digits = 10,
                P = BigInteger.Parse("83497"),
                Q = BigInteger.Parse("85691"),
            },
            new SampleComposite
            {
                Digits = 12,
                P = BigInteger.Parse("906869"),
                Q = BigInteger.Parse("422759"),
            },
            new SampleComposite
            {
                Digits = 14,
                P = BigInteger.Parse("7901401"),
                Q = BigInteger.Parse("3580393"),
            },
            new SampleComposite
            {
                Digits = 16,
                P = BigInteger.Parse("38900077"),
                Q = BigInteger.Parse("71049871"),
            },
            new SampleComposite
            {
                Digits = 18,
                P = BigInteger.Parse("646868797"),
                Q = BigInteger.Parse("400433141"),
            },
            new SampleComposite
            {
                Digits = 20,
                P = BigInteger.Parse("6359727791"),
                Q = BigInteger.Parse("4501387931"),
            },
            new SampleComposite
            {
                Digits = 22,
                P = BigInteger.Parse("81112462157"),
                Q = BigInteger.Parse("65534533327"),
            },
            new SampleComposite
            {
                Digits = 24,
                P = BigInteger.Parse("922920006683"),
                Q = BigInteger.Parse("718097069881"),
            },
            new SampleComposite
            {
                Digits = 26,
                P = BigInteger.Parse("9752697519233"),
                Q = BigInteger.Parse("6069293365573"),
            },
            new SampleComposite
            {
                Digits = 28,
                P = BigInteger.Parse("68645165989399"),
                Q = BigInteger.Parse("16044355135579"),
            },
            new SampleComposite
            {
                Digits = 30,
                P = BigInteger.Parse("600557957340779"),
                Q = BigInteger.Parse("931078023997103"),
            },
            new SampleComposite
            {
                Digits = 32,
                P = BigInteger.Parse("3860645359479661"),
                Q = BigInteger.Parse("6660723000417049"),
            },
            new SampleComposite
            {
                Digits = 34,
                P = BigInteger.Parse("92857397119223941"),
                Q = BigInteger.Parse("56396174139495167"),
            },
            new SampleComposite
            {
                Digits = 36,
                P = BigInteger.Parse("589014875010582533"),
                Q = BigInteger.Parse("736526309666503507"),
            },
            new SampleComposite
            {
                Digits = 38,
                P = BigInteger.Parse("7581008776610031599"),
                Q = BigInteger.Parse("4941861830454018857"),
            },
            new SampleComposite
            {
                Digits = 40,
                P = BigInteger.Parse("43760554345460349857"),
                Q = BigInteger.Parse("59780183007898320881"),
            },
            new SampleComposite
            {
                Digits = 42,
                P = BigInteger.Parse("166737023217956147843"),
                Q = BigInteger.Parse("266578926331401913067"),
            },
            new SampleComposite
            {
                Digits = 44,
                P = BigInteger.Parse("2341864285495243696819"),
                Q = BigInteger.Parse("6862234825035212041109"),
            },
            new SampleComposite
            {
                Digits = 46,
                P = BigInteger.Parse("81173737403430771942307"),
                Q = BigInteger.Parse("64443839329396315580339"),
            },
            new SampleComposite
            {
                Digits = 48,
                P = BigInteger.Parse("547109861053076293701181"),
                Q = BigInteger.Parse("218393091309815530058033"),
            },
            new SampleComposite
            {
                Digits = 50,
                P = BigInteger.Parse("3150497848572778166077033"),
                Q = BigInteger.Parse("1419806924383975860451933"),
            },
            new SampleComposite
            {
                Digits = 52,
                P = BigInteger.Parse("71553049119880264914826637"),
                Q = BigInteger.Parse("47084627876852469799938151"),
            },
            new SampleComposite
            {
                Digits = 54,
                P = BigInteger.Parse("668223788916828667974184073"),
                Q = BigInteger.Parse("475483409844077547574254721"),
            },
            new SampleComposite
            {
                Digits = 56,
                P = BigInteger.Parse("8816136275150582711618365859"),
                Q = BigInteger.Parse("3044375869342823579225836457"),
            },
            new SampleComposite
            {
                Digits = 58,
                P = BigInteger.Parse("79616313635618041020736352419"),
                Q = BigInteger.Parse("36109767180534668555739700277"),
            },
            new SampleComposite
            {
                Digits = 60,
                P = BigInteger.Parse("289209603456915754116025390283"),
                Q = BigInteger.Parse("651974473535965720429005879311"),
            },
            new SampleComposite
            {
                Digits = 62,
                P = BigInteger.Parse("2135862096280413518185169595631"),
                Q = BigInteger.Parse("4946257487231393668368584144179"),
            },
            new SampleComposite
            {
                Digits = 64,
                P = BigInteger.Parse("90484786924920304861107250841203"),
                Q = BigInteger.Parse("93819551150572322228605590151427"),
            },
            new SampleComposite
            {
                Digits = 66,
                P = BigInteger.Parse("752171408379662548633208575616947"),
                Q = BigInteger.Parse("727656629739503749143743912576693"),
            },
            new SampleComposite
            {
                Digits = 68,
                P = BigInteger.Parse("1452509806874688515960900257523821"),
                Q = BigInteger.Parse("6091273916310546401352905015178757"),
            },
            new SampleComposite
            {
                Digits = 70,
                P = BigInteger.Parse("39590771509308181367548580586664897"),
                Q = BigInteger.Parse("10420963447932920373972059082256531"),
            },
            new SampleComposite
            {
                Digits = 72,
                P = BigInteger.Parse("780748679899154536912466202699047849"),
                Q = BigInteger.Parse("379148146974432114134205653148097117"),
            },
            new SampleComposite
            {
                Digits = 74,
                P = BigInteger.Parse("1944657516979563550209812173755027707"),
                Q = BigInteger.Parse("8281713719538507681810654356958903461"),
            },
            new SampleComposite
            {
                Digits = 76,
                P = BigInteger.Parse("20221947695032137682483252436640950611"),
                Q = BigInteger.Parse("90083006931593945195319860191440222337"),
            },
            new SampleComposite
            {
                Digits = 78,
                P = BigInteger.Parse("234578372300126118738414197724923928917"),
                Q = BigInteger.Parse("789827369472404641442870779900199537177"),
            },
            new SampleComposite
            {
                Digits = 80,
                P = BigInteger.Parse("4866868528098421482780624850039639204477"),
                Q = BigInteger.Parse("7613720546717210496574998828026372396927"),
            },
            new SampleComposite
            {
                Digits = 82,
                P = BigInteger.Parse("53017745789889873705716127297403311033037"),
                Q = BigInteger.Parse("63409172442847344082610334892821781304101"),
            },
            new SampleComposite
            {
                Digits = 84,
                P = BigInteger.Parse("702295892578093110207938507434822530060641"),
                Q = BigInteger.Parse("342910649327212340825095863090818452864541"),
            },
            new SampleComposite
            {
                Digits = 86,
                P = BigInteger.Parse("8060168753150190457135241985925594282360297"),
                Q = BigInteger.Parse("5042676674972408672634448697950601300773747"),
            },
            new SampleComposite
            {
                Digits = 88,
                P = BigInteger.Parse("75263710689283058958166085987687804905069441"),
                Q = BigInteger.Parse("33942351450819488061478933627681638281912579"),
            },
            new SampleComposite
            {
                Digits = 90,
                P = BigInteger.Parse("452844597483732905842925404011206264228330341"),
                Q = BigInteger.Parse("205800250143096562811007221948045665761100741"),
            },
            new SampleComposite
            {
                Digits = 92,
                P = BigInteger.Parse("2167793402583216605038837020418528979230958321"),
                Q = BigInteger.Parse("6664463122751216293054375161162506956965538477"),
            },
            new SampleComposite
            {
                Digits = 94,
                P = BigInteger.Parse("73732608257939745378758147488940890654818425039"),
                Q = BigInteger.Parse("57602646692423215509569301754817211741313757471"),
            },
            new SampleComposite
            {
                Digits = 96,
                P = BigInteger.Parse("961585810072942641419044210351839016563811857989"),
                Q = BigInteger.Parse("669160867166029748266517367050911502647054495753"),
            },
            new SampleComposite
            {
                Digits = 98,
                P = BigInteger.Parse("3250569350138484738612442549714146714656127558157"),
                Q = BigInteger.Parse("9560816127493079446607942921966503110052699050311"),
            },
            new SampleComposite
            {
                Digits = 100,
                P = BigInteger.Parse("83268231017568575873466457691034899781471011837411"),
                Q = BigInteger.Parse("74270905395447293272574406310979848354158902086579"),
            },
            new SampleComposite
            {
                Digits = 102,
                P = BigInteger.Parse("940105563157766963437551355357600515466121772468943"),
                Q = BigInteger.Parse("300886532442207423100152787577295958421952407123783"),
            },
            new SampleComposite
            {
                Digits = 104,
                P = BigInteger.Parse("5536329130409356621737988070075431271124056972802493"),
                Q = BigInteger.Parse("5851320369599896666775748578020494083502437377067011"),
            },
            new SampleComposite
            {
                Digits = 106,
                P = BigInteger.Parse("95900006031561380146239548171173219385987444898113507"),
                Q = BigInteger.Parse("46423691457059816496580873421795656785311585406499793"),
            },
            new SampleComposite
            {
                Digits = 108,
                P = BigInteger.Parse("838036008579477237125953812571666198225228325843227391"),
                Q = BigInteger.Parse("308620559987457306004333345866922348787381950691797739"),
            },
            new SampleComposite
            {
                Digits = 110,
                P = BigInteger.Parse("9524348126127653470279298471608115636122260642046731149"),
                Q = BigInteger.Parse("5702432440887332287364432342498409227833457737037139601"),
            },
            new SampleComposite
            {
                Digits = 112,
                P = BigInteger.Parse("64133826838501466019612032067826809933583367965508355767"),
                Q = BigInteger.Parse("99826430536817182079559885207489818716914667714908780143"),
            },
            new SampleComposite
            {
                Digits = 114,
                P = BigInteger.Parse("712133265209559049499052940959542547383824060164846054611"),
                Q = BigInteger.Parse("241200306130797999905681076607776919657048436860510005749"),
            },
            new SampleComposite
            {
                Digits = 116,
                P = BigInteger.Parse("5449429390031135421251122886094693214317098752213974003953"),
                Q = BigInteger.Parse("4928204216229834813833069935962566956055542612200013379597"),
            },
            new SampleComposite
            {
                Digits = 118,
                P = BigInteger.Parse("67357275717568959943813687439058017341150058378917075494077"),
                Q = BigInteger.Parse("97759569419716544178270844827738415358315121039984564684707"),
            },
            new SampleComposite
            {
                Digits = 120,
                P = BigInteger.Parse("886161863194892046675575153172350650178464242408235539428789"),
                Q = BigInteger.Parse("700580068918316266402809321693665323840644845195667826345171"),
            },
        };
    }
}
