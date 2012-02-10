using System;
using System.IO;
using System.Text;

namespace Sandbox
{
    public class ConsoleLogger : TextWriter
    {
        TextWriter console;
        TextWriter log;

        public ConsoleLogger(string logFile)
        {
            console = Console.Out;
            log = File.AppendText(logFile);
        }

        public override Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }

        public override void Write(char value)
        {
            console.Write(value);
            log.Write(value);
            if (value == Environment.NewLine[Environment.NewLine.Length - 1])
                log.Flush();
        }
    }
}
