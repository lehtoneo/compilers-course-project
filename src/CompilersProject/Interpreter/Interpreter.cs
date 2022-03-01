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
        private MiniPLExceptionThrower miniPLExceptionThrower = new MiniPLExceptionThrower("Runtime");
        public Interpreter(IScanner scanner, IParser parser)
        {
            this.MiniPLHelper = new MiniPLHelper(miniPLExceptionThrower);
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
                Console.WriteLine("AST:");
                printNode(n);

                Console.WriteLine("Program: ");

                Console.WriteLine("------------");

                interpret(n);
                Console.WriteLine("");
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
                Console.Write(expressionValue.ToString());

            }
            else if (node.value == "read")
            {
                string identifierName = node.children[0].value;
                Operand identifier = identifiers[identifierName];

                string readValue = Console.ReadLine();

                if (identifier.type == "int")
                {
                    if (!MiniPLHelper.isInt(readValue))
                    {
                        miniPLExceptionThrower
                            .throwMiniPLException($"Cannot cast {readValue} to type 'int'");
                    }
                }


                this.identifiers[identifierName].value = readValue;
            }
            else if (node.value == "assert")
            {
                Node<string> expressionNode = node.children[0];

                Operand expressionOperand = getExpressionValue(expressionNode);

                if (expressionOperand.type != "bool")
                {
                    miniPLExceptionThrower
                        .throwMiniPLException($"expected a bool expression for assert statement. Found {expressionOperand.type}");
                }
                Console.WriteLine("");
                Console.WriteLine($"ASSERT {expressionOperand.value}");
            }
            else if (node.value == "forloop")
            {
                Node<string> loopVariableNode = node.children[0];
                string loopVariableName = loopVariableNode.value;

                Operand initLoopVarValues = identifiers[loopVariableName];
                Operand initialLoopOperand = new Operand(initLoopVarValues.value, initLoopVarValues.type);

                Node<string> firstExpressionNode = node.children[1];
                Node<string> secondExpressionNode = node.children[2];
                Node<string> statementsNode = node.children[3];
                int firstExpressionValue = getExpressionValue(firstExpressionNode).valueToInt();
                int secondExpressionValue = getExpressionValue(secondExpressionNode).valueToInt();

                identifiers[loopVariableName].value = firstExpressionValue.ToString();

                while (true)
                {
                    string loopVariableValueString = identifiers[loopVariableName].value;
                    int.TryParse(loopVariableValueString, out int i);
                    if (i > secondExpressionValue)
                    {
                        break;
                    }
                    interpret(statementsNode);
                    int newI = i + 1;
                    identifiers[loopVariableName].value = newI.ToString();
                }

                identifiers[loopVariableName] = initialLoopOperand;

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
                else if (op == "-")
                {
                    return operand1 - operand2;
                }
                else if (op == "/")
                {
                    return operand1 / operand2;
                }
                else if (op == "&")
                {
                    return operand1 & operand2;
                }
                else if (op == "=")
                {
                    return operand1 == operand2;
                }
                else if (op == "<")
                {
                    return operand1 < operand2;
                }
                else
                {
                    throw new NotImplementedException();
                }

            }
            else if (expressionNode.children.Count == 2)
            {

                Node<string> operandNode = expressionNode.children[1];

                Operand opnd = getOperandValue(operandNode);
                return !opnd;
            }
            else if (expressionNode.children.Count == 1)
            {
                Node<string> operandNode = expressionNode.children[0];
                return getOperandValue(operandNode);
            }
            else
            {
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
