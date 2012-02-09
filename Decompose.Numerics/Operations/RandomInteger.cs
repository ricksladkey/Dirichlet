using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public class RandomInteger<T> : IRandomNumberAlgorithm<Integer<T>>
    {
        IRandomNumberAlgorithm<T> random;

        public RandomInteger(uint seed)
        {
            random = new MersenneTwister(seed).Create<T>();
        }

        public Integer<T> Next(Integer<T> n)
        {
            return random.Next(n);
        }

        public IEnumerable<Integer<T>> Sequence(Integer<T> n)
        {
            while (true)
                yield return Next(n);
        }
    }
}
