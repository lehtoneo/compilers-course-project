using System;
using System.Collections.Generic;
using System.Text;
using MiniPLInterpreter.Implementations;

namespace MiniPLInterpreter.Interfaces
{
    public interface IScanner
    {
        public ScannerResult scan(string[] program);
    }
}
