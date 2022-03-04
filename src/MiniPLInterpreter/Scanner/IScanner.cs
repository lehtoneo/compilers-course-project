using System;
using System.Collections.Generic;
using System.Text;
using MiniPLInterpreter.Implementations;

namespace MiniPLInterpreter.Interfaces
{
    public interface IScanner
    {
        public List<Token> scan(string[] program);
    }
}
