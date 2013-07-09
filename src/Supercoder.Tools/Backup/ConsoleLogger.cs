using System;

namespace Supercoder.Tools
{
    public static class ConsoleLogger
    {
        public static void Logger (string s)
        {
            string logLine = String.Format("{0:G}: {1}", DateTime.Now, s);
            Console.WriteLine(logLine);
        }
    }
}
