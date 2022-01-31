using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject.Interfaces
{
    public interface IInterpreter
    {
        public void interpret(string[] miniPlProgram);

        public void interpret();
    }
}
