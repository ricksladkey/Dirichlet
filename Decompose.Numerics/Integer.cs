using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
    public class Integer
    {
        private int capacity;
        private uint[] bits;

        public Integer()
            : this(0)
        {
        }

        public Integer(int n)
        {
            Capacity = 32;
            bits[0] = (uint)n;
        }

        public static Integer Parse(string s)
        {
            var n = BigInteger.Parse(s);
            var bytes = n.ToByteArray();
            int length = (bytes.Length + 3) / 4;
            if (4 * length != bytes.Length)
                bytes = bytes.Concat(new byte[4 * length - bytes.Length]).ToArray();
            var result = new Integer();
            var bits = result.bits;
            for (int i = 0; i < length; i++)
                bits[i] = (uint)BitConverter.ToInt32(bytes, 4 * i);
            return result;
        }

        public Integer Add(Integer n)
        {
            Capacity = n.Capacity;
            ulong carry = 0;
            int length1 = n.bits.Length;
            for (int i = 0; i < length1; i++)
            {
                var sum = (ulong)bits[i] + (ulong)n.bits[i] + carry;
                bits[i] = (uint)sum;
                carry = sum >> 32;
            }
            int length2 = bits.Length;
            for (int i = length1; i < length2; i++)
            {
                var sum = (ulong)bits[i] + carry;
                bits[i] = (uint)sum;
                carry = sum >> 32;
            }
            Debug.Assert(carry == 0);
            return this;
        }

        public Integer Mult(Integer n)
        {
            Capacity = n.Capacity;
            ulong carry = 0;
            int length1 = n.bits.Length;
            for (int i = 0; i < length1; i++)
            {
                var sum = (ulong)bits[i] + (ulong)n.bits[i] + carry;
                bits[i] = (uint)sum;
                carry = sum >> 32;
            }
            int length2 = bits.Length;
            for (int i = length1; i < length2; i++)
            {
                var sum = (ulong)bits[i] + carry;
                bits[i] = (uint)sum;
                carry = sum >> 32;
            }
            Debug.Assert(carry == 0);
            return this;
        }

        public int Capacity
        {
            get { return capacity; }
            set
            {
                if (capacity < value)
                {
                    var bytes = (value + 31) / 4;
                    var ints = (bytes + 3) / 4;
                    var newBits = new uint[ints];
                    if (bits != null)
                        bits.CopyTo(newBits, 0);
                    bits = newBits;
                    capacity = ints * 4 * 32;
                }
            }
        }
    }
}
