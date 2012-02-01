using System;
using System.Linq;
using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class Word32Integer : IComparable<Word32Integer>, IEquatable<Word32Integer>
    {
        private const int wordLength = 32;

        private uint[] bits;
        private int index;
        private int length;
        private int last;
        private int sign;

        public uint[] Bits { get { return bits; } }
        public int Index { get { return index; } }
        public int Length { get { return length; } }
        public int Last { get { return last; } }

        public int GetBitLength()
        {
            CheckValid();
            if (last == 0 && bits[index] == 0)
                return 0;
            var b = bits[index + last];
            return 32 * last + bits[index + last].GetBitLength();
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

        public int Sign
        {
            get { return last == 0 && bits[index] == 0 ? 0 : sign; }
        }

        public bool IsZero
        {
            get { return last == 0 && bits[index] == 0; }
        }

        public bool IsOne
        {
            get { return sign == 1 && last == 0 && bits[index] == 1; }
        }

        public bool IsEven
        {
            get { return (bits[index] & 1) == 0; }
        }

        public int LengthInWords
        {
            get { return last + 1; }
        }

        public static int WordLength
        {
            get { return wordLength; }
        }

        public Word32Integer(int length)
            : this(new uint[length], 0, length)
        {
        }

        public Word32Integer(uint[] bits, int index, int length)
        {
            this.bits = bits;
            this.index = index;
            this.length = length;
            this.sign = 1;
            SetLast(length - 1);
        }

        public unsafe Word32Integer Clear()
        {
            CheckValid();
            fixed (uint* wbits = &bits[index])
            {
                int wlast = last;
                for (int i = 0; i <= wlast; i++)
                    wbits[i] = 0;
            }
            last = 0;
            CheckValid();
            return this;
        }

        public Word32Integer Set(uint a)
        {
            CheckValid();
            bits[index] = a;
            for (int i = 1; i <= last; i++)
                bits[index + i] = 0;
            last = 0;
            sign = 1;
            CheckValid();
            return this;
        }

        public Word32Integer Set(ulong a)
        {
            CheckValid();
            bits[index] = (uint)a;
            bits[index + 1] = (uint)(a >> 32);
            for (int i = 2; i <= last; i++)
                bits[index + i] = 0;
            last = bits[index + 1] != 0 ? 1 : 0;
            sign = 1;
            CheckValid();
            return this;
        }

        public Word32Integer Set(BigInteger a)
        {
            CheckValid();
            Debug.Assert(a.GetBitLength() <= 32 * length);
            var nBits = GetBits(a);
            nBits.CopyTo(bits, index);
            for (int i = nBits.Length; i <= last; i++)
                bits[index + i] = 0;
            last = Math.Max(nBits.Length - 1, 0);
            sign = 1;
            CheckValid();
            return this;
        }

        public unsafe Word32Integer Set(Word32Integer a)
        {
            CheckValid();
            Debug.Assert(length == a.length);
            Debug.Assert(!object.ReferenceEquals(this, a));
            fixed (uint* wbits = &bits[index], abits = &a.bits[a.index])
            {
                int alast = a.last;
                int wlast = last;
                for (int i = 0; i <= alast; i++)
                    wbits[i] = abits[i];
                for (int i = alast + 1; i <= wlast; i++)
                    wbits[i] = 0;
                last = alast;
                sign = a.sign;
            }
            CheckValid();
            return this;
        }

        public Word32Integer SetMasked(Word32Integer a, int n)
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
            return SetLast(alast);
        }

        public Word32Integer Copy()
        {
            CheckValid();
            var newBits = new uint[length];
            Array.Copy(bits, index, newBits, 0, last + 1);
            return new Word32Integer(newBits, 0, length);
        }

        public uint ToUInt32()
        {
            CheckValid();
            Debug.Assert(last == 0);
            return bits[index];
        }

        public ulong ToUInt64()
        {
            CheckValid();
            Debug.Assert(last < 2);
            return last == 0 ? bits[index] : (ulong)bits[index + 1] << 32 | bits[index];
        }

        public BigInteger ToBigInteger()
        {
            CheckValid();
            var bytes = new byte[(last + 1) * 4 + 1];
            for (int i = 0; i <= last; i++)
                BitConverter.GetBytes(bits[index + i]).CopyTo(bytes, i * 4);
            return new BigInteger(bytes);
        }

        public override string ToString()
        {
            return ToBigInteger().ToString();
        }

        public bool Equals(Word32Integer other)
        {
            if ((object)other == null)
                return false;
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Word32Integer))
                return false;
            return Equals((Word32Integer)obj);
        }

        public static bool operator ==(Word32Integer a, Word32Integer b)
        {
            if ((object)a == (object)b)
                return true;
            if ((object)a == null || (object)b == null)
                return false;
            return a.CompareTo(b) == 0;
        }

        public static bool operator ==(Word32Integer a, uint b)
        {
            if ((object)a == null)
                return false;
            return a.CompareTo(b) == 0;
        }

        public static bool operator ==(Word32Integer a, ulong b)
        {
            if ((object)a == null)
                return false;
            return a.CompareTo(b) == 0;
        }

        public static bool operator ==(uint a, Word32Integer b)
        {
            if ((object)b == null)
                return false;
            return b.CompareTo(a) == 0;
        }

        public static bool operator ==(ulong a, Word32Integer b)
        {
            if ((object)b == null)
                return false;
            return b.CompareTo(a) == 0;
        }

        public static bool operator !=(Word32Integer a, Word32Integer b)
        {
            return !(a == b);
        }

        public static bool operator !=(Word32Integer a, uint b)
        {
            return !(a == b);
        }

        public static bool operator !=(Word32Integer a, ulong b)
        {
            return !(a == b);
        }

        public static bool operator !=(uint a, Word32Integer b)
        {
            return !(a == b);
        }

        public static bool operator !=(ulong a, Word32Integer b)
        {
            return !(a == b);
        }

        public static bool operator <(Word32Integer a, Word32Integer b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(Word32Integer a, uint b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(Word32Integer a, ulong b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(uint a, Word32Integer b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <(ulong a, Word32Integer b)
        {
            return b.CompareTo(a) > 0;
        }

        public static bool operator <=(Word32Integer a, Word32Integer b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(Word32Integer a, uint b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(Word32Integer a, ulong b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator <=(uint a, Word32Integer b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator <=(ulong a, Word32Integer b)
        {
            return b.CompareTo(a) >= 0;
        }

        public static bool operator >(Word32Integer a, Word32Integer b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(Word32Integer a, uint b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(Word32Integer a, ulong b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >(uint a, Word32Integer b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >(ulong a, Word32Integer b)
        {
            return b.CompareTo(a) < 0;
        }

        public static bool operator >=(Word32Integer a, Word32Integer b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(Word32Integer a, uint b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(Word32Integer a, ulong b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(uint a, Word32Integer b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static bool operator >=(ulong a, Word32Integer b)
        {
            return b.CompareTo(a) <= 0;
        }

        public static explicit operator uint(Word32Integer a)
        {
            return a.ToUInt32();
        }

        public static explicit operator ulong(Word32Integer a)
        {
            return a.ToUInt64();
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i <= last; i++)
                hash ^= (int)bits[index + i];
            return hash;
        }

        public unsafe int CompareTo(Word32Integer other)
        {
            CheckValid();
            Debug.Assert(length == other.length);
            if (sign != other.sign)
                return sign < other.sign ? -1 : 1;
            var diff = last - other.last;
            if (diff != 0)
                return diff;
            fixed (uint* wbits = &bits[index], obits = &other.bits[other.index])
            {
                for (int i = last; i >= 0; i--)
                {
                    uint wi = wbits[i];
                    uint oi = obits[i];
                    if (wi < oi)
                        return -sign;
                    if (wi > oi)
                        return sign;
                }
            }
            return 0;
        }

        public unsafe int UnsignedCompareTo(Word32Integer other)
        {
            CheckValid();
            Debug.Assert(length == other.length);
            var diff = last - other.last;
            if (diff != 0)
                return diff;
            fixed (uint* wbits = &bits[index], obits = &other.bits[other.index])
            {
                for (int i = last; i >= 0; i--)
                {
                    uint wi = wbits[i];
                    uint oi = obits[i];
                    if (wi < oi)
                        return -1;
                    if (wi > oi)
                        return 1;
                }
            }
            return 0;
        }

        public int CompareTo(uint other)
        {
            CheckValid();
            if (last > 0)
                return 1;
            return bits[index].CompareTo(other);
        }

        public int CompareTo(ulong other)
        {
            CheckValid();
            if (last > 1)
                return 1;
            return ((ulong)bits[index + 1] << 32 | bits[index]).CompareTo(other);
        }

        public unsafe Word32Integer Mask(int n)
        {
            CheckValid();
            int i = n / 32;
            int j = n - 32 * i;
            fixed (uint* wbits = &bits[index])
            {
                if (j == 0)
                {
                    for (int k = last; k >= i; k--)
                        wbits[k] = 0;
                    if (i > 0)
                        --i;
                }
                else
                {
                    for (int k = last; k > i; k--)
                        wbits[k] = 0;
                    wbits[i] &= (1u << j) - 1;
                }
                while (i > 0 && wbits[i] == 0)
                    --i;
                last = i;
            }
            CheckValid();
            return this;
        }

        public Word32Integer LeftShift(int n)
        {
            CheckValid();
            Debug.Assert(GetBitLength() + n <= 32 * length);
            if (n == 0)
                return this;
            int i = n / 32;
            int j = n - 32 * i;
            if (j == 0)
            {
                for (int k = last; k >= 0; k--)
                    bits[index + k + i] = bits[index + k];
                for (int k = 0; k < i; k++)
                    bits[index + k] = 0;
                last += i;
                CheckValid();
                return this;
            }
            else
            {
                int jneg = 32 - j;
                bits[index + last + i + 1] = bits[index + last] >> jneg;
                for (int k = last - 1; k >= 0; k--)
                    bits[index + k + i + 1] = bits[index + k + 1] << j | bits[index + k] >> jneg;
                bits[index + i] = bits[index] << j;
                for (int k = 0; k < i; k++)
                    bits[index + k] = 0;
                return SetLast(last + i + 1);
            }
        }

        public Word32Integer RightShift(int n)
        {
            CheckValid();
            int i = n / 32;
            if (i > last)
            {
                Clear();
                return this;
            }
            int j = n - 32 * i;
            int limit = last - i;
            if (j == 0)
            {
                for (int k = 0; k <= limit; k++)
                    bits[index + k] = bits[index + k + i];
                for (int k = limit + 1; k <= last; k++)
                    bits[index + k] = 0;
            }
            else
            {
                int jneg = 32 - j;
                for (int k = 0; k < limit; k++)
                    bits[index + k] = bits[index + i + k + 1] << jneg | bits[index + i + k] >> j;
                bits[index + limit] = bits[index + i + limit] >> j;
                for (int k = limit + 1; k <= last; k++)
                    bits[index + k] = 0;
            }
            return SetLast(limit);
        }

        public Word32Integer AddPowerOfTwo(int n)
        {
            CheckValid();
            Debug.Assert(n % 32 == 0);
            int i = n / 32;
            int j = n - 32 * i;
            Debug.Assert(bits[index + i] == 0);
            ++bits[index + i];
            last = Math.Max(last, i);
            CheckValid();
            return this;
        }

        public Word32Integer Add(Word32Integer a)
        {
            return SetSum(this, a);
        }

        public Word32Integer SetSum(Word32Integer a, Word32Integer b)
        {
            SetSignedSum(a, b, false);
            return this;
        }

        private unsafe void SetUnsignedSum(Word32Integer a, Word32Integer b)
        {
            CheckValid();
            Debug.Assert(length == a.length && length == b.length);
            ulong carry = 0;
            int limit = Math.Max(a.last, b.last);
            int wlast = last;
            fixed (uint* wbits = &bits[index], abits = &a.bits[a.index], bbits = &b.bits[b.index])
            {
                for (int i = 0; i <= limit; i++)
                {
                    carry += (ulong)abits[i] + bbits[i];
                    wbits[i] = (uint)carry;
                    carry >>= 32;
                }
                if (carry != 0)
                {
                    Debug.Assert(limit + 1 < length);
                    ++limit;
                    wbits[limit] = (uint)carry;
                }
                for (int i = limit + 1; i <= wlast; i++)
                    wbits[i] = 0;
                last = limit;
            }
            CheckValid();
        }

        public Word32Integer Increment()
        {
            return SetSum(this, 1);
        }

        public Word32Integer Add(uint a)
        {
            return SetSum(this, a);
        }

        public unsafe Word32Integer SetSum(Word32Integer a, uint b)
        {
            CheckValid();
            Debug.Assert(length == a.length);
            fixed (uint* wbits = &bits[index], abits = &a.bits[a.index])
            {
                int alast = a.last;
                int wlast = last;
                ulong carry = (ulong)abits[0] + b;
                wbits[0] = (uint)carry;
                carry >>= 32;
                for (int i = 1; i <= alast; i++)
                {
                    carry += abits[i];
                    wbits[i] = (uint)carry;
                    carry >>= 32;
                }
                if (carry != 0)
                {
                    Debug.Assert(alast + 1 < length);
                    ++alast;
                    wbits[alast] = (uint)carry;
                }
                for (int i = alast + 1; i <= wlast; i++)
                    wbits[i] = 0;
                last = alast;
            }
            CheckValid();
            return this;
        }

        public unsafe Word32Integer AddModulo(Word32Integer a, Word32Integer n)
        {
            CheckValid();
            Debug.Assert(length == a.length && length == n.length);
            ulong carry = 0;
            int limit = Math.Max(last, a.last);
            fixed (uint* wbits = &bits[index], abits = &a.bits[a.index], nbits = &n.bits[n.index])
            {
                for (int i = 0; i <= limit; i++)
                {
                    carry += (ulong)wbits[i] + abits[i];
                    wbits[i] = (uint)carry;
                    carry >>= 32;
                }
                if (carry != 0)
                {
                    Debug.Assert(limit + 1 < length);
                    ++limit;
                    wbits[limit] = (uint)carry;
                }
                last = limit;
            }
            if (CompareTo(n) >= 0)
                SetDifference(this, n);
            CheckValid();
            return this;
        }

        public Word32Integer Subtract(Word32Integer a)
        {
            return SetDifference(this, a);
        }

        public Word32Integer SetDifference(Word32Integer a, Word32Integer b)
        {
            SetSignedSum(a, b, true);
            return this;
        }

        private unsafe void SetUnsignedDifference(Word32Integer a, Word32Integer b)
        {
            CheckValid();
            Debug.Assert(length == a.length && length == b.length);
            fixed (uint* wbits = &bits[index], abits = &a.bits[a.index], bbits = &b.bits[b.index])
            {
                ulong borrow = 0;
                var limit = Math.Max(a.last, b.last);
                for (int i = 0; i <= limit; i++)
                {
                    borrow += (ulong)abits[i] - bbits[i];
                    wbits[i] = (uint)borrow;
                    borrow = (ulong)((long)borrow >> 32);
                }
                for (int i = limit + 1; i <= last; i++)
                    wbits[i] = 0;
                while (limit > 0 && wbits[limit] == 0)
                    --limit;
                last = limit;
            }
            CheckValid();
        }

        public unsafe Word32Integer SubtractModulo(Word32Integer a, Word32Integer n)
        {
            CheckValid();
            Debug.Assert(length == a.length && length == n.length);
            if (CompareTo(a) < 0)
                SetSum(this, n);
            fixed (uint* wbits = &bits[index], abits = &a.bits[a.index], nbits = &n.bits[n.index])
            {
                ulong borrow = 0;
                var limit = Math.Max(last, a.last);
                for (int i = 0; i <= limit; i++)
                {
                    borrow += (ulong)wbits[i] - abits[i];
                    wbits[i] = (uint)borrow;
                    borrow = (ulong)((long)borrow >> 32);
                }
                for (int i = limit + 1; i <= last; i++)
                    wbits[i] = 0;
                while (limit > 0 && wbits[limit] == 0)
                    --limit;
                last = limit;
            }
            CheckValid();
            return this;
        }

        public void SetSignedSum(Word32Integer a, Word32Integer b, bool subtraction)
        {
            var asign = a.sign;
            var bsign = subtraction ? -b.sign : b.sign;
            if (asign == bsign)
            {
                SetUnsignedSum(a, b);
                sign = asign;
            }
            else
            {
                if (a.CompareTo(b) < 0)
                {
                    SetUnsignedDifference(b, a);
                    sign = -asign;
                }
                else
                {
                    SetUnsignedDifference(a, b);
                    sign = asign;
                }
            }
        }

        public Word32Integer Multiply(Word32Integer a, Word32Integer reg1)
        {
            reg1.Set(this);
            if (object.ReferenceEquals(this, a))
                return SetSquare(reg1);
            return SetProduct(reg1, a);
        }

        public Word32Integer SetSquare(Word32Integer a)
        {
            return SetProduct(a, a);
        }

        public unsafe Word32Integer SetSquareSlow(Word32Integer a)
        {
            // Use operand scanning algorithm.
            CheckValid();
            Debug.Assert(2 * a.GetBitLength() <= 32 * length);
            fixed (uint* wbits = &bits[index], abits = &a.bits[a.index])
            {
                for (int i = 0; i <= last; i++)
                    wbits[i] = 0;
                int alast = a.last;
                for (int i = 0; i <= alast; i++)
                {
                    ulong avalue = abits[i];
                    ulong carry = avalue * avalue + wbits[2 * i];
                    wbits[2 * i] = (uint)carry;
                    carry >>= 32;
                    for (int j = i + 1; j <= alast; j++)
                    {
                        ulong value = avalue * abits[j];
                        ulong eps = value >> 63;
                        value <<= 1;
                        carry += value + wbits[i + j];
                        wbits[i + j] = (uint)carry;
                        carry >>= 32;
                        carry += eps << 32;
                    }
                    int k = i + alast + 1;
                    carry += wbits[k];
                    wbits[k] = (uint)carry;
                    carry >>= 32;
                    wbits[k + 1] = (uint)carry;
                }
                int limit = 2 * alast + 1;
                while (limit > 0 && wbits[limit] == 0)
                    --limit;
                last = limit;
            }
            CheckValid();
            return this;
        }

        public unsafe Word32Integer SetProduct(Word32Integer a, Word32Integer b)
        {
            // Use operand scanning algorithm.
            CheckValid();
            Debug.Assert(a.GetBitLength() + b.GetBitLength() <= 32 * length);
            Debug.Assert(!object.ReferenceEquals(this, a) && !object.ReferenceEquals(this, b));

            fixed (uint* wbits = &bits[index], abits = &a.bits[a.index], bbits = &b.bits[b.index])
            {
                int wlast = last;
                int alast = a.last;
                int blast = b.last;
                ulong ai = abits[0];
                ulong carry = 0;
                for (int j = 0; j <= blast; j++)
                {
                    carry += ai * bbits[j];
                    wbits[j] = (uint)carry;
                    carry >>= 32;
                }
                wbits[blast + 1] = (uint)carry;
                for (int i = 1; i <= alast; i++)
                {
                    ai = abits[i];
                    carry = 0;
                    for (int j = 0; j <= blast; j++)
                    {
                        carry += (ulong)wbits[i + j] + ai * bbits[j];
                        wbits[i + j] = (uint)carry;
                        carry >>= 32;
                    }
                    wbits[i + blast + 1] = (uint)carry;
                }
                wlast = alast + blast + 1;
                for (int i = wlast + 1; i <= last; i++)
                    wbits[i] = 0;
                while (wlast > 0 && wbits[wlast] == 0)
                    --wlast;
                last = wlast;
            }

            CheckValid();
            return this;
        }

        public Word32Integer Multiply(uint a)
        {
            return SetProduct(this, a);
        }

        public Word32Integer SetProduct(Word32Integer a, uint b)
        {
            // Use operand scanning algorithm.
            CheckValid();
            Debug.Assert(a.GetBitLength() + b.GetBitLength() <= 32 * length);
            ulong carry = 0;
            for (int j = 0; j <= a.last; j++)
            {
                carry += (ulong)b * a.bits[a.index + j];
                bits[index + j] = (uint)carry;
                carry >>= 32;
            }
            bits[index + a.last + 1] = (uint)carry;
            for (int j = a.last + 2; j <= last; j++)
                bits[index + j] = 0;
            return SetLast(a.last + 1);
        }

        public Word32Integer SetProductMasked(Word32Integer a, Word32Integer b, int n)
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
                    carry += (ulong)bits[index + i + j] + (ulong)a.bits[a.index + i] * b.bits[b.index + j];
                    bits[index + i + j] = (uint)carry;
                    carry >>= 32;
                }
                if (i + blast < clast)
                    bits[index + i + blast + 1] = (uint)carry;
            }
            return SetLast(clast);
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
        public Word32Integer SetProductShifted(Word32Integer a, Word32Integer b, int n)
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
                    ulong uv = (ulong)a.bits[a.index + i] * b.bits[b.index + j];
                    r0 += (uint)uv;
                    eps = r0 >> 32;
                    r0 = (uint)r0;
                    r1 += (uv >> 32) + eps;
                    eps = r1 >> 32;
                    r1 = (uint)r1;
                    r2 += eps;
                }
                if (k >= shifted)
                    bits[index + k - shifted] = (uint)r0;
                r0 = r1;
                r1 = r2;
                r2 = 0;
            }
            bits[index + clast - shifted] = (uint)r0;
            return SetLast(clast - shifted);
        }

        public Word32Integer Divide(Word32Integer a, Word32Integer reg1)
        {
            reg1.Set(this);
            DivMod(reg1, a, this);
            return this;
        }

        public Word32Integer Modulo(Word32Integer a)
        {
            DivMod(this, a, null);
            return this;
        }

        public Word32Integer SetQuotient(Word32Integer a, Word32Integer b, Word32Integer reg1)
        {
            reg1.Set(a);
            DivMod(reg1, b, this);
            return this;
        }

        public Word32Integer SetQuotientWithRemainder(Word32Integer a, Word32Integer b, Word32Integer remainder)
        {
            remainder.Set(a);
            DivMod(remainder, b, this);
            return this;
        }

        public Word32Integer SetRemainder(Word32Integer a, Word32Integer b)
        {
            if (!object.ReferenceEquals(this, a))
                Set(a);
            DivMod(this, b, null);
            return this;
        }

        private static unsafe void DivMod(Word32Integer u, Word32Integer v, Word32Integer q)
        {
            if (u.CompareTo(v) < 0)
            {
                if (q != null)
                    q.Clear();
                return;
            }
            if (v.IsZero)
                throw new DivideByZeroException();
            int n = v.last + 1;
            if (n == 1)
            {
                DivMod(u, v.bits[v.index], q);
                return;
            }
            int dneg = v.bits[v.index + v.last].GetBitLength();
            int d = 32 - dneg;
            int m = u.last + 1 - n;
            fixed (uint* ubits = &u.bits[u.index], vbits = &v.bits[v.index])
            {
                uint v1 = vbits[v.last];
                uint v2 = vbits[v.last - 1];
                if (d != 0)
                {
                    uint v3 = n >= 2 ? vbits[v.last - 2] : 0;
                    v1 = v1 << d | v2 >> dneg;
                    v2 = v2 << d | v3 >> dneg;
                }
                for (int j = 0; j <= m; j++)
                {
                    int left = n + m - j;
                    uint u0 = ubits[left];
                    uint u1 = ubits[left - 1];
                    uint u2 = ubits[left - 2];
                    if (d != 0)
                    {
                        uint u3 = j < m ? ubits[left - 3] : 0;
                        u0 = u0 << d | u1 >> dneg;
                        u1 = u1 << d | u2 >> dneg;
                        u2 = u2 << d | u3 >> dneg;
                    }
                    ulong u0u1 = (ulong)u0 << 32 | u1;
                    ulong qhat = u0 == v1 ? (1ul << 32) - 1 : u0u1 / v1;
                    ulong r = u0u1 - qhat * v1;
                    if (r == (uint)r && v2 * qhat > (r << 32 | u2))
                    {
                        --qhat;
                        r = u0u1 - qhat * v1;
                        if (r == (uint)r && v2 * qhat > (r << 32 | u2))
                            --qhat;
                    }
                    ulong carry = 0;
                    ulong borrow = 0;
                    for (int i = 0; i < n; i++)
                    {
                        carry += qhat * vbits[i];
                        borrow += (ulong)ubits[left - n + i] - (uint)carry;
                        carry >>= 32;
                        ubits[left - n + i] = (uint)borrow;
                        borrow = (ulong)((long)borrow >> 32);
                    }
                    borrow += ubits[left] - carry;
                    ubits[left] = 0;

                    if (borrow != 0)
                    {
                        --qhat;
                        carry = 0;
                        for (int i = 0; i < n; i++)
                        {
                            carry += (ulong)ubits[left - n + i] + vbits[i];
                            ubits[left - n + i] = (uint)carry;
                            carry >>= 32;
                        }
                    }
                    if (q != null)
                        q.bits[q.index + m - j] = (uint)qhat;
                }
            }
            if (q != null)
            {
                for (int i = m + 1; i <= q.last; i++)
                    q.bits[q.index + i] = 0;
                q.SetLast(m);
            }
            u.SetLast(n - 1);
        }

        public Word32Integer Divide(uint a, Word32Integer reg1)
        {
            reg1.Set(this);
            DivMod(reg1, a, this);
            return this;
        }

        public Word32Integer Modulo(uint a)
        {
            DivMod(this, a, null);
            return this;
        }

        public Word32Integer SetQuotient(Word32Integer a, uint b, Word32Integer reg1)
        {
            reg1.Set(a);
            DivMod(reg1, b, this);
            return this;
        }

        public Word32Integer SetQuotientWithRemainder(Word32Integer a, uint b, Word32Integer remainder)
        {
            remainder.Set(a);
            DivMod(remainder, b, this);
            return this;
        }

        public Word32Integer SetRemainder(Word32Integer a, uint b)
        {
            return Set(a.GetRemainder(b));
        }

        public unsafe uint GetRemainder(uint v)
        {
            fixed (uint* wbits = &bits[index])
            {
                if (v == 0)
                    throw new DivideByZeroException();
                var u0 = (ulong)(wbits[last] % v);
                for (int j = last - 1; j >= 0; j--)
                    u0 = (u0 << 32 | wbits[j]) % v;
                Debug.Assert(ToBigInteger() % v == u0);
                return (uint)u0;
            }
        }

        private static unsafe void DivMod(Word32Integer u, uint v, Word32Integer q)
        {
            if (q == null)
            {
                var result = u.GetRemainder(v);
                u.Clear();
                u.bits[u.index + 0] = result;
                return;
            }
            if (v == 0)
                throw new DivideByZeroException();
            int m = u.last;
            fixed (uint* ubits = &u.bits[u.index])
            {
                for (int j = 0; j <= m; j++)
                {
                    int left = 1 + m - j;
                    uint u0 = ubits[left];
                    uint u1 = ubits[left - 1];
                    ulong u0u1 = (ulong)u0 << 32 | u1;
                    ulong qhat = u0 == v ? (1ul << 32) - 1 : u0u1 / v;
                    ubits[left - 1] = (uint)(u0u1 - qhat * v);
                    ubits[left] = 0;
                    q.bits[q.index + m - j] = (uint)qhat;
                }
            }
            for (int i = m + 1; i <= q.last; i++)
                q.bits[q.index + i] = 0;
            q.SetLast(m);
            u.last = 0;
        }

        public Word32Integer SetGreatestCommonDivisor(Word32Integer a, Word32Integer b, Word32Integer reg1)
        {
            if (a.IsZero)
                Set(b);
            else if (b.IsZero)
                Set(a);
            else
            {
                reg1.Set(a);
                Set(b);
                while (true)
                {
                    DivMod(reg1, this, null);
                    if (reg1.IsZero)
                        break;
                    DivMod(this, reg1, null);
                    if (IsZero)
                    {
                        Set(reg1);
                        break;
                    }
                }
            }
            return this;
        }

        public Word32Integer SetModularInverse(Word32Integer a, Word32Integer b, Word32Integer reg1, Word32Integer reg2, Word32Integer reg3, Word32Integer reg4, Word32Integer reg5, Word32Integer reg6)
        {
            var p = reg1.Set(a);
            var q = reg2.Set(b);
            var x0 = reg3.Set(0);
            var s0 = 1;
            var x1 = this.Set(1);
            var s1 = 1;
            var quotient = reg4;
            var remainder = reg5;
            var product = reg6;
            while (!q.IsZero)
            {
                DivMod(remainder.Set(p), q, quotient);
                var tmpp = p;
                p = q;
                q = tmpp.Set(remainder);
                var tmpx = x0;
                x0 = x1.Subtract(product.SetProduct(quotient, x0));
                x1 = tmpx;
            }
            if (!object.ReferenceEquals(x1, this))
                this.Set(x1);
            if (sign == -1)
                Add(b);
            return this;
        }

        public unsafe Word32Integer BarrettReduction(Word32Integer z, Word32Integer mu, int k)
        {
            // Use product scanning algorithm.
            CheckValid();
            Clear();
            fixed (uint* wbits = &bits[index], abits = &z.bits[z.index], mubits = &mu.bits[mu.index])
            {
                ulong r0 = 0;
                ulong r1 = 0;
                ulong r2 = 0;
                ulong eps = 0;
                var clast = z.last + mu.last + 1;
                uint* zbits = wbits + k - 1;
                for (int ij = k - 2; ij < clast; ij++)
                {
                    var min = Math.Max(ij - mu.last, 0);
                    var max = Math.Min(ij, z.last - (k - 1));
                    for (int i = min; i <= max; i++)
                    {
                        int j = ij - i;
                        ulong uv = (ulong)zbits[i] * mubits[j];
                        r0 += (uint)uv;
                        eps = r0 >> 32;
                        r0 = (uint)r0;
                        r1 += (uv >> 32) + eps;
                        eps = r1 >> 32;
                        r1 = (uint)r1;
                        r2 += eps;
                    }
                    if (ij >= k - 1)
                        wbits[ij - k - 1] = (uint)r0;
                    r0 = r1;
                    r1 = r2;
                    r2 = 0;
                }
                wbits[clast - (k - 1)] = (uint)r0;
                return SetLast(clast - (k - 1));
            }
        }

        public unsafe Word32Integer MontgomerySOS(Word32Integer n, uint k0)
        {
            // SOS Method - Separated Operand Scanning
            CheckValid();
            int s = n.last + 1;
            fixed (uint* wbits = &bits[index], nbits = &n.bits[n.index])
            {
                for (int i = 0; i < s; i++)
                {
                    ulong carry = 0;
                    ulong m = wbits[i] * k0;
                    for (int j = 0; j < s; j++)
                    {
                        carry += (ulong)wbits[i + j] + m * nbits[j];
                        wbits[i + j] = (uint)carry;
                        carry >>= 32;
                    }
                    for (int j = s; carry != 0; j++)
                    {
                        carry += wbits[i + j];
                        wbits[i + j] = (uint)carry;
                        carry >>= 32;
                    }
                }
                for (int i = 0; i <= s; i++)
                {
                    wbits[i] = wbits[i + s];
                    wbits[i + s] = 0;
                }
                while (s > 0 && wbits[s] == 0)
                    --s;
                last = s;
            }
            return this;
        }

        public unsafe Word32Integer MontgomeryCIOS(Word32Integer u, Word32Integer v, Word32Integer n, uint k0)
        {
            // CIOS Method - Coarsely Integrated Operand Scanning
            CheckValid();
            int s = n.last + 1;
            fixed (uint* wbits = &bits[index], ubits = &u.bits[u.index], vbits = &v.bits[v.index], nbits = &n.bits[n.index])
            {
                int wlast = last;
                for (int i = 0; i <= wlast; i++)
                    wbits[i] = 0;
                for (int i = 0; i < s; i++)
                {
                    ulong carry = 0;
                    ulong ui = ubits[i];
                    for (int j = 0; j < s; j++)
                    {
                        carry += (ulong)wbits[j] + ui * vbits[j];
                        wbits[j] = (uint)carry;
                        carry >>= 32;
                    }
                    carry += wbits[s];
                    wbits[s] = (uint)carry;
                    wbits[s + 1] = (uint)(carry >> 32);
                    ulong m = bits[index] * k0;
                    carry = bits[index] + m * nbits[0];
                    carry >>= 32;
                    for (int j = 1; j < s; j++)
                    {
                        carry += (ulong)wbits[j] + m * nbits[j];
                        wbits[j - 1] = (uint)carry;
                        carry >>= 32;
                    }
                    carry += wbits[s];
                    wbits[s - 1] = (uint)carry;
                    carry >>= 32;
                    wbits[s] = wbits[s + 1] + (uint)carry;
                }
                wbits[s + 1] = 0;
                while (s > 0 && wbits[s] == 0)
                    --s;
                last = s;
            }
            return this;
        }

        private unsafe Word32Integer SetLast(int n)
        {
            if (n < 0)
                last = 0;
            else
            {
                int i = n;
                if (i == length)
                    --i;
                fixed (uint* wbits = &bits[index])
                {
                    while (i > 0 && wbits[i] == 0)
                        --i;
                }
                last = i;
            }
            CheckValid();
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
        private void CheckValid()
        {
            CheckValid(this);
        }

        [Conditional("DEBUG")]
        private static void CheckValid(Word32Integer x)
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
