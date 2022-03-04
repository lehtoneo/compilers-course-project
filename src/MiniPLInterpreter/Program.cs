using MiniPLInterpreter.Implementations;

namespace MiniPLInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            MiniPLFileNameInput miniPLInput = new MiniPLFileNameInput();

            while (true)
            {
                string[] program = miniPLInput.readMiniPLProgram();

                MiniPLScanner scanner = new MiniPLScanner();
                MiniPLParser parser = new MiniPLParser();
                Interpreter interpreter = new Interpreter(scanner, parser);

                interpreter.interpret(program);
            }
        }
    }
}
