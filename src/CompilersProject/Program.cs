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

            MiniPLScanner scanner = new MiniPLScanner(simpleCommentRemover);
            Interpreter interpreter = new Interpreter(program, scanner);

            interpreter.interpret();
        }
    }
}
