using System;
using CompilersProject.Interfaces;
using CompilersProject.Exceptions;
using System.Collections.Generic;

namespace CompilersProject.Implementations
{
    public class Interpreter : IInterpreter
    {
        public string[] MiniPlProgram { get; set; }
        private IScanner Scanner;
        private IParser Parser;
        

        public Interpreter(IScanner scanner, IParser parser)
        {
            this.Scanner = scanner;
            this.Parser = parser;
        }

        public Interpreter(string[] miniPLProgram, IScanner scanner, IParser parser) : this(scanner, parser)
        {
            this.MiniPlProgram = miniPLProgram;
        }

        
        public void interpret(string[] miniPlProgram)
        {
            try
            {
                List<Token> tokens = Scanner.scan(miniPlProgram);
                Parser.parse(tokens);

            } catch (MiniPLException e)
            {
                Console.WriteLine("There was an error interpreting the minipl program");
                Console.WriteLine(e.Message);
            }
        }

        public void interpret()
        {
            if (this.MiniPlProgram == null)
            {
                throw new InvalidOperationException("MiniPlProgram not defined");
            }

            interpret(this.MiniPlProgram);
        }
    }
}
