using System;
using System.Globalization;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.Decompose.Numerics.Test
{
    [TestClass]
    public class UInt256ProgressTests
    {
        [TestMethod]
        public void Create_uint()
        {
            UInt256.Create(out UInt256 result, 1U, 2U, 3U, 4U, 5U, 6U, 7U, 8U);
            Assert.AreEqual((2UL << 32) + 1, result.S0);
            Assert.AreEqual(4 * (1UL << 32) + 3, result.S1);
            Assert.AreEqual(6 * (1UL << 32) + 5, result.S2);
            Assert.AreEqual(8 * (1UL << 32) + 7, result.S3);
        }
        
        [TestMethod]
        public void Create_ulong()
        {
            UInt256.Create(out UInt256 result, 1UL, 2Ul, 3UL, 4UL);
            Assert.AreEqual(1UL, result.S0);
            Assert.AreEqual(2UL, result.S1);
            Assert.AreEqual(3UL, result.S2);
            Assert.AreEqual(4UL, result.S3);
        }
        
        [TestMethod]
        public void Create_UInt128()
        {
            UInt256.Create(out UInt256 result, 1, 2);
            Assert.AreEqual(1UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(2UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Create_single_long()
        {
            UInt256.Create(out UInt256 result, 1L);
            Assert.AreEqual(1UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Create_single_engative_long()
        {
            UInt256.Create(out UInt256 result, -1L);
            unchecked
            {
                Assert.AreEqual((ulong)-1L, result.S0);    
            }
            
            Assert.AreEqual(ulong.MaxValue, result.S1);
            Assert.AreEqual(ulong.MaxValue, result.S2);
            Assert.AreEqual(ulong.MaxValue, result.S3);
        }
        
        [TestMethod]
        public void Create_single_ulong()
        {
            UInt256.Create(out UInt256 result, 1UL);
            Assert.AreEqual(1UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Create_span()
        {
            byte[] bytes = new byte[32];
            bytes[7] = 1;
            bytes[15] = 2;
            bytes[23] = 3;
            bytes[31] = 4;
            
            UInt256.CreateFromBigEndian(out UInt256 result, bytes);
            Assert.AreEqual(1UL, result.S3);
            Assert.AreEqual(2UL, result.S2);
            Assert.AreEqual(3UL, result.S1);
            Assert.AreEqual(4UL, result.S0);
        }
        
        [TestMethod]
        public void Create_span_missing_byte()
        {
            byte[] bytes = new byte[31];
            bytes[6] = 1;
            bytes[14] = 2;
            bytes[22] = 3;
            
            UInt256.CreateFromBigEndian(out UInt256 result, bytes);
            Assert.AreEqual(1UL, result.S3);
            Assert.AreEqual(2UL, result.S2);
            Assert.AreEqual(3UL, result.S1);
            Assert.AreEqual(0UL, result.S0);
        }
        
        [TestMethod]
        public void Create_span_3_bytes()
        {
            byte[] bytes = new byte[24];
            bytes[7] = 1;
            bytes[15] = 2;
            bytes[23] = 3;
            
            UInt256.CreateFromBigEndian(out UInt256 result, bytes);
            Assert.AreEqual(0UL, result.S3);
            Assert.AreEqual(1UL, result.S2);
            Assert.AreEqual(2UL, result.S1);
            Assert.AreEqual(3UL, result.S0);
        }
        
        [TestMethod]
        public void Create_span_empty()
        {
            byte[] bytes = new byte[0];
            
            UInt256.CreateFromBigEndian(out UInt256 result, bytes);
            Assert.AreEqual(0UL, result.S3);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S0);
        }
        
        [TestMethod]
        public void Add()
        {
            byte[] bytesA = new byte[32];
            bytesA[7] = 1;
            bytesA[15] = 2;
            bytesA[23] = 3;
            bytesA[31] = 4;
            
            byte[] bytesB = new byte[32];
            bytesB[7] = 10;
            bytesB[15] = 20;
            bytesB[23] = 30;
            bytesB[31] = 40;
            
            UInt256.CreateFromBigEndian(out UInt256 a, bytesA);
            UInt256.CreateFromBigEndian(out UInt256 b, bytesB);
            var result = a + b;
            Assert.AreEqual(result, b + a);
            
            Assert.AreEqual(11UL, result.S3);
            Assert.AreEqual(22UL, result.S2);
            Assert.AreEqual(33UL, result.S1);
            Assert.AreEqual(44UL, result.S0);
        }
        
        [TestMethod]
        public void Add_carry_all()
        {
            UInt256.Create(out UInt256 a, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            UInt256.Create(out UInt256 b, 1);
            UInt256.Add(out UInt256 c, ref a, ref b, false);;
            
            Assert.AreEqual(0UL, c.S0);
            Assert.AreEqual(0UL, c.S1);
            Assert.AreEqual(0UL, c.S2);
            Assert.AreEqual(0UL, c.S3);
        }
        
        [TestMethod]
        public void Add_carry_one()
        {
            UInt256.Create(out UInt256 a, ulong.MaxValue, 0, 0, 0);
            UInt256.Create(out UInt256 b, 1);
            var result = a + b;
            Assert.AreEqual(result, b + a);
            
            Assert.AreEqual(0UL, result.S0);
            Assert.AreEqual(1UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Add_carry_twice()
        {
            UInt256.Create(out UInt256 a, 1);
            UInt256.Create(out UInt256 b, ulong.MaxValue, ulong.MaxValue, 0, 0);
            var result = a + b;
            Assert.AreEqual(result, b + a);
            
            Assert.AreEqual(0UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(1UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Add_carry_thrice()
        {
            UInt256.Create(out UInt256 a, 1);
            UInt256.Create(out UInt256 b, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, 0);
            var result = a + b;
            Assert.AreEqual(result, b + a);
            
            Assert.AreEqual(0UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(1UL, result.S3);
        }
        
        [TestMethod]
        public void Add_carry_all_UL()
        {
            UInt256.Create(out UInt256 a, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            var result = a + 1UL;
            Assert.AreEqual(result, 1UL + a);
            
            Assert.AreEqual(0UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Add_carry_one_UL()
        {
            UInt256.Create(out UInt256 a, ulong.MaxValue, 0, 0, 0);
            var result = a + 1UL;
            Assert.AreEqual(result, 1UL + a);
            
            Assert.AreEqual(0UL, result.S0);
            Assert.AreEqual(1UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Add_regression()
        {
            UInt256.Create(out UInt256 a, 0, 0, 0, 7809331261766606202);
            UInt256.Create(out UInt256 b, 0, 18446744069414584320, 18446744073709551615, 18446744073709551615);
            UInt256.Add(out UInt256 result, ref a, ref b, false);
            
            Assert.AreEqual(0UL, result.S0);
            Assert.AreEqual(18446744069414584320UL, result.S1);
            Assert.AreEqual(18446744073709551615UL, result.S2);
            Assert.AreEqual(7809331261766606201UL, result.S3);
        }
        
        [TestMethod]
        public void Add_regression_2()
        {
            UInt256.Create(out UInt256 b, 13479156459573198416, 18446744073709551615, 18446744073709551615, 18446744073709551615);
            UInt256.Create(out UInt256 a, 6553255926290448384, 1, 0, 0);
            UInt256.Add(out UInt256 result, ref a, ref b, false);
            
            Console.WriteLine(result);
//            Assert.AreEqual((BigInteger)(a + b) % ((BigInteger)1 << 256), (BigInteger)a + (BigInteger)b);
        }
        
        [TestMethod]
        public void Subtract_carry_all()
        {
            UInt256.Create(out UInt256 a, 0, 0, 0, 0);
            UInt256.Create(out UInt256 b, 1);
            var result = a - b;
            
            Assert.AreEqual(ulong.MaxValue, result.S0);
            Assert.AreEqual(ulong.MaxValue, result.S1);
            Assert.AreEqual(ulong.MaxValue, result.S2);
            Assert.AreEqual(ulong.MaxValue, result.S3);
        }
        
        [TestMethod]
        public void Subtract_carry_one()
        {
            UInt256.Create(out UInt256 a, 0, 1, 0, 0);
            UInt256.Create(out UInt256 b, 1);
            var result = a - b;
            
            Assert.AreEqual(ulong.MaxValue, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Subtract_carry_all_UL()
        {
            UInt256.Create(out UInt256 a, 0, 0, 0, 0);
            var result = a - 1UL;
            
            Assert.AreEqual(ulong.MaxValue, result.S0);
            Assert.AreEqual(ulong.MaxValue, result.S1);
            Assert.AreEqual(ulong.MaxValue, result.S2);
            Assert.AreEqual(ulong.MaxValue, result.S3);
        }
        
        [TestMethod]
        public void Subtract_carry_one_UL()
        {
            UInt256.Create(out UInt256 a, 0, 1, 0, 0);
            var result = a - 1UL;
            
            Assert.AreEqual(ulong.MaxValue, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void BigEndian_there_and_back_again()
        {
            int length = 32;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)(length - (byte)i - 1);
            }
            
            UInt256.CreateFromBigEndian(out UInt256 a, bytes);
            a.ToBigEndian(bytes);
            
            for (int i = length - 1; i >= 0; i--)
            {
                Assert.AreEqual((byte)(length - (byte)i - 1), bytes[i], $"{i}");
            }
        }
        
        [TestMethod]
        public void Multiply64_ulong()
        {
            UInt256 a = 1UL;
            ulong b = 1UL;
            var result = a * b;
            Assert.AreEqual(result, b * a);
            
            Assert.AreEqual(1UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Multiply64_uint()
        {
            UInt256 a = 1UL;
            uint b = 1U;
            var result = a * b;
            Assert.AreEqual(result, b * a);
            
            Assert.AreEqual(1UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Multiply64_ulong_max()
        {
            UInt256 a = new UInt256(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            ulong b = ulong.MaxValue;
            var result = a * b;
            Assert.AreEqual(result, b * a);
        }
        
        [TestMethod]
        public void Multiply64_uint_max()
        {
            UInt256 a = new UInt256(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            uint b = uint.MaxValue;
            var result = a * b;
            Assert.AreEqual(result, b * a);
        }
        
        [TestMethod]
        public void Shifts()
        {
            UInt256 a = new UInt256(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            for (int i = 0; i < 256; i++)
            {
                UInt256 b = a << i;
                var c = b >> i;
                var d = c << i;
                Assert.AreEqual(b, d, $"{i}");
            }
            
            for (int i = 0; i < 256; i++)
            {
                UInt256 b = a >> i;
                var c = b << i;
                var d = c >> i;
                Assert.AreEqual(b, d, $"{i}");
            }
        }
        
        [TestMethod]
        public void Arithmetic_shift()
        {
            UInt256 a = UInt256.Zero;
            UInt256.ArithmeticRightShift(ref a, 1);
            Assert.AreEqual(UInt256.Zero, a);
        }
        
        [TestMethod]
        public void Span_add()
        {
            byte[] bytes = new byte[64];
            for (int i = 0; i < 64; i++)
            {
                bytes[i] = (byte)i;
            }
            
            Span<byte> span = bytes.AsSpan();
            UInt256.AddInPlace(span.Slice(0, 32), span.Slice(32, 32));
        }
        
        [TestMethod]
        public void To_big_endian_and_back_regression()
        {
            UInt256 a = UInt256.Parse("1000000000000000000000000000000");
            byte[] bigEndian = new byte[32];
            a.ToBigEndian(bigEndian);
            
            UInt256.CreateFromBigEndian(out UInt256 b, bigEndian);
            var c = UInt256.Parse(a.ToString());
            
            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);
        }
        
        [TestMethod]
        public void To_big_endian_and_back_regression_goerli()
        {
            UInt256.TryParse("84595161401484a000000".Replace("0x", string.Empty), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out UInt256 a);
            string stringValue = a.ToString();
            Assert.AreEqual("10000000000000000000000000", stringValue);
        }

        [TestMethod]
        public void To_big_endian_and_back_max_value()
        {
            UInt256 a = UInt256.MaxValue;
            byte[] bigEndian = new byte[32];
            a.ToBigEndian(bigEndian);
            UInt256.CreateFromBigEndian(out UInt256 b, bigEndian);
            var c = UInt256.Parse(a.ToString());
            
            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);
        }

        [TestMethod] public void Parse_throws_on_just_too_large()
        {
            BigInteger max = UInt256.MaxValue;
            BigInteger maxPlusOne = max + 1;
            
            var maxUInt = UInt256.Parse(max.ToString());
            Assert.ThrowsException<OverflowException>(() => UInt256.Parse(maxPlusOne.ToString()));
        }   
    }
}