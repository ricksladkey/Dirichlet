using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Decompose.Numerics
{
    public class SquareFreeCounting
    {
        private static BigInteger[] data10 = new BigInteger[]
        {
            BigInteger.Parse("1"),
            BigInteger.Parse("7"),
            BigInteger.Parse("61"),
            BigInteger.Parse("608"),
            BigInteger.Parse("6083"),
            BigInteger.Parse("60794"),
            BigInteger.Parse("607926"),
            BigInteger.Parse("6079291"),
            BigInteger.Parse("60792694"),
            BigInteger.Parse("607927124"),
            BigInteger.Parse("6079270942"),
            BigInteger.Parse("60792710280"),
            BigInteger.Parse("607927102274"),
            BigInteger.Parse("6079271018294"),
            BigInteger.Parse("60792710185947"),
            BigInteger.Parse("607927101854103"),
            BigInteger.Parse("6079271018540405"),
            BigInteger.Parse("60792710185403794"),
            BigInteger.Parse("607927101854022750"),
            BigInteger.Parse("6079271018540280875"),
            BigInteger.Parse("60792710185402613302"),
            BigInteger.Parse("607927101854026645617"),
            BigInteger.Parse("6079271018540266153468"),
            BigInteger.Parse("60792710185402662868753"),
            BigInteger.Parse("607927101854026628773299"),
            BigInteger.Parse("6079271018540266286424910"),
            BigInteger.Parse("60792710185402662866945299"),
            BigInteger.Parse("607927101854026628664226541"),
            BigInteger.Parse("6079271018540266286631251028"),
            BigInteger.Parse("60792710185402662866327383816"),
            BigInteger.Parse("607927101854026628663278087296"),
            BigInteger.Parse("6079271018540266286632795633943"),
            BigInteger.Parse("60792710185402662866327694188957"),
            BigInteger.Parse("607927101854026628663276901540346"),
            BigInteger.Parse("6079271018540266286632767883637220"),
            BigInteger.Parse("60792710185402662866327677953999263"),
            BigInteger.Parse("607927101854026628663276779463775476"),
        };

        public static BigInteger PowerOfTen(int n)
        {
            return data10[n];
        }
    }
}
