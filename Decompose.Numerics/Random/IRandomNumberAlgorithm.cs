using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public interface IRandomNumberAlgorithm
    {
        object SyncRoot { get; }
        uint Next();
        IRandomNumberGenerator<T> CreateInstance<T>();
    }
}
