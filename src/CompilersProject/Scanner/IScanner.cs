using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Implementations;

namespace CompilersProject.Interfaces
{
    public interface IScanner
    {
        public List<Token> scan(string[] program);
    }
}
