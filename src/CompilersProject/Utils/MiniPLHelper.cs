using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Exceptions;
using CompilersProject.Implementations;
namespace CompilersProject.Utils
{
    public class MiniPLHelper
    {
        public Dictionary<char, bool> operatorDict { get; set; }
        public Dictionary<char, bool> symbolDict { get; set; }
        public Dictionary<string, bool> reservedKeywords { get; set; }
        public MiniPLExceptionThrower miniPLExceptionThrower = new MiniPLExceptionThrower();
        public MiniPLHelper(MiniPLExceptionThrower miniPLExceptionThrower)
        {
            this.miniPLExceptionThrower = miniPLExceptionThrower;

            char[] miniPLOperators = new[] { '+', '-', '*', '/', '<', '=', '&', '!' };
            char[] otherSymbols = new[] { ';', ':', '(', ')' };

            string[] reservedKeywords = new[] { "var", "for", "end", "in", "do", "read", "print", "int", "string", "bool", "assert" };

            this.symbolDict = new Dictionary<char, bool>();
            this.reservedKeywords = new Dictionary<string, bool>();
            this.operatorDict = new Dictionary<char, bool>();
            foreach (string rkw in reservedKeywords)
            {
                this.reservedKeywords.Add(rkw, true);
            }
            foreach (char c in miniPLOperators)
            {
                this.operatorDict.Add(c, true);
                this.symbolDict.Add(c, true);
            }
            foreach (char c in otherSymbols)
            {
                this.symbolDict.Add(c, true);
            }

        }

        public bool isSymbol(char c)
        {
            return this.symbolDict.GetValueOrDefault(c, false);
        }

        public bool isReservedKeyword(string s)
        {
            return this.reservedKeywords.GetValueOrDefault(s, false);
        }

        public bool isOperator(char c)
        {
            return this.operatorDict.GetValueOrDefault(c, false);
        }

        public bool isOperator(string s)
        {
            if (s.Length != 1)
            {
                return false;
            }
            else
            {
                return isOperator(s[0]);
            }
        }

        public bool isUnaryOperator(char c)
        {
            return c == '!';
        }

        public bool isUnaryOperator(string s)
        {
            return s == "!";
        }

        public bool isString(string s)
        {
            return s[0] == '"' && s[s.Length - 1] == '"';
        }
        public bool isBoolean(string s)
        {
            return s == "true" || s == "false";
        }


        public bool isInt(string s)
        {
            return int.TryParse(s, out _);
        }
        public bool checkTokenThrowsMiniPLError(Token t, string expectedValue)
        {
            if (t.value != expectedValue)
            {
                miniPLExceptionThrower
                    .throwUnExpectedValueError(t.row, t.value, expectedValue);
            }
            return true;
        }

        public bool isValidType(string value)
        {
            List<string> validTypes = new List<string> { "int", "string", "bool" };

            return validTypes.Contains(value);
        }

        public bool isValidType(Token t) { return isValidType(t.value); }

        public string getReturnTypeFromOperator(string op)
        {
            if (op == "+")
            {
                throw new Exception("Cannot declare return type for operator + (can be int or string)");
            }
            if (!isOperator(op))
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Invalid operator '{op}'");
            }

            List<string> intOperators = new List<string> { "-", "*", "/" };
            if (intOperators.Contains(op))
            {
                return "int";
            }

            List<string> boolOperators = new List<string> { "&", "!", "=", "<" };

            if (boolOperators.Contains(op))
            {
                return "bool";
            }

            // should never be here
            return "";
        }


        public bool isValidOperatorForType(string op, string type)
        {
            if (!isValidType(type))
            {
                throw new Exception("Is valid operator got invalid type");
            }

            List<string> validCommonOperands = new List<string> { "=", "<" };

            if (validCommonOperands.Contains(op))
            {
                return true;
            }

            if (type == "string")
            {
                List<string> validStringOperands = new List<string> { "+" };
                return validStringOperands.Contains(op);

            }
            else if (type == "int")
            {
                List<string> validIntOperands = new List<string> { "+", "-", "/", "*" };
                return validIntOperands.Contains(op);

            }
            else if (type == "bool")
            {
                List<string> validBoolOperands = new List<string> { "&", "!" };
                return validBoolOperands.Contains(op);
            }
            else
            {
                // should never come here
                return false;
            }
        }
    }
}
