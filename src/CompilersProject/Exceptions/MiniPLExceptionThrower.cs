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
            }
            else
            {
                finalError = error;
            }

            throw new MiniPLException(finalError);
        }

        public void throwUnExpectedSymbolError(int row, char symbol)
        {
            string error = $"UnExpected symbol '{symbol}' at row {row}";
            throwMiniPLExepction(error);
        }



        public void throwUnExpectedValueError(int row, string value, string expected)
        {
            string error = $"UnExpected value '{value}' at row {row} expected '{expected}'";
            throwMiniPLExepction(error);
        }

        public void throwExpectedSomethingFoundNothingError(int row, string expected)
        {
            string finalError = "";
            if (row == -1)
            {
                finalError = $"Expected {expected}, found nothing.";
            }
            else
            {
                finalError = $"Expected {expected} at row {row} found nothing.";
            }

            throwMiniPLExepction(finalError);
        }

        public void throwInvalidError(int row, string invalid)
        {
            string error = $"Invalid '{invalid}' at row {row}";
            throwMiniPLExepction(error);
        }

        public void throwUndefinedVariableError(int row, string variable)
        {
            string error = $"Variable '{variable}' is undefined";
            throwMiniPLExepction(error);
        }

    }
}
