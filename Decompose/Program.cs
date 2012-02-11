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
                if (text == null)
                    break;
                try
                {
                    var value = new Parser().Compile(engine, CodeType.Statement, text).Root.Get(engine);
                    if (value != null)
                    {
                        engine.SetVariable("@", value);
                        if (value is IEnumerable)
                            Console.WriteLine(string.Join(", ", (value as IEnumerable).Cast<object>().Select(item => item.ToString())));
                        else
                            Console.WriteLine(value);
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
