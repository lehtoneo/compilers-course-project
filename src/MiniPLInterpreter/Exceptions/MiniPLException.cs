using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLInterpreter.Exceptions
{
    public class MiniPLException : Exception
    {
        public MiniPLException()
        {
        }

        public MiniPLException(string message)
            : base(message)
        {
        }

        public MiniPLException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
