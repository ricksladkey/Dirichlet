using System.Numerics;
using Decompose.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decompose.Tests
{
    [TestClass]
    public class DecomposeTests
    {
        [TestMethod]
        public void ScriptingTests()
        {
            Assert.AreEqual((BigInteger)3, Evaluate("1+2"));
        }

        private object Evaluate(string text)
        {
            var engine = new Engine();
            return new Parser().Compile(engine, CodeType.Script, text).Root.Get(engine);
        }
    }
}
