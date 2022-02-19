using System;
using CompilersProject.Interfaces;
using CompilersProject.Exceptions;
using System.Collections.Generic;
using CompilersProject.Utils;
namespace CompilersProject.Implementations
{
    public class Interpreter : IInterpreter
    {
        public string[] MiniPlProgram { get; set; }
        private IScanner Scanner;
        private IParser Parser;
        private Dictionary<string, Operand> identifiers;
        private MiniPLHelper MiniPLHelper;
        public Interpreter(IScanner scanner, IParser parser)
        {
            this.MiniPLHelper = new MiniPLHelper(null);
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

                this.identifiers = new Dictionary<string, Operand>();

                List<Token> tokens = Scanner.scan(miniPlProgram);
                Console.WriteLine("Tokens:");
                foreach (Token t in tokens)
                {
                    Console.WriteLine(t.value);
                }
                Node<String> n = Parser.parse(tokens);

                printNode(n);

                Console.WriteLine("Program: ");

                Console.WriteLine("------------");

                interpret(n);

                Console.WriteLine("------------");

            }
            catch (MiniPLException e)
            {
                Console.WriteLine("There was an error interpreting the minipl program");
                Console.WriteLine(e.Message);
            }
        }

        public void interpret(Node<String> node)
        {
            if (node.value == "$$")
            {
                return;
            }
            else if (node.value == "program")
            {
                foreach (Node<String> n in node.children)
                {
                    interpret(n);
                }
            }
            else if (node.value == "stmt_list")
            {
                foreach (Node<string> s in node.children)
                {
                    interpret(s);
                }
            }
            else if (node.value == "var_assignment")
            {
                string identifierName = node.children[0].value;
                string identifierType = node.children[1].value;

                // assignment without expression
                if (node.children.Count == 2)
                {
                    this.identifiers.Add(identifierName, new Operand(null, identifierType));
                    return;
                }

                Node<string> expressionNode = node.children[2];
                Operand expressionValue = getExpressionValue(expressionNode);

                this.identifiers.Add(identifierName, expressionValue);


            }
            else if (node.value == "assignment")
            {
                string identifierName = node.children[0].value;

                Node<string> expressionNode = node.children[1];
                Operand expressionValue = getExpressionValue(expressionNode);

                identifiers[identifierName].value = expressionValue.value;

            }
            else if (node.value == "print")
            {
                Node<string> expressionNode = node.children[0];

                Operand expressionValue = getExpressionValue(expressionNode);
                Console.WriteLine(expressionValue.value);

            }
            else if (node.value == "read")
            {
                string identifierName = node.children[0].value;

                string readValue = Console.ReadLine();
                string type;

                if (MiniPLHelper.isInt(readValue))
                {
                    type = "int";
                }
                else
                {
                    type = "string";
                }

                this.identifiers.Add(identifierName, new Operand(readValue, type));
            }
        }

        public Operand getExpressionValue(Node<String> expressionNode)
        {
            if (expressionNode.children.Count == 3)
            {

                Operand operand1 = getOperandValue(expressionNode.children[0]);
                string op = expressionNode.children[1].value;
                Operand operand2 = getOperandValue(expressionNode.children[2]);

                if (op == "+")
                {
                    return operand1 + operand2;
                }
                else if (op == "*")
                {
                    return operand1 * operand2;
                }
                else
                {
                    throw new NotImplementedException();
                }



            }
            else if (expressionNode.children.Count == 1)
            {
                Node<string> childNode = expressionNode.children[0];
                return getOperandValue(childNode);

            }
            else
            {   // unary opnd
                throw new NotImplementedException();
            }
        }

        public Operand getOperandValue(Node<string> operandNode)
        {
            Node<string> childNode = operandNode.children[0];
            if (childNode.value == "expression")
            {
                return getExpressionValue(childNode);
            }
            else
            {
                string idOrType = childNode.value.Split('(')[0];
                string value = childNode.value.Split('(')[1].Split(')')[0];
                if (idOrType == "id")
                {
                    return this.identifiers.GetValueOrDefault(value);
                }

                return new Operand(value, idOrType);
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
