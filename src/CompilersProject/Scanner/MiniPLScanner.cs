﻿using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Interfaces;
using CompilersProject.Abstracts;
using CompilersProject.Exceptions;

namespace CompilersProject.Implementations
{
    public class MiniPLScanner : AScanner
    {
        public Dictionary<char, bool> operatorDict;
        public Dictionary<char, bool> reservedSymbolDict;
        public MiniPLExceptionThrower miniPLExceptionThrower;
        public MiniPLScanner(ICommentRemover commentRemover) : base(commentRemover) {
            this.miniPLExceptionThrower = new MiniPLExceptionThrower();
            char[] miniPLOperators = new[] { '+', '-', '*', '/', '<', '=', '&', '!' };
            char[] otherReservedSymbols = new[] { ';', ':', '(', ')' };
            this.reservedSymbolDict = new Dictionary<char, bool>();
            
            foreach (char c in miniPLOperators)
            {
                this.reservedSymbolDict.Add(c, true);
            }
            foreach (char c in otherReservedSymbols)
            {
                this.reservedSymbolDict.Add(c, true);
            }
        }

        public override List<Token> scan(string[] program)
        {
            string[] commentsRemoved = this.commentRemover.removeComments(program);
            
            List<Token> tokenList = new List<Token>();
            int lineI = 0;
            bool inString = false;
            foreach (string line in commentsRemoved)
            {
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
                            tokenList.Add(new Token(token, lineI, colI));
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
                            tokenList.Add(new Token(token, lineI, colI));
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
                                    tokenList.Add(new Token(token, lineI, colI - token.Length));
                                    token = "";
                                }
                                tokenList.Add(new Token("..", lineI, colI));
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

                    if (isReservedSymbol(c))
                    {
                        if (token != "")
                        {
                            tokenList.Add(new Token(token, lineI, colI));
                            token = "";
                        }
                        if (c == ':')
                        {
                            if (line.Length > colI + 1)
                            {
                                if (line[colI + 1] == '=')
                                {
                                    tokenList.Add(new Token(":=", lineI, colI));
                                    colI++;
                                    continue;
                                }
                            }
                        }

                        tokenList.Add(new Token(c, lineI, colI));
                    } else
                    {
                        token = token + c;
                    }
                }
                if (token != "")
                {
                    tokenList.Add(new Token(token, lineI, colI - token.Length));
                    token = "";
                }
                lineI++;
            }


            return tokenList;
        }
        public bool isReservedSymbol(char c)
        {
            return this.reservedSymbolDict.GetValueOrDefault(c, false);
        }
    }
}