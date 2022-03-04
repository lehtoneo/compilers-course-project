using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLInterpreter.Exceptions
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

        public MiniPLException throwMiniPLException(string error)
        {
            string finalError = "";
            if (this.errorPlace != null)
            {
                finalError = $"{errorPlace} error: {error}";
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
            throwMiniPLException(error);
        }



        public void throwUnExpectedValueError(int row, string value, string expected)
        {
            string error = $"UnExpected value '{value}' at row {row} expected '{expected}'";
            throwMiniPLException(error);
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

            throwMiniPLException(finalError);
        }

        public void throwInvalidError(int row, string invalid)
        {
            string error = $"Invalid '{invalid}' at row {row}";
            throwMiniPLException(error);
        }

        public void throwUndefinedVariableError(int row, string variable)
        {
            string error = $"Undefined variable at row {row} : Variable '{variable}' is undefined";
            throwMiniPLException(error);
        }

        public void throwInvalidExpressionError(int row, string expectedType, string receivedType)
        {
            string error = $"Invalid expression at row {row} : expected type {expectedType}, received {receivedType}";
            throwMiniPLException(error);
        }

        public void throwInvalidUsageOfOperatorError(int row, string op, string type)
        {
            string error = $"Invalid expression at row {row} : invalid operator '{op}' for type '{type}'";
            throwMiniPLException(error);
        }

        public void throwInvalidOperatorError(int row, string op, string type)
        {
            string error = $"Invalid operator '{op}' for type {type} at row {row}: ";
            throwMiniPLException(error);
        }

    }
}
