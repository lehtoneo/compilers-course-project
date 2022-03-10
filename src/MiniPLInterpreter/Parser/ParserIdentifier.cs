using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLInterpreter.Parser
{
    public class ParserIdentifier
    {
        public int forLoopIndex { get; set; }
        public string type { get; set; }
        public ParserIdentifier(string type, int forLoopIndex)
        {
            this.type = type;
            this.forLoopIndex = forLoopIndex;
        }
    }
}
