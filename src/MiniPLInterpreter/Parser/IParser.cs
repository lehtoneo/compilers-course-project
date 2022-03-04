using System;
using System.Collections.Generic;
using System.Text;
using MiniPLInterpreter.Implementations;
namespace MiniPLInterpreter.Interfaces
{
    public interface IParser
    {
        public Node<String> parse(List<Token> tokens);
    }
}
