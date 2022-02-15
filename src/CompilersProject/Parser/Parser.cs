using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Interfaces;
using CompilersProject.Exceptions;
namespace CompilersProject.Implementations
{
    public class Parser : IParser
    {
        public List<Token> Tokens;
        public MiniPLExceptionThrower miniPLExceptionThrower;

        public Parser ()
        {
            this.miniPLExceptionThrower = new MiniPLExceptionThrower();
        }
       
        public void parse(List<Token> tokens)
        {

            this.Tokens = tokens;
            List<List<Token>> statements = new List<List<Token>>();
            List<Token> statement = new List<Token>();

            foreach(Token t in tokens) 
            {
                string tokenValue = t.value;
                
                if (tokenValue == ";")
                {

                    statements.Add(statement);
                    statement = new List<Token>();
                    continue;
                } else
                {
                    statement.Add(t);
                }
            }
            foreach(List<Token> statementList in statements)
            {
                stmt(statementList);
            }
        }

        

        public void stmt(List<Token> statement)
        {
            Token firstToken = statement[0];
            if (firstToken.value == ";")
            {
                return;
            }

            if (firstToken.value == "var")
            {
                var(statement);
                return;
            }
        }

        public void var(List<Token> statement)
        {
            Token first = statement[0];
            if (statement.Count < 6)
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid usage of var at row {first.line}");
            }
            int identI = 1;
            int typeI = 3;
            int assignMentI = 4;
            
            this.identifier(statement[1]);
            
            if (statement[2].value != ":")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.line, statement[2].value, ":");
            }
            
            this.type(statement[typeI]);

            if (statement[assignMentI].value != ":=")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.line, statement[assignMentI].value, ":=");
            }

            List<Token> expression = new List<Token>();
            int i = assignMentI + 1;
            while (i < statement.Count)
            {
                expression.Add(statement[i]);
                i++;
            }
            expr(expression);
        }

        public void expr(List<Token> tokens)
        {

        }

        public string identifier(Token t)
        {
            string value = t.value;
            char firstChar = value[0];
            if (!Char.IsLetter(firstChar))
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid identifier at row ${t.line}");
            }

            foreach(char c in value)
            {
                if (!Char.IsNumber(c) && !Char.IsLetter(c))
                {
                    miniPLExceptionThrower.throwMiniPLExepction($"Invalid identifier at row ${t.line}");
                }
            }

            return value;
        }

        public string type(Token t)
        {
            string value = t.value;
            List<string> validTypes = new List<string>{ "int", "string", "bool" };
            if (!validTypes.Contains(value))
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid type '{value}' at row ${t.line}");
            }

            return value;


        }
    }
}
