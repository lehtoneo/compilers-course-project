using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject.Exceptions
{
    public class MiniPLExceptionThrower
    {
        private string errorPlace;
        public MiniPLExceptionThrower()
        {

        }

        public MiniPLExceptionThrower(string errorPlace)
        {
            this.errorPlace = errorPlace;
        }

        public void throwMiniPLExepction(string error)
        {
            string finalError = "";
            if (this.errorPlace != null)
            {
                finalError = $"Error in {errorPlace}: {error}";
            } else
            {
                finalError = error;
            }
            throw new MiniPLException(finalError);
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

        public void throwExpectedSomethingFoundNothingError(int line, string expected)
        {
            string error = $"Expected {expected} at line {line} found nothing.";
            throwMiniPLExepction(error);
        }

        public void throwInvalidError(int line, string invalid)
        {
            string error = $"Invalid '{invalid}' at line {line}";
            throwMiniPLExepction(error);
        }

        public void throwUndefinedVariableError(int line, string variable)
        {
            string error = $"Variable '{variable}' is undefined";
            throwMiniPLExepction(error);
        }

    }
}
