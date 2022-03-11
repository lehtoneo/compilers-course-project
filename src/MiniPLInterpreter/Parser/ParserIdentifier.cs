using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLInterpreter.Parser
{
    public class ParserIdentifier
    {
        public int forLoopIndex { get; set; }
        public string type { get; set; }

        public bool isControlVariable { get; set; } = false;
        public ParserIdentifier(string type, int forLoopIndex)
        {
            this.type = type;
            this.forLoopIndex = forLoopIndex;
        }

        public ParserIdentifier(string type, int forLoopIndex, bool isControlVariable) : this(type, forLoopIndex)
        {
            this.isControlVariable = isControlVariable;
        }
    }
}
