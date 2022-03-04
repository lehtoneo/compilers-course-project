using System;
using System.Collections.Generic;
using System.Text;
using MiniPLInterpreter.Utils;
using MiniPLInterpreter.Interfaces;
using MiniPLInterpreter.Abstracts;
using MiniPLInterpreter.Exceptions;

namespace MiniPLInterpreter.Implementations
{
    public class MiniPLScanner : AScanner
    {
        public MiniPLHelper miniPLHelper;
        public MiniPLExceptionThrower miniPLExceptionThrower;
        public MiniPLScanner()
        {
            this.miniPLExceptionThrower = new MiniPLExceptionThrower("Scanner");
            this.miniPLHelper = new MiniPLHelper(miniPLExceptionThrower);
        }

        public override List<Token> scan(string[] program)
        {

            List<Token> tokenList = new List<Token>();
            int lineI = 0;
            bool inString = false;
            bool stringEscaping = false;
            int multiLineCommentIndex = 0;
            foreach (string line in program)
            {
                lineI++;
                string token = "";
                int colI = -1;

                if (line == "")
                {
                    continue;
                }
                while (colI + 1 < line.Length)
                {

                    colI++;
                    char c = line[colI];
                    // comment handling
                    if (multiLineCommentIndex > 0)
                    {
                        if (c == '*')
                        {
                            if (colI + 1 < line.Length)
                            {
                                char next = line[colI + 1];
                                if (next == '/')
                                {

                                    multiLineCommentIndex--;
                                    colI++;
                                    continue;
                                }
                            }
                        }
                    }

                    if (c == '/')
                    {
                        if (colI + 1 < line.Length)
                        {
                            char next = line[colI + 1];
                            if (next == '/')
                            {
                                colI = line.Length;
                                continue;
                            }
                            else if (next == '*')
                            {
                                multiLineCommentIndex++;
                                colI++;
                                continue;
                            }
                            // otherwise continue normally
                        }
                        else
                        {
                            miniPLExceptionThrower.throwUnExpectedSymbolError(lineI, '/');
                        }
                    }

                    if (multiLineCommentIndex > 0)
                    {
                        continue;
                    }
                    if (inString)
                    {
                        token = token + c;
                        if (stringEscaping)
                        {
                            stringEscaping = false;
                        }
                        else if (c == '\\')
                        {
                            stringEscaping = true;
                        }
                        else if (c == '"')
                        {
                            tokenList.Add(new Token(token, lineI, colI, "string"));
                            token = "";
                            inString = false;
                            stringEscaping = false;
                        }
                    }
                    else if (c == '"')
                    {
                        inString = true;
                        char h = '"';
                        token = $"{h}";
                    }
                    else if (c == ' ' || c == '\t')
                    {
                        if (token != "")
                        {
                            tokenList.Add(new Token(token, lineI, colI, "identifier"));
                            token = "";
                        }
                    }
                    else if (c == '.')
                    {

                        if (colI + 1 < line.Length)
                        {
                            char next = line[colI + 1];
                            if (next == '.')
                            {
                                if (token != "")
                                {
                                    tokenList.Add(new Token(token, lineI, colI - token.Length, "identifier"));
                                    token = "";
                                }
                                tokenList.Add(new Token("..", lineI, colI, "symbol"));
                                colI = colI + 1;
                            }
                            else
                            {
                                miniPLExceptionThrower.throwUnExpectedSymbolError(lineI, '.');
                            }
                        }
                        else
                        {
                            miniPLExceptionThrower.throwUnExpectedSymbolError(lineI, '.');
                        }
                    }
                    else if (miniPLHelper.isSymbol(c))
                    {
                        if (token != "")
                        {
                            tokenList.Add(new Token(token, lineI, colI, "identifier"));
                            token = "";
                        }
                        if (c == ':')
                        {
                            if (line.Length > colI + 1)
                            {
                                if (line[colI + 1] == '=')
                                {
                                    tokenList.Add(new Token(":=", lineI, colI, "symbol"));
                                    colI++;
                                    continue;
                                }
                            }
                        }

                        tokenList.Add(new Token(c, lineI, colI, "symbol"));
                    }
                    else
                    {
                        token = token + c;
                    }
                }
                if (token != "")
                {
                    tokenList.Add(new Token(token, lineI, colI - token.Length, "identifier"));

                }
            }
            int i = 0;
            while (i < tokenList.Count)
            {
                Token t = tokenList[i];

                if (miniPLHelper.isReservedKeyword(t.value))
                {
                    Token newToken = new Token(t.value, t.row, t.column, "keyword");
                    tokenList[i] = newToken;
                }
                i++;
            }


            return tokenList;
        }

    }
}
