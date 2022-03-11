using MiniPLInterpreter.Interfaces;

namespace MiniPLInterpreter.Scanner
{
    public abstract class AScanner : IScanner
    {
        public AScanner()
        {
        }


        public abstract ScannerResult scan(string[] program);
    }
}
