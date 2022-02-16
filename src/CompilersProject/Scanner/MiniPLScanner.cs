using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Utils;
using CompilersProject.Interfaces;
using CompilersProject.Abstracts;
using CompilersProject.Exceptions;

namespace CompilersProject.Implementations
{
    public class MiniPLScanner : AScanner
    {
        public MiniPLHelper miniPLHelper;
        public MiniPLExceptionThrower miniPLExceptionThrower;
        public MiniPLScanner(ICommentRemover commentRemover) : base(commentRemover) {
            this.miniPLExceptionThrower = new MiniPLExceptionThrower("Scanner");
            this.miniPLHelper = new MiniPLHelper();
        }

        public override List<Token> scan(string[] program)
        {
            string[] commentsRemoved = this.commentRemover.removeComments(program);
            
            List<Token> tokenList = new List<Token>();
            int lineI = -1;
            bool inString = false;
            foreach (string line in commentsRemoved)
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

                    if (inString)
                    {
                        token = token + c;
                        if (c == '"')
                        {
                            tokenList.Add(new Token(token, lineI, colI, "identifier"));
                            token = "";
                            inString = false;
                        }
                        continue;
                    }
                    if (c == '"')
                    {
                        inString = true;
                        char h = '"';
                        token = $"{h}";
                        continue;
                    }
                    if (c == ' ')
                    {
                        if (token != "")
                        {
                            tokenList.Add(new Token(token, lineI, colI, "identifier"));
                            token = "";
                        }
                        continue;
                    }

                    if (c == '.')
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
                                continue;
                            } else
                            {
                                miniPLExceptionThrower.throwUnExpectedSymbolError(lineI, '.');
                            }
                        } else
                        {
                            miniPLExceptionThrower.throwUnExpectedSymbolError(lineI, '.');
                        }
                    }

                    if (miniPLHelper.isSymbol(c))
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
                    } else
                    {
                        token = token + c;
                    }
                }
                if (token != "")
                {
                    tokenList.Add(new Token(token, lineI, colI - token.Length, "identifier"));
                    token = "";
                }
                lineI++;
            }
            int i = 0;
            while (i < tokenList.Count)
            {
                Token t = tokenList[i];
                if (miniPLHelper.isReservedKeyword(t.value))
                {
                    Token newToken = new Token(t.value, t.line, t.column, "keyword");
                    tokenList[i] = newToken;
                }
                i++;
            }


            return tokenList;
        }
        
    }
}
