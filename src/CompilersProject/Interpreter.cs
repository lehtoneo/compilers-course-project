using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Interfaces;
namespace CompilersProject.Implementations
{
    public class Interpreter : IInterpreter
    {
        private IInput input;
        public Interpreter(IInput input)
        {
            this.input = input;
        }

        public void interpret()
        {
            string [] programLines = input.readMiniPLProgram();
        }
    }
}
