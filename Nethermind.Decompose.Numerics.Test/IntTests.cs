using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.Decompose.Numerics.Test
{
    [TestClass]
    public class IntTests
    {
        [TestMethod]
        [DataRow((ulong)1 << 20, (ulong)1 << 20)]
        [DataRow((ulong)1 << 40, (ulong)1 << 20)]
        [DataRow((ulong)1 << 60, (ulong)1 << 20)]
        [DataRow((ulong)1 << 20, (ulong)1 << 40)]
        [DataRow((ulong)1 << 40, (ulong)1 << 40)]
        [DataRow((ulong)1 << 60, (ulong)1 << 40)]
        [DataRow((ulong)1 << 20, (ulong)1 << 60)]
        [DataRow((ulong)1 << 40, (ulong)1 << 60)]
        [DataRow((ulong)1 << 60, (ulong)1 << 60)]
//        [DataRow(uint.MaxValue, uint.MaxValue)]
//        [DataRow(ulong.MaxValue, ulong.MaxValue)]
        public void UInt128Test(ulong factorMax, ulong modulusMax)
        {
            var random = new MersenneTwister(0).Create<ulong>();
            for (int i = 0; i < 10000; i++)
            {
                var n = random.Next(modulusMax - 1) + 1;
                var a = random.Next(factorMax) % n;
                var b = random.Next(factorMax) % n;
                var c = random.Next(factorMax) % n;
                var d = random.Next(factorMax) % n;
                var s = (int)(b % 32);
                var value = (UInt128)0;
                Assert.AreEqual((BigInteger)a << s, (UInt128)a << s);
                Assert.AreEqual((BigInteger)a >> s, (UInt128)a >> s);
                Assert.AreEqual((BigInteger)a & b, (UInt128)a & b);
                Assert.AreEqual((BigInteger)a & b, a & (UInt128)b);
                Assert.AreEqual((BigInteger)a & b, (UInt128)a & (UInt128)b);
                Assert.AreEqual((BigInteger)a | b, (UInt128)a | b);
                Assert.AreEqual((BigInteger)a | b, a | (UInt128)b);
                Assert.AreEqual((BigInteger)a | b, (UInt128)a | (UInt128)b);
                Assert.AreEqual((BigInteger)a ^ b, (UInt128)a ^ b);
                Assert.AreEqual((BigInteger)a ^ b, a ^ (UInt128)b);
                Assert.AreEqual((BigInteger)a ^ b, (UInt128)a ^ (UInt128)b);
                if (a <= long.MaxValue)
                    Assert.AreEqual(~(BigInteger)a, (long)~(UInt128)a);
                Assert.AreEqual((BigInteger)a + b, (UInt128)a + b);
                Assert.AreEqual((BigInteger)a + b, a + (UInt128)b);
                Assert.AreEqual((BigInteger)a + b, (UInt128)a + (UInt128)b);
                Assert.AreEqual(((BigInteger)a * n + (BigInteger)b * n) % ((BigInteger)1 << 128), (UInt128)a * n + (UInt128)b * n);
                if (a >= b)
                {
                    Assert.AreEqual((BigInteger)a - b, (UInt128)a - b);
                    Assert.AreEqual((BigInteger)a - b, a - (UInt128)b);
                    Assert.AreEqual((BigInteger)a - b, (UInt128)a - (UInt128)b);
                    Assert.AreEqual((BigInteger)a * n - (BigInteger)b * n, (UInt128)a * n - (UInt128)b * n);
                }
                Assert.AreEqual(+(BigInteger)a, +(Int128)a);
                value = a; Assert.AreEqual((BigInteger)a + 1, ++value);
                value = a; Assert.AreEqual((BigInteger)a, value++);
                value = (UInt128)a * b; Assert.AreEqual((BigInteger)a * b + 1, ++value);
                value = (UInt128)a * b; Assert.AreEqual((BigInteger)a * b, value++);
                if (a > 0)
                {
                    value = a; Assert.AreEqual((BigInteger)a - 1, --value);
                    value = a; Assert.AreEqual((BigInteger)a, value--);
                }
                if (a > 0 && b > 0)
                {
                    value = (UInt128)a * b; Assert.AreEqual((BigInteger)a * b - 1, --value);
                    value = (UInt128)a * b; Assert.AreEqual((BigInteger)a * b, value--);
                }
                if (n <= uint.MaxValue)
                {
                    Assert.AreEqual((BigInteger)a * n, (UInt128)a * (uint)n);
                    Assert.AreEqual((BigInteger)b * n, (UInt128)b * (uint)n);
                    Assert.AreEqual((BigInteger)a * b * n % ((BigInteger)1 << 128), (UInt128)a * b * (uint)n);
                    Assert.AreEqual((BigInteger)n * a, (uint)n * (UInt128)a);
                    Assert.AreEqual((BigInteger)n * b, (uint)n * (UInt128)b);
                    Assert.AreEqual((BigInteger)n * a * b % ((BigInteger)1 << 128), (uint)n * ((UInt128)a * (UInt128)b));
                }
                Assert.AreEqual((BigInteger)a * b, a * (UInt128)b);
                Assert.AreEqual((BigInteger)a * b, (UInt128)a * b);
                Assert.AreEqual((BigInteger)a * b, a * (UInt128)b);
                Assert.AreEqual((BigInteger)a * b, (UInt128)a * (UInt128)b);
                if (b > 0)
                {
                    Assert.AreEqual((BigInteger)a % b, (UInt128)a % b);
                    Assert.AreEqual((BigInteger)a % b, a % (UInt128)b);
                    Assert.AreEqual((BigInteger)a % b, (UInt128)a % (UInt128)b);
                }
                Assert.AreEqual((BigInteger)a * b % n, (UInt128)a * b % n);
                Assert.AreEqual((BigInteger)a * b % n, a * (UInt128)b % n);
                Assert.AreEqual((BigInteger)a * b % n, (UInt128)a * (UInt128)b % (UInt128)n);
                if (c > 0 && d > 0)
                {
                    Assert.AreEqual((BigInteger)a * b / ((BigInteger)c * d), (UInt128)a * (UInt128)b / ((UInt128)c * (UInt128)d));
                    Assert.AreEqual((BigInteger)a * b % ((BigInteger)c * d), (UInt128)a * (UInt128)b % ((UInt128)c * (UInt128)d));
                }
                if (b > 0)
                {
                    Assert.AreEqual((BigInteger)a / b, (UInt128)a / b);
                    Assert.AreEqual((BigInteger)a / b, a / (UInt128)b);
                    Assert.AreEqual((BigInteger)a / b, (UInt128)a / (UInt128)b);
                }
                Assert.AreEqual((BigInteger)a * b / n, (UInt128)a * b / n);
                Assert.AreEqual((BigInteger)a * b / n, a * (UInt128)b / n);
                Assert.AreEqual((BigInteger)a * b / n, (UInt128)a * (UInt128)b / (UInt128)n);
                Assert.AreEqual((BigInteger)a < b, (UInt128)a < b);
                Assert.AreEqual((BigInteger)a < b, a < (UInt128)b);
                Assert.AreEqual((BigInteger)a < b, (UInt128)a < (UInt128)b);
                Assert.AreEqual((BigInteger)a <= b, (UInt128)a <= b);
                Assert.AreEqual((BigInteger)a <= b, a <= (UInt128)b);
                Assert.AreEqual((BigInteger)a <= b, (UInt128)a <= (UInt128)b);
                Assert.AreEqual((BigInteger)a > b, (UInt128)a > b);
                Assert.AreEqual((BigInteger)a > b, a > (UInt128)b);
                Assert.AreEqual((BigInteger)a > b, (UInt128)a > (UInt128)b);
                Assert.AreEqual((BigInteger)a >= b, (UInt128)a >= b);
                Assert.AreEqual((BigInteger)a >= b, a >= (UInt128)b);
                Assert.AreEqual((BigInteger)a >= b, (UInt128)a >= (UInt128)b);
                Assert.AreEqual((BigInteger)a == b, (UInt128)a == b);
                Assert.AreEqual((BigInteger)a == b, a == (UInt128)b);
                Assert.AreEqual((BigInteger)a == b, (UInt128)a == (UInt128)b);
                Assert.AreEqual((BigInteger)a != b, (UInt128)a != b);
                Assert.AreEqual((BigInteger)a != b, a != (UInt128)b);
                Assert.AreEqual((BigInteger)a != b, (UInt128)a != (UInt128)b);
                Assert.AreEqual((BigInteger)a * a, UInt128.Square(a));
                Assert.AreEqual(BigInteger.Abs(a), UInt128.Abs(a));
                Assert.AreEqual(BigInteger.Abs((BigInteger)a * b), UInt128.Abs((UInt128)a * b));
                Assert.AreEqual(BigInteger.Min(a, b), UInt128.Min(a, b));
                Assert.AreEqual(BigInteger.Min((BigInteger)a * n, (BigInteger)b * n), UInt128.Min((UInt128)a * n, (UInt128)b * n));
                Assert.AreEqual(BigInteger.Max(a, b), UInt128.Max(a, b));
                Assert.AreEqual(BigInteger.Max((BigInteger)a * n, (BigInteger)b * n), UInt128.Max((UInt128)a * n, (UInt128)b * n));
                for (var j = 0; j < 2; j++)
                {
                    var m = UInt128.Abs(j == 0 ? (UInt128)a * (UInt128)b : (UInt128)n * (UInt128)n);
                    var floorsqrt = UInt128.FloorSqrt(m);
                    Assert.IsTrue((BigInteger)floorsqrt * floorsqrt <= m && (BigInteger)(floorsqrt + 1) * (floorsqrt + 1) > m);
                    var ceilingsqrt = UInt128.CeilingSqrt(m);
                    Assert.IsTrue((BigInteger)(ceilingsqrt - 1) * (ceilingsqrt - 1) < m && (BigInteger)ceilingsqrt * ceilingsqrt >= m);
                }
                for (var j = 0; j < 2; j++)
                {
                    var m = j == 0 ? (UInt128)a * (UInt128)b : (UInt128)BigInteger.Pow((BigInteger)Math.Floor(Math.Pow((double)((BigInteger)a * b), (double)1 / 3)), 3);
                    var floorcbrt = UInt128.FloorCbrt(m);
                    Assert.IsTrue((BigInteger)floorcbrt * floorcbrt * floorcbrt <= m && (BigInteger)(floorcbrt + 1) * (floorcbrt + 1) * (floorcbrt + 1) > m);
                    var ceilingcbrt = UInt128.CeilingCbrt(m);
                    Assert.IsTrue((BigInteger)(ceilingcbrt - 1) * (ceilingcbrt - 1) * (ceilingcbrt - 1) < m && (BigInteger)ceilingcbrt * ceilingcbrt * ceilingcbrt >= m);
                }
                Assert.AreEqual(BigInteger.GreatestCommonDivisor((BigInteger)a, (BigInteger)b), UInt128.GreatestCommonDivisor((UInt128)a, (UInt128)b));
                Assert.AreEqual(BigInteger.GreatestCommonDivisor((BigInteger)a * b, (BigInteger)c * d), UInt128.GreatestCommonDivisor((UInt128)a * b, (UInt128)c * d));
                Assert.AreEqual(0, 0);
            }
        }

        [TestMethod]
        [DataRow((long)1 << 20, (long)1 << 20)]
        [DataRow((long)1 << 40, (long)1 << 20)]
        [DataRow((long)1 << 60, (long)1 << 20)]
        [DataRow((long)1 << 20, (long)1 << 40)]
        [DataRow((long)1 << 40, (long)1 << 40)]
        [DataRow((long)1 << 60, (long)1 << 40)]
        [DataRow((long)1 << 20, (long)1 << 60)]
        [DataRow((long)1 << 40, (long)1 << 60)]
        [DataRow((long)1 << 60, (long)1 << 60)]
//        [DataRow(int.MaxValue, int.MaxValue)]
//        [DataRow(uint.MaxValue, uint.MaxValue)]
//        [DataRow(long.MaxValue, long.MaxValue)]
        public void Int128Test(long factorMax, long modulusMax)
        {       
            var random = new MersenneTwister(0).Create<long>();
            for (int i = 0; i < 10000; i++)
            {
                var n = random.Next(modulusMax - 1) + 1;
                var a = random.Next(factorMax) % n - factorMax / 2;
                var b = random.Next(factorMax) % n - factorMax / 2;
                var s = (int)(Math.Abs(b) % 32);
                var value = (Int128)0;
                Assert.AreEqual((BigInteger)a << s, (Int128)a << s);
                Assert.AreEqual((BigInteger)a >> s, (Int128)a >> s);
                Assert.AreEqual((BigInteger)a & b, (Int128)a & b);
                Assert.AreEqual((BigInteger)a & b, a & (Int128)b);
                Assert.AreEqual((BigInteger)a & b, (Int128)a & (Int128)b);
                Assert.AreEqual((BigInteger)a | b, (Int128)a | b);
                Assert.AreEqual((BigInteger)a | b, a | (Int128)b);
                Assert.AreEqual((BigInteger)a | b, (Int128)a | (Int128)b);
                Assert.AreEqual((BigInteger)a ^ b, (Int128)a ^ b);
                Assert.AreEqual((BigInteger)a ^ b, a ^ (Int128)b);
                Assert.AreEqual((BigInteger)a ^ b, (Int128)a ^ (Int128)b);
                if (a <= long.MaxValue)
                    Assert.AreEqual(~(BigInteger)a, (long)~(Int128)a);
                Assert.AreEqual((BigInteger)a + b, (Int128)a + b);
                Assert.AreEqual((BigInteger)a + b, a + (Int128)b);
                Assert.AreEqual((BigInteger)a + b, (Int128)a + (Int128)b);
                Assert.AreEqual(((BigInteger)a * n + (BigInteger)b * n) % ((BigInteger)1 << 128), (Int128)a * n + (Int128)b * n);
                Assert.AreEqual((BigInteger)a - b, (Int128)a - b);
                Assert.AreEqual((BigInteger)a - b, a - (Int128)b);
                Assert.AreEqual((BigInteger)a - b, (Int128)a - (Int128)b);
                Assert.AreEqual(((BigInteger)a * n - (BigInteger)b * n) % ((BigInteger)1 << 128), (Int128)a * n - (Int128)b * n);
                Assert.AreEqual(+(BigInteger)a, +(Int128)a);
                Assert.AreEqual(-(BigInteger)a, -(Int128)a);
                value = a; Assert.AreEqual((BigInteger)a + 1, ++value);
                value = a; Assert.AreEqual((BigInteger)a, value++);
                value = (Int128)a * b; Assert.AreEqual((BigInteger)a * b + 1, ++value);
                value = (Int128)a * b; Assert.AreEqual((BigInteger)a * b, value++);
                value = a; Assert.AreEqual((BigInteger)a - 1, --value);
                value = a; Assert.AreEqual((BigInteger)a, value--);
                value = (Int128)a * b; Assert.AreEqual((BigInteger)a * b - 1, --value);
                value = (Int128)a * b; Assert.AreEqual((BigInteger)a * b, value--);
                Assert.AreEqual((BigInteger)a * b, (Int128)a * b);
                Assert.AreEqual((BigInteger)a * b, a * (Int128)b);
                Assert.AreEqual((BigInteger)a * b, (Int128)a * (Int128)b);
                Assert.AreEqual((BigInteger)a * b % n, (Int128)a * b % n);
                Assert.AreEqual((BigInteger)a * b % n, a * (Int128)b % n);
                Assert.AreEqual((BigInteger)a * b % n, (Int128)a * (Int128)b % (Int128)n);
                Assert.AreEqual((BigInteger)a * b / n, (Int128)a * b / n);
                Assert.AreEqual((BigInteger)a * b / n, a * (Int128)b / n);
                Assert.AreEqual((BigInteger)a * b / n, (Int128)a * (Int128)b / (Int128)n);
                Assert.AreEqual((BigInteger)a < b, (Int128)a < b);
                Assert.AreEqual((BigInteger)a < b, a < (Int128)b);
                Assert.AreEqual((BigInteger)a < b, (Int128)a < (Int128)b);
                Assert.AreEqual((BigInteger)a <= b, (Int128)a <= b);
                Assert.AreEqual((BigInteger)a <= b, a <= (Int128)b);
                Assert.AreEqual((BigInteger)a <= b, (Int128)a <= (Int128)b);
                Assert.AreEqual((BigInteger)a > b, (Int128)a > b);
                Assert.AreEqual((BigInteger)a > b, a > (Int128)b);
                Assert.AreEqual((BigInteger)a > b, (Int128)a > (Int128)b);
                Assert.AreEqual((BigInteger)a >= b, (Int128)a >= b);
                Assert.AreEqual((BigInteger)a >= b, a >= (Int128)b);
                Assert.AreEqual((BigInteger)a >= b, (Int128)a >= (Int128)b);
                Assert.AreEqual((BigInteger)a == b, (Int128)a == b);
                Assert.AreEqual((BigInteger)a == b, a == (Int128)b);
                Assert.AreEqual((BigInteger)a == b, (Int128)a == (Int128)b);
                Assert.AreEqual((BigInteger)a != b, (Int128)a != b);
                Assert.AreEqual((BigInteger)a != b, a != (Int128)b);
                Assert.AreEqual((BigInteger)a != b, (Int128)a != (Int128)b);
                Assert.AreEqual(BigInteger.Abs(a), Int128.Abs(a));
                Assert.AreEqual(BigInteger.Abs((BigInteger)a * b), Int128.Abs((Int128)a * b));
                Assert.AreEqual(BigInteger.Min(a, b), Int128.Min(a, b));
                Assert.AreEqual(BigInteger.Min((BigInteger)a * n, (BigInteger)b * n), Int128.Min((Int128)a * n, (Int128)b * n));
                Assert.AreEqual(BigInteger.Max(a, b), Int128.Max(a, b));
                Assert.AreEqual(BigInteger.Max((BigInteger)a * n, (BigInteger)b * n), Int128.Max((Int128)a * n, (Int128)b * n));
                for (var j = 0; j < 2; j++)
                {
                    var m = Int128.Abs(j == 0 ? (Int128)a * (Int128)b : (Int128)n * (Int128)n);
                    var floorsqrt = Int128.FloorSqrt(m);
                    Assert.IsTrue((BigInteger)floorsqrt * floorsqrt <= m && (BigInteger)(floorsqrt + 1) * (floorsqrt + 1) > m);
                    var ceilingsqrt = Int128.CeilingSqrt(m);
                    Assert.IsTrue((BigInteger)(ceilingsqrt - 1) * (ceilingsqrt - 1) < m && (BigInteger)ceilingsqrt * ceilingsqrt >= m);
                }
                for (var j = 0; j < 2; j++)
                {
                    var m = j == 0 ? (Int128)a * (Int128)b : (Int128)(Math.Sign(a) * Math.Sign(b) * BigInteger.Pow((BigInteger)Math.Floor(Math.Pow((double)BigInteger.Abs((BigInteger)a * b), (double)1 / 3)), 3));
                    var floorcbrt = Int128.FloorCbrt(m);
                    Assert.IsTrue(Math.Sign(floorcbrt) == m.Sign && (BigInteger)Math.Abs(floorcbrt) * Math.Abs(floorcbrt) * Math.Abs(floorcbrt) <= BigInteger.Abs(m) && (BigInteger)(Math.Abs(floorcbrt) + 1) * (Math.Abs(floorcbrt) + 1) * (Math.Abs(floorcbrt) + 1) > BigInteger.Abs(m));
                    var ceilingcbrt = Int128.CeilingCbrt(m);
                    Assert.IsTrue(Math.Sign(ceilingcbrt) == m.Sign && (BigInteger)(Math.Abs(ceilingcbrt) - 1) * (Math.Abs(ceilingcbrt) - 1) * (Math.Abs(ceilingcbrt) - 1) < BigInteger.Abs(m) && (BigInteger)Math.Abs(ceilingcbrt) * Math.Abs(ceilingcbrt) * Math.Abs(ceilingcbrt) >= BigInteger.Abs(m));
                }
            }
        }
    }
}