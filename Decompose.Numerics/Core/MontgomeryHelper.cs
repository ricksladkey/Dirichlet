namespace Decompose.Numerics
{
    public class MontgomeryHelper
    {
        public static ulong Reduce(ulong u, ulong v, ulong n, uint k0)
        {
            var u0 = (uint)u;
            var u1 = (uint)(u >> 32);
            var v0 = (uint)v;
            var v1 = (uint)(v >> 32);
            var n0 = (uint)n;
            var n1 = (uint)(n >> 32);

            if (n1 == 0)
                return Reduce(u0, v0, n0, k0);

            var carry = (ulong)u0 * v0;
            var t0 = (uint)carry;
            carry = (carry >> 32) + (ulong)u1 * v0;
            var t1 = (uint)carry;
            var t2 = (uint)(carry >> 32);

            var m = t0 * k0;
            carry = t0 + (ulong)m * n0;
            carry = (carry >> 32) + t1 + (ulong)m * n1;
            t0 = (uint)carry;
            carry = (carry >> 32) + t2;
            t1 = (uint)carry;
            t2 = (uint)(carry >> 32);

            carry = t0 + (ulong)u0 * v1;
            t0 = (uint)carry;
            carry = (carry >> 32) + t1 + (ulong)u1 * v1;
            t1 = (uint)carry;
            carry = (carry >> 32) + t2;
            t2 = (uint)carry;
            var t3 = (uint)(carry >> 32);

            m = t0 * k0;
            carry = t0 + (ulong)m * n0;
            carry = (carry >> 32) + t1 + (ulong)m * n1;
            t0 = (uint)carry;
            carry = (carry >> 32) + t2;
            t1 = (uint)carry;
            t2 = t3 + (uint)(carry >> 32);

            var t = (ulong)t1 << 32 | t0;
            if (t2 != 0 || t >= n)
                t -= n;
            return t;
        }

        public static uint Reduce(uint u0, uint v0, uint n0, uint k0)
        {
#if false
            var carry = (ulong)u0 * v0;
            var t0 = (uint)carry;
            var t1 = (uint)(carry >> 32);

            var m = (ulong)(t0 * k0);
            carry = t0 + m * n0;
            var t = (carry >> 32) + t1;

            if (t >= n0)
                t -= n0;
            return (uint)t;
#endif
#if false
            var uv = (ulong)u0 * v0;
            var mn = (ulong)((uint)uv * k0) * n0;
            var t = (uv >> 32) + (mn >> 32);
            if ((ulong)(uint)uv + (uint)mn >> 32 != 0)
                ++t;
            if (t >= n0)
                t -= n0;
            return (uint)t;
#endif
#if false
            // Only works if n0 <= int.MaxValue.
            var uv = (ulong)u0 * v0;
            var mn = (ulong)((uint)uv * k0) * n0;
            var t = (uv + mn) >> 32;
            if (t >= n0)
                t -= n0;
            return (uint)t;
#endif
#if true
            var uv = (ulong)u0 * v0;
            var mn = (ulong)(0 - (uint)uv * k0) * n0;
            if (uv < mn)
                return (uint)(n0 - ((mn - uv) >> 32));
            return (uint)((uv - mn) >> 32);
#endif
        }
    }
}
