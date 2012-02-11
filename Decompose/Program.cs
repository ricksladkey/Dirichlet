using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Decompose.Numerics;

namespace Decompose
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Engine();
            while (true)
            {
                Console.Write(">> ");
                var text = Console.ReadLine();
                if (text == null)
                    break;
                var value = new Parser().Compile(engine, CodeType.Statement, text).Root.Get(engine);
                if (value != null)
                {
                    engine.SetVariable("@", value);
                    Console.WriteLine(value);
                }
            }
        }
    }
}
