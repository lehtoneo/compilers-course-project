using System.Collections.Generic;

namespace MiniPLInterpreter.Parser
{
    public interface IParser
    {
        public ParserResult parse(List<Token> tokens);
    }
}
