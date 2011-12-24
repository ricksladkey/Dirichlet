using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Decompose.Numerics
{
#if false
    public class ModuloInteger
    {
        private BigInteger modulus;
        private Radix32 modulusRep;

        public ModuloInteger(BigInteger n)
        {
            modulus = n;
            modulusRep = Radix32.GetBits(n);
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
            bits = Radix32.GetBits(n % modulus);
            return this;
        }

        public ModuloInteger Add(ModuloInteger n)
        {
            Radix32.Add(bits, 0, bits, 0, n.bits, 0, length);
            if (Radix32.Compare(bits, 0, modulusBits, 0, length) >= 0)
                Radix32.Subtract(bits, 0, bits, 0, modulusBits, 0, length);
            return this;
        }

        public ModuloInteger Mult(ModuloInteger n)
        {
            return this;
        }
    }
#endif
}
