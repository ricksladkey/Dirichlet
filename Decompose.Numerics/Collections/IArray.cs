﻿using System.Collections.Generic;

namespace Decompose.Numerics
{
    public interface IArray<T> : IEnumerable<T>
    {
        int Length { get; }
        T this[int index] { get; set; }
    }
}
