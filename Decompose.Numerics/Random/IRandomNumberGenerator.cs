﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public interface IRandomNumberGenerator
    {
        object SyncRoot { get; }
        uint Next();
        IRandomNumberAlgorithm<T> Create<T>();
    }
}
