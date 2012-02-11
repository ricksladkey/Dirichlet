using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompose
{
    public enum TraceFlags
    {
        Path,
    }

    public class Engine
    {
        public static string AttachedKey { get { return "@Attached"; } }
        public static string ContextKey { get { return "@Context"; } }
        public static string AssociatedObjectKey { get { return "@AssociatedObject"; } }
        public object Throw(string message) { throw new Exception(message); return null; }
        public void Trace(TraceFlags flags, string message, params object[] args) { }
    }
}
