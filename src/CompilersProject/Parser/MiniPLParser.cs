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
        public Dictionary<string, ParserIdentifier> identifiers = new Dictionary<string, ParserIdentifier>();
        public MiniPLExceptionThrower miniPLExceptionThrower;
        public MiniPLHelper miniPLHelper;
        private int forLoopIndex = 0;
        public MiniPLParser()
        {
            this.miniPLExceptionThrower = new MiniPLExceptionThrower("Parser");
            this.miniPLHelper = new MiniPLHelper(miniPLExceptionThrower);
        }
        public Node<String> parse(List<Token> tokens)
        {
            this.identifiers = new Dictionary<string, ParserIdentifier>();
            this.forLoopIndex = 0;

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

            Node<String> stmtListNode = new Node<String>("stmt_list");
            root.children.Add(stmtListNode);
            List<Token> firstStatement = new List<Token>();
            bool inLoop = false;
            int innerLoopCount = 0;
            int i = -1;
            while (i < tokens.Count)
            {

                i++;
                Token t = tokens[i];
                string tokenValue = t.value;
                if (i == 0)
                {
                    if (tokenValue == "for")
                    {
                        firstStatement.Add(t);
                        inLoop = true;
                        continue;
                    }
                }

                if (inLoop)
                {
                    if (tokenValue == "end")
                    {
                        firstStatement.Add(t);

                        i++;
                        if (tokens.Count < i)
                        {
                            miniPLExceptionThrower
                                .throwExpectedSomethingFoundNothingError(t.row, "for");
                        }
                        Token shouldBeFor = tokens[i];
                        miniPLHelper.checkTokenThrowsMiniPLError(shouldBeFor, "for");

                        i++;
                        if (tokens.Count < i)
                        {
                            miniPLExceptionThrower
                                .throwExpectedSomethingFoundNothingError(t.row, ";");
                        }

                        Token shouldBeEndOfLine = tokens[i];
                        miniPLHelper.checkTokenThrowsMiniPLError(shouldBeEndOfLine, ";");

                        firstStatement.Add(shouldBeFor);

                        if (innerLoopCount == 0)
                        {

                            break;
                        }
                        else
                        {
                            firstStatement.Add(shouldBeEndOfLine);
                            innerLoopCount--;
                        }
                    }
                    else if (tokenValue == "for")
                    {
                        innerLoopCount++;
                        firstStatement.Add(t);
                    }
                    else
                    {
                        firstStatement.Add(t);
                    }

                }
                else
                {
                    if (tokenValue == ";")
                    {
                        i++;
                        break;
                    }
                    else
                    {
                        firstStatement.Add(t);
                    }
                }
            }
            stmt(stmtListNode, firstStatement);
            List<Token> restOfStatements = new List<Token>();
            while (i < tokens.Count)
            {
                Token t = tokens[i];
                restOfStatements.Add(t);
                i++;
            }

            stmt_list(stmtListNode, restOfStatements);
        }
        public void stmt(Node<String> parent, List<Token> statement)
        {

            Node<string> statementNode = parent;

            if (statement.Count == 0)
            {
                statementNode.children.Add(new Node<string>("$$"));
                return;
            }
            Token firstToken = statement[0];

            if (firstToken.value == "var")
            {
                var(statementNode, statement);
            }
            else if (firstToken.value == "read")
            {
                read(statementNode, statement);
            }
            else if (firstToken.value == "print")
            {
                print(statementNode, statement);
            }
            else if (firstToken.value == "assert")
            {
                assert(statementNode, statement);
            }
            else if (firstToken.value == "for")
            {
                forLoop(statementNode, statement);
            }
            else
            {
                identAssignment(statementNode, statement);
            }
        }

        public void forLoop(Node<String> parent, List<Token> tokens)
        {
            forLoopIndex++;

            Node<String> loopNode = parent;
            Token first = tokens[0];
            loopNode.children.Add(new Node<string>("forloop"));
            if (tokens.Count < 2)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "identifier");
            }
            Token identifierToken = tokens[1];
            this.identifierCheck(identifierToken);
            loopNode.children.Add(new Node<string>(identifierToken.value));

            if (tokens.Count < 3)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "in");
            }
            Token inToken = tokens[2];
            if (inToken.value != "in")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(
                    first.row,
                    inToken.value,
                    "in"
                );
            }

            loopNode.children.Add(new Node<string>("in"));

            if (tokens.Count < 4)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(
                    first.row,
                    "For loop expression"
                );
            }
            int firstExpressionStartIndex = 3;
            int i = firstExpressionStartIndex;
            List<Token> firstExpressionTokens = new List<Token>();

            while (i < tokens.Count)
            {
                Token t = tokens[i];
                if (t.value == "..")
                {
                    expr(loopNode, firstExpressionTokens);
                    loopNode.children.Add(new Node<string>(".."));
                    i++;
                    break;
                }
                else
                {
                    firstExpressionTokens.Add(t);
                }
                i++;
            }
            if (firstExpressionTokens.Count == 0)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "expression");
            }

            List<Token> secondExpressionTokens = new List<Token>();
            while (i < tokens.Count)
            {
                Token t = tokens[i];

                if (t.value == "do")
                {
                    break;
                }
                else
                {
                    secondExpressionTokens.Add(t);
                }
                i++;
            }

            expr(loopNode, secondExpressionTokens);
            Token shouldBeDoToken = tokens[i];

            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeDoToken, "do");

            loopNode.children.Add(new Node<string>("do"));

            List<Token> statements = new List<Token>();
            i++;
            if (i >= tokens.Count)
            {
                miniPLExceptionThrower
                    .throwExpectedSomethingFoundNothingError(first.row + 1, "statements");
            }

            while (i < tokens.Count - 2)
            {
                Token t = tokens[i];
                statements.Add(t);
                i++;
            }

            Token shouldBeEndToken = tokens[i];
            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeEndToken, "end");

            stmt_list(loopNode, statements);


            loopNode.children.Add(new Node<string>("end"));
            i++;
            if (i >= tokens.Count)
            {
                miniPLExceptionThrower
                    .throwExpectedSomethingFoundNothingError(shouldBeEndToken.row, "for");
            }

            Token shouldBeForToken = tokens[i];
            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeForToken, "for");

            loopNode.children.Add(new Node<string>("for"));
            foreach (string key in identifiers.Keys)
            {
                ParserIdentifier pi = identifiers.GetValueOrDefault(key, new ParserIdentifier("undefined", -1));
                if (pi.forLoopIndex == forLoopIndex)
                {
                    identifiers.Remove(key);
                }
            }
            forLoopIndex--;
        }

        public void identAssignment(Node<String> parent, List<Token> tokens)
        {
            Token ident = tokens[0];
            string identifier = ident.value;
            Node<string> assignmentNode = new Node<string>("assignment");

            parent.children.Add(assignmentNode);

            if (identifiers.GetValueOrDefault(identifier, new ParserIdentifier("undefined", -1)).type == "undefined")
            {
                miniPLExceptionThrower.throwUndefinedVariableError(ident.row, identifier);
            }
            assignmentNode.children.Add(new Node<string>(identifier));
            if (tokens.Count < 2)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(ident.row, ":=");
            }
            Token assignmentToken = tokens[1];
            if (assignmentToken.value != ":=")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(
                    assignmentToken.row,
                    assignmentToken.value,
                    ":="
                );
            }

            List<Token> expressionTokens = new List<Token>();
            int i = 2;
            while (i < tokens.Count)
            {
                Token curToken = tokens[i];
                expressionTokens.Add(curToken);
                i++;
            }
            expr(assignmentNode, expressionTokens);

        }
        public void assert(Node<String> parent, List<Token> tokens)
        {
            Node<String> assertNode = parent;
            assertNode.children.Add(new Node<string>("assert"));
            Token first = tokens[0];
            if (tokens.Count < 2)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "(");
            }
            Token leftParenthesis = tokens[1];
            if (leftParenthesis.value != "(")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(
                    first.row,
                    leftParenthesis.value,
                    "("
                );
            }
            List<Token> expressionTokens = new List<Token>();
            int i = 2;
            while (i < tokens.Count)
            {
                Token currentT = tokens[i];
                if (currentT.value != ")")
                {
                    expressionTokens.Add(currentT);
                }
                else
                {
                    expr(assertNode, expressionTokens);
                    return;
                }
                i++;
            }
            miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, ")");


        }
        public void print(Node<String> parent, List<Token> tokens)
        {
            Node<String> printNode = new Node<string>("print");
            parent.children.Add(printNode);
            List<Token> expressionTokens = tokens.GetRange(1, tokens.Count - 1);
            expr(printNode, expressionTokens);

        }

        public void read(Node<String> parent, List<Token> tokens)
        {
            Node<String> readNode = new Node<string>("read");

            parent.children.Add(readNode);

            if (tokens.Count != 2)
            {
                miniPLExceptionThrower.throwMiniPLExepction("Invalid usage of read");
            }
            else
            {
                Token varIdent = tokens[1];
                this.identifierCheck(varIdent);
                readNode.children.Add(new Node<string>(varIdent.value));
                this.identifiers.Add(varIdent.value, new ParserIdentifier("", forLoopIndex));
            }

        }

        public void var(Node<String> parent, List<Token> tokens)
        {
            Token first = tokens[0];
            Node<String> varAssignmentNode = new Node<string>("var_assignment");
            parent.children.Add(varAssignmentNode);

            if (tokens.Count < 4)
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid usage of var at row {first.row}");
            }
            int identI = 1;
            Token identToken = tokens[identI];
            int typeI = 3;
            int assignMentI = 4;

            this.identifierCheck(identToken);

            varAssignmentNode.children.Add(new Node<String>(identToken.value));

            if (tokens[2].value != ":")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.row, tokens[2].value, ":");
            }

            string type = this.validTypeCheck(tokens[typeI]);

            if (this.identifiers.GetValueOrDefault(identToken.value, new ParserIdentifier("undefined", -1)).type != "undefined")
            {
                miniPLExceptionThrower
                    .throwMiniPLExepction($"Duplicate variable assignment at row {identToken.row}");
            }
            else
            {
                this.identifiers.Add(identToken.value, new ParserIdentifier(type, forLoopIndex));
            }

            varAssignmentNode.children.Add(new Node<String>(type));
            if (tokens.Count == 4)
            {
                return;
            }

            Token assignmentToken = tokens[assignMentI];
            if (assignmentToken.value != ":=")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.row, assignmentToken.value, ":=");
            }

            if (tokens.Count < 6)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "expression");
            }

            List<Token> expression = new List<Token>();
            int i = assignMentI + 1;
            while (i < tokens.Count)
            {
                expression.Add(tokens[i]);
                i++;
            }
            expr(varAssignmentNode, expression);
        }

        public string expr(Node<String> parent, List<Token> tokens)
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
                    miniPLExceptionThrower.throwInvalidError(first.row, "expression");
                }
                expressionNode.children.Add(new Node<string>("!"));

                string operandType = Operand(expressionNode, tokens.GetRange(1, tokens.Count - 1));
                if (operandType != "bool")
                {
                    miniPLExceptionThrower.throwInvalidUsageOfOperatorError(first.row, "!", operandType);
                }
                return "bool";
            }
            else
            {
                List<Token> firstOperandTokens = new List<Token>();
                Token operatorToken = null;
                int i = 0;
                while (i < tokens.Count)
                {
                    Token t = tokens[i];
                    if (miniPLHelper.isOperator(t.value))
                    {
                        operatorToken = t;
                        i++;
                        break;
                    }
                    else
                    {
                        firstOperandTokens.Add(t);
                        i++;
                    }
                }
                if (operatorToken == null)
                {

                    return this.Operand(expressionNode, firstOperandTokens);
                }
                else
                {
                    string firstOperandType = this.Operand(expressionNode, firstOperandTokens);
                    string op = operatorToken.value;

                    if (!miniPLHelper.isValidOperatorForType(op, firstOperandType))
                    {
                        miniPLExceptionThrower.throwInvalidOperatorError(operatorToken.row, op, firstOperandType);
                    }


                    expressionNode.children.Add(new Node<string>(op));

                    List<Token> secondOperandTokens = new List<Token>();

                    while (i < tokens.Count)
                    {
                        Token t = tokens[i];


                        secondOperandTokens.Add(t);
                        i++;
                    }
                    string secondOperandType = Operand(expressionNode, secondOperandTokens);
                    if (firstOperandType != secondOperandType)
                    {
                        miniPLExceptionThrower.throwInvalidExpressionError(operatorToken.row, firstOperandType, secondOperandType);
                    }
                    return secondOperandType;
                }
            }



        }

        public string Operand(Node<String> parent, List<Token> tokens)
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
                bool rightParFound = false;
                while (i < tokens.Count)
                {
                    Token t = tokens[i];
                    if (t.value == ")")
                    {
                        rightParFound = true;
                        break;
                    }
                    else
                    {
                        expression.Add(t);
                    }
                    i++;
                }
                // ")" not found
                if (rightParFound)
                {
                    return expr(operandNode, expression);
                }
                else
                {
                    miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, ")");
                    // to satisfy c# compiler
                    return "";
                }


            }
            else
            {
                if (tokens.Count != 1)
                {
                    miniPLExceptionThrower.throwMiniPLExepction($"Invalid operand at row {first.row}");
                }

                string value = first.value;

                bool isNumeric = int.TryParse(value, out int numericValue);
                if (isNumeric)
                {
                    operandNode.children.Add(new Node<String>($"int({numericValue})"));
                    return "int";
                }
                else if (miniPLHelper.isString(value))
                {
                    string splitted = value.Substring(1, value.Length - 2);
                    operandNode.children.Add(new Node<String>($"string({splitted})"));
                    return "string";
                }
                else if (miniPLHelper.isBoolean(value))
                {
                    operandNode.children.Add(new Node<String>($"bool({value})"));
                    return "bool";
                }
                else
                {
                    identifierCheck(first);
                    bool isIdentDefined = isIdentifierDefined(value);
                    if (!isIdentDefined)
                    {
                        miniPLExceptionThrower.throwUndefinedVariableError(first.row, value);
                    }
                    ParserIdentifier p = this.identifiers.GetValueOrDefault(value);

                    operandNode.children.Add(new Node<String>($"id({value})"));
                    return p.type;
                }

            }
        }

        public string identifierCheck(Token t)
        {
            string value = t.value;
            char firstChar = value[0];
            if (!Char.IsLetter(firstChar))
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid identifier '{value}' at row {t.row}");
            }

            foreach (char c in value)
            {
                if (!Char.IsNumber(c) && !Char.IsLetter(c) && c != '_')
                {
                    miniPLExceptionThrower.throwMiniPLExepction($"Invalid identifier '{value}' at row {t.row}");
                }
            }


            return value;
        }

        public string validTypeCheck(Token t)
        {

            if (!miniPLHelper.isValidType(t))
            {
                miniPLExceptionThrower.throwMiniPLExepction($"Invalid type '{t.value}' at row {t.row}");
            }

            return t.value;


        }

        private bool isIdentifierDefined(string ident)
        {
            ParserIdentifier p = this.identifiers.GetValueOrDefault(ident, new ParserIdentifier("undefined", -1));

            return p.type != "undefined";
        }
    }


}
