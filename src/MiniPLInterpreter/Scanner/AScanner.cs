using System;
using System.Collections.Generic;
using System.Text;
using MiniPLInterpreter.Implementations;
using MiniPLInterpreter.Interfaces;

namespace MiniPLInterpreter.Abstracts
{
    public abstract class AScanner : IScanner
    {
        public AScanner()
        {
        }


        public abstract ScannerResult scan(string[] program);
    }
}
