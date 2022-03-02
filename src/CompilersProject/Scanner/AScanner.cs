using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Implementations;
using CompilersProject.Interfaces;

namespace CompilersProject.Abstracts
{
    public abstract class AScanner : IScanner
    {
        public AScanner()
        {
        }


        public abstract List<Token> scan(string[] program);
    }
}
