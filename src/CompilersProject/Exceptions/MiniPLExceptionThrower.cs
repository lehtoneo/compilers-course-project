using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject.Exceptions
{
    public class MiniPLExceptionThrower
    {

        void throwMiniPLExepction(string error)
        {
            throw new MiniPLException(error);
        }

        public void throwUnExpectedSymbolError(int line, char symbol)
        {
            string error = $"UnExpected symbol '{symbol}' at line {line}";
            throwMiniPLExepction(error);
        }
    }
}
