using System;
using CompilersProject.Implementations;

namespace CompilersProject
{
    class Program
    {
        static void Main(string[] args)
        {
            MiniPLFileNameInput miniPLInput = new MiniPLFileNameInput();

            string[] program = miniPLInput.readMiniPLProgram();

            SimpleCommentRemover simpleCommentRemover = new SimpleCommentRemover();

            Interpreter interpreter = new Interpreter(program, simpleCommentRemover);

            interpreter.interpret();
        }
    }
}
