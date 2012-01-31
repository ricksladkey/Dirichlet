using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public abstract class Random : IRandomNumberAlgorithm
    {
        private abstract class RandomNumberGenerator<T> : IRandomNumberGenerator<T>
        {
            protected IRandomNumberAlgorithm random;

            protected RandomNumberGenerator(IRandomNumberAlgorithm random)
            {
                this.random = random;
            }

            public abstract T Next(T n);

            public IEnumerable<T> Sequence(T n)
            {
                while (true)
                    yield return Next(n);
            }
        }

        private class Int32RandomNumberGenerator : RandomNumberGenerator<int>
        {
            public Int32RandomNumberGenerator(IRandomNumberAlgorithm random)
                : base(random)
            {
            }

            public override int Next(int n)
            {
                lock (random.SyncRoot)
                {
                    var next = (int)(random.Next() >> 1);
                    return n == 0 ? next : next % n;
                }
            }
        }

        private class UInt32RandomNumberGenerator : RandomNumberGenerator<uint>
        {
            public UInt32RandomNumberGenerator(IRandomNumberAlgorithm random)
                : base(random)
            {
            }

            public override UInt32 Next(uint n)
            {
                lock (random.SyncRoot)
                {
                    var next = random.Next();
                    return n == 0 ? next : next % n;
                }
            }
        }

        private class Int64RandomNumberGenerator : RandomNumberGenerator<long>
        {
            public Int64RandomNumberGenerator(IRandomNumberAlgorithm random)
                : base(random)
            {
            }

            public override long Next(long n)
            {
                lock (random.SyncRoot)
                {
                    var next = (long)((ulong)(random.Next() >> 1) << 32 | random.Next());
                    return n == 0 ? next : next % n;
                }
            }
        }

        private class UInt64RandomNumberGenerator : RandomNumberGenerator<ulong>
        {
            public UInt64RandomNumberGenerator(IRandomNumberAlgorithm random)
                : base(random)
            {
            }

            public override ulong Next(ulong n)
            {
                lock (random.SyncRoot)
                {
                    var next = (ulong)random.Next() << 32 | random.Next();
                    return n == 0 ? next : next % n;
                }
            }
        }

        private class BigIntegerRandomNumberGenerator : RandomNumberGenerator<BigInteger>
        {
            public BigIntegerRandomNumberGenerator(IRandomNumberAlgorithm random)
                : base(random)
            {
            }

            public override BigInteger Next(BigInteger n)
            {
                lock (random.SyncRoot)
                {
                    var c = (n.ToByteArray().Length + 3) / 4 * 4;
                    var bytes = new byte[c + 1];
                    for (int i = 0; i < c; i += 4)
                        BitConverter.GetBytes(random.Next()).CopyTo(bytes, i);
                    return new BigInteger(bytes) % n;
                }
            }
        }

        private object syncRoot = new object();

        public object SyncRoot { get { return syncRoot; } }

        public abstract uint Next();

        public IRandomNumberGenerator<T> CreateInstance<T>()
        {
            var type = typeof(T);
            if (type == typeof(int))
                return (IRandomNumberGenerator<T>)new Int32RandomNumberGenerator(this);
            if (type == typeof(uint))
                return (IRandomNumberGenerator<T>)new UInt32RandomNumberGenerator(this);
            if (type == typeof(long))
                return (IRandomNumberGenerator<T>)new Int64RandomNumberGenerator(this);
            if (type == typeof(ulong))
                return (IRandomNumberGenerator<T>)new UInt64RandomNumberGenerator(this);
            if (type == typeof(BigInteger))
                return (IRandomNumberGenerator<T>)new BigIntegerRandomNumberGenerator(this);
            throw new NotImplementedException("type not supported");
        }
    }
}
