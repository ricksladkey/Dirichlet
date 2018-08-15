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
            bytes[0] = 1;
            bytes[8] = 2;
            bytes[16] = 3;
            bytes[24] = 4;
            
            UInt256.Create(out UInt256 result, bytes);
            Assert.AreEqual(1UL, result.S0);
            Assert.AreEqual(2UL, result.S1);
            Assert.AreEqual(3UL, result.S2);
            Assert.AreEqual(4UL, result.S3);
        }
        
        [TestMethod]
        public void Add()
        {
            byte[] bytesA = new byte[32];
            bytesA[0] = 1;
            bytesA[8] = 2;
            bytesA[16] = 3;
            bytesA[24] = 4;
            
            byte[] bytesB = new byte[32];
            bytesB[0] = 10;
            bytesB[8] = 20;
            bytesB[16] = 30;
            bytesB[24] = 40;
            
            UInt256.Create(out UInt256 a, bytesA);
            UInt256.Create(out UInt256 b, bytesB);
            var result = a + b;
            
            Assert.AreEqual(11UL, result.S0);
            Assert.AreEqual(22UL, result.S1);
            Assert.AreEqual(33UL, result.S2);
            Assert.AreEqual(44UL, result.S3);
        }
        
        [TestMethod]
        public void Add_carry_all()
        {
            UInt256.Create(out UInt256 a, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            UInt256.Create(out UInt256 b, 1);
            var result = a + b;
            
            Assert.AreEqual(0UL, result.S0);
            Assert.AreEqual(0UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
        
        [TestMethod]
        public void Add_carry_one()
        {
            UInt256.Create(out UInt256 a, ulong.MaxValue, 0, 0, 0);
            UInt256.Create(out UInt256 b, 1);
            var result = a + b;
            
            Assert.AreEqual(0UL, result.S0);
            Assert.AreEqual(1UL, result.S1);
            Assert.AreEqual(0UL, result.S2);
            Assert.AreEqual(0UL, result.S3);
        }
    }
}