using System;
using System.Collections;
using System.Linq;

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
                if (text == null || text == "quit")
                    break;
                try
                {
                    var value = new Parser().Compile(engine, CodeType.Script, text).Root.Get(engine);
                    if (value != null)
                    {
                        engine.SetVariable("@", value);
                        engine.Print(value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}
