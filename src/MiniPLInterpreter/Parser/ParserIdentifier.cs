using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLInterpreter.Parser
{
    public class ParserIdentifier
    {
        public string type { get; set; }

        public bool isControlVariable { get; set; } = false;
        public ParserIdentifier(string type)
        {
            this.type = type;
        }

        public ParserIdentifier(string type, bool isControlVariable) : this(type)
        {
            this.isControlVariable = isControlVariable;
        }
    }
}
