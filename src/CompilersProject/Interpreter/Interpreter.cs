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

        

        public Interpreter(IScanner commentRemover)
        {
            this.Scanner = commentRemover;
        }

        public Interpreter(string[] miniPLProgram, IScanner scanner) : this(scanner)
        {
            this.MiniPlProgram = miniPLProgram;
        }

        
        public void interpret(string[] miniPlProgram)
        {
            try
            {
                List<Token> tokens = Scanner.scan(miniPlProgram);
                Console.WriteLine("Tokens:");
                foreach (Token t in tokens)
                {
                    Console.WriteLine(t.value);
                }
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
