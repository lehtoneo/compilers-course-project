using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLInterpreter.Implementations
{
    public class ScannerResult
    {
        public List<Token> Tokens { get; set; }
        public List<String> Errors { get; set; }

        public ScannerResult(List<Token> tokens, List<String> errors)
        {
            Tokens = tokens;
            Errors = errors;
        }
    }
}
