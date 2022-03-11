using System;
using MiniPLInterpreter.Interfaces;

namespace MiniPLInterpreter.Interpreter
{
    public class ConsoleIO : IConsoleIO
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Write(string s)
        {
            Console.Write(s);
        }

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }
    }
}
