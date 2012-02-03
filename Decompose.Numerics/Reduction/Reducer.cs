using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Numerics
{
    public abstract class Reducer<TReduction, TValue> : IReducer<TValue>
        where TReduction : IReductionAlgorithm<TValue>
    {
        protected TReduction reduction;
        protected TValue modulus;

        protected Reducer(TReduction reduction, TValue modulus)
        {
            this.reduction = reduction;
            this.modulus = modulus;
        }

        public IReductionAlgorithm<TValue> Reduction
        {
            get { return reduction; }
        }

        public TValue Modulus
        {
            get { return modulus; }
        }

        public abstract IResidue<TValue> ToResidue(TValue x);
    }
}
