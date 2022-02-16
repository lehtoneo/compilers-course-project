using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Utils;
using CompilersProject.Interfaces;
using CompilersProject.Exceptions;

namespace CompilersProject.Implementations
{
    public class MiniPLParser : IParser
    {
        public List<Token> Tokens;
        public Dictionary<string, bool> identifiers = new Dictionary<string, bool>();
        public MiniPLExceptionThrower miniPLExceptionThrower = new MiniPLExceptionThrower("Parser");
        public MiniPLHelper miniPLHelper = new MiniPLHelper();
        public Node<String> parse(List<Token> tokens)
        {

            this.Tokens = tokens;

            Node<String> programNode = new Node<String>("program");

            stmt_list(programNode, tokens);

            return programNode;

        }



        public void stmt_list(Node<String> root, List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                root.children.Add(new Node<string>("$$"));
                return;
            }

            Node<String> node = new Node<String>("stmt_list");
            root.children.Add(node);
            List<Token> firstStatement = new List<Token>();
            List<Token> restOfStatements = new List<Token>();
            bool firstStatementCaptured = false;
            foreach (Token t in tokens)
            {
                string tokenValue = t.value;

                if (tokenValue == ";" && !firstStatementCaptured)
                {
                    firstStatementCaptured = true;
                    continue;
                }
                if (!firstStatementCaptured)
                {
                    firstStatement.Add(t);
                } else
                {
                    restOfStatements.Add(t);
                }
            }
            stmt(node, firstStatement);
            stmt_list(node, restOfStatements);
        }
        public void stmt(Node<String> parent, List<Token> statement)
        {
            
            Node<string> statementNode = new Node<string>("statement");
            parent.children.Add(statementNode);
            if (statement.Count == 0)
            {
                statementNode.children.Add(new Node<string>("$$"));
                return;
            }
            Token firstToken = statement[0];
            
            if (firstToken.value == "var")
            {
                var(statementNode, statement);
            } else if (firstToken.value == "read")
            {
                read(statementNode, statement);
            } else if (firstToken.value == "print")
            {
                print(statementNode, statement);
            } else if (firstToken.value == "assert")
            {
                assert(statementNode, statement);
            } else if (firstToken.value == "for")
            {
                // todo
            } else
            {
                identAssignment(statementNode, statement);
            }
        }

