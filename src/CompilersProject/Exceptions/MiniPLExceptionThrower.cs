using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject.Exceptions
{
    public class MiniPLExceptionThrower
    {

        public void throwMiniPLExepction(string error)
        {
            throw new MiniPLException(error);
        }

        public void throwUnExpectedSymbolError(int line, char symbol)
        {
            string error = $"UnExpected symbol '{symbol}' at line {line}";
            throwMiniPLExepction(error);
        }

        public void throwUnExpectedValueError(int line, string value, string expected)
        {
            string error = $"UnExpected value '{value}' at line {line} expected '{expected}'";
            throwMiniPLExepction(error);
        }

    }
}
