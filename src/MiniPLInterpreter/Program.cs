using MiniPLInterpreter.Input;
using MiniPLInterpreter.Parser;
using MiniPLInterpreter.Interpreter;
using MiniPLInterpreter.Scanner;
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
                MPLInterpreter interpreter = new MPLInterpreter(scanner, parser);

                interpreter.interpret(program);
            }
        }
    }
}
