using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public struct UInt64Division0 : IDivisionAlgorithm<ulong, uint>
    {
        private uint d;

        public UInt64Division0(uint d)
        {
            this.d = d;
        }

        public ulong Divide(ulong k)
        {
            return k / d;
        }

        public uint Modulus(ulong k)
        {
            return (uint)(k % d);
        }

        public bool IsDivisible(ulong k)
        {
            return k % d == 0;
        }
    }

    public struct UInt64Division1 : IDivisionAlgorithm<ulong, uint>
    {
        private uint d;
        private ulong m;
        private int sh1;
        private int sh2;

        public UInt64Division1(uint d)
        {
            this.d = d;
            var l = (int)Math.Ceiling(Math.Log(d, 2));
            m = (ulong)(((((UInt128)1 << l) - d) << 64) / d + 1);
            sh1 = Math.Min(l, 1);
            sh2 = Math.Max(l - 1, 0);
        }

        public ulong Divide(ulong k)
        {
            var t = UInt128.MultiplyHigh(m, k);
            return (((k - t) >> sh1) + t) >> sh2;
        }

        public uint Modulus(ulong k)
        {
            var t = UInt128.MultiplyHigh(m, k);
            return (uint)(k - ((((k - t) >> sh1) + t) >> sh2) * d);
        }

        public bool IsDivisible(ulong k)
        {
            return Modulus(k) == 0;
        }
    }

    public struct UInt64Division3 : IDivisionAlgorithm<ulong, uint>
    {
        private ulong recip;
        private uint d;

        public UInt64Division3(uint d)
        {
            this.d = d;
            recip = (ulong)(((UInt128)1 << 64) / d + 1);
        }

        public ulong Divide(ulong k)
        {
            return UInt128.MultiplyHigh(recip, k);
        }

        public uint Modulus(ulong k)
        {
            return (uint)(k - UInt128.MultiplyHigh(recip, k) * d);
        }

        public bool IsDivisible(ulong k)
        {
            return Modulus(k) == 0;
        }
    }
}
