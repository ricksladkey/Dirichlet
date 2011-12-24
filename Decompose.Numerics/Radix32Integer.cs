using System;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
    public struct Radix32Integer : IComparable<Radix32Integer>
    {
        private uint[] bits;
        private int index;
        private int length;

        public uint[] Bits { get { return bits; } }
        public int Index { get { return index; } }
        public int Length { get { return length; } }

        public Radix32Integer(uint[] bits, int index, int length)
        {
            this.bits = bits;
            this.index = index;
            this.length = length;
        }

        public void Clear()
        {
            for (int i = 0; i < length; i++)
                bits[index + i] = 0;
        }

        public void Set(BigInteger n)
        {
            Clear();
            GetBits(n).CopyTo(bits, index);
        }

        public void Set(Radix32Integer n)
        {
            for (int i = 0; i < length; i++)
                bits[index + i] = n.Bits[n.index + i];
        }

        public BigInteger ToBigInteger()
        {
            var bytes = new byte[length * 4 + 1];
            for (int i = 0; i < length; i++)
                BitConverter.GetBytes(bits[index + i]).CopyTo(bytes, i * 4);
            return new BigInteger(bytes);
        }

        public int CompareTo(Radix32Integer other)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                var a0 = bits[index + i];
                var b0 = other.bits[other.index + i];
                if (a0 < b0)
                    return -1;
                if (a0 > b0)
                    return 1;
            }
            return 0;
        }

        public void Mask(int n)
        {
            int i = n / 32;
            int j = n - i * 32;
            if (j == 0)
            {
                for (int k = length - 1; k >= i; k--)
                    bits[index + k] = 0;
            }
            else
            {
                for (int k = length - 1; k > i; k--)
                    bits[index + k] = 0;
                bits[index + i] &= (1u << j) - 1;
            }
        }

        public void SetSum(Radix32Integer a, Radix32Integer b)
        {
            ulong carry = 0;
            for (int i = 0; i < length; i++)
            {
                var sum = (ulong)a.bits[a.index + i] + (ulong)b.bits[b.index + i] + carry;
                bits[index + i] = (uint)sum;
                carry = sum >> 32;
            }
        }

        public void SetDifference(Radix32Integer a, Radix32Integer b)
        {
            ulong borrow = 0;
            for (int i = 0; i < length; i++)
            {
                var diff = (ulong)a.bits[a.index + i] - (ulong)b.bits[b.index + i] - borrow;
                bits[index + i] = (uint)diff;
                borrow = (ulong)(-(int)(diff >> 32));
            }
        }

        public void SetProduct(Radix32Integer a, Radix32Integer b)
        {
            Clear();
            ulong carry = 0;
            for (int i = 0; i < a.length; i++)
            {
                if (a.bits[a.index + i] == 0)
                    continue;
                for (int j = 0; j < b.length; j++)
                {
                    if (b.bits[b.index + j] == 0)
                        continue;
                    carry = (ulong)a.bits[a.index + i] * (ulong)b.bits[b.index + j];
                    for (int k = i + j; k < length; k++)
                    {
                        var sum = (ulong)bits[index + k] + carry;
                        bits[index + k] = (uint)sum;
                        carry = (sum >> 32);
                        if (carry == 0)
                            break;
                    }
                }
            }
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
    }
}
