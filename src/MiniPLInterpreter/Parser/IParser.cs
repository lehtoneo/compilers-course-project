using System;
using System.Collections.Generic;
using System.Text;
using MiniPLInterpreter.Implementations;
namespace MiniPLInterpreter.Parser
{
    public interface IParser
    {
        public ParserResult parse(List<Token> tokens);
    }
}
