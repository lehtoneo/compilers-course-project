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

        public bool checkTokenThrowsMiniPLError(Token t, string expectedValue)
        {
            if (t.value != expectedValue)
            {
                miniPLExceptionThrower
                    .throwUnExpectedValueError(t.row, t.value, expectedValue);
            }
            return true;
        }
    }
}
