using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLInterpreter.Parser
{
    public class ParserResult
    {
        public Node<string> AST { get; set; }
        public List<string> Errors { get; set; }
        public bool hasErrors { get; }
        public ParserResult(Node<string> ast, List<string> errors)
        {
            this.AST = ast;
            this.Errors = errors;
            this.hasErrors = errors.Count > 0;
        }
    }
}
