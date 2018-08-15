using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.Decompose.Numerics.Test
{
    [TestClass]
    public class UInt256Tests
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
        public void UInt256Test(ulong factorMax, ulong modulusMax)
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
                var value = (UInt256)0;
                Assert.AreEqual((BigInteger)a << s, (UInt256)a << s);
                Assert.AreEqual((BigInteger)a >> s, (UInt256)a >> s);
                Assert.AreEqual((BigInteger)a & b, (UInt256)a & b);
                Assert.AreEqual((BigInteger)a & b, a & (UInt256)b);
                Assert.AreEqual((BigInteger)a & b, (UInt256)a & (UInt256)b);
                Assert.AreEqual((BigInteger)a | b, (UInt256)a | b);
                Assert.AreEqual((BigInteger)a | b, a | (UInt256)b);
                Assert.AreEqual((BigInteger)a | b, (UInt256)a | (UInt256)b);
                Assert.AreEqual((BigInteger)a ^ b, (UInt256)a ^ b);
                Assert.AreEqual((BigInteger)a ^ b, a ^ (UInt256)b);
                Assert.AreEqual((BigInteger)a ^ b, (UInt256)a ^ (UInt256)b);
                if (a <= long.MaxValue)
                    Assert.AreEqual(~(BigInteger)a, (long)~(UInt256)a);
                Assert.AreEqual((BigInteger)a + b, (UInt256)a + b);
                Assert.AreEqual((BigInteger)a + b, a + (UInt256)b);
                Assert.AreEqual((BigInteger)a + b, (UInt256)a + (UInt256)b);
                Assert.AreEqual(((BigInteger)a * n + (BigInteger)b * n) % ((BigInteger)1 << 128), (UInt256)a * n + (UInt256)b * n);
                if (a >= b)
                {
                    Assert.AreEqual((BigInteger)a - b, (UInt256)a - b);
                    Assert.AreEqual((BigInteger)a - b, a - (UInt256)b);
                    Assert.AreEqual((BigInteger)a - b, (UInt256)a - (UInt256)b);
                    Assert.AreEqual((BigInteger)a * n - (BigInteger)b * n, (UInt256)a * n - (UInt256)b * n);
                }
                Assert.AreEqual(+(BigInteger)a, +(Int128)a);
                value = a; Assert.AreEqual((BigInteger)a + 1, ++value);
                value = a; Assert.AreEqual((BigInteger)a, value++);
                value = (UInt256)a * b; Assert.AreEqual((BigInteger)a * b + 1, ++value);
                value = (UInt256)a * b; Assert.AreEqual((BigInteger)a * b, value++);
                if (a > 0)
                {
                    value = a; Assert.AreEqual((BigInteger)a - 1, --value);
                    value = a; Assert.AreEqual((BigInteger)a, value--);
                }
                if (a > 0 && b > 0)
                {
                    value = (UInt256)a * b; Assert.AreEqual((BigInteger)a * b - 1, --value);
                    value = (UInt256)a * b; Assert.AreEqual((BigInteger)a * b, value--);
                }
                if (n <= uint.MaxValue)
                {
                    Assert.AreEqual((BigInteger)a * n, (UInt256)a * (uint)n);
                    Assert.AreEqual((BigInteger)b * n, (UInt256)b * (uint)n);
                    Assert.AreEqual((BigInteger)a * b * n % ((BigInteger)1 << 128), (UInt256)a * b * (uint)n);
                    Assert.AreEqual((BigInteger)n * a, (uint)n * (UInt256)a);
                    Assert.AreEqual((BigInteger)n * b, (uint)n * (UInt256)b);
                    Assert.AreEqual((BigInteger)n * a * b % ((BigInteger)1 << 128), (uint)n * ((UInt256)a * (UInt256)b));
                }
                Assert.AreEqual((BigInteger)a * b, a * (UInt256)b);
                Assert.AreEqual((BigInteger)a * b, (UInt256)a * b);
                Assert.AreEqual((BigInteger)a * b, a * (UInt256)b);
                Assert.AreEqual((BigInteger)a * b, (UInt256)a * (UInt256)b);
                if (b > 0)
                {
                    Assert.AreEqual((BigInteger)a % b, (UInt256)a % b);
                    Assert.AreEqual((BigInteger)a % b, a % (UInt256)b);
                    Assert.AreEqual((BigInteger)a % b, (UInt256)a % (UInt256)b);
                }
                Assert.AreEqual((BigInteger)a * b % n, (UInt256)a * b % n);
                Assert.AreEqual((BigInteger)a * b % n, a * (UInt256)b % n);
                Assert.AreEqual((BigInteger)a * b % n, (UInt256)a * (UInt256)b % (UInt256)n);
                if (c > 0 && d > 0)
                {
                    Assert.AreEqual((BigInteger)a * b / ((BigInteger)c * d), (UInt256)a * (UInt256)b / ((UInt256)c * (UInt256)d));
                    Assert.AreEqual((BigInteger)a * b % ((BigInteger)c * d), (UInt256)a * (UInt256)b % ((UInt256)c * (UInt256)d));
                }
                if (b > 0)
                {
                    Assert.AreEqual((BigInteger)a / b, (UInt256)a / b);
                    Assert.AreEqual((BigInteger)a / b, a / (UInt256)b);
                    Assert.AreEqual((BigInteger)a / b, (UInt256)a / (UInt256)b);
                }
                Assert.AreEqual((BigInteger)a * b / n, (UInt256)a * b / n);
                Assert.AreEqual((BigInteger)a * b / n, a * (UInt256)b / n);
                Assert.AreEqual((BigInteger)a * b / n, (UInt256)a * (UInt256)b / (UInt256)n);
                Assert.AreEqual((BigInteger)a < b, (UInt256)a < b);
                Assert.AreEqual((BigInteger)a < b, a < (UInt256)b);
                Assert.AreEqual((BigInteger)a < b, (UInt256)a < (UInt256)b);
                Assert.AreEqual((BigInteger)a <= b, (UInt256)a <= b);
                Assert.AreEqual((BigInteger)a <= b, a <= (UInt256)b);
                Assert.AreEqual((BigInteger)a <= b, (UInt256)a <= (UInt256)b);
                Assert.AreEqual((BigInteger)a > b, (UInt256)a > b);
                Assert.AreEqual((BigInteger)a > b, a > (UInt256)b);
                Assert.AreEqual((BigInteger)a > b, (UInt256)a > (UInt256)b);
                Assert.AreEqual((BigInteger)a >= b, (UInt256)a >= b);
                Assert.AreEqual((BigInteger)a >= b, a >= (UInt256)b);
                Assert.AreEqual((BigInteger)a >= b, (UInt256)a >= (UInt256)b);
                Assert.AreEqual((BigInteger)a == b, (UInt256)a == b);
                Assert.AreEqual((BigInteger)a == b, a == (UInt256)b);
                Assert.AreEqual((BigInteger)a == b, (UInt256)a == (UInt256)b);
                Assert.AreEqual((BigInteger)a != b, (UInt256)a != b);
                Assert.AreEqual((BigInteger)a != b, a != (UInt256)b);
                Assert.AreEqual((BigInteger)a != b, (UInt256)a != (UInt256)b);
                Assert.AreEqual((BigInteger)a * a, UInt256.Square(a));
                Assert.AreEqual(BigInteger.Abs(a), UInt256.Abs(a));
                Assert.AreEqual(BigInteger.Abs((BigInteger)a * b), UInt256.Abs((UInt256)a * b));
                Assert.AreEqual(BigInteger.Min(a, b), UInt256.Min(a, b));
                Assert.AreEqual(BigInteger.Min((BigInteger)a * n, (BigInteger)b * n), UInt256.Min((UInt256)a * n, (UInt256)b * n));
                Assert.AreEqual(BigInteger.Max(a, b), UInt256.Max(a, b));
                Assert.AreEqual(BigInteger.Max((BigInteger)a * n, (BigInteger)b * n), UInt256.Max((UInt256)a * n, (UInt256)b * n));
                for (var j = 0; j < 2; j++)
                {
                    var m = UInt256.Abs(j == 0 ? (UInt256)a * (UInt256)b : (UInt256)n * (UInt256)n);
                    var floorsqrt = UInt256.FloorSqrt(m);
                    Assert.IsTrue((BigInteger)floorsqrt * floorsqrt <= m && (BigInteger)(floorsqrt + 1) * (floorsqrt + 1) > m);
                    var ceilingsqrt = UInt256.CeilingSqrt(m);
                    Assert.IsTrue((BigInteger)(ceilingsqrt - 1) * (ceilingsqrt - 1) < m && (BigInteger)ceilingsqrt * ceilingsqrt >= m);
                }
                for (var j = 0; j < 2; j++)
                {
                    var m = j == 0 ? (UInt256)a * (UInt256)b : (UInt256)BigInteger.Pow((BigInteger)Math.Floor(Math.Pow((double)((BigInteger)a * b), (double)1 / 3)), 3);
                    var floorcbrt = UInt256.FloorCbrt(m);
                    Assert.IsTrue((BigInteger)floorcbrt * floorcbrt * floorcbrt <= m && (BigInteger)(floorcbrt + 1) * (floorcbrt + 1) * (floorcbrt + 1) > m);
                    var ceilingcbrt = UInt256.CeilingCbrt(m);
                    Assert.IsTrue((BigInteger)(ceilingcbrt - 1) * (ceilingcbrt - 1) * (ceilingcbrt - 1) < m && (BigInteger)ceilingcbrt * ceilingcbrt * ceilingcbrt >= m);
                }
                Assert.AreEqual(BigInteger.GreatestCommonDivisor((BigInteger)a, (BigInteger)b), UInt256.GreatestCommonDivisor((UInt256)a, (UInt256)b));
                Assert.AreEqual(BigInteger.GreatestCommonDivisor((BigInteger)a * b, (BigInteger)c * d), UInt256.GreatestCommonDivisor((UInt256)a * b, (UInt256)c * d));
                Assert.AreEqual(0, 0);
            }
        }
    }
}