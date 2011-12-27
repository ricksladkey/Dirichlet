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

        public int GetBitLength()
        {
            CheckValid();
            if (last == 0 && bits[index] == 0)
                return 0;
            var b = bits[index + last];
            return last * 32 + bits[index + last].GetBitLength();
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
            last = length - 1;
            SetLast(length - 1);
        }

        public Radix32Integer Clear()
        {
            CheckValid();
            for (int i = 0; i <= last; i++)
                bits[index + i] = 0;
            last = 0;
            CheckValid();
            return this;
        }

        public Radix32Integer Set(BigInteger a)
        {
            CheckValid();
            Debug.Assert(a.GetBitLength() <= 32 * length);
            var nBits = GetBits(a);
            nBits.CopyTo(bits, index);
            for (int i = nBits.Length; i < length; i++)
                bits[index + i] = 0;
            last = Math.Max(nBits.Length - 1, 0);
            CheckValid();
            return this;
        }

        public Radix32Integer Set(Radix32Integer a)
        {
            CheckValid();
            Debug.Assert(length == a.length);
            for (int i = 0; i <= a.last; i++)
                bits[index + i] = a.Bits[a.index + i];
            for (int i = a.last + 1; i <= last; i++)
                bits[index + i] = 0;
            last = a.last;
            CheckValid();
            return this;
        }

        public Radix32Integer SetMasked(Radix32Integer a, int n)
        {
            CheckValid();
            Debug.Assert(length == a.length);
            Debug.Assert(n % 32 == 0 && n > 0);
            int clast = (n + 31) / 32 - 1;
            int alast = Math.Min(a.last, clast);
            for (int i = 0; i <= alast; i++)
                bits[index + i] = a.Bits[a.index + i];
            for (int i = alast + 1; i <= last; i++)
                bits[index + i] = 0;
            SetLast(alast);
            return this;
        }

        public Radix32Integer Copy()
        {
            CheckValid();
            var newBits = new uint[length];
            Array.Copy(bits, index, newBits, 0, last + 1);
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
            CheckValid();
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
            CheckValid();
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
            SetLast(i - 1);
            return this;
        }

        public Radix32Integer RightShift(int n)
        {
            CheckValid();
            Debug.Assert(n % 32 == 0);
            int i = n / 32;
            int j = n - i * 32;
            for (int k = 0; k < length - i; k++)
                bits[index + k] = bits[index + k + i];
            for (int k = length - i; k <= last; k++)
                bits[index + k] = 0;
            last -= i;
            if (last < 0)
                last = 0;
            CheckValid();
            return this;
        }

        public Radix32Integer AddPowerOfTwo(int n)
        {
            CheckValid();
            Debug.Assert(n % 32 == 0);
            int i = n / 32;
            int j = n - i * 32;
            Debug.Assert(bits[index + i] == 0);
            ++bits[index + i];
            last = Math.Max(last, i);
            CheckValid();
            return this;
        }

        public Radix32Integer SetSum(Radix32Integer a, Radix32Integer b)
        {
            CheckValid();
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
            CheckValid();
            return this;
        }

        public Radix32Integer Add(Radix32Integer a)
        {
            return SetSum(this, a);
        }

        public Radix32Integer SetDifference(Radix32Integer a, Radix32Integer b)
        {
            CheckValid();
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
            CheckValid();
            return this;
        }

        public Radix32Integer Subtract(Radix32Integer a)
        {
            return SetDifference(this, a);
        }

        public Radix32Integer SetProduct(Radix32Integer a, Radix32Integer b)
        {
            // Use operand scanning algorithm.
            CheckValid();
            Debug.Assert(a.GetBitLength() + b.GetBitLength() <= length * 32);
            Clear();
            for (int i = 0; i <= a.last; i++)
            {
                ulong avalue = (ulong)a.bits[a.index + i];
                ulong carry = 0;
                for (int j = 0; j <= b.last; j++)
                {
                    carry += (ulong)bits[index + i + j] + avalue * (ulong)b.bits[b.index + j];
                    bits[index + i + j] = (uint)carry;
                    carry >>= 32;
                }
                bits[index + i + b.last + 1] = (uint)carry;
            }
            SetLast(a.last + b.last + 1);
            return this;
        }

        public Radix32Integer SetProduct(uint a, Radix32Integer b)
        {
            // Use operand scanning algorithm.
            CheckValid();
            Debug.Assert(a.GetBitLength() + b.GetBitLength() <= length * 32);
            ulong carry = 0;
            for (int j = 0; j <= b.last; j++)
            {
                carry += a * (ulong)b.bits[b.index + j];
                bits[index + j] = (uint)carry;
                carry >>= 32;
            }
            bits[index + b.last + 1] = (uint)carry;
            for (int j = b.last + 2; j <= last; j++)
                bits[index + j] = 0;
            SetLast(b.last + 1);
            return this;
        }

        public Radix32Integer Multiply(uint a)
        {
            return SetProduct(a, this);
        }

        public Radix32Integer SetProductMasked(Radix32Integer a, Radix32Integer b, int n)
        {
            CheckValid();
            Debug.Assert(n % 32 == 0);
            Clear();
            int clast = (n + 31) / 32 - 1;
            int alast = Math.Min(a.last, clast);
            for (int i = 0; i <= alast; i++)
            {
                int blast = Math.Min(b.last, clast - i);
                ulong carry = 0;
                for (int j = 0; j <= blast; j++)
                {
                    carry += (ulong)bits[index + i + j] + (ulong)a.bits[a.index + i] * (ulong)b.bits[b.index + j];
                    bits[index + i + j] = (uint)carry;
                    carry >>= 32;
                }
                if (i + blast < clast)
                    bits[index + i + blast + 1] = (uint)carry;
            }
            SetLast(clast);
            return this;
        }

        /// <summary>
        /// Evaluate the product of two numbers and discard some of the lower bits.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <remarks>
        /// Note: the result may be less than the result of separate multiplication
        /// and shifting operations by at most one.
        /// </remarks>
        public Radix32Integer SetProductShifted(Radix32Integer a, Radix32Integer b, int n)
        {
            // Use product scanning algorithm.
            CheckValid();
            Debug.Assert(n % 32 == 0 && n > 0);
            int shifted = n / 32;
            Clear();
            ulong r0 = 0;
            ulong r1 = 0;
            ulong r2 = 0;
            ulong eps = 0;
            var clast = a.last + b.last + 1;
            for (int k = shifted - 1; k < clast; k++)
            {
                var min = Math.Max(k - b.last, 0);
                var max = Math.Min(k, a.last);
                for (int i = min; i <= max; i++)
                {
                    int j = k - i;
                    var uv = (ulong)a.bits[a.index + i] * (ulong)b.bits[b.index + j];
                    r0 += (uint)uv;
                    eps = r0 >> 32;
                    r0 &= (1ul << 32) - 1;
                    r1 += (uv >> 32) + eps;
                    eps = r1 >> 32;
                    r1 &= (1ul << 32) - 1;
                    r2 += eps;
                }
                if (k >= shifted)
                    bits[index + k - shifted] = (uint)r0;
                r0 = r1;
                r1 = r2;
                r2 = 0;
            }
            bits[index + clast - shifted] = (uint)r0;
            SetLast(clast - shifted);
            return this;
        }

        public Radix32Integer MontgomeryOperation(Radix32Integer n, uint k0, int rLength)
        {
            CheckValid();
            Debug.Assert(rLength % 32 == 0);
            int s = rLength / 32;
            for (int i = 0; i < s; i++)
            {
                ulong carry = 0;
                ulong m = ((ulong)bits[index + i] * (ulong)k0) & ((1ul << 32) - 1);
                for (int j = 0; j <= n.last; j++)
                {
                    carry += (ulong)bits[index + i + j] + m * n.bits[n.index + j];
                    bits[index + i + j] = (uint)carry;
                    carry >>= 32;
                }
                for (int j = n.last + 1; carry != 0; j++)
                {
                    carry += (ulong)bits[index + i + j];
                    bits[index + i + j] = (uint)carry;
                    carry >>= 32;
                }
            }
            SetLast(2 * s);
            RightShift(rLength);
            return this;
        }

        public Radix32Integer SetSquare(Radix32Integer a)
        {
            // Use operand scanning algorithm.
            CheckValid();
            Debug.Assert(2 * a.GetBitLength() <= length * 32);
            Clear();
            for (int i = 0; i <= a.last; i++)
            {
                ulong avalue = (ulong)a.bits[a.index + i];
                ulong carry = avalue * avalue + (ulong)bits[index + 2 * i];
                bits[index + 2 * i] = (uint)carry;
                carry >>= 32;
                for (int j = i + 1; j <= a.last; j++)
                {
                    ulong value = avalue * (ulong)a.bits[a.index + j];
                    var eps = value >> 63;
                    value <<= 1;
                    carry += value + (ulong)bits[index + i + j];
                    bits[index + i + j] = (uint)carry;
                    carry >>= 32;
                    carry += eps << 32;
                }
                int k = index + i + a.last + 1;
                carry += bits[k];
                bits[k] = (uint)carry;
                carry >>= 32;
                if (carry != 0)
                    bits[k + 1] = (uint)carry;
            }
            SetLast(2 * a.last + 1);
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

        private void SetLast(int n)
        {
            int i = n;
            if (i == length)
                --i;
            while (i > 0 && bits[index + i] == 0)
                --i;
            last = i;
            CheckValid();
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
        private void CheckValid()
        {
            CheckValid(this);
        }

        [Conditional("DEBUG")]
        private static void CheckValid(Radix32Integer x)
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
