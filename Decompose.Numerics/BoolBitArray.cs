using System.Collections.Generic;

namespace Decompose.Numerics
{
    public class BoolBitArray : List<bool>, IBitArray
    {
        public int Length
        {
            get { return Count; }
            set
            {
                Clear();
                for (int i = 0; i < value; i++)
                    Add(false);
            }
        }
    }
}
