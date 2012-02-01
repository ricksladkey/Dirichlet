using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public abstract class RandomNumberGenerator : IRandomNumberGenerator
    {
        private abstract class RandomNumberAlgorithm<T> : IRandomNumberAlgorithm<T>
        {
            protected IRandomNumberGenerator random;

            protected RandomNumberAlgorithm(IRandomNumberGenerator random)
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

        private class Int32RandomNumberAlgorithm : RandomNumberAlgorithm<int>
        {
            public Int32RandomNumberAlgorithm(IRandomNumberGenerator random)
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

        private class UInt32RandomNumberAlgorithm : RandomNumberAlgorithm<uint>
        {
            public UInt32RandomNumberAlgorithm(IRandomNumberGenerator random)
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

        private class Int64RandomNumberAlgorithm : RandomNumberAlgorithm<long>
        {
            public Int64RandomNumberAlgorithm(IRandomNumberGenerator random)
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

        private class UInt64RandomNumberAlgorithm : RandomNumberAlgorithm<ulong>
        {
            public UInt64RandomNumberAlgorithm(IRandomNumberGenerator random)
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

        private class BigIntegerRandomNumberAlgorithm : RandomNumberAlgorithm<BigInteger>
        {
            public BigIntegerRandomNumberAlgorithm(IRandomNumberGenerator random)
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

        public IRandomNumberAlgorithm<T> Create<T>()
        {
            var type = typeof(T);
            if (type == typeof(int))
                return (IRandomNumberAlgorithm<T>)new Int32RandomNumberAlgorithm(this);
            if (type == typeof(uint))
                return (IRandomNumberAlgorithm<T>)new UInt32RandomNumberAlgorithm(this);
            if (type == typeof(long))
                return (IRandomNumberAlgorithm<T>)new Int64RandomNumberAlgorithm(this);
            if (type == typeof(ulong))
                return (IRandomNumberAlgorithm<T>)new UInt64RandomNumberAlgorithm(this);
            if (type == typeof(BigInteger))
                return (IRandomNumberAlgorithm<T>)new BigIntegerRandomNumberAlgorithm(this);
            throw new NotImplementedException("type not supported");
        }
    }
}
