using System;
using CompilersProject.Implementations;

namespace CompilersProject
{
    class Program
    {
        static void Main(string[] args)
        {
            MiniPLFileNameInput miniPLInput = new MiniPLFileNameInput();

            Interpreter interpreter = new Interpreter(miniPLInput);
            interpreter.interpret();
        }
    }
}
