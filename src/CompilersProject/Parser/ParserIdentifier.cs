using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject.Implementations
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
