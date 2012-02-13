using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose.Scripting
{
    public interface IOperatorMap
    {
        object Operator(Op op, params object[] args);
    }
}
