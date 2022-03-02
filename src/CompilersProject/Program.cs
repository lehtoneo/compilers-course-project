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
            MiniPLParser parser = new MiniPLParser();
            Interpreter interpreter = new Interpreter(scanner, parser);

            interpreter.interpret(program);
        }
    }
}
