using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
    public class ModuloInteger
    {
        private BigInteger modulus;
        private uint[] modulusBits;
        private int length;
        private uint[] bits;

        public ModuloInteger(BigInteger n)
        {
            modulus = n;
            modulusBits = GetBits(n);
            length = modulusBits.Length;
            bits = new uint[length];
        }

        public ModuloInteger(ModuloInteger parent)
        {
            modulus = parent.modulus;
            modulusBits = parent.modulusBits;
            length = parent.length;
            bits = new uint[modulusBits.Length];
        }

        public ModuloInteger Assign(BigInteger n)
        {
            bits = GetBits(n % modulus);
            return this;
        }

        public ModuloInteger Add(ModuloInteger n)
        {
            Add(bits, n.bits, length);
            if (Compare(bits, modulusBits) >= 0)
                Subtract(bits, modulusBits, length);
            return this;
        }

        private int Compare(uint[] a, uint[] b)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                var a0 = a[i];
                var b0 = b[i];
                if (a0 < b0)
                    return -1;
                if (a0 > b0)
                    return 1;
            }
            return 0;
        }

        private static uint Add(uint[] a, uint[] b, int length)
        {
            ulong carry = 0;
            for (int i = 0; i < length; i++)
            {
                var sum = (ulong)a[i] + (ulong)b[i] + carry;
                a[i] = (uint)sum;
                carry = sum >> 32;
            }
            return (uint)carry;
        }

        private static uint Subtract(uint[] a, uint[] b, int length)
        {
            ulong borrow = 0;
            for (int i = 0; i < length; i++)
            {
                var diff = (ulong)a[i] - (ulong)b[i] - borrow;
                a[i] = (uint)diff;
                borrow = (ulong)(-(uint)((int)(diff >> 32)));
            }
            return (uint)borrow;
        }

        private static uint Multiply(uint[] a, uint[] b, int length)
        {
            ulong carry = 0;
            for (int i = 0; i < length; i++)
            {
                var sum = (ulong)a[i] + (ulong)b[i] + carry;
                a[i] = (uint)sum;
                carry = sum >> 32;
            }
            return (uint)carry;
        }

        private static uint[] GetBits(BigInteger n)
        {
            var bytes = n.ToByteArray();
            int length = (bytes.Length + 3) / 4;
            if (4 * length != bytes.Length)
                bytes = bytes.Concat(new byte[4 * length - bytes.Length]).ToArray();
            var bits = new uint[length];
            for (int i = 0; i < length; i++)
                bits[i] = (uint)BitConverter.ToInt32(bytes, 4 * i);
            return bits;
        }

        public ModuloInteger Mult(ModuloInteger n)
        {
            return this;
        }
    }
}
