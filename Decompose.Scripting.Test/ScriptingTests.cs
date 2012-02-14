using System.Numerics;
using Decompose.Numerics;
using Decompose.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decompose.Scripting.Tests
{
    [TestClass]
    public class DecomposeTests
    {
        [TestMethod]
        public void ScriptingTests()
        {
            Assert.AreEqual((BigInteger)3, Evaluate("1+2"));
            Assert.AreEqual(3, Evaluate("int 1 + int 2"));
            Assert.AreEqual((BigInteger)5, Evaluate("a=2; b=3; n=17; a+b mod n"));
            Assert.AreEqual((BigInteger)16, Evaluate("a=2; b=3; n=17; a-b mod n"));
            Assert.AreEqual((BigInteger)6, Evaluate("a=2; b=3; n=17; a*b mod n"));
            Assert.AreEqual((BigInteger)12, Evaluate("a=2; b=3; n=17; a/b mod n"));
            Assert.AreEqual((BigInteger)8, Evaluate("a=2; b=3; n=17; a**b mod n"));
            Assert.AreEqual((BigInteger)6, Evaluate("a=2; b=3; n=17; a**(1/2) mod n"));
            Assert.AreEqual(Rational.Parse("1606938044258990275541962092341162602522202993782792835301375/8796093022207"),
                Evaluate("((1<<200)-1)/((1<<43)-1)"));
        }

        private object Evaluate(string text)
        {
            var engine = new Engine();
            return new Parser().Compile(engine, CodeType.Script, text).Root.Get(engine);
        }
    }
}
