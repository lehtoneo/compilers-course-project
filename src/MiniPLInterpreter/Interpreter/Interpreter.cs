using System;
using MiniPLInterpreter.Interfaces;
using MiniPLInterpreter.Exceptions;
using System.Collections.Generic;
using MiniPLInterpreter.Utils;
using MiniPLInterpreter.Parser;
namespace MiniPLInterpreter.Implementations
{
    public class Interpreter : IInterpreter
    {
        private IScanner Scanner;
        private IParser Parser;
        private Dictionary<string, Operand> identifiers;
        private MiniPLHelper MiniPLHelper;
        private MiniPLExceptionThrower miniPLExceptionThrower = new MiniPLExceptionThrower("Runtime");
        private IConsoleIO consoleIO = new ConsoleIO();


        public Interpreter(IScanner scanner, IParser parser)
        {
            this.MiniPLHelper = new MiniPLHelper(miniPLExceptionThrower);
            this.Scanner = scanner;
            this.Parser = parser;
        }

        public Interpreter(IScanner scanner, IParser parser, IConsoleIO consoleIO) : this(scanner, parser)
        {
            this.consoleIO = consoleIO;
        }



        public void printNode(Node<string> node)
        {
            consoleIO.WriteLine(node.value);
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

                ScannerResult scannerResult = Scanner.scan(miniPlProgram);

                if (scannerResult.Errors.Count > 0)
                {
                    consoleIO.WriteLine("Scanner errors:");
                    scannerResult.Errors.ForEach(error =>
                    {
                        consoleIO.WriteLine(error);
                    });
                    return;
                }
                ParserResult parserResult = Parser.parse(scannerResult.Tokens);

                if (parserResult.Errors.Count > 0)
                {
                    consoleIO.WriteLine("Errors:");
                    parserResult.Errors.ForEach(error =>
                    {
                        consoleIO.WriteLine(error);
                    });
                    return;
                }

                Node<String> ast = parserResult.AST;
                consoleIO.WriteLine("AST:");
                ast.PrintPretty("", true);



                consoleIO.WriteLine("Program: ");

                consoleIO.WriteLine("------------");

                interpret(ast);
                consoleIO.WriteLine("");
                consoleIO.WriteLine("------------");

            }
            catch (MiniPLException e)
            {
                consoleIO.WriteLine("There was an error interpreting the minipl program");
                consoleIO.WriteLine(e.Message);
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
                consoleIO.Write(expressionValue.ToString());

            }
            else if (node.value == "read")
            {
                string identifierName = node.children[0].value;
                Operand identifier = identifiers[identifierName];

                string readValue = consoleIO.ReadLine();

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
                consoleIO.WriteLine("");
                consoleIO.WriteLine($"ASSERT {expressionOperand.value}");
            }
            else if (node.value == "forloop")
            {
                Node<string> loopVariableNode = node.children[0];
                string loopVariableName = loopVariableNode.value;

                Operand initLoopVarValues = identifiers[loopVariableName];
                Operand initialLoopOperand = new Operand(initLoopVarValues.value, initLoopVarValues.type);

                Node<string> firstExpressionNode = node.children[1];
                Node<string> secondExpressionNode = node.children[2];

                List<Node<string>> statements = new List<Node<string>>();
                int statementsStartIndex = 3;
                int stmtI = statementsStartIndex;
                while (true)
                {
                    Node<string> statementNode = node.children[stmtI];
                    if (statementNode.value == "end")
                    {
                        break;
                    }
                    else
                    {
                        statements.Add(statementNode);
                    }
                    stmtI++;
                }

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

                    foreach (Node<string> stmt in statements)
                    {
                        interpret(stmt);
                    }
                    int newI = i + 1;
                    identifiers[loopVariableName].value = newI.ToString();
                }

                identifiers[loopVariableName] = initialLoopOperand;

            }
        }

        public Operand getExpressionValue(Node<String> expressionNode)
        {
            string value = expressionNode.value;
            bool isOperator = MiniPLHelper.isOperator(value);

            if (isOperator)
            {
                string op = value;
                Operand operand1 = getExpressionValue(expressionNode.children[0]);
                if (op == "!")
                {
                    return !operand1;
                }
                else
                {
                    Operand operand2 = getExpressionValue(expressionNode.children[1]);
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
            }
            {
                string usedValue = value;


                string idOrType = usedValue.Split('(')[0];
                string operandValue = usedValue.Split('(')[1].Split(')')[0];
                if (idOrType == "id")
                {
                    return this.identifiers.GetValueOrDefault(operandValue);
                }

                return new Operand(operandValue, idOrType);
            }


        }




    }
}
