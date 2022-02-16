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

        public void printNode(Node<string> node)
        {
            Console.WriteLine(node.value);
            if (node.children.Count > 0)
            {
                foreach (Node<string> n in node.children)
                {
                    printNode(n);
                }
            }
        }
        public void interpret(string[] miniPlProgram)
        {
            try
            {
                List<Token> tokens = Scanner.scan(miniPlProgram);
                Console.WriteLine("Tokens:");
                foreach(Token t in tokens)
                {
                    Console.WriteLine(t.value);
                }
                Node<String> n = Parser.parse(tokens);
                printNode(n);


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