        public void identAssignment(Node<String> parent, List<Token> tokens)
        {
            Token ident = tokens[0];
            string identifier = ident.value;
            if (!identifiers.GetValueOrDefault(identifier, false))
            {
                miniPLExceptionThrower.throwUndefinedVariableError(ident.line, identifier);
            }
            parent.children.Add(new Node<string>(identifier));
            if (tokens.Count < 2)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(ident.line, ":=");
            }
            Token assignmentToken = tokens[1];
            if (assignmentToken.value != ":=")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(assignmentToken.line, assignmentToken.value, ":=");
            }
            parent.children.Add(new Node<string>(":="));
            List<Token> expressionTokens = new List<Token>();
            int i = 2;
            while (i < tokens.Count)
            {
                Token curToken = tokens[i];
                expressionTokens.Add(curToken);
                i++;
            }
            expr(parent, expressionTokens);

        }
        public void assert(Node<String> parent, List<Token> tokens)
        {
            Node<String> assertNode = parent;
            assertNode.children.Add(new Node<string>("assert"));
            Token first = tokens[0];
            if (tokens.Count < 2)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.line, "(");
            }
            Token leftParenthesis = tokens[1];
            if (leftParenthesis.value != "(")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.line, leftParenthesis.value, "(");
            }
            List<Token> expressionTokens = new List<Token>();
            int i = 2;
            while (i < tokens.Count)
            {
                Token currentT = tokens[i];
                if (currentT.value != ")")
                {
                    expressionTokens.Add(currentT);
                } else
                {
                    expr(assertNode, expressionTokens);
                    return;
                }
                i++;
            }
            miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.line, ")");

            
        }
        public void print(Node<String> parent, List<Token> tokens)
        {
            Node<String> printNode = parent;
            printNode.children.Add(new Node<string>("print"));
            List<Token> expressionTokens = tokens.GetRange(1, tokens.Count - 1);
            expr(printNode, expressionTokens);

        }

        public void read(Node<String> parent, List<Token> tokens)
        {
            Node<String> readNode = parent;
            readNode.children.Add(new Node<string>("read"));
            if (tokens.Count != 2)
            {
                miniPLExceptionThrower.throwMiniPLExepction("Invalid usage of read");
            } else
            {
                Token varIdent = tokens[1];
                string identifier = this.identifier(varIdent);
                readNode.children.Add(new Node<string>($"id({identifier})"));
            }

        }

        public void var(Node<String> parent, List<Token> tokens)
        {
            Token first = tokens[0];
            Node<String> assignmentNode = parent;

            if (tokens.Count < 4)
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid usage of var at row {first.line}");
            }
            int identI = 1;
            int typeI = 3;
            int assignMentI = 4;
            
            string identifier = this.identifier(tokens[identI]);
            
            if (tokens[2].value != ":")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.line, tokens[2].value, ":");
            }
            
            string type = this.type(tokens[typeI]);
            assignmentNode.children.Add(new Node<String>($"id({identifier})({type})"));
            if (tokens.Count == 4)
            {
                return;
            }

            Token assignmentToken = tokens[assignMentI];
            if (assignmentToken.value != ":=")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.line, assignmentToken.value, ":=");
            }
            assignmentNode.children.Add(new Node<String>($":="));
            if (tokens.Count < 6)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.line, "expression");
            }

            List<Token> expression = new List<Token>();
            int i = assignMentI + 1;
            while (i < tokens.Count)
            {
                expression.Add(tokens[i]);
                i++;
            }
            expr(assignmentNode, expression);
        }

        public void expr(Node<String> parent, List<Token> tokens)
        {
            if (tokens.Count < 1)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(-1, "expression");
            }
            Node<String> expressionNode = new Node<String>("expression");
            parent.children.Add(expressionNode);
            Token first = tokens[0];
            if (miniPLHelper.isUnaryOperator(first.value))
            {
                if (tokens.Count < 2)
                {
                    miniPLExceptionThrower.throwInvalidError(first.line, "expression");
                }
                expressionNode.children.Add(new Node<string>("!"));
                operand(expressionNode, tokens.GetRange(1, tokens.Count - 1));
            } else
            {
                List<Token> firstOperandTokens = new List<Token>();
                Token operatorToken = null;
                int i = 0;
                while (i < tokens.Count)
                {
                    Token t = tokens[i];
                    if(miniPLHelper.isOperator(t.value))
                    {
                        operatorToken = t;
                        i++;
                        break;
                    } else
                    {
                        firstOperandTokens.Add(t);
                        i++;
                    }
                }
                if (operatorToken == null)
                {
                    this.operand(expressionNode, firstOperandTokens);
                    return;
                } else
                {
                    this.operand(expressionNode, firstOperandTokens);
                    expressionNode.children.Add(new Node<string>($"{operatorToken.value}"));
                    List<Token> secondOperandTokens = new List<Token>();

                    while (i < tokens.Count)
                    {
                        Token t = tokens[i];
                        
                        
                         secondOperandTokens.Add(t);
                         i++;
                    }
                    this.operand(expressionNode, secondOperandTokens);
                }
            }



        }

        public void operand(Node<String> parent, List<Token> tokens)
        {
            Node<String> operandNode = new Node<string>("operand");
            parent.children.Add(operandNode);

            if (tokens.Count < 1)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(-1, "operand");
            }
            Token first = tokens[0];
            if (first.value == "(")
            {
                int i = 1;
                List<Token> expression = new List<Token>();
                while (i < tokens.Count)
                {
                    Token t = tokens[i];
                    if (t.value == ")")
                    {
                        expr(operandNode, expression);
                        return;
                    }
                    else
                    {
                        expression.Add(t);
                    }
                    i++;
                }
                // ")" not found
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.line, ")");
            } else
            {
                if (tokens.Count != 1)
                {
                    miniPLExceptionThrower.throwMiniPLExepction($"Invalid operand at line {first.line}");
                }

                string value = first.value;

                bool isNumeric = int.TryParse(value, out int numericValue);
                if (isNumeric)
                {
                    operandNode.children.Add(new Node<String>($"number({numericValue})"));
                    return;
                } else if (miniPLHelper.isString(value))
                {
                    operandNode.children.Add(new Node<String>($"string({value})"));
                    return;
                } else
                {
                    string identifier = this.identifier(first);
                    operandNode.children.Add(new Node<String>($"id({identifier})"));
                    return;
                }

            }
        }

        public string identifier(Token t)
        {
            string value = t.value;
            char firstChar = value[0];
            if (!Char.IsLetter(firstChar))
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid identifier '{value}' at row {t.line}");
            }

            foreach(char c in value)
            {
                if (!Char.IsNumber(c) && !Char.IsLetter(c) && c != '_')
                {
                    miniPLExceptionThrower.throwMiniPLExepction($"Invalid identifier '{value}' at row {t.line}");
                }
            }

            this.identifiers.TryAdd(value, true);

            return value;
        }

        public string type(Token t)
        {
            string value = t.value;
            List<string> validTypes = new List<string>{ "int", "string", "bool" };
            if (!validTypes.Contains(value))
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid type '{value}' at row {t.line}");
            }

            return value;


        }
    }
}
