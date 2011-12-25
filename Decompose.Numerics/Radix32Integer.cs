using System;
using System.Linq;
using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class Radix32Integer : IComparable<Radix32Integer>
    {
        private uint[] bits;
        private int index;
        private int length;
        private int last;

        public uint[] Bits { get { return bits; } }
        public int Index { get { return index; } }
        public int Length { get { return length; } }
        public int Last { get { return last; } set { last = value; } }

        public int BitLength
        {
            get
            {
                Debug.Assert(last == 0 || bits[index] != 0);
                if (last == 0 && bits[index] == 0)
                    return 0;
                var b = bits[index + last];
                int j = 0;
                while (b > 0)
                {
                    b >>= 1;
                    j++;
                }
                return last * 32 + j;
            }
        }

        private uint[] DebugBits
        {
            get
            {
                var debugBits = new uint[length];
                Array.Copy(bits, index, debugBits, 0, length);
                return debugBits;
            }
        }

        public bool IsZero
        {
            get { return last == 0 && bits[index] == 0; }
        }

        public bool IsOne
        {
            get { return last == 0 && bits[index] == 1; }
        }

        public Radix32Integer(uint[] bits, int index, int length)
        {
            this.bits = bits;
            this.index = index;
            this.length = length;
            last = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                if (bits[index + i] != 0)
                {
                    last = i;
                    break;
                }
            }
            AssertValid();
        }

        public Radix32Integer Clear()
        {
            AssertValid();
            for (int i = 0; i <= last; i++)
                bits[index + i] = 0;
            last = 0;
            AssertValid();
            return this;
        }

        public Radix32Integer Set(BigInteger n)
        {
            AssertValid();
            Debug.Assert(BigIntegerUtils.GetBitLength(n) <= 32 * length);
            var nBits = GetBits(n);
            nBits.CopyTo(bits, index);
            for (int i = nBits.Length; i < length; i++)
                bits[index + i] = 0;
            last = nBits.Length - 1;
            AssertValid();
            return this;
        }

        public Radix32Integer Set(Radix32Integer n)
        {
            AssertValid();
            Debug.Assert(length == n.length);
            for (int i = 0; i <= n.last; i++)
                bits[index + i] = n.Bits[n.index + i];
            for (int i = n.last + 1; i <= last; i++)
                bits[index + i] = 0;
            last = n.last;
            AssertValid();
            return this;
        }

        public Radix32Integer Copy()
        {
            AssertValid();
            var newBits = new uint[length];
            Array.Copy(bits, index, newBits, 0, length);
            return new Radix32Integer(newBits, 0, length);
        }

        public BigInteger ToBigInteger()
        {
            var bytes = new byte[(last + 1) * 4 + 1];
            for (int i = 0; i <= last; i++)
                BitConverter.GetBytes(bits[index + i]).CopyTo(bytes, i * 4);
            return new BigInteger(bytes);
        }

        public override string ToString()
        {
            return ToBigInteger().ToString();
        }

        public int CompareTo(Radix32Integer other)
        {
            AssertValid();
            Debug.Assert(length == other.length);
            var diff = last - other.last;
            if (diff != 0)
                return diff;
            for (int i = last; i >= 0; i--)
            {
                diff = bits[index + i].CompareTo(other.bits[other.index + i]);
                if (diff != 0)
                    return diff;
            }
            return 0;
        }

        public Radix32Integer Mask(int n)
        {
            AssertValid();
            int i = n / 32;
            int j = n - i * 32;
            if (j == 0)
            {
                for (int k = last; k >= i; k--)
                    bits[index + k] = 0;
            }
            else
            {
                for (int k = last; k > i; k--)
                    bits[index + k] = 0;
                bits[index + i] &= (1u << j) - 1;
            }
            last = i - 1;
            while (last > 0 && bits[index + last] == 0)
                --last;
            AssertValid();
            return this;
        }

        public Radix32Integer RightShift(int n)
        {
            AssertValid();
            int i = n / 32;
            int j = n - i * 32;
            if (j == 0)
            {
                for (int k = 0; k < length - i; k++)
                    bits[index + k] = bits[index + k + i];
                for (int k = length - i; k <= last; k++)
                    bits[index + k] = 0;
                last -= i;
                if (last < 0)
                    last = 0;
            }
            else
            {
                throw new InvalidOperationException();
            }
            AssertValid();
            return this;
        }

        public Radix32Integer AddPowerOfTwo(int n)
        {
            AssertValid();
            int i = n / 32;
            int j = n - i * 32;
            ++bits[index + i];
            last = Math.Max(last, i);
            AssertValid();
            return this;
        }

        public Radix32Integer SetSum(Radix32Integer a, Radix32Integer b)
        {
            AssertValid();
            Debug.Assert(length == a.length && length == b.length);
            ulong carry = 0;
            var limit = Math.Max(a.last, b.last);
            for (int i = 0; i <= limit; i++)
            {
                var sum = (ulong)a.bits[a.index + i] + (ulong)b.bits[b.index + i] + carry;
                bits[index + i] = (uint)sum;
                carry = sum >> 32;
            }
            if (carry != 0)
            {
                Debug.Assert(limit + 1 < length);
                ++limit;
                bits[index + limit] = (uint)carry;
            }
            for (int i = limit + 1; i <= last; i++)
                bits[index + i] = 0;
            last = limit;
            AssertValid();
            return this;
        }

        public Radix32Integer Add(Radix32Integer a)
        {
            return SetSum(this, a);
        }

        public Radix32Integer SetDifference(Radix32Integer a, Radix32Integer b)
        {
            AssertValid();
            Debug.Assert(length == a.length && length == b.length);
            Debug.Assert(a.CompareTo(b) >= 0);
            ulong borrow = 0;
            var limit = Math.Max(a.last, b.last);
            for (int i = 0; i <= limit; i++)
            {
                var diff = (ulong)a.bits[a.index + i] - (ulong)b.bits[b.index + i] - borrow;
                bits[index + i] = (uint)diff;
                borrow = (ulong)(-(int)(diff >> 32));
            }
            for (int i = limit + 1; i <= last; i++)
                bits[index + i] = 0;
            while (limit > 0 && bits[index + limit] == 0)
                --limit;
            last = limit;
            AssertValid();
            return this;
        }

        public Radix32Integer Subtract(Radix32Integer a)
        {
            return SetDifference(this, a);
        }

        public Radix32Integer SetProduct(Radix32Integer a, Radix32Integer b)
        {
            AssertValid();
            Debug.Assert(a.BitLength + b.BitLength <= length * 32);
            Clear();
            if (a.IsZero || b.IsZero)
                return this;
            if (a.IsOne)
                return Set(b);
            if (b.IsOne)
                return Set(a);
            ulong carry = 0;
            for (int i = 0; i <= a.last; i++)
            {
                for (int j = 0; j <= b.last; j++)
                {
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
            last = a.last + b.last + 1;
            if (last == length + 1)
                --last;
            if (bits[index + last] == 0)
                --last;
            AssertValid();
            return this;
        }

        public Radix32Integer SetGreatestCommonDivisor(Radix32Integer a, Radix32Integer b)
        {
#if false
            if (a.IsZero)
                Set(b);
            else if (b.IsZero)
                Set(a);
            else
            {
                reg1.Set(a);
                reg2.Set(b);
                while (true)
                {
                    reg1.Modulus(reg2);
                    if (reg1.IsZero)
                    {
                        Set(reg2);
                        break;
                    }
                    reg2.Modulus(reg1);
                    if (reg2.IsZero)
                    {
                        Set(reg1);
                        break;
                    }
                }
            }
#else
            Set(BigInteger.GreatestCommonDivisor(a.ToBigInteger(), b.ToBigInteger()));
#endif
            return this;
        }

        private static uint[] GetBits(BigInteger n)
        {
            var bytes = n.ToByteArray();
            var byteLength = bytes.Length;
            if (bytes[byteLength - 1] == 0)
                --byteLength;
            int length = (byteLength + 3) / 4;
            if (4 * length > bytes.Length)
            {
                var newBytes = new byte[4 * length];
                bytes.CopyTo(newBytes, 0);
                bytes = newBytes;
            }
            var bits = new uint[length];
            for (int i = 0; i < length; i++)
                bits[i] = (uint)BitConverter.ToInt32(bytes, 4 * i);
            return bits;
        }

        [Conditional("DEBUG")]
        private void AssertValid()
        {
            AssertValid(this);
        }

        [Conditional("DEBUG")]
        private static void AssertValid(Radix32Integer x)
        {
            if (x.last == 0 && x.bits[x.index] == 0)
                return;
            if (x.last >= x.length)
                throw new InvalidOperationException("overrun");
            if (x.bits[x.index + x.last] == 0)
                throw new InvalidOperationException("last miscount");
            for (int i = x.last + 1; i < x.length; i++)
                if (x.bits[x.index + i] != 0)
                    throw new InvalidOperationException("not zeroed");
        }
    }
}
