using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public static class Operations
    {
        private static Dictionary<Type, IOperations> operations = new Dictionary<Type, IOperations>
        {
            { typeof(double), new DoubleOperations() },
            { typeof(int), new Int32Operations() },
            { typeof(uint), new UInt32Operations() },
            { typeof(long), new Int64Operations() },
            { typeof(ulong), new UInt64Operations() },
            { typeof(BigInteger), new BigIntegerOperations() },
            { typeof(Complex), new ComplexOperations() },
            { typeof(Rational), new RationalOperations() },
        };

        public static IOperations<T> Get<T>()
        {
            var type = typeof(T);
            IOperations ops;
            if (!operations.TryGetValue(typeof(T), out ops))
                throw new NotImplementedException("type not supported");
            return (IOperations<T>)ops;
        }
    }
}
