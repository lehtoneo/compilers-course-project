using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Implementations;
namespace CompilersProject.Interfaces
{
    public interface IParser
    {
        public Node<String> parse(List<Token> tokens);
    }
}
